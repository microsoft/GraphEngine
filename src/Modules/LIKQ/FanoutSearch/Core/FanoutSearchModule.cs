// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity;
using Trinity.Diagnostics;
using Trinity.Network;

using System.Linq.Expressions;
using Trinity.Storage;

using System.Diagnostics;
using FanoutSearch.Protocols.TSL;
using System.Runtime.CompilerServices;
using System.Collections;
using Newtonsoft.Json.Linq;
using Trinity.Network.Messaging;

namespace FanoutSearch
{
    public partial class FanoutSearchModule : FanoutSearchBase
    {
        public delegate IEnumerable<long> IndexServiceMatchDelegate(object matchObject, string typeString = null);

        #region Fields
        private int m_globalTransactionId = 0;//only used by master server
        private QueryResultCache m_cache;
        private QueryPredicateCompiler m_compiler;
        private ConcurrentDictionary<int, AggregationObject> m_aggregationObjects = new ConcurrentDictionary<int, AggregationObject>();
        private static Func<long, ICellAccessor> s_useICellFunc = null;
        private static Action<FanoutQueryMessageReader, FanoutResultMessageWriter> s_metadataUpdateFunc = null;
        private static IndexServiceMatchDelegate s_indexServiceFunc = null;
        private static bool s_enable_external_query = true;
        private static bool s_timeout_return_partial_results = true;
        internal static bool s_force_run_as_client = false;
        private static bool s_cache_enabled = true;

        private const Action c_action_inv_continue      = ~Action.Continue;
        private const Action c_action_inv_return        = ~Action.Return;
        private const Action c_action_none              = ~(Action)0;
        private const long   c_default_query_time_quota = 800 /*ms*/;
        private static long  s_query_time_quota         = c_default_query_time_quota /*ms*/;
        private const int    c_master_server_id         = 0;
        internal static int s_max_fanoutmsg_size         = int.MaxValue;
        internal static int s_max_rsp_size               = int.MaxValue;


        #endregion

        public FanoutSearchModule()
        {
            this.Started += OnStart;

            m_compiler = new QueryPredicateCompiler();
            m_cache    = new QueryResultCache(m_compiler);
        }

        internal static FanoutSearchModule GetClientModule()
        {
            FanoutSearchModule ret = new FanoutSearchModule();
            ret.ClientInitialize(RunningMode.Server);
            return ret;
        }

        protected override void RegisterMessageHandler()
        {
            base.RegisterMessageHandler();
            // then, override FanoutSearch_impl
            MessageRegistry.RegisterMessageHandler((ushort)(this.AsynReqIdOffset + (ushort)global::FanoutSearch.Protocols.TSL.TSL.CommunicationModule.FanoutSearch.AsynReqMessageType.FanoutSearch_impl), FanoutSearch_impl_Recv);
        }

        public override string GetModuleName()
        {
            return "LIKQ";
        }

        private static void AddNode(ref FanoutPathDescriptor desc, int hop, long id)
        {
            switch (hop)
            {
                case 0:
                    throw new NotImplementedException();
                case 1:
                    desc.hop_1 = id;
                    break;
                case 2:
                    desc.hop_2 = id;
                    break;
                case 3:
                    desc.hop_3 = id;
                    break;
                default:
                    if (desc.hop_n == null)
                        desc.hop_n = new List<long>();
                    desc.hop_n.Add(id);
                    break;
            }
        }


        private void OnStart()
        {
            if (s_useICellFunc == null)
                s_useICellFunc = Global.LocalStorage.UseGenericCell;

            if (s_metadataUpdateFunc == null)
                s_metadataUpdateFunc = (_, __) => { };

            if (s_indexServiceFunc == null)
                s_indexServiceFunc = (o, t) => { throw new IndexingServiceNotRegisteredException(); };

            //SetLocalParallelism(3);

            new Task(() =>
            {
                while (true)
                {
                    Thread.Sleep(5000);
                    Log.Flush();
                }
            }).Start();
        }

        public static void RegisterUseICellOperationMethod(Func<long, ICellAccessor> func)
        {
            s_useICellFunc = func;
        }

        public static void RegisterUpdateQueryMetadataMethod(Action<FanoutQueryMessageReader, FanoutResultMessageWriter> func)
        {
            s_metadataUpdateFunc = func;
        }

        public static void RegisterIndexService(IndexServiceMatchDelegate func)
        {
            s_indexServiceFunc = func;
        }

        public static void RegisterExpressionSerializerFactory(Func<IExpressionSerializer> func)
        {
            ExpressionSerializer.SetSerializerFactory(func);
        }

        public static void RegisterQueryWhilelistType(Type t)
        {
            TraverseActionSecurityChecker.RegisterQueryWhitelistType(t);
        }

        public static void EnableExternalQuery(bool enabled)
        {
            s_enable_external_query = enabled;
        }
        
        public static void SetQueryTimeout(long milliseconds)
        {
            s_query_time_quota = milliseconds;
        }

        /// <returns>If query timeout is enabled, return the timeout time, in milliseconds, otherwise, return -1.</returns>
        public static long GetQueryTimeout()
        {
            if (!_QueryTimeoutEnabled()) return -1;
            return s_query_time_quota;
        }

        internal static bool _QueryTimeoutEnabled()
        {
            return (s_query_time_quota > 0);
        }

        public static void SetCacheEnabled(bool val)
        {
            s_cache_enabled = val;
        }

        public static void SetMaximumTraversalMessageSize(int size)
        {
            if (size <= TrinityProtocol.MsgHeader) throw new ArgumentException();
            s_max_fanoutmsg_size = size;
        }

        public static void SetMaximumResponseMessageSize(int size)
        {
            if (size <= 0) throw new ArgumentException();
            s_max_rsp_size = size;
        }

        /// <summary>
        /// Sets whether partial results already retrieved are returned as query result, when the query times out.
        /// When disabled, returns time out error message to the client.
        /// </summary>
        public static void QueryTimeoutReturnPartialResults(bool enabled)
        {
            s_timeout_return_partial_results = enabled;
        }

        /// <summary>
        /// Force the fanout search module to be initialized as a client module.
        /// </summary>
        /// <param name="value"></param>
        public static void ForceRunAsClient(bool value)
        {
            s_force_run_as_client = value;
        }

        private unsafe FanoutPathDescriptor GetPathDescriptor(long* ptr, int hop)
        {
            FanoutPathDescriptor path = new FanoutPathDescriptor();
            if (hop > 3) { path.hop_n = new List<long>(Enumerable.Repeat(0L, hop - 3)); }
            for (int i = 0; i <= hop; ++i)
                SetCellIdInPath(ref path, i, ptr[i]);
            return path;
        }

        private long GetCellIdInPath(FanoutPathDescriptor path, int hop)
        {
            switch (hop)
            {
                case 0:
                    return path.hop_0;
                case 1:
                    return path.hop_1.Value;
                case 2:
                    return path.hop_2.Value;
                case 3:
                    return path.hop_3.Value;
                default:
                    return path.hop_n[hop - 4];
            }
        }

        /// <summary>
        /// Caller should guarantee that path has sufficient storage space. No check on path.hop_n
        /// </summary>
        private void SetCellIdInPath(ref FanoutPathDescriptor path, int hop, long id)
        {
            switch (hop)
            {
                case 0:
                    path.hop_0 = id;
                    break;
                case 1:
                    path.hop_1 = id;
                    break;
                case 2:
                    path.hop_2 = id;
                    break;
                case 3:
                    path.hop_3 = id;
                    break;
                default:
                    path.hop_n[hop - 4] = id;
                    break;
            }
        }

        internal int GetWaitCount(int max_hop)
        {
            //MC             = machine count
            //m_i            = ((MC)*^i)
            //W              = m_(MAX_HOP)
            int W = 1;
            for (int i = 1; i <= max_hop; ++i)
            {
                W *= Global.CloudStorage.PartitionCount;
            }
            return W;
        }

    }
}

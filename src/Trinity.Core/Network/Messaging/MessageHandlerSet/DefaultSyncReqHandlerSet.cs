// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.Diagnostics;
using Trinity.Network.Sockets;

namespace Trinity.Network.Messaging
{
    internal static class DefaultSyncReqHandlerSet
    {
        private static List<TypeSyncRequestHandlerTuple> tupleList;
        private static bool initialized = false;

        internal unsafe static List<TypeSyncRequestHandlerTuple> MessageHandlerList
        {
            get
            {
                if (initialized)
                {
                    return tupleList;
                }

                tupleList = new List<TypeSyncRequestHandlerTuple>();

                #region Heartbeat
                tupleList.Add(new TypeSyncRequestHandlerTuple
                {
                    Id = (ushort)RequestType.Heartbeat,
                    Handler = delegate(SynReqArgs args)
                    {
                        return Task.CompletedTask;
                    }
                });
                #endregion

                #region P2PBarrier
                tupleList.Add(new TypeSyncRequestHandlerTuple
                {
                    Id = (ushort)RequestType.P2PBarrier,
                    Handler = delegate(SynReqArgs args)
                    {
                        BSP.P2PBarrierHandler(args);
                        return Task.CompletedTask;
                    }
                });
                #endregion

                #region ReportProxy
                tupleList.Add(new TypeSyncRequestHandlerTuple
                {
                    Id = (ushort)RequestType.ReportProxy,
                    Handler = delegate(SynReqArgs args)
                    {
                        //IPAddress(4)+Port(4)
                        IPAddress aggregator_address = BitHelper.ToIPAddress(args.Buffer, args.Offset);
                        int aggregator_port = *(int*)(args.Buffer + args.Offset + 4);
                        ServerInfo si = new ServerInfo(aggregator_address.ToString(), aggregator_port, "", LogLevel.Info);
                        lock (Global.ProxyTable)
                        {
                            Global.ProxyTable[si.EndPoint] = new Storage.RemoteStorage(si, TrinityConfig.ClientMaxConn);
                        }

                        return Task.CompletedTask;
                    }
                });
                #endregion

                #region LoadStorage
                tupleList.Add(new TypeSyncRequestHandlerTuple
                {
                    Id = (ushort)RequestType.LoadStorage,
                    Handler = delegate(SynReqArgs args)
                    {
                        Global.LocalStorage.LoadStorage();
                        return Task.CompletedTask;
                    }
                });
                #endregion

                #region SaveStorage
                tupleList.Add(new TypeSyncRequestHandlerTuple
                {
                    Id = (ushort)RequestType.SaveStorage,
                    Handler = delegate(SynReqArgs args)
                    {
                        Global.LocalStorage.SaveStorage();
                        return Task.CompletedTask;
                    }
                });
                #endregion

                #region ResetStorage
                tupleList.Add(new TypeSyncRequestHandlerTuple
                {
                    Id = (ushort)RequestType.ResetStorage,
                    Handler = delegate(SynReqArgs args)
                    {
                        Global.LocalStorage.ResetStorage();
                        return Task.CompletedTask;
                    }
                });
                #endregion

                initialized = true;
                return tupleList;
            }
        }
    }
}

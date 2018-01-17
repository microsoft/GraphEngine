// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.Network.Http;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.Network
{
    /// <summary>
    /// Represents a module that can be plugged into 
    /// a CommunicationInstance or another CommunicationModule.
    /// </summary>
    public abstract class CommunicationModule : CommunicationProtocolGroup
    {
        private ushort[] m_MessageIdOffsets = new ushort[4];
        protected internal MemoryCloud m_memorycloud = null;

        /// <summary>
        /// It is guaranteed that CloudStorage can be accessed (native server started)
        /// before an instance calls module Initialize() method.
        /// </summary>
        internal unsafe void Initialize(CommunicationInstance instance)
        {
            m_memorycloud = Global.CloudStorage;
            ICommunicationSchema schema = this.GetCommunicationSchema();

            this.SynReqIdOffset = instance.SynReqIdOffset;
            this.SynReqRspIdOffset = instance.SynReqRspIdOffset;
            this.AsynReqIdOffset = instance.AsynReqIdOffset;
            this.AsynReqRspIdOffset = instance.AsynReqRspIdOffset;

            checked
            {
                instance.SynReqIdOffset += (ushort)schema.SynReqProtocolDescriptors.Count();
                instance.SynReqRspIdOffset += (ushort)schema.SynReqRspProtocolDescriptors.Count();
                instance.AsynReqIdOffset += (ushort)schema.AsynReqProtocolDescriptors.Count();
                instance.AsynReqRspIdOffset += (ushort)schema.AsynReqRspProtocolDescriptors.Count();
                // each ASYNC_WITH_RSP message comes with a response handler.
                instance.AsynReqIdOffset += (ushort)schema.AsynReqRspProtocolDescriptors.Count();
            }

            this.RegisterMessageHandler();
        }

        /// <summary>
        /// It is guaranteed that CloudStorage can be accessed (server started)
        /// before a client calls module ClientInitialize() method.
        /// </summary>
        public unsafe void ClientInitialize(RunningMode remoteRunningMode)
        {
            ClientInitialize(remoteRunningMode, Global.CloudStorage);
        }

        /// <summary>
        /// It is guaranteed that CloudStorage can be accessed (server started)
        /// before a client calls module ClientInitialize() method.
        /// </summary>
        public unsafe void ClientInitialize(RunningMode remoteRunningMode, MemoryCloud mc)
        {
            m_memorycloud = mc;
            string moduleName = GetModuleName();
            RemoteStorage rs = remoteRunningMode == RunningMode.Server ?
               mc.StorageTable.FirstOrDefault(_ => _ is RemoteStorage) as RemoteStorage :
               mc.ProxyList.FirstOrDefault();

            if (null == rs)
            {
                string msg = "ClientInitialize: " + moduleName + ": No remote communication instance found.";
                Log.WriteLine(LogLevel.Error, msg);
                throw new InvalidOperationException(msg);
            }

            ushort synReqOffset;
            ushort synReqRspOffset;
            ushort asynReqOffset;
            ushort asynReqRspOffset;

            if (!rs.GetCommunicationModuleOffset(moduleName, out synReqOffset, out synReqRspOffset, out asynReqOffset, out asynReqRspOffset))
            {
                string msg = "CommunicationModule " + moduleName + " not found on the remote side.";
                Log.WriteLine(LogLevel.Error, msg);
                throw new InvalidOperationException(msg);
            }
            else
            {
                this.SynReqIdOffset = synReqOffset;
                this.SynReqRspIdOffset = synReqRspOffset;
                this.AsynReqIdOffset = asynReqOffset;
                this.AsynReqRspIdOffset = asynReqRspOffset;
            }
        }

        /// <summary>
        /// Returns a valid comm instance only when it's started. Otherwise return null.
        /// </summary>
        internal sealed override CommunicationInstance GetCommunicationInstance()
        {
            return Global.CommunicationInstance;
        }

        #region Message passing
        /// <summary>
        /// Sends multiple buffers sequentially, as a single binary message to the specified communication module.
        /// The message should contain the original message type id defined by the modules,
        /// and the offsets will be automatically applied.
        /// </summary>
        /// <param name="endpoint">A message passing endpoint.</param>
        /// <param name="buffers">Binary message buffers.</param>
        /// <param name="sizes">The size of the message.</param>
        /// <param name="count">The number of buffers.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SendMessage(IMessagePassingEndpoint endpoint, byte** buffers, int* sizes, int count)
        {
            byte b_msg_type = PointerHelper.GetByte(buffers, sizes, TrinityProtocol.MsgTypeOffset);
            PointerHelper.Add(buffers, sizes, TrinityProtocol.MsgIdOffset, m_MessageIdOffsets[b_msg_type]);

            endpoint.SendMessage(buffers, sizes, count);
        }

        /// <summary>
        /// Sends multiple buffers sequentially, as a single binary message to the specified communication module.
        /// The message should contain the original message type id defined by the modules,
        /// and the offsets will be automatically applied.
        /// </summary>
        /// <param name="endpoint">A message passing endpoint.</param>
        /// <param name="buffers">Binary message buffers.</param>
        /// <param name="sizes">The size of the message.</param>
        /// <param name="count">The number of buffers.</param>
        /// <param name="response">The TrinityResponse object returned by the remote module.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SendMessage(IMessagePassingEndpoint endpoint, byte** buffers, int* sizes, int count, out TrinityResponse response)
        {
            byte b_msg_type = PointerHelper.GetByte(buffers, sizes, TrinityProtocol.MsgTypeOffset);
            PointerHelper.Add(buffers, sizes, TrinityProtocol.MsgIdOffset, m_MessageIdOffsets[b_msg_type]);

            endpoint.SendMessage(buffers, sizes, count, out response);
        }

        /// <summary>
        /// Sends a binary message to the specified communication module.
        /// The message should contain the original message type id defined by the modules,
        /// and the offsets will be automatically applied.
        /// </summary>
        /// <param name="endpoint">A message passing endpoint.</param>
        /// <param name="buffer">A binary message buffer.</param>
        /// <param name="size">The size of the message.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SendMessage(IMessagePassingEndpoint endpoint, byte* buffer, int size)
        {
            byte b_msg_type = *(buffer + TrinityProtocol.MsgTypeOffset);
            *(ushort*)(buffer + TrinityProtocol.MsgIdOffset) += m_MessageIdOffsets[b_msg_type];

            endpoint.SendMessage(buffer, size);
        }

        /// <summary>
        /// Sends a binary message to the specified communication module.
        /// The message should contain the original message type id defined by the modules,
        /// and the offsets will be automatically applied.
        /// </summary>
        /// <param name="endpoint">A message passing endpoint.</param>
        /// <param name="buffer">A binary message buffer.</param>
        /// <param name="size">The size of the message.</param>
        /// <param name="response">The TrinityResponse object returned by the remote module.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SendMessage(IMessagePassingEndpoint endpoint, byte* buffer, int size, out TrinityResponse response)
        {
            byte b_msg_type = *(buffer + TrinityProtocol.MsgTypeOffset);
            *(ushort*)(buffer + TrinityProtocol.MsgIdOffset) += m_MessageIdOffsets[b_msg_type];

            endpoint.SendMessage(buffer, size, out response);
        }
        #endregion

        internal HttpHandler GetRootHttpHandler() { return this.RootHttpHandler; }
        internal HttpRequestDispatcher GetHttpRequestDispatcher() { return this.DispatchHttpRequest; }

        #region Public
        /// <summary>
        /// Gets the name of the module. The name will be used during communication schema verification.
        /// Also, Http endpoints will be available at http://root_endpoint/module_name/. For example,
        /// if the module name is <c>foo</c>, an Http protocol <c>bar</c> is defined within the module, and the host name is <c>addr</c>,
        /// then the endpoint is presented at http://addr/foo/bar/.
        /// </summary>
        /// <returns>The name of the module.</returns>
        public abstract string GetModuleName();
        #endregion

        #region Message id offsets
        /// <summary>
        /// The message type id offset of synchronous-request-no-response messages.
        /// This property is only valid when the hosting communication instance has started,
        /// and initialized the communication modules.
        /// </summary>
        protected internal ushort SynReqIdOffset
        {
            get { return m_MessageIdOffsets[0]; }
            internal set { m_MessageIdOffsets[0] = value; }
        }

        /// <summary>
        /// The message type id offset of synchronous-request-with-response messages.
        /// This property is only valid when the hosting communication instance has started,
        /// and initialized the communication modules.
        /// </summary>
        protected internal ushort SynReqRspIdOffset
        {
            get { return m_MessageIdOffsets[1]; }
            internal set { m_MessageIdOffsets[1] = value; }
        }

        /// <summary>
        /// The message type id offset of asynchronous-request-no-response messages.
        /// This property is only valid when the hosting communication instance has started,
        /// and initialized the communication modules.
        /// </summary>
        protected internal ushort AsynReqIdOffset
        {
            get { return m_MessageIdOffsets[2]; }
            internal set { m_MessageIdOffsets[2] = value; }
        }

        protected internal ushort AsynReqRspIdOffset
        {
            get { return m_MessageIdOffsets[3]; }
            internal set { m_MessageIdOffsets[3] = value; }
        }
        #endregion
    }
}

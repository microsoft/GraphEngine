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
        internal unsafe delegate void SendMessageDeleate(int instanceId, byte* buffer, int size);
        internal unsafe delegate void SendMessageWithResponseDeleate(int instanceId, byte* buffer, int size, out TrinityResponse response);

        private ushort[] m_MessageIdOffsets = new ushort[3];

        private SendMessageDeleate m_sendMessage;
        private SendMessageWithResponseDeleate m_sendMessageWithResponse;

        private MemoryCloud m_memoryCloud;

        /// <summary>
        /// It is guaranteed that CloudStorage can be accessed (native server started)
        /// before an instance calls module Initialize() method.
        /// </summary>
        internal unsafe void Initialize(CommunicationInstance instance)
        {
            //Debug.Assert(TrinityConfig.CurrentRunningMode != RunningMode.Client);
            //Debug.Assert(TrinityConfig.CurrentRunningMode != RunningMode.Embedded);
            //Debug.Assert(TrinityConfig.CurrentRunningMode != RunningMode.Undefined);

            ICommunicationSchema schema = this.GetCommunicationSchema();
            m_memoryCloud = instance.CloudStorage;

            this.SynReqIdOffset = instance.SynReqIdOffset;
            this.SynReqRspIdOffset = instance.SynReqRspIdOffset;
            this.AsynReqIdOffset = instance.AsynReqIdOffset;

            checked
            {
                instance.SynReqIdOffset += (ushort)schema.SynReqProtocolDescriptors.Count();
                instance.SynReqRspIdOffset += (ushort)schema.SynReqRspProtocolDescriptors.Count();
                instance.AsynReqIdOffset += (ushort)schema.AsynReqProtocolDescriptors.Count();
            }

            this.RegisterMessageHandler();
            SetupMessagePassingInterfaces(instance.RunningMode);
        }

        /// <summary>
        /// It is guaranteed that CloudStorage can be accessed (server started)
        /// before a client calls module ClientInitialize() method.
        /// </summary>
        internal protected unsafe void ClientInitialize(RunningMode remoteRunningMode, MemoryCloud mc = null)
        {
            //Debug.Assert(TrinityConfig.CurrentRunningMode == RunningMode.Client);
            //Debug.Assert(remoteRunningMode == RunningMode.Server || remoteRunningMode == RunningMode.Proxy);

            if (mc == null)
                mc = Global.CloudStorage;
            m_memoryCloud = mc;

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

            if (!rs.GetCommunicationModuleOffset(moduleName, out synReqOffset, out synReqRspOffset, out asynReqOffset))
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
            }

            SetupMessagePassingInterfaces(remoteRunningMode);
        }

        /// <summary>
        /// Returns a valid comm instance only when it's started. Otherwise return null.
        /// </summary>
        internal sealed override CommunicationInstance GetCommunicationInstance()
        {
            return Global.CommunicationInstance;
        }

        #region Message passing
        private unsafe void SetupMessagePassingInterfaces(RunningMode remoteRunningMode)
        {

            switch (remoteRunningMode)
            {
                case RunningMode.Server:
                    m_sendMessage = m_memoryCloud.SendMessageToServer;
                    m_sendMessageWithResponse = m_memoryCloud.SendMessageToServer;
                    break;
                case RunningMode.Proxy:
                    m_sendMessage = m_memoryCloud.SendMessageToProxy;
                    m_sendMessageWithResponse = m_memoryCloud.SendMessageToProxy;
                    break;
            }
        }

        /// <summary>
        /// Sends a binary message to the specified communication module.
        /// The message should contain the original message type id defined by the modules,
        /// and the offsets will be automatically applied.
        /// </summary>
        /// <param name="moduleId">A 32-bit module id.</param>
        /// <param name="buffer">A binary message buffer.</param>
        /// <param name="size">The size of the message.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe void SendMessage(int moduleId, byte* buffer, int size)
        {
            byte b_msg_type = *(buffer + TrinityProtocol.MsgTypeOffset);
            *(ushort*)(buffer + TrinityProtocol.MsgIdOffset) += m_MessageIdOffsets[b_msg_type];

            m_sendMessage(moduleId, buffer, size);
        }

        /// <summary>
        /// Sends a binary message to the specified communication module.
        /// The message should contain the original message type id defined by the modules,
        /// and the offsets will be automatically applied.
        /// </summary>
        /// <param name="moduleId">A 32-bit module id.</param>
        /// <param name="buffer">A binary message buffer.</param>
        /// <param name="size">The size of the message.</param>
        /// <param name="response">The TrinityResponse object returned by the remote module.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe void SendMessage(int moduleId, byte* buffer, int size, out TrinityResponse response)
        {
            byte b_msg_type = *(buffer + TrinityProtocol.MsgTypeOffset);
            *(ushort*)(buffer + TrinityProtocol.MsgIdOffset) += m_MessageIdOffsets[b_msg_type];

            m_sendMessageWithResponse(moduleId, buffer, size, out response);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe void SendMessageToInstance(int instanceId, byte* buffer, int size)
        {
            m_sendMessage(instanceId, buffer, size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe void SendMessageToInstance(int instanceId, byte* buffer, int size, out TrinityResponse response)
        {
            m_sendMessageWithResponse(instanceId, buffer, size, out response);
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
        #endregion
    }
}

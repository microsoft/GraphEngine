// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;

using Trinity;
using Trinity.Core.Lib;
using Trinity.Storage;
using Trinity.Network.Messaging;
using Trinity.Utilities;
using Trinity.Diagnostics;
using System.Runtime.InteropServices;
using Trinity.Daemon;
using Trinity.FaultTolerance;
using System.Diagnostics;
using System.Threading;
using Trinity.Configuration;

namespace Trinity.Network.Client
{
    unsafe partial class SynClient : IDisposable
    {
        public UInt64 socket = NativeNetwork.INVALID_SOCKET;
        internal IPEndPoint ipe;
        internal bool sock_connected = false;
        internal int ServerId;
        private int m_connect_retry = NetworkConfig.Instance.ClientReconnectRetry;

        bool disposed = false;
        static byte* HeartbeatBuffer;

        static SynClient()
        {
            InternalCalls.__init();
            HeartbeatBuffer = (byte*)Memory.malloc(TrinityProtocol.MsgHeader);
            *(int*)HeartbeatBuffer = TrinityProtocol.TrinityMsgHeader;
            *(HeartbeatBuffer + TrinityProtocol.MsgTypeOffset) = (byte)TrinityMessageType.PRESERVED_SYNC;
            *(ushort*)(HeartbeatBuffer + TrinityProtocol.MsgIdOffset) = (ushort)RequestType.Heartbeat;
        }

        public SynClient(IPEndPoint dest_ipe)
        {
            this.ipe = dest_ipe;

            DoConnect(TrinityConfig.MaxSocketReconnectNum);//TODO not configurable
            ServerId = -1;
        }

        public SynClient(IPEndPoint dest_ipe, int serverId)
        {
            this.ipe = dest_ipe;
            DoConnect(TrinityConfig.MaxSocketReconnectNum);
            ServerId = serverId;
        }

        internal bool DoConnect(int retry)
        {
            for (int i = 0; i < retry; i++)
            {
                socket = CNativeNetwork.CreateClientSocket();
                if (NativeNetwork.INVALID_SOCKET != socket && CNativeNetwork.ClientSocketConnect(socket, BitConverter.ToUInt32(ipe.Address.GetAddressBytes(), 0), (ushort)ipe.Port))
                {
                    sock_connected = true;
                    break;
                }
                else
                {
                    sock_connected = false;
                    Thread.Sleep(10);
                    FailureHandlerRegistry.ConnectionFailureHandling(ipe);
                }
            }

            return sock_connected;
        }

        public void Close()
        {
            if (sock_connected)
            {
                if (socket != NativeNetwork.INVALID_SOCKET)
                {
                    CNativeNetwork.CloseClientSocket(socket);
                    socket = NativeNetwork.INVALID_SOCKET;
                }
                sock_connected = false;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                Close();
                this.disposed = true;
            }
        }

        ~SynClient()
        {
            Dispose(false);
        }
    }
}

// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using Microsoft.Win32.SafeHandles;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using Trinity.Configuration;
using Trinity.Core.Lib;
using Trinity.FaultTolerance;
using Trinity.Network.Messaging;

namespace Trinity.Network.Client
{
    unsafe partial class SynClient : IDisposable
    {
        private static byte* HeartbeatBuffer;
        private static int HeartbeatBufferLen;

        public UInt64 socket = NativeNetwork.INVALID_SOCKET;
        private ThreadPoolBoundHandle threadPoolBoundHandle;
        internal IPEndPoint ipe;
        internal bool sock_connected = false;
        internal int ServerId;
        private int m_connect_retry = NetworkConfig.Instance.ClientReconnectRetry;
        private byte* intBuff;

        private bool disposed = false;

        static SynClient()
        {
            TrinityC.Init();
            HeartbeatBuffer = (byte*)Memory.malloc(TrinityProtocol.MsgHeader);
            *(int*)HeartbeatBuffer = TrinityProtocol.TrinityMsgHeader;
            *(TrinityMessageType*)(HeartbeatBuffer + TrinityProtocol.MsgTypeOffset) = TrinityMessageType.PRESERVED_SYNC;
            *(RequestType*)(HeartbeatBuffer + TrinityProtocol.MsgIdOffset) = RequestType.Heartbeat;
        }

        public SynClient(IPEndPoint dest_ipe)
        {
            this.ipe = dest_ipe;
            this.intBuff = (byte*)Memory.malloc(sizeof(int));

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
                using (var socketHandle = new SocketSafeHandle((IntPtr)socket))
                    threadPoolBoundHandle = ThreadPoolBoundHandle.BindHandle(socketHandle);
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
                if (threadPoolBoundHandle != null)
                {
                    threadPoolBoundHandle.Dispose();
                    threadPoolBoundHandle = null;
                }

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
                if (disposing)
                {
                    Close();
                }

                Memory.free(this.intBuff);
                this.disposed = true;
            }
        }

        ~SynClient()
        {
            Dispose(false);
        }

        private class SocketSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public SocketSafeHandle(IntPtr handle)
                : base(false)
            {
                this.SetHandle(handle);
            }

            protected override bool ReleaseHandle()
            {
                throw new InvalidOperationException();
            }
        }
    }
}

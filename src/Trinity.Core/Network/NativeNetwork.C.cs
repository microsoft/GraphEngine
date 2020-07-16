// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Trinity.Network
{
    unsafe partial class CNativeNetwork
    {
        #region Server
        [DllImport(TrinityC.AssemblyName)]
        internal static extern int StartSocketServer(UInt16 port);
        [DllImport(TrinityC.AssemblyName)]
        internal static extern int StopSocketServer();
        #endregion

        #region Client
        [DllImport(TrinityC.AssemblyName)]
        internal static extern UInt64 CreateClientSocket();
        [DllImport(TrinityC.AssemblyName)]
        internal static extern bool ClientSocketConnect(UInt64 socket, UInt32 ip, UInt16 port);
        [DllImport(TrinityC.AssemblyName)]
        internal static extern bool ClientSend(UInt64 socket, byte* buf, Int32 len);
        [DllImport(TrinityC.AssemblyName)]
        internal static extern bool ClientSendMulti(UInt64 socket, byte** bufs, Int32* lens, Int32 count);
        [DllImport(TrinityC.AssemblyName)]
        internal static extern TrinityErrorCode ClientReceive(UInt64 socket, out byte* buf, out Int32 len);
        [DllImport(TrinityC.AssemblyName)]
        internal static extern TrinityErrorCode WaitForAckPackage(UInt64 socket);
        [DllImport(TrinityC.AssemblyName)]
        internal static extern void CloseClientSocket(UInt64 socket);

        [DllImport(TrinityC.AssemblyName)]
        internal static extern Int32 BeginClientSend(UInt64 socket, byte* buf, Int32 len, NativeOverlapped* overlapped);
        [DllImport(TrinityC.AssemblyName)]
        internal static extern Int32 BeginClientSendMulti(UInt64 socket, byte** bufs, Int32* lens, Int32 count, NativeOverlapped* overlapped);
        [DllImport(TrinityC.AssemblyName)]
        internal static extern Int32 BeginClientReceive(UInt64 socket, byte* buf, Int32 len, NativeOverlapped* overlapped);

        internal const Int32 ERROR_SUCCESS = 0;
        internal const Int32 ERROR_IO_PENDING = 997;
        #endregion
    }
}

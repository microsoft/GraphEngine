// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.Network.Sockets;

namespace Trinity.Network
{
    /// <summary>
    /// Specifies data exchange format between communication instances and message dispatchers.
    /// </summary>
    /// <remarks>
    /// The message passing framework adapts Request-Response pattern.
    /// The data flow and call trace of a request-response message passing action
    /// is described as follow:
    /// <list type="number">
    /// <item><description>
    ///   When a message is received from a communication instance,
    ///   the communication listener of the instance is responsible for
    ///   buffer allocation, and the dispatcher should not manage the buffer.
    /// </description></item>
    /// <item><description>
    ///   Then the MessageBuff is handed from the comm. instance to the dispatcher.
    ///   The dispatcher calls the appropriate message handler, which will process the message,
    ///   allocate and populate the response message.
    /// </description></item>
    /// <item><description>
    ///   After the message is processed, the response buffer is returned to the comm. instance,
    ///   and it is then responsible for sending the response back to where it comes from.
    ///   When the response message is sent, or errors occur during the transmission, the comm. instance
    ///   is responsible for message buffer deallocation.
    /// </description></item>
    /// </list>
    /// Note that, the comm. instance may start a comm. listener in the unmanaged space. The dispatcher
    /// should never inspect memory address beyond what is specified in the MessageBuff, or attempt to
    /// reallocate, deallocate or cache-and-reuse the receiving buffer. If the response buffer is to be
    /// passed into unmanaged space, the buffer must be allocated with <see cref="Trinity.Core.Lib.Memory.malloc(ulong)"/>.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MessageBuff
    {
        /// <summary>
        /// Points to the trinity message data payload, for both
        /// received request, and the response to be sent.
        /// </summary>
        public byte* Buffer; // allocate after receiving the message header
        /// <summary>
        /// For RECV: Indicates the number of bytes received from the communication instance. Excluding SOCKET_HEADER.
        /// For SEND: Indicates the number of bytes in the response message, produced by the corresponding message handler. Including SOCKET_HEADER.
        /// </summary>
        public UInt32 Length;
    }

    unsafe partial class NativeNetwork
    {
        static NativeNetwork()
        {
            TrinityC.Init();
        }

        /// <summary>
        /// Value = 0xFFFFFFFFFFFFFFFF
        /// </summary>
        public const UInt64 INVALID_SOCKET = 0xFFFFFFFFFFFFFFFF;

        /// <summary>
        /// Value = 8192
        /// </summary>
        public const UInt32 RecvBufferSize = 8192;

        public static void StartTrinityServer(UInt16 port)
        {
            if(CNativeNetwork.StartSocketServer(port) == -1)
            {
                throw new System.Net.Sockets.SocketException();
            }
        }

        public static void StopTrinityServer()
        {
            CNativeNetwork.StopSocketServer();
        }
    }
}

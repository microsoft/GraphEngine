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
    [StructLayout(LayoutKind.Sequential)]
    unsafe struct MessageBuff// This is for data exchange between unmanaged and managed worlds
    {
        public byte* Buffer; // allocate after receiving the message header
        public UInt32 BytesReceived;
        public UInt32 BytesToSend;
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
            // config thread pool
            int cpu_core_count = Environment.ProcessorCount;
            CNativeNetwork.StartSocketServer(port);
            StartWorkerThreadPool();
        }

        public static void StopTrinityServer()
        {
            CNativeNetwork.StopSocketServer();
        }

        internal static void StartWorkerThreadPool()
        {
            int ThreadPoolSize = Environment.ProcessorCount << 1;
            for (int t = 0; t < ThreadPoolSize; t++)
            {
                (new Thread(WorkerThreadProc)).Start();
            }
        }

        internal static void WorkerThreadProc()
        {
            CNativeNetwork.EnterSocketServerThreadPool();
            while (true)
            {
                void* pContext = null;
                CNativeNetwork.AwaitRequest(out pContext);
                // a null pContext means that the completion port is closing.
                if (pContext == null)
                {
                    break;
                }
                MessageBuff* sendRecvBuff = (MessageBuff*)pContext;
                HandleMessage(sendRecvBuff);
                CNativeNetwork.SendResponse(pContext); // Send response back to the client
            }
            CNativeNetwork.ExitSocketServerThreadPool();
        }

        internal static void HandleMessage(MessageBuff* sendRecvBuff)
        {
            //Console.WriteLine("Received (managed world): {0}", sendRecvBuff->BytesReceived);
            MessageHandlers.DefaultParser.DispatchMessage(sendRecvBuff);
        }
    }
}

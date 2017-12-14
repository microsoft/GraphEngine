// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.Network.Messaging;

namespace Trinity.Network
{
#pragma warning disable 0420
    /// <summary>
    /// Provides methods for global bulk synchronization.
    /// </summary>
    public unsafe static partial class BSP
    {
        static volatile int default_task_sn = 1073741823; // Int32.MaxValue >> 1
        static Dictionary<int, int> BSPCheckInCount = new Dictionary<int, int>();

        static volatile int spin_lock = 0;
        internal static void P2PBarrierHandler(SynReqArgs args)
        {
            int taskId;

            taskId = *(int*)(args.Buffer + args.Offset);

            SpinLockInt32.GetLock(ref spin_lock);
            if (BSPCheckInCount.ContainsKey(taskId))
            {
                BSPCheckInCount[taskId]++;
            }
            else
            {
                BSPCheckInCount[taskId] = 1;
            }
            SpinLockInt32.ReleaseLock(ref spin_lock);
        }

        /// <summary>
        /// Sets a global barrier synchronization point for a specified BSP task. A process participating current BSP task waits until all other processes also reach this point.
        /// </summary>
        /// <param name="taskId">A user-specified task Id used to differentiate different barrier synchronization tasks.</param>
        public static void BarrierSync(this Trinity.Storage.FixedMemoryCloud storage, int taskId)
        {
            storage.P2PBarrierRequest(taskId);
        }

        /// <summary>
        /// Sets a global barrier synchronization point. A process participating current BSP task waits until all other processes also reach this point.
        /// </summary>
        public static void BarrierSync(this Trinity.Storage.FixedMemoryCloud storage)
        {
            storage.P2PBarrierRequest(default_task_sn);
            default_task_sn++;
        }

        internal static void P2PBarrierRequest(this Trinity.Storage.FixedMemoryCloud storage, int taskId)
        {
            TrinityMessage msg = new TrinityMessage(TrinityMessageType.PRESERVED_SYNC, (ushort)RequestType.P2PBarrier, sizeof(int));
            *(int*)(msg.Buffer + TrinityMessage.Offset) = taskId;

            SpinLockInt32.GetLock(ref spin_lock);
            if (BSPCheckInCount.ContainsKey(taskId))
            {
                BSPCheckInCount[taskId]++;
            }
            else
            {
                BSPCheckInCount[taskId] = 1;
            }
            SpinLockInt32.ReleaseLock(ref spin_lock);

            Parallel.For(0, Global.ServerCount, i =>
                {
                    if (i != Global.MyServerId)
                    {
                        storage.SendMessageToServer(i, msg.Buffer, msg.Size);
                    }
                }
            );

            int retry = 2048;
            while (true)
            {
                SpinLockInt32.GetLock(ref spin_lock);
                if (BSPCheckInCount[taskId] == Global.ServerCount)
                {
                    BSPCheckInCount.Remove(taskId);
                    SpinLockInt32.ReleaseLock(ref spin_lock);
                    return;
                }
                SpinLockInt32.ReleaseLock(ref spin_lock);
                if (--retry < 1024)
                {
                    if (retry > 0)
                        Thread.Yield();
                    else
                        Thread.Sleep(1);
                }
            }
        }

        internal static void BarrierSyncProfiling(this Trinity.Storage.FixedMemoryCloud storage, int count)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < count; i++)
                storage.BarrierSync();
            sw.Stop();
            Console.WriteLine("{0} Barrier Sync took {1}", count, sw.ElapsedMilliseconds);
        }
    }
}

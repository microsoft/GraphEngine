// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.Diagnostics;
using Trinity.Network.Messaging;

namespace Trinity.Network
{
#pragma warning disable 0420
    /// <summary>
    /// Provides methods for global bulk synchronization.
    /// </summary>
    public static partial class BSP
    {
        static volatile int default_task_sn = 1073741823; // Int32.MaxValue >> 1
        static Dictionary<int, int> BSPCheckInCount = new Dictionary<int, int>();

        static volatile int spin_lock = 0;
        internal unsafe static void P2PBarrierHandler(SynReqArgs args)
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
        public static void BarrierSync(this Trinity.Storage.MemoryCloud storage, int taskId)
        {
            storage.P2PBarrierRequestAsync(taskId).Wait();
        }

        /// <summary>
        /// Sets a global barrier synchronization point. A process participating current BSP task waits until all other processes also reach this point.
        /// </summary>
        public static void BarrierSync(this Trinity.Storage.MemoryCloud storage)
        {
            storage.P2PBarrierRequestAsync(default_task_sn).Wait();
            default_task_sn++;
        }

        //TODO in a multi-replica partition, the semantic of the BSP message should be configured to BROADCAST
        internal unsafe static Task P2PBarrierRequestAsync(this Trinity.Storage.MemoryCloud storage, int taskId)
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

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < Global.ServerCount; i++)
            {
                if (i != Global.MyPartitionId)
                {
                    Task task = storage[i].SendMessageAsync(msg.Buffer, msg.Size);
                    tasks.Add(task);
                }
            }

            return Task.WhenAll(tasks).ContinueWith(_ => P2PBarrierResponseAsync(taskId)).Unwrap();
        }

        private static async Task P2PBarrierResponseAsync(int taskId)
        {
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
                        await Task.Yield();
                    else
                        await Task.Delay(1);
                }
            }
        }

        internal static void BarrierSyncProfiling(this Trinity.Storage.MemoryCloud storage, int count)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < count; i++)
                storage.BarrierSync();
            sw.Stop();
            Log.WriteLine("{0} Barrier Sync took {1}", count, sw.ElapsedMilliseconds);
        }
    }
}

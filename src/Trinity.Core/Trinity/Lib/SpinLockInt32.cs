// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Trinity.Core.Lib
{
#pragma warning disable 0420
    /// <summary>
    /// Represents a mutual exclusive spinlock.
    /// </summary>
    public class SpinLockInt32
    {
        private static readonly int COUNTER_THRESHOLD_1 = 20000;
        private static readonly int COUNTER_THRESHOLD_2 = COUNTER_THRESHOLD_1 + 20;

        /// <summary>
        /// Acquires the lock.
        /// </summary>
        /// <param name="spinlock">A 32-bit integer that represents the spinlock.</param>
        public static unsafe void GetLock(ref int spinlock)
        {
            if (Interlocked.CompareExchange(ref spinlock, 1, 0) != 0)
            {
                int counter = 0;
                while (true)
                {
                    if (spinlock == 0 && Interlocked.CompareExchange(ref spinlock, 1, 0) == 0)
                        break;
                    ++counter;
                    if (counter < COUNTER_THRESHOLD_1)
                        continue;
                    if (counter < COUNTER_THRESHOLD_2)
                        Thread.Sleep(0);
                    else
                        Thread.Sleep(1);
                }
            }
        }

        private static readonly int COUNTER_THRESHOLD_FOR_AGGRESSIVE_METHOD = 200000000;

        /// <summary>
        /// Accquires the lock in an aggressive manner.
        /// </summary>
        /// <param name="spinlock">A 32-bit integer that represents the spinlock.</param>
        public static unsafe void GetLock_Aggressive(ref int spinlock)
        {
            if (Interlocked.CompareExchange(ref spinlock, 1, 0) != 0)
            {
                int counter = 0;

                while (true)
                {
                    if (spinlock == 0 && Interlocked.CompareExchange(ref spinlock, 1, 0) == 0)
                        break;
                    ++counter;
                    if (counter < COUNTER_THRESHOLD_FOR_AGGRESSIVE_METHOD)
                        continue;
                    Thread.Sleep(1);
                }
            }
        }

        internal static unsafe void GetLock(ref int spinlock, ref int pending_flag)
        {
            if (Interlocked.CompareExchange(ref spinlock, 1, 0) != 0)
            {
                pending_flag = 1;
                int counter = 0;
                while (true)
                {
                    if (spinlock == 0 && Interlocked.CompareExchange(ref spinlock, 1, 0) == 0)
                    {
                        pending_flag = 0;
                        break;
                    }
                    ++counter;
                    if (counter < COUNTER_THRESHOLD_1)
                        continue;
                    if (counter < COUNTER_THRESHOLD_2)
                        Thread.Sleep(0);
                    else
                        Thread.Sleep(1);
                }
            }
        }

        /// <summary>
        /// Attempts to acquire the lock.
        /// </summary>
        /// <param name="spinlock">A 32-bit integer that represents the spinlock.</param>
        /// <returns>true if the lock is acquired; otherwise, false.</returns>
        public static unsafe bool TryGetLock(ref int spinlock)
        {
            return Interlocked.CompareExchange(ref spinlock, 1, 0) == 0;
        }

        /// <summary>
        /// Attempts to acquire the lock at most <paramref name="retry"/> times.
        /// </summary>
        /// <param name="spinlock">A 32-bit integer that represents the spinlock.</param>
        /// <param name="retry">A integer that indicates the max retry times.</param>
        /// <returns>true if the lock is acquired; otherwise, false.</returns>
        public static unsafe bool TryGetLock(ref int spinlock, int retry)
        {
            int counter = retry;
            while (Interlocked.CompareExchange(ref spinlock, 1, 0) != 0)
            {
                if (--counter < 0)
                    return false;
                Thread.Sleep(1);
            }
            return true;
        }

        /// <summary>
        /// Release the lock.
        /// </summary>
        /// <param name="spinlock">A 32-bit integer that represents the spinlock.</param>
        public static unsafe void ReleaseLock(ref int spinlock)
        {
            Thread.MemoryBarrier();
            spinlock = 0;
        }
    }
}

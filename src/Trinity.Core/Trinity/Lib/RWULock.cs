// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Trinity.Core.Lib
{
#pragma warning disable 0420
    /// <summary>
    /// RWULock: Tri-state lock
    /// Mutual table:
    /// 
    ///   +---------+--------+--------+--------+
    ///   |Enter/St.|Read    |Upgrade |Write   |
    ///   +---------+--------+--------+--------+
    ///   |Reader   |Enter   |Enter   |Block   |
    ///   +---------+--------+--------+--------+
    ///   |Upgrader |Enter   |Block   |N/A     |
    ///   +---------+--------+--------+--------+
    ///   |Writer   |Block   |N/A     |N/A     |
    ///   +---------+--------+--------+--------+
    /// 
    ///   Write state can only be entered from Upgrade state. So Writer cannot meet Write status.
    /// 
    /// ReaderWriterUpgrader lock features similar interfaces to ReaderWriterLockSlim
    /// </summary>
    class RWULock
    {

        internal const int COUNTER_THRESHOLD_1 = 20000;
        internal const int COUNTER_THRESHOLD_2 = COUNTER_THRESHOLD_1 + 20;

        public void EnterReadLock()
        {
        RdLock_Start:
            if ((Interlocked.Add(ref rwLock, 2) & 1) != 0)
            {
                Interlocked.Add(ref rwLock, -2);
                int counter = 0;
                while (true)
                {
                    if ((rwLock & 1) == 0)
                        goto RdLock_Start;
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

        public void EnterReadLockAggressively()
        {
            if ((Interlocked.Add(ref rwLock, 2) & 1) != 0)
            {
                int counter = 0;
                while (true)
                {
                    if ((rwLock & 1) == 0)
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

        public bool TryEnterReadLock()
        {
            if ((Interlocked.Add(ref rwLock, 2) & 1) != 0)
            {
                Interlocked.Add(ref rwLock, -2);
                return false;
            }
            return true;
        }

        public bool TryEnterReadLock(int retry)
        {
            int counter = retry;
        RdLock_Start:
            if ((Interlocked.Add(ref rwLock, 2) & 1) != 0)
            {
                Interlocked.Add(ref rwLock, -2);
                while (true)
                {
                    if (--counter < 0)
                        return false;

                    if ((rwLock & 1) == 0)
                        goto RdLock_Start;

                    Thread.Sleep(1);
                }
            }
            return true;
        }

        public void ExitReadLock()
        {
            Interlocked.Add(ref rwLock, -2);
        }

        public void EnterWriteLock()
        {
        WriteLock_Start:
            if (1 == Interlocked.Increment(ref rwLock))//Clean entrance, no readers
            {
                return;
            }
            int counter = 0;
            while (true)
            {
                if (rwLock == 1)
                    break;
                ++counter;
                if (counter < COUNTER_THRESHOLD_1)
                {
                    Thread.Sleep(0);
                    continue;
                }
                Interlocked.Decrement(ref rwLock);
                Thread.Sleep(1);
                goto WriteLock_Start;
            }
        }

        public void ExitWriteLock()
        {
            Interlocked.Decrement(ref rwLock);
        }

        public void EnterUpgradeLock()
        {
            if (Interlocked.CompareExchange(ref updateLock, 1, 0) != 0)
            {
                int counter = 0;
                while (true)
                {
                    if (updateLock == 0 && Interlocked.CompareExchange(ref updateLock, 1, 0) == 0)
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

        public void ExitUpgradeLock()
        {
            Thread.MemoryBarrier();
            updateLock = 0;
        }

        public bool TryEnterUWLock()
        {
            if (Interlocked.CompareExchange(ref updateLock, 1, 0) == 0)
            {
                if (1 == Interlocked.Increment(ref rwLock))
                    return true;
                else
                {
                    Interlocked.Decrement(ref rwLock);
                    updateLock = 0;
                    return false;
                }
            }
            return false;
        }

        public void EnterUWLock()
        {
            EnterUpgradeLock();
            EnterWriteLock();
        }

        public void ExitWULock()
        {
            Interlocked.Decrement(ref rwLock);
            updateLock = 0;
        }

        private volatile int updateLock = 0;
        private volatile int rwLock = 0;
    }
}

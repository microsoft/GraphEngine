// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Diagnostics;

namespace Trinity.Storage
{
    public class CellLockOverflowException : Exception
    {
        internal long m_cellid;

        public long CellId { get { return m_cellid; } }

        public CellLockOverflowException(long cellid)
            : base("Cell lock overflowed.")
        {
            m_cellid = cellid;
        }
    }
    public class DeadlockException : Exception
    {
        public DeadlockException() : base("Deadlock detected.") { }
    }
    /// <summary>
    /// Provides thread-specific cell lock context.
    /// Note, a ThreadContext does not record affinity
    /// with a specific managed/unmanaged thread.
    /// It is up to the storage subsystem to leverage
    /// TLS (thread local storage) to establish a bound.
    /// </summary>
    internal unsafe class ThreadContext : IDisposable
    {
        internal void* m_lock_ctx;
        internal int  m_managedThreadId;
        internal bool m_disposed = false;

        internal ThreadContext()
        {
            m_lock_ctx = CLocalMemoryStorage.CThreadContextAllocate();
            m_managedThreadId = Thread.CurrentThread.ManagedThreadId;
            Log.WriteLine(LogLevel.Debug, "Created thread context 0x{0:X16}", (ulong)m_lock_ctx);
        }

        ~ThreadContext()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                Log.WriteLine(LogLevel.Debug, "Releasing thread context 0x{0:X16}", (ulong)m_lock_ctx);
                CLocalMemoryStorage.CThreadContextDeallocate(m_lock_ctx);
                m_lock_ctx = null;
            }
        }
    }
}

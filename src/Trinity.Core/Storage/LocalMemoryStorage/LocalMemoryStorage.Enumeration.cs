// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Storage
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 22)]
    internal unsafe struct LOCAL_MEMORY_STORAGE_ENUMERATOR
    {
        internal byte*   CellPtr;
        internal long    CellId;
        internal int     CellEntryIndex;
        internal ushort  CellType;
        /* Members invisible to C# are omitted. */
    }

    /// <summary>
    /// Represents the storage information for a cell.
    /// </summary>
    public unsafe struct CellInfo
    {
        /// <summary>
        /// Get a pointer to the content of the cell. Extra care should be taken
        /// when manipulating directly on the cell buffer, as no boundary checks
        /// are employed. Improper operations on the cell buffer will cause
        /// data corruption and system crash.
        /// </summary>
        public byte*   CellPtr { get; internal set;}
        /// <summary>
        /// Get the id of the cell.
        /// </summary>
        public long    CellId { get; internal set;}

        /// <summary>
        /// The index of the underlying hash slot correspondig to the current cell.
        /// </summary>
        public int     CellEntryIndex{ get; internal set;}
        /// <summary>
        /// Get the type of the current cell. Not available when type system not enabled.
        /// </summary>
        public ushort  CellType{ get; internal set;}
        /// <summary>
        /// Gets the size of current cell in bytes.
        /// </summary>
        public int CellSize
        {
            get
            {
                int size;
                if (TrinityErrorCode.E_SUCCESS == CLocalMemoryStorage.CLockedGetCellSize(CellId, CellEntryIndex, out size))
                    return size;
                throw new Exception("Failed to get cell size");
            }
        }

        internal unsafe CellInfo(LOCAL_MEMORY_STORAGE_ENUMERATOR* m_enumerator) : this()
        {
            CellPtr        = m_enumerator->CellPtr;
            CellId         = m_enumerator->CellId;
            CellEntryIndex = m_enumerator->CellEntryIndex;
            CellType       = m_enumerator->CellType;
        }

    }

    public unsafe partial class LocalMemoryStorage : Storage, IDisposable, IEnumerable<CellInfo>, IEnumerable
    {
        /// <summary>
        /// Returns an enumerator for all the cells stored in the local memory storage.
        /// </summary>
        /// <returns>An cell enumerator.</returns>
        public IEnumerator<CellInfo> GetEnumerator()
        {
            return new LocalMemoryStorageEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

    internal unsafe class LocalMemoryStorageEnumerator : IEnumerator<CellInfo>
    {
        LOCAL_MEMORY_STORAGE_ENUMERATOR *m_enumerator;

        public unsafe LocalMemoryStorageEnumerator()
        {
            if (TrinityErrorCode.E_SUCCESS != CLocalMemoryStorage.CLocalMemoryStorageEnumeratorAllocate(out m_enumerator))
            {
                throw new Exception("Failed to allocate local memory storage enumerator");
            }
        }

        public CellInfo Current
        {
            get { return new CellInfo(m_enumerator); }
        }

        void IDisposable.Dispose()
        {
            if (TrinityErrorCode.E_SUCCESS != CLocalMemoryStorage.CLocalMemoryStorageEnumeratorDeallocate(m_enumerator))
            {
                throw new Exception("Failed to deallocate local memory storage enumerator");
            }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        bool System.Collections.IEnumerator.MoveNext()
        {
            TrinityErrorCode err;
            do { err = CLocalMemoryStorage.CLocalMemoryStorageEnumeratorMoveNext(m_enumerator); }
            while (err != TrinityErrorCode.E_CELL_NOT_FOUND);
            if (err == TrinityErrorCode.E_ENUMERATION_END) return false;
            if (err == TrinityErrorCode.E_CELL_LOCK_OVERFLOW) throw new CellLockOverflowException(0);
            if (err == TrinityErrorCode.E_DEADLOCK) throw new DeadlockException();
            else return (err == TrinityErrorCode.E_SUCCESS);
        }

        void System.Collections.IEnumerator.Reset()
        {
            if (TrinityErrorCode.E_SUCCESS != CLocalMemoryStorage.CLocalMemoryStorageEnumeratorReset(m_enumerator))
            {
                throw new Exception("Failed to reset local memory storage enumerator");
            }
        }
    }

}

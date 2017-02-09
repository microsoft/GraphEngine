// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Trinity.Utilities;
using Trinity.Diagnostics;
using Trinity;
using Trinity.Daemon;
using System.Diagnostics;
using Trinity.Core.Lib;
using System.Runtime.CompilerServices;
using Trinity.TSL.Lib;
using System.Security;

namespace Trinity.Storage
{
    public unsafe partial class LocalMemoryStorage
    {
        /// <summary>
        /// Logs a cell action to the persistent storage.
        /// </summary>
        /// <param name="cellId">The 64-bit cell id.</param>
        /// <param name="cellPtr">A pointer pointing to the underlying cell buffer.</param>
        /// <param name="cellSize">The size of the cell in bytes.</param>
        /// <param name="cellType">A 16-bit unsigned integer indicating the cell type.</param>
        /// <param name="options">An flag indicating a cell access option.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void CWriteAheadLog(long cellId, byte* cellPtr, int cellSize, ushort cellType, CellAccessOptions options)
        {
            CLocalMemoryStorage.CWriteAheadLog(cellId, cellPtr, cellSize, cellType, options);
        }

    }

    /// <summary>
    /// InternalCall bindings for LocalMemoryStorage.
    /// </summary>
    internal unsafe class CLocalMemoryStorage
    {

        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern TrinityErrorCode CInitialize();
        

        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern ulong CCellCount();
        

        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool CResetStorage();
        

        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void CDispose();
        

        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern byte* CResizeCell(long cell_id, int cellEntryIndex, int offset, int delta);
        
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool CSaveStorage();
        
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool CLoadStorage();
        

        #region Cell operations
        // Non-logging cell operations
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern TrinityErrorCode CSaveCell(long cellId, byte* buff, int size, ushort cellType);
        
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern TrinityErrorCode CAddCell(long cellId, byte* buff, int size, ushort cellType);
        
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern TrinityErrorCode CUpdateCell(long cellId, byte* buff, int size);
        
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern TrinityErrorCode CRemoveCell(long cellId);
        
        ////////////////////////////////////////////

        // Logging cell opeartions
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern TrinityErrorCode CSaveCell(long cellId, byte* buff, int size, ushort cellType, CellAccessOptions options);
        
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern TrinityErrorCode CAddCell(long cellId, byte* buff, int size, ushort cellType, CellAccessOptions options);
        
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern TrinityErrorCode CUpdateCell(long cellId, byte* buff, int size, CellAccessOptions options);
        
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern TrinityErrorCode CRemoveCell(long cellId, CellAccessOptions options);
        
        #endregion

        #region GetLockedCellInfo
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern TrinityErrorCode CGetLockedCellInfo4SaveCell(long cellId, int size, ushort type, out byte* cellPtr, out int entryIndex);

        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern TrinityErrorCode CGetLockedCellInfo4AddCell(long cellId, int size, ushort type, out byte* cellPtr, out int entryIndex);

        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern TrinityErrorCode CGetLockedCellInfo4UpdateCell(long cellId, int size, out byte* cellPtr, out int entryIndex);
        
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern TrinityErrorCode CGetLockedCellInfo4LoadCell(long cellId, out int size, out byte* cellPtr, out int entryIndex);
        
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern TrinityErrorCode CGetLockedCellInfo4CellAccessor(long cellId, out int size, out ushort type, out byte* cellPtr, out int entryIndex);
        
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern TrinityErrorCode CGetLockedCellInfo4AddOrUseCell(long cellId, ref int size, ushort type, out byte* cellPtr, out int entryIndex);
        
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern TrinityErrorCode CGetCellType(long cellId, out ushort cellType);


        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void CReleaseCellLock(long cellId, int entryIndex);
        
        #endregion

        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool CContains(long cellId);
        
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern ulong CTrunkCommittedMemorySize();
        
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern ulong CMTHashCommittedMemorySize();
        
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern ulong CTotalCommittedMemorySize();
        
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern ulong CTotalCellSize();
        

        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern TrinityErrorCode CLocalMemoryStorageEnumeratorAllocate(out LOCAL_MEMORY_STORAGE_ENUMERATOR* p_enum);
        

        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern TrinityErrorCode CLocalMemoryStorageEnumeratorDeallocate(LOCAL_MEMORY_STORAGE_ENUMERATOR* p_enum);
        

        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern TrinityErrorCode CLocalMemoryStorageEnumeratorMoveNext(LOCAL_MEMORY_STORAGE_ENUMERATOR* p_enum);
        

        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern TrinityErrorCode CLocalMemoryStorageEnumeratorReset(LOCAL_MEMORY_STORAGE_ENUMERATOR* p_enum);
        

        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern TrinityErrorCode CLockedGetCellSize(long cellId, int entryIndex, out int size);
        

        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern TrinityErrorCode CGetTrinityImageSignature(TRINITY_IMAGE_SIGNATURE* pSignature);
        
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal unsafe static extern void CWriteAheadLog(long cellId, byte* cellPtr, int cellSize, ushort cellType, CellAccessOptions options);


        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal unsafe static extern void CSetWriteAheadLogFile(void* fp);
        

        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern TrinityErrorCode CStartDebugger(bool suspendOtherThreads);
        
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetDefragmentationPaused(bool value);
        

        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void RestartDefragmentation();
        

    }
}

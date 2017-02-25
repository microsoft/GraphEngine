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
using System.Runtime.InteropServices;

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

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern TrinityErrorCode CInitialize();


#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern ulong CCellCount();


#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern bool CResetStorage();


#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern void CDispose();


#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern byte* CResizeCell(long cell_id, int cellEntryIndex, int offset, int delta);

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern bool CSaveStorage();

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern bool CLoadStorage();


        #region Cell operations
        // Non-logging cell operations
#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern TrinityErrorCode CSaveCell(long cellId, byte* buff, int size, ushort cellType);

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern TrinityErrorCode CAddCell(long cellId, byte* buff, int size, ushort cellType);

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern TrinityErrorCode CUpdateCell(long cellId, byte* buff, int size);

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern TrinityErrorCode CRemoveCell(long cellId);

        ////////////////////////////////////////////

        // Logging cell opeartions
#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern TrinityErrorCode CLoggedSaveCell(long cellId, byte* buff, int size, ushort cellType, CellAccessOptions options);

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern TrinityErrorCode CLoggedAddCell(long cellId, byte* buff, int size, ushort cellType, CellAccessOptions options);

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern TrinityErrorCode CLoggedUpdateCell(long cellId, byte* buff, int size, CellAccessOptions options);

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern TrinityErrorCode CLoggedRemoveCell(long cellId, CellAccessOptions options);

        #endregion

        #region GetLockedCellInfo
#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern TrinityErrorCode CGetLockedCellInfo4SaveCell(long cellId, int size, ushort type, out byte* cellPtr, out int entryIndex);

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern TrinityErrorCode CGetLockedCellInfo4AddCell(long cellId, int size, ushort type, out byte* cellPtr, out int entryIndex);

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern TrinityErrorCode CGetLockedCellInfo4UpdateCell(long cellId, int size, out byte* cellPtr, out int entryIndex);

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern TrinityErrorCode CGetLockedCellInfo4LoadCell(long cellId, out int size, out byte* cellPtr, out int entryIndex);

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern TrinityErrorCode CGetLockedCellInfo4CellAccessor(long cellId, out int size, out ushort type, out byte* cellPtr, out int entryIndex);

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern TrinityErrorCode CGetLockedCellInfo4AddOrUseCell(long cellId, ref int size, ushort type, out byte* cellPtr, out int entryIndex);

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern TrinityErrorCode CGetCellType(long cellId, out ushort cellType);


#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern void CReleaseCellLock(long cellId, int entryIndex);

        #endregion

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern bool CContains(long cellId);

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern ulong CTrunkCommittedMemorySize();

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern ulong CMTHashCommittedMemorySize();

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern ulong CTotalCommittedMemorySize();

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern ulong CTotalCellSize();


#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern TrinityErrorCode CLocalMemoryStorageEnumeratorAllocate(out LOCAL_MEMORY_STORAGE_ENUMERATOR* p_enum);


#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern TrinityErrorCode CLocalMemoryStorageEnumeratorDeallocate(LOCAL_MEMORY_STORAGE_ENUMERATOR* p_enum);


#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern TrinityErrorCode CLocalMemoryStorageEnumeratorMoveNext(LOCAL_MEMORY_STORAGE_ENUMERATOR* p_enum);


#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern TrinityErrorCode CLocalMemoryStorageEnumeratorReset(LOCAL_MEMORY_STORAGE_ENUMERATOR* p_enum);


#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern TrinityErrorCode CLockedGetCellSize(long cellId, int entryIndex, out int size);


#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern TrinityErrorCode CGetTrinityImageSignature(TRINITY_IMAGE_SIGNATURE* pSignature);

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal unsafe static extern void CWriteAheadLog(long cellId, byte* cellPtr, int cellSize, ushort cellType, CellAccessOptions options);


#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal unsafe static extern void CSetWriteAheadLogFile(void* fp);


#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern TrinityErrorCode CStartDebugger(bool suspendOtherThreads);

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern void SetDefragmentationPaused(bool value);


#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern void RestartDefragmentation();


    }
}

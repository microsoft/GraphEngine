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
    /// <summary>
    /// PInvoke bindings for LocalMemoryStorage.
    /// </summary>
    internal unsafe class CLocalMemoryStorage
    {

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode CInitialize();


        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern ulong CCellCount();


        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode CResetStorage();


        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void CDispose();


        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode CResizeCell(long cell_id, int cellEntryIndex, int offset, int delta, out byte* cellPtr);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode CSaveStorage();

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode CLoadStorage();

        [DllImport(TrinityC.AssemblyName, CharSet = CharSet.Unicode)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode LoadIncrementalDiskImage(string trunkfile, string hashfile, string lofile);


        #region Cell operations
        // Non-logging cell operations
        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode CSaveCell(long cellId, byte* buff, int size, ushort cellType);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode CAddCell(long cellId, byte* buff, int size, ushort cellType);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode CUpdateCell(long cellId, byte* buff, int size);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode CRemoveCell(long cellId);

        ////////////////////////////////////////////

        // Logging cell operations
        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode CLoggedSaveCell(long cellId, byte* buff, int size, ushort cellType, CellAccessOptions options);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode CLoggedAddCell(long cellId, byte* buff, int size, ushort cellType, CellAccessOptions options);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode CLoggedUpdateCell(long cellId, byte* buff, int size, CellAccessOptions options);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode CLoggedRemoveCell(long cellId, CellAccessOptions options);

        #endregion

        #region GetLockedCellInfo
        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode CGetLockedCellInfo4LoadCell(long cellId, out int size, out byte* cellPtr, out int entryIndex);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode CGetLockedCellInfo4CellAccessor(long cellId, out int size, out ushort type, out byte* cellPtr, out int entryIndex);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode CGetLockedCellInfo4AddOrUseCell(long cellId, ref int size, ushort type, out byte* cellPtr, out int entryIndex);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode CGetCellType(long cellId, out ushort cellType);


        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void CReleaseCellLock(long cellId, int entryIndex);

        #endregion

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode CContains(long cellId);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern ulong CTrunkCommittedMemorySize();

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern ulong CMTHashCommittedMemorySize();

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern ulong CTotalCommittedMemorySize();

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern ulong CTotalCellSize();


        #region Enumerator
        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode CLocalMemoryStorageEnumeratorAllocate(out LOCAL_MEMORY_STORAGE_ENUMERATOR* p_enum);


        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode CLocalMemoryStorageEnumeratorDeallocate(LOCAL_MEMORY_STORAGE_ENUMERATOR* p_enum);


        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode CLocalMemoryStorageEnumeratorMoveNext(LOCAL_MEMORY_STORAGE_ENUMERATOR* p_enum);


        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode CLocalMemoryStorageEnumeratorReset(LOCAL_MEMORY_STORAGE_ENUMERATOR* p_enum);
        #endregion

        #region ThreadContext
        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void* CThreadContextAllocate();

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void CThreadContextSet(void* ctx);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void CThreadContextDeallocate(void* ctx);
        #endregion


        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode CLockedGetCellSize(long cellId, int entryIndex, out int size);


        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode CGetTrinityImageSignature(TRINITY_IMAGE_SIGNATURE* pSignature);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal unsafe static extern void CWriteAheadLog(long cellId, byte* cellPtr, int cellSize, ushort cellType, CellAccessOptions options);


        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal unsafe static extern void CSetWriteAheadLogFile(void* fp);


        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode CStartDebugger(bool suspendOtherThreads);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void SetDefragmentationPaused(bool value);


        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void RestartDefragmentation();


        [DllImport(TrinityC.AssemblyName)]
        internal static extern char* CGetStorageSlot(int isPrimary);

        #region Tx

        #region Cell operations
        // Non-logging cell operations
        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode TxCSaveCell(void* ctx, long cellId, byte* buff, int size, ushort cellType);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode TxCAddCell(void* ctx, long cellId, byte* buff, int size, ushort cellType);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode TxCUpdateCell(void* ctx, long cellId, byte* buff, int size);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode TxCRemoveCell(void* ctx, long cellId);

        ////////////////////////////////////////////

        // Logging cell operations
        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode TxCLoggedSaveCell(void* ctx, long cellId, byte* buff, int size, ushort cellType, CellAccessOptions options);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode TxCLoggedAddCell(void* ctx, long cellId, byte* buff, int size, ushort cellType, CellAccessOptions options);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode TxCLoggedUpdateCell(void* ctx, long cellId, byte* buff, int size, CellAccessOptions options);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode TxCLoggedRemoveCell(void* ctx, long cellId, CellAccessOptions options);

        #endregion

        #region GetLockedCellInfo
        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode TxCGetLockedCellInfo4LoadCell(void* ctx, long cellId, out int size, out byte* cellPtr, out int entryIndex);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode TxCGetLockedCellInfo4CellAccessor(void* ctx, long cellId, out int size, out ushort type, out byte* cellPtr, out int entryIndex);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode TxCGetLockedCellInfo4AddOrUseCell(void* ctx, long cellId, ref int size, ushort type, out byte* cellPtr, out int entryIndex);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode TxCResizeCell(void* ctx, long cell_id, int cellEntryIndex, int offset, int delta, out byte* cellPtr);


        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern TrinityErrorCode TxCGetCellType(void* ctx, long cellId, out ushort cellType);


        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void TxCReleaseCellLock(void* ctx, long cellId, int entryIndex);

        #endregion

        #endregion
    }
}

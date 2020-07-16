// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
//#define _TWO_PHASE_CELL_MANIPULATION_
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Trinity.Configuration;
using Trinity.Core.Lib;

namespace Trinity.Storage
{
    public unsafe partial class LocalMemoryStorage : IStorage
    {
        /// <summary>
        /// Releases the cell lock associated with the current cell.
        /// Do not call this method to release a cell that is not acquired
        /// by this thread or task.
        /// </summary>
        /// <param name="cellId">A 64-bit cell id.</param>
        /// <param name="entryIndex">The hash slot index corresponding to the current cell.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReleaseCellLock(long cellId, int entryIndex)
        {
            // TODO: in Trinity.C we should check whether the cell is locked by the current thread ctx.
            // TODO: add error code. return E_CELL_NOT_FOUND when the cell is not possessed by the current
            // thread.
            CLocalMemoryStorage.CReleaseCellLock(cellId, entryIndex);
        }

        /// <summary>
        /// Locks the current cell and gets its underlying storage information.
        /// </summary>
        /// <param name="cellId">A 64-bit cell id.</param>
        /// <param name="size">The size of the cell in bytes.</param>
        /// <param name="type">A 16-bit unsigned integer indicating the cell type.</param>
        /// <param name="cellPtr">A pointer pointing to the underlying cell buffer.</param>
        /// <param name="entryIndex">The hash slot index corresponding to the current cell.</param>
        /// <returns><c>TrinityErrorCode.E_SUCCESS</c> if the operation succeeds; <c>TrinityErrorCode.E_CELL_NOT_FOUND</c> if the specified cell is not found. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode GetLockedCellInfo(long cellId, out int size, out ushort type, out byte* cellPtr, out int entryIndex)
        {
            TrinityErrorCode eResult = CLocalMemoryStorage.CGetLockedCellInfo4CellAccessor(cellId, out size, out type, out cellPtr, out entryIndex);
            return eResult;
        }

        /// <summary>
        /// Adds a new cell with the specified cell id or uses the cell if a cell with the specified cell id already exists.
        /// </summary>
        /// <param name="cellId">A 64-bit cell id.</param>
        /// <param name="cellBuff">The buffer of the cell to be added.</param>
        /// <param name="size">The size of the cell to be added. It will be overwritten to the size of the existing cell with specified cell id if it exists.</param>
        /// <param name="cellType">A 16-bit unsigned integer indicating the cell type.</param>
        /// <param name="cellPtr">A pointer pointing to the underlying cell buffer.</param>
        /// <param name="cellEntryIndex">The hash slot index corresponding to the current cell.</param>
        /// <returns><c>TrinityErrorCode.E_CELL_FOUND</c> if the specified cell is found; <c>TrinityErrorCode.E_WRONG_CELL_TYPE</c> if the cell type of the cell with the specified cellId does not match the specified cellType; <c>TrinityErrorCode.E_CELL_NOT_FOUND</c> if the specified cell is not found.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode AddOrUse(long cellId, byte[] cellBuff, ref int size, ushort cellType, out byte* cellPtr, out int cellEntryIndex)
        {
            TrinityErrorCode eResult = CLocalMemoryStorage.CGetLockedCellInfo4AddOrUseCell(cellId, ref size, cellType, out cellPtr, out cellEntryIndex);
            if (eResult == TrinityErrorCode.E_CELL_NOT_FOUND)
            {
                Memory.Copy(cellBuff, cellPtr, size);
            }
            return eResult;
        }

        /// <summary>
        /// Resizes the cell with the specified cell id.
        /// Do not call this method to resize a cell that is not locked
        /// by this thread/task.
        /// </summary>
        /// <param name="cell_id">A 64-bit cell id.</param>
        /// <param name="cellEntryIndex">The hash slot index corresponding to the current cell.</param>
        /// <param name="offset">The offset starting which the underlying storage needs to expand or shrink.</param>
        /// <param name="delta">The size to expand or shrink, in bytes.</param>
        /// <returns>The pointer pointing to the underlying cell buffer after resizing.</returns>
        /// <exception cref="System.OutOfMemoryException">Resize fails because the system runs out of memory.</exception>
        /// <exception cref="System.ArgumentException">Resize fails because the given parameters are not valid.</exception>
        /// <exception cref="System.Exception">Resize fails because of other errors.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte* ResizeCell(long cell_id, int cellEntryIndex, int offset, int delta)
        {
            byte* cellPtr;
            TrinityErrorCode code = CLocalMemoryStorage.CResizeCell(cell_id, cellEntryIndex, offset, delta, out cellPtr);
            if (code == TrinityErrorCode.E_SUCCESS) return cellPtr;

            string err_msg = "ResizeCell encountered an error.";
            if (code == TrinityErrorCode.E_NOMEM) throw new OutOfMemoryException(err_msg);
            if (code == TrinityErrorCode.E_INVALID_ARGUMENTS) throw new ArgumentException(err_msg);
            throw new Exception(err_msg);
        }

        /// <summary>
        /// Resizes the cell with the specified cell id.
        /// </summary>
        /// <param name="cell_id">A 64-bit cell id.</param>
        /// <param name="cellEntryIndex">The hash slot index corresponding to the current cell.</param>
        /// <param name="offset">The offset starting which the underlying storage needs to expand or shrink.</param>
        /// <param name="delta">The size to expand or shrink, in bytes.</param>
        /// <param name="cellPtr">The pointer pointing to the underlying cell buffer after resizing.</param>
        /// <returns>The status code, E_SUCCESS for a succeeded resize operation. When the operation does not complete successfully, the original cell pointer and the content are not affected, but the out parameter cellPtr is undefined.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode ResizeCell(long cell_id, int cellEntryIndex, int offset, int delta, out byte* cellPtr)
        {
            return CLocalMemoryStorage.CResizeCell(cell_id, cellEntryIndex, offset, delta, out cellPtr);
        }

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="cellType">Indicates the cell type.</param>
        /// <returns><c>TrinityErrorCode.E_SUCCESS</c> if saving succeeds; otherwise, an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode SaveCell(long cellId, byte[] buff, ushort cellType)
        {
            fixed (byte* p = buff)
            {
                return SaveCell(cellId, p, buff.Length, cellType);
            }
        }

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="cellSize">The size of the cell.</param>
        /// <param name="cellType">Indicates the cell type.</param>
        /// <returns><c>TrinityErrorCode.E_SUCCESS</c> if saving succeeds; otherwise, an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode SaveCell(long cellId, byte* buff, int cellSize, ushort cellType)
        {
            TrinityErrorCode eResult = CLocalMemoryStorage.CSaveCell(cellId, buff, cellSize, cellType);
            return eResult;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<TrinityErrorCode> SaveCellAsync(long cellId, byte* buff, int cellSize, ushort cellType)
        {
            return Task.FromResult(SaveCell(cellId, buff, cellSize, cellType));
        }

        /// <summary>
        /// Adds a new cell to the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="cellSize">The size of the cell.</param>
        /// <param name="cellType">Indicates the cell type.</param>
        /// <returns><c>TrinityErrorCode.E_SUCCESS</c> if adding succeeds; otherwise, an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode AddCell(long cellId, byte* buff, int cellSize, ushort cellType)
        {
            TrinityErrorCode eResult = CLocalMemoryStorage.CAddCell(cellId, buff, cellSize, cellType);
            return eResult;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<TrinityErrorCode> AddCellAsync(long cellId, byte* buff, int cellSize, ushort cellType)
        {
            return Task.FromResult(AddCell(cellId, buff, cellSize, cellType));
        }

        /// <summary>
        /// Updates an existing cell in the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="cellSize">The size of the cell.</param>
        /// <returns><c>TrinityErrorCode.E_SUCCESS</c> if updating succeeds; otherwise, an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode UpdateCell(long cellId, byte* buff, int cellSize)
        {
            TrinityErrorCode eResult = CLocalMemoryStorage.CUpdateCell(cellId, buff, cellSize);
            return eResult;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<TrinityErrorCode> UpdateCellAsync(long cellId, byte* buff, int cellSize)
        {
            return Task.FromResult(UpdateCell(cellId, buff, cellSize));
        }

        /// <summary>
        /// Loads the bytes of the cell with the specified cell Id.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="cellBuff">The bytes of the cell. An empty byte array is returned if the cell is not found.</param>
        /// <param name="cellType">The type of the cell, represented with a 16-bit unsigned integer.</param>
        /// <returns>A Trinity error code. Possible values are E_SUCCESS and E_NOT_FOUND.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LoadCellResponse LoadCell(long cellId)
        {
            byte[] cellBuff;
            ushort cellType;
            int index, cellSize;
            byte* cellPtr = null;
            TrinityErrorCode eResult = CLocalMemoryStorage.CGetLockedCellInfo4CellAccessor(cellId, out cellSize, out cellType, out cellPtr, out index);
            if (eResult == TrinityErrorCode.E_CELL_NOT_FOUND)
            {
                cellBuff = Array.Empty<byte>();
                cellType = StorageConfig.c_UndefinedCellType;
                return new LoadCellResponse(eResult, cellBuff, cellType);
            }
            cellBuff = new byte[cellSize];
            Memory.Copy(cellPtr, 0, cellBuff, 0, cellSize);
            CLocalMemoryStorage.CReleaseCellLock(cellId, index);
            return new LoadCellResponse(TrinityErrorCode.E_SUCCESS, cellBuff, cellType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<LoadCellResponse> LoadCellAsync(long cellId)
        {
            return Task.FromResult(LoadCell(cellId));
        }

        /// <summary>
        /// Loads the bytes of the cell with the specified cell Id.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="cellBuff">The bytes of the cell. An empty byte array is returned if the cell is not found.</param>
        /// <param name="cellType">The type of the cell, represented with a 16-bit unsigned integer.</param>
        /// <returns>A Trinity error code. Possible values are E_SUCCESS and E_NOT_FOUND.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LoadCellUnsafeResponse LoadCellUnsafe(long cellId)
        {
            byte* cellBuff;
            int size;
            ushort cellType;
            int index;
            byte* cellPtr = null;
            TrinityErrorCode eResult = CLocalMemoryStorage.CGetLockedCellInfo4CellAccessor(cellId, out size, out cellType, out cellPtr, out index);
            if (eResult == TrinityErrorCode.E_CELL_NOT_FOUND)
            {
                cellBuff = null;
                size = -1;
                cellType = StorageConfig.c_UndefinedCellType;
                return new LoadCellUnsafeResponse(eResult, cellBuff, size, cellType);
            }
            cellBuff = (byte*)Memory.malloc((ulong)size);
            Memory.Copy(cellPtr, 0, cellBuff, 0, size);
            CLocalMemoryStorage.CReleaseCellLock(cellId, index);
            return new LoadCellUnsafeResponse(TrinityErrorCode.E_SUCCESS, cellBuff, size, cellType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<LoadCellUnsafeResponse> LoadCellUnsafeAsync(long cellId)
        {
            return Task.FromResult(LoadCellUnsafe(cellId));
        }

        /// <summary>
        /// Removes the cell with the specified cell Id from the key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns><c>TrinityErrorCode.E_SUCCESS</c> if removing succeeds; otherwise, an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode RemoveCell(long cellId)
        {
            TrinityErrorCode eResult = CLocalMemoryStorage.CRemoveCell(cellId);
            return eResult;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<TrinityErrorCode> RemoveCellAsync(long cellId)
        {
            return Task.FromResult(RemoveCell(cellId));
        }

        /// <summary>
        /// Gets the type of the cell with specified cell Id.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="cellType">The type of the cell specified by cellId.</param>
        /// <returns>A Trinity error code. Possible values are E_SUCCESS and E_NOT_FOUND.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode GetCellType(long cellId, out ushort cellType)
        {
            TrinityErrorCode eResult = CLocalMemoryStorage.CGetCellType(cellId, out cellType);
            return eResult;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<(TrinityErrorCode ErrorCode, ushort CellType)> GetCellTypeAsync(long cellId)
        {
            TrinityErrorCode eResult = GetCellType(cellId, out ushort cellType);
            return Task.FromResult((eResult, cellType));
        }

        /// <summary>
        /// Determines whether there is a cell with the specified cell Id in Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell id.</param>
        /// <returns>true if a cell whose Id is cellId is found; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(long cellId)
        {
            TrinityErrorCode eResult = CLocalMemoryStorage.CContains(cellId);
            switch (eResult)
            {
                case TrinityErrorCode.E_CELL_FOUND:
                    return true;
                case TrinityErrorCode.E_DEADLOCK:
                    throw new DeadlockException();
                case TrinityErrorCode.E_CELL_NOT_FOUND:
                default:
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> ContainsAsync(long cellId)
        {
            return Task.FromResult(Contains(cellId));
        }

        /// <summary>
        /// Gets the size of the current cell. The caller must hold the cell lock before calling this function.
        /// </summary>
        /// <param name="cellId">A 64-bit cell id.</param>
        /// <param name="entryIndex">The hash slot index corresponding to the current cell.</param>
        /// <param name="size">The size of the specified cell.</param>
        /// <returns>A Trinity error code. This function always returns E_SUCCESS.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode LockedGetCellSize(long cellId, int entryIndex, out int size)
        {
            return CLocalMemoryStorage.CLockedGetCellSize(cellId, entryIndex, out size);
        }
    }
}

// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
//#define _TWO_PHASE_CELL_MANIPULATION_
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

using Trinity;
using System.Runtime.CompilerServices;
using Trinity.Core.Lib;

namespace Trinity.Storage
{
    public unsafe partial class LocalMemoryStorage : Storage
    {
        /// <summary>
        /// Releases the cell lock associated with the current cell.
        /// </summary>
        /// <param name="cellId">A 64-bit cell id.</param>
        /// <param name="entryIndex">The hash slot index corresponding to the current cell.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReleaseCellLock(long cellId, int entryIndex)
        {
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
            return CLocalMemoryStorage.CGetLockedCellInfo4CellAccessor(cellId, out size, out type, out cellPtr, out entryIndex);
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
            var eResult = CLocalMemoryStorage.CGetLockedCellInfo4AddOrUseCell(cellId, ref size, cellType, out cellPtr, out cellEntryIndex);
            if (eResult == TrinityErrorCode.E_CELL_NOT_FOUND)
            {
                Memory.Copy(cellBuff, cellPtr, size);
            }
            return eResult;
        }

        /// <summary>
        /// Resizes the cell with the specified cell id.
        /// </summary>
        /// <param name="cell_id">A 64-bit cell id.</param>
        /// <param name="cellEntryIndex">The hash slot index corresponding to the current cell.</param>
        /// <param name="offset">The offset starting which the underlying storage needs to expand or shrink.</param>
        /// <param name="delta">The size to expand or shrink, in bytes.</param>
        /// <returns>The pointer pointing to the underlying cell buffer after resizing.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte* ResizeCell(long cell_id, int cellEntryIndex, int offset, int delta)
        {
            return CLocalMemoryStorage.CResizeCell(cell_id, cellEntryIndex, offset, delta);
        }

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <returns><c>TrinityErrorCode.E_SUCCESS</c> if saving succeeds; otherwise, an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override TrinityErrorCode SaveCell(long cellId, byte[] buff)
        {
#if _TWO_PHASE_CELL_MANIPULATION_
            byte* cellPtr;
            int entryIndex;
            CGetLockedCellInfo4SaveCell(cellId, buff.Length, TrinityConfig.UndefinedCellType, out cellPtr, out entryIndex);
            Memory.Copy(buff, cellPtr, buff.Length);
            CReleaseCellLock(cellId, entryIndex);
            return TrinityErrorCode.E_SUCCESS;
#else
            fixed (byte* p = buff)
            {
                return CLocalMemoryStorage.CSaveCell(cellId, p, buff.Length, ushort.MaxValue);
            }
#endif
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
#if _TWO_PHASE_CELL_MANIPULATION_
            byte* cellPtr;
            int entryIndex;
            CGetLockedCellInfo4SaveCell(cellId, buff.Length, cellType, out cellPtr, out entryIndex);
            Memory.Copy(buff, cellPtr, buff.Length);
            CReleaseCellLock(cellId, entryIndex);
            return TrinityErrorCode.E_SUCCESS;
#else
            fixed (byte* p = buff)
            {
                return CLocalMemoryStorage.CSaveCell(cellId, p, buff.Length, cellType);
            }
#endif
        }

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="offset">The byte offset into the buff.</param>
        /// <param name="cellSize">The size of the cell.</param>
        /// <returns><c>TrinityErrorCode.E_SUCCESS</c> if saving succeeds; otherwise, an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override TrinityErrorCode SaveCell(long cellId, byte[] buff, int offset, int cellSize)
        {
#if _TWO_PHASE_CELL_MANIPULATION_
            byte* cellPtr;
            int entryIndex;
            CGetLockedCellInfo4SaveCell(cellId, cellSize, TrinityConfig.UndefinedCellType, out cellPtr, out entryIndex);
            Memory.Copy(buff, offset, cellPtr, cellSize);
            CReleaseCellLock(cellId, entryIndex);
            return TrinityErrorCode.E_SUCCESS;
#else
            fixed (byte* p = buff)
            {
                return CLocalMemoryStorage.CSaveCell(cellId, p + offset, cellSize, ushort.MaxValue);
            }
#endif
        }

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="offset">The byte offset into the buff.</param>
        /// <param name="cellSize">The size of the cell.</param>
        /// <param name="cellType">Indicates the cell type.</param>
        /// <returns><c>TrinityErrorCode.E_SUCCESS</c> if saving succeeds; otherwise, an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override TrinityErrorCode SaveCell(long cellId, byte[] buff, int offset, int cellSize, ushort cellType)
        {
#if _TWO_PHASE_CELL_MANIPULATION_
            byte* cellPtr;
            int entryIndex;
            CGetLockedCellInfo4SaveCell(cellId, cellSize, cellType, out cellPtr, out entryIndex);
            Memory.Copy(buff, offset, cellPtr, cellSize);
            CReleaseCellLock(cellId, entryIndex);
            return TrinityErrorCode.E_SUCCESS;
#else
            fixed (byte* p = buff)
            {
                return CLocalMemoryStorage.CSaveCell(cellId, p + offset, cellSize, cellType);
            }
#endif

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
        public override TrinityErrorCode SaveCell(long cellId, byte* buff, int cellSize, ushort cellType)
        {
#if _TWO_PHASE_CELL_MANIPULATION_
            byte* cellPtr;
            int entryIndex;
            CGetLockedCellInfo4SaveCell(cellId, cellSize, cellType, out cellPtr, out entryIndex);
            Memory.Copy(buff, offset, cellPtr, 0, cellSize);
            CReleaseCellLock(cellId, entryIndex);
            return TrinityErrorCode.E_SUCCESS;
#else
            return CLocalMemoryStorage.CSaveCell(cellId, buff, cellSize, cellType);
#endif
        }

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="cellSize">The size of the cell.</param>
        /// <returns><c>TrinityErrorCode.E_SUCCESS</c> if saving succeeds; otherwise, an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override TrinityErrorCode SaveCell(long cellId, byte* buff, int cellSize)
        {
#if _TWO_PHASE_CELL_MANIPULATION_
            byte* cellPtr;
            int entryIndex;
            CGetLockedCellInfo4SaveCell(cellId, cellSize, TrinityConfig.UndefinedCellType, out cellPtr, out entryIndex);
            Memory.Copy(buff, cellPtr, cellSize);
            CReleaseCellLock(cellId, entryIndex);
            return TrinityErrorCode.E_SUCCESS;
#else
            return CLocalMemoryStorage.CSaveCell(cellId, buff, cellSize, ushort.MaxValue);
#endif
        }

        /// <summary>
        /// Adds a new cell to the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="cellSize">The size of the cell.</param>
        /// <returns><c>TrinityErrorCode.E_SUCCESS</c> if adding succeeds; otherwise, an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override TrinityErrorCode AddCell(long cellId, byte* buff, int cellSize)
        {
#if _TWO_PHASE_CELL_MANIPULATION_
            byte* cellPtr;
            int entryIndex;
            TrinityErrorCode eResult;

            if ((eResult = CGetLockedCellInfo4AddCell(cellId, cellSize, TrinityConfig.UndefinedCellType, out cellPtr, out entryIndex)) == TrinityErrorCode.E_DUPLICATED_CELL)
            {
                return eResult;
            }
            Memory.Copy(buff, cellPtr, cellSize);
            CReleaseCellLock(cellId, entryIndex);
            return TrinityErrorCode.E_SUCCESS;
#else
            return CLocalMemoryStorage.CAddCell(cellId, buff, cellSize, ushort.MaxValue);
#endif
        }

        /// <summary>
        /// Adds a new cell to the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <returns><c>TrinityErrorCode.E_SUCCESS</c> if adding succeeds; otherwise, an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override TrinityErrorCode AddCell(long cellId, byte[] buff)
        {
#if _TWO_PHASE_CELL_MANIPULATION_
            byte* cellPtr;
            int entryIndex;
            TrinityErrorCode eResult;

            if ((eResult = CGetLockedCellInfo4AddCell(cellId, buff.Length, TrinityConfig.UndefinedCellType, out cellPtr, out entryIndex)) == TrinityErrorCode.E_DUPLICATED_CELL)
            {
                return eResult;
            }
            Memory.Copy(buff, cellPtr, buff.Length);
            CReleaseCellLock(cellId, entryIndex);
            return TrinityErrorCode.E_SUCCESS;
#else
            fixed (byte* p = buff)
            {
                return CLocalMemoryStorage.CAddCell(cellId, p, buff.Length, ushort.MaxValue);
            }
#endif
        }

        /// <summary>
        /// Adds a new cell to the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="offset">The byte offset into the buff.</param>
        /// <param name="cellSize">The size of the cell.</param>
        /// <returns><c>TrinityErrorCode.E_SUCCESS</c> if adding succeeds; otherwise, an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override TrinityErrorCode AddCell(long cellId, byte[] buff, int offset, int cellSize)
        {
#if _TWO_PHASE_CELL_MANIPULATION_
            byte* cellPtr;
            int entryIndex;
            TrinityErrorCode eResult;

            if ((eResult = CGetLockedCellInfo4AddCell(cellId, cellSize, TrinityConfig.UndefinedCellType, out cellPtr, out entryIndex)) == TrinityErrorCode.E_DUPLICATED_CELL)
            {
                return eResult;
            }
            Memory.Copy(buff, offset, cellPtr, 0, cellSize);
            CReleaseCellLock(cellId, entryIndex);
            return TrinityErrorCode.E_SUCCESS;
#else
            fixed (byte* p = buff)
            {
                return CLocalMemoryStorage.CAddCell(cellId, p + offset, cellSize, ushort.MaxValue);
            }
#endif
        }

        /// <summary>
        /// Adds a new cell to the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="offset">The byte offset into the buff.</param>
        /// <param name="cellSize">The size of the cell.</param>
        /// <param name="cellType">Indicates the cell type.</param>
        /// <returns><c>TrinityErrorCode.E_SUCCESS</c> if adding succeeds; otherwise, an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override TrinityErrorCode AddCell(long cellId, byte[] buff, int offset, int cellSize, ushort cellType)
        {
#if _TWO_PHASE_CELL_MANIPULATION_
            byte* cellPtr;
            int entryIndex;
            TrinityErrorCode eResult;

            if ((eResult = CGetLockedCellInfo4AddCell(cellId, cellSize, cellType, out cellPtr, out entryIndex)) == TrinityErrorCode.E_DUPLICATED_CELL)
            {
                return eResult;
            }
            Memory.Copy(buff, offset, cellPtr, 0, cellSize);
            CReleaseCellLock(cellId, entryIndex);
            return TrinityErrorCode.E_SUCCESS;
#else
            fixed (byte* p = buff)
            {
                return CLocalMemoryStorage.CAddCell(cellId, p + offset, cellSize, cellType);
            }
#endif
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
        public override TrinityErrorCode AddCell(long cellId, byte* buff, int cellSize, ushort cellType)
        {
#if _TWO_PHASE_CELL_MANIPULATION_
            byte* cellPtr;
            int entryIndex;
            TrinityErrorCode eResult;

            if ((eResult = CGetLockedCellInfo4AddCell(cellId, cellSize, cellType, out cellPtr, out entryIndex)) == TrinityErrorCode.E_DUPLICATED_CELL)
            {
                return eResult;
            }
            Memory.Copy(buff, offset, cellPtr, 0, cellSize);
            CReleaseCellLock(cellId, entryIndex);
            return TrinityErrorCode.E_SUCCESS;
#else
            return CLocalMemoryStorage.CAddCell(cellId, buff, cellSize, cellType);
#endif
        }

        /// <summary>
        /// Updates an existing cell in the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="cellSize">The size of the cell.</param>
        /// <returns><c>TrinityErrorCode.E_SUCCESS</c> if updating succeeds; otherwise, an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override TrinityErrorCode UpdateCell(long cellId, byte* buff, int cellSize)
        {
#if _TWO_PHASE_CELL_MANIPULATION_
            byte* cellPtr;
            int entryIndex;
            TrinityErrorCode eResult;

            if ((eResult = CGetLockedCellInfo4UpdateCell(cellId, cellSize, out cellPtr, out entryIndex)) == TrinityErrorCode.E_CELL_NOT_FOUND)
            {
                return eResult;
            }
            Memory.Copy(buff, cellPtr, cellSize);
            CReleaseCellLock(cellId, entryIndex);
            return TrinityErrorCode.E_SUCCESS;
#else
            return CLocalMemoryStorage.CUpdateCell(cellId, buff, cellSize);
#endif
        }

        /// <summary>
        /// Updates an existing cell in the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <returns><c>TrinityErrorCode.E_SUCCESS</c> if updating succeeds; otherwise, an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override TrinityErrorCode UpdateCell(long cellId, byte[] buff)
        {
#if _TWO_PHASE_CELL_MANIPULATION_
            byte* cellPtr;
            int entryIndex;
            TrinityErrorCode eResult;

            if ((eResult = CGetLockedCellInfo4UpdateCell(cellId, buff.Length, out cellPtr, out entryIndex)) == TrinityErrorCode.E_CELL_NOT_FOUND)
            {
                return eResult;
            }
            Memory.Copy(buff, cellPtr, buff.Length);
            CReleaseCellLock(cellId, entryIndex);
            return TrinityErrorCode.E_SUCCESS;
#else
            fixed (byte* p = buff)
            {
                return CLocalMemoryStorage.CUpdateCell(cellId, p, buff.Length);
            }
#endif
        }

        /// <summary>
        /// Updates an existing cell in the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="offset">The byte offset into the buff.</param>
        /// <param name="cellSize">The size of the cell.</param>
        /// <returns><c>TrinityErrorCode.E_SUCCESS</c> if updating succeeds; otherwise, an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override TrinityErrorCode UpdateCell(long cellId, byte[] buff, int offset, int cellSize)
        {
#if _TWO_PHASE_CELL_MANIPULATION_
            byte* cellPtr;
            int entryIndex;
            TrinityErrorCode eResult;

            if ((eResult = CGetLockedCellInfo4UpdateCell(cellId, cellSize, out cellPtr, out entryIndex)) == TrinityErrorCode.E_CELL_NOT_FOUND)
            {
                return eResult;
            }
            Memory.Copy(buff, offset, cellPtr, 0, cellSize);
            CReleaseCellLock(cellId, entryIndex);
            return TrinityErrorCode.E_SUCCESS;
#else
            fixed (byte* p = buff)
            {
                return CLocalMemoryStorage.CUpdateCell(cellId, p + offset, cellSize);
            }
#endif
        }

        /// <summary>
        /// Loads the bytes of the cell with the specified cell Id.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="cellBuff">The bytes of the cell. An empty byte array is returned if the cell is not found.</param>
        /// <param name="cellType">The type of the cell, represented with a 16-bit unsigned integer.</param>
        /// <returns>A Trinity error code. Possible values are E_SUCCESS and E_NOT_FOUND.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override TrinityErrorCode LoadCell(long cellId, out byte[] cellBuff, out ushort cellType)
        {
            int index, cellSize;
            byte* cellPtr = null;
            TrinityErrorCode eResult;
            if ((eResult = CLocalMemoryStorage.CGetLockedCellInfo4CellAccessor(cellId, out cellSize, out cellType, out cellPtr, out index)) == TrinityErrorCode.E_CELL_NOT_FOUND)
            {
                cellBuff = new byte[0];
                cellType = TrinityConfig.UndefinedCellType;
                return eResult;
            }
            cellBuff = new byte[cellSize];
            Memory.Copy(cellPtr, 0, cellBuff, 0, cellSize);
            CLocalMemoryStorage.CReleaseCellLock(cellId, index);
            return TrinityErrorCode.E_SUCCESS;
        }
        
        /// <summary>
        /// Loads the bytes of the cell with the specified cell Id.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="cellBuff">The bytes of the cell. An empty byte array is returned if the cell is not found.</param>
        /// <returns>A Trinity error code. Possible values are E_SUCCESS and E_NOT_FOUND.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override TrinityErrorCode LoadCell(long cellId, out byte[] cellBuff)
        {
            int index, cellSize;
            byte* cellPtr = null;
            TrinityErrorCode eResult;
            if ((eResult = CLocalMemoryStorage.CGetLockedCellInfo4LoadCell(cellId, out cellSize, out cellPtr, out index)) == TrinityErrorCode.E_CELL_NOT_FOUND)
            {
                cellBuff = new byte[0];
                return eResult;
            }
            cellBuff = new byte[cellSize];
            Memory.Copy(cellPtr, 0, cellBuff, 0, cellSize);
            CLocalMemoryStorage.CReleaseCellLock(cellId, index);
            return TrinityErrorCode.E_SUCCESS;
        }

        /// <summary>
        /// Removes the cell with the specified cell Id from the key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns><c>TrinityErrorCode.E_SUCCESS</c> if removing succeeds; otherwise, an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override TrinityErrorCode RemoveCell(long cellId)
        {
            return CLocalMemoryStorage.CRemoveCell(cellId);
        }

        /// <summary>
        /// Gets the type of the cell with specified cell Id.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="cellType">The type of the cell specified by cellId.</param>
        /// <returns>A Trinity error code. Possible values are E_SUCCESS and E_NOT_FOUND.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override TrinityErrorCode GetCellType(long cellId, out ushort cellType)
        {
            return CLocalMemoryStorage.CGetCellType(cellId, out cellType);
        }

        /// <summary>
        /// Determines whether there is a cell with the specified cell Id in Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell id.</param>
        /// <returns>true if a cell whose Id is cellId is found; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Contains(long cellId)
        {
            return CLocalMemoryStorage.CContains(cellId);
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

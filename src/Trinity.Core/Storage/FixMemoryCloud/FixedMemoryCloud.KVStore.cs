// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using Trinity;
using Trinity.Network.Messaging;

namespace Trinity.Storage
{
    public partial class FixedMemoryCloud
    {
        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public TrinityErrorCode SaveCell(long cellId, byte[] buff)
        {
            return GetStorageByCellId(cellId).SaveCell(cellId, buff);
        }

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="offset">The byte offset into the buff.</param>
        /// <param name="size">The size of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public TrinityErrorCode SaveCell(long cellId, byte[] buff, int offset, int size)
        {
            return GetStorageByCellId(cellId).SaveCell(cellId, buff, offset, size);
        }

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="size">The size of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe TrinityErrorCode SaveCell(long cellId, byte* buff, int size)
        {
            return GetStorageByCellId(cellId).SaveCell(cellId, buff, size);
        }

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="offset">The byte offset into the buff.</param>
        /// <param name="size">The size of the cell.</param>
        /// <param name="cellType">Indicates the cell type.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public TrinityErrorCode SaveCell(long cellId, byte[] buff, int offset, int size, ushort cellType)
        {
            return GetStorageByCellId(cellId).SaveCell(cellId, buff, offset, size, cellType);
        }

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="cellType">Indicates the cell type.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public TrinityErrorCode SaveCell(long cellId, byte[] buff, ushort cellType)
        {
            return GetStorageByCellId(cellId).SaveCell(cellId, buff, 0, buff.Length, cellType);
        }

        /// <summary>
        /// Loads the bytes of the cell with the specified cell Id.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="cellBuff">The bytes of the cell. An empty byte array is returned if the cell is not found.</param>
        /// <returns>A Trinity error code. Possible values are E_SUCCESS and E_NOT_FOUND.</returns>
        public TrinityErrorCode LoadCell(long cellId, out byte[] cellBuff)
        {
            return GetStorageByCellId(cellId).LoadCell(cellId, out cellBuff);
        }

        /// <summary>
        /// Loads the type and the content of the cell with the specified cell Id.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="cellBuff">The bytes of the cell. An empty byte array is returned if the cell is not found.</param>
        /// <param name="cellType">The type of the cell, represented with a 16-bit unsigned integer.</param>
        /// <returns>A Trinity error code. Possible values are E_SUCCESS and E_NOT_FOUND.</returns>
        public TrinityErrorCode LoadCell(long cellId, out byte[] cellBuff, out ushort cellType)
        {
            return GetStorageByCellId(cellId).LoadCell(cellId, out cellBuff, out cellType);
        }

        /// <summary>
        /// Removes the cell with the specified cell Id from the key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns>true if removing succeeds; otherwise, false.</returns>
        public TrinityErrorCode RemoveCell(long cellId)
        {
            return GetStorageByCellId(cellId).RemoveCell(cellId);
        }

        /// <summary>
        /// Gets the type of the cell with specified cell Id.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="cellType">The type of the cell specified by cellId.</param>
        /// <returns>A Trinity error code. Possible values are E_SUCCESS and E_NOT_FOUND.</returns>
        public unsafe TrinityErrorCode GetCellType(long cellId, out ushort cellType)
        {
            return GetStorageByCellId(cellId).GetCellType(cellId, out cellType);
        }

        /// <summary>
        /// Determines whether there is a cell with the specified cell Id in Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns>true if a cell whose Id is cellId is found; otherwise, false.</returns>
        public unsafe bool Contains(long cellId)
        {
            return GetStorageByCellId(cellId).Contains(cellId);
        }

        /// <summary>
        /// Adds a new cell to the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="size">The size of the cell.</param>
        /// <returns>true if adding succeeds; otherwise, false.</returns>
        public unsafe TrinityErrorCode AddCell(long cellId, byte* buff, int size)
        {
            return GetStorageByCellId(cellId).AddCell(cellId, buff, size);
        }

        /// <summary>
        /// Adds a new cell to the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <returns>true if adding succeeds; otherwise, false.</returns>
        public unsafe TrinityErrorCode AddCell(long cellId, byte[] buff)
        {
            return GetStorageByCellId(cellId).AddCell(cellId, buff);
        }

        /// <summary>
        /// Adds a new cell to the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="offset">The byte offset into the buff.</param>
        /// <param name="size">The size of the cell.</param>
        /// <returns>true if adding succeeds; otherwise, false.</returns>
        public unsafe TrinityErrorCode AddCell(long cellId, byte[] buff, int offset, int size)
        {
            return GetStorageByCellId(cellId).AddCell(cellId, buff, offset, size);
        }


        /// <summary>
        /// Updates an existing cell in the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="size">The size of the cell.</param>
        /// <returns>true if updating succeeds; otherwise, false.</returns>
        public unsafe TrinityErrorCode UpdateCell(long cellId, byte* buff, int size)
        {
            return GetStorageByCellId(cellId).UpdateCell(cellId, buff, size);
        }

        /// <summary>
        /// Updates an existing cell in the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <returns>true if updating succeeds; otherwise, false.</returns>
        public unsafe TrinityErrorCode UpdateCell(long cellId, byte[] buff)
        {
            return GetStorageByCellId(cellId).UpdateCell(cellId, buff);
        }

        /// <summary>
        /// Updates an existing cell in the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="offset">The byte offset into the buff.</param>
        /// <param name="size">The size of the cell.</param>
        /// <returns>true if updating succeeds; otherwise, false.</returns>
        public unsafe TrinityErrorCode UpdateCell(long cellId, byte[] buff, int offset, int size)
        {
            return GetStorageByCellId(cellId).UpdateCell(cellId, buff, offset, size);
        }
    }
}

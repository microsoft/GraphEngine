// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Trinity;
using Trinity.Network.Messaging;
using Trinity.TSL.Lib;
using Trinity.Diagnostics;
namespace Trinity.Storage
{
    /// <summary>
    /// Represents an abstract storage class. It defines a set of cell accessing and manipulation interfaces.
    /// </summary>
    public unsafe abstract class Storage : IDisposable
    {
        static Storage()
        {
            try
            {
                TrinityConfig.LoadTrinityConfig();
            }
            catch
            {
                Log.WriteLine(LogLevel.Error, "Failure to load config file, falling back to default Storage behavior");
            }
        }

        #region Key-value Store interfaces
        /// <summary>
        /// Determines whether there is a cell with the specified cell Id in Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns>true if a cell whose Id is cellId is found; otherwise, false.</returns>
        public abstract bool Contains(long cellId);

        /// <summary>
        /// Gets the type of the cell with specified cell Id.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="cellType">The type of the cell specified by cellId.</param>
        /// <returns>A Trinity error code. Possible values are E_SUCCESS and E_NOT_FOUND.</returns>
        public abstract TrinityErrorCode GetCellType(long cellId, out ushort cellType);

        /// <summary>
        /// Adds a new cell to the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="size">The size of the cell.</param>
        /// <param name="cellType">Indicates the cell type.</param>
        /// <returns>true if adding succeeds; otherwise, false.</returns>
        public abstract TrinityErrorCode AddCell(long cellId, byte* buff, int size, ushort cellType);

        /// <summary>
        /// Adds a new cell to the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="size">The size of the cell.</param>
        /// <returns>true if adding succeeds; otherwise, false.</returns>
        public abstract TrinityErrorCode AddCell(long cellId, byte* buff, int size);

        /// <summary>
        /// Adds a new cell to the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <returns>true if adding succeeds; otherwise, false.</returns>
        public abstract TrinityErrorCode AddCell(long cellId, byte[] buff);

        /// <summary>
        /// Adds a new cell to the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="offset">The byte offset into the buff.</param>
        /// <param name="size">The size of the cell.</param>
        /// <returns>true if adding succeeds; otherwise, false.</returns>
        public abstract TrinityErrorCode AddCell(long cellId, byte[] buff, int offset, int size);

        /// <summary>
        /// Adds a new cell to the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="offset">The byte offset into the buff.</param>
        /// <param name="size">The size of the cell.</param>
        /// <param name="cellType">Indicates the cell type.</param>
        /// <returns>true if adding succeeds; otherwise, false.</returns>
        public abstract TrinityErrorCode AddCell(long cellId, byte[] buff, int offset, int size, ushort cellType);

        /// <summary>
        /// Updates an existing cell in the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="size">The size of the cell.</param>
        /// <returns>true if updating succeeds; otherwise, false.</returns>
        public abstract TrinityErrorCode UpdateCell(long cellId, byte* buff, int size);

        /// <summary>
        /// Updates an existing cell in the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <returns>true if updating succeeds; otherwise, false.</returns>
        public abstract TrinityErrorCode UpdateCell(long cellId, byte[] buff);

        /// <summary>
        /// Updates an existing cell in the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="offset">The byte offset into the buff.</param>
        /// <param name="size">The size of the cell.</param>
        /// <returns>true if updating succeeds; otherwise, false.</returns>
        public abstract TrinityErrorCode UpdateCell(long cellId, byte[] buff, int offset, int size);

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="size">The size of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public abstract TrinityErrorCode SaveCell(long cellId, byte* buff, int size);

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="cell_id">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public abstract TrinityErrorCode SaveCell(long cell_id, byte[] buff);

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="cell_id">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="offset">The byte offset into the buff.</param>
        /// <param name="size">The size of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public abstract TrinityErrorCode SaveCell(long cell_id, byte[] buff, int offset, int size);

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="offset">The byte offset into the buff.</param>
        /// <param name="size">The size of the cell.</param>
        /// <param name="cellType">Indicates the cell type.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public abstract TrinityErrorCode SaveCell(long cellId, byte[] buff, int offset, int size, ushort cellType);

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="size">The size of the cell.</param>
        /// <param name="cellType">Indicates the cell type.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public abstract TrinityErrorCode SaveCell(long cellId, byte* buff, int size, ushort cellType);

        /// <summary>
        /// Loads the bytes of the cell with the specified cell Id.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="cellBuff">The bytes of the cell. An empty byte array is returned if the cell is not found.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public abstract TrinityErrorCode LoadCell(long cellId, out byte[] cellBuff);

        /// <summary>
        /// Loads the bytes of the cell with the specified cell Id.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="cellBuff">The bytes of the cell. An empty byte array is returned if the cell is not found.</param>
        /// <param name="cellType">The type of the cell, represented with a 16-bit unsigned integer.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public abstract TrinityErrorCode LoadCell(long cellId, out byte[] cellBuff, out ushort cellType);

        /// <summary>
        /// Removes the cell with the specified cell Id from the key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns>true if removing succeeds; otherwise, false.</returns>
        public abstract TrinityErrorCode RemoveCell(long cellId);
        #endregion

        #region Message Sending Interfaces
        public abstract void SendMessage(TrinityMessage message);
        public abstract void SendMessage(TrinityMessage message, out TrinityResponse response);
        /*------------------------------------------------------------------------------*/
        public abstract void SendMessage(byte* message, int size);
        public abstract void SendMessage(byte* message, int size, out TrinityResponse response);
        #endregion

        /// <summary>
        /// Releases the resources used by the current storage instance.
        /// </summary>
        public abstract void Dispose();
    }
}

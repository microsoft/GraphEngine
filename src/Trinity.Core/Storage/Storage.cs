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

namespace Trinity.Storage
{
    /// <summary>
    /// Represents an abstract storage class. It defines a set of cell accessing and manipulation interfaces.
    /// </summary>
    public unsafe abstract class Storage : IDisposable
    {
        static Storage()
        {
            TrinityConfig.LoadTrinityConfig();
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
        internal abstract void SendMessage(TrinityMessage message);
        internal abstract void SendMessage(TrinityMessage message, out TrinityResponse response);
        /*------------------------------------------------------------------------------*/
        internal abstract void SendMessage(byte* message, int size);
        internal abstract void SendMessage(byte* message, int size, out TrinityResponse response);
        /*------------------------------------------------------------------------------*/
        internal abstract void SendMessage(byte** message, int* sizes, int count);
        internal abstract void SendMessage(byte** message, int* sizes, int count, out TrinityResponse response);
        #endregion

        /// <summary>
        /// Releases the resources used by the current storage instance.
        /// </summary>
        public abstract void Dispose();
    }

    /// <summary>
    /// Exposes methods for generic cell manipulation.
    /// </summary>
    public interface IGenericCellOperations
    {
        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// Note that the generic cell will be saved as a strongly typed cell. It can then be loaded into either a strongly-typed cell or a generic cell.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> object.</param>
        /// <param name="cell">The cell to be saved.</param>
        void SaveGenericCell(LocalMemoryStorage storage, ICell cell);

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// Note that the generic cell will be saved as a strongly typed cell. It can then be loaded into either a strongly-typed cell or a generic cell.
        /// The <paramref name="cellId"/> overrides the cell id in <paramref name="cell"/>.
        /// </summary>
        /// <param name="cellId">A 64-bit cell id.</param>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> object.</param>
        /// <param name="cell">The cell to be saved.</param>
        void SaveGenericCell(LocalMemoryStorage storage, long cellId, ICell cell);

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// Note that the generic cell will be saved as a strongly typed cell. It can then be loaded into either a strongly-typed cell or a generic cell.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> object.</param>
        /// <param name="writeAheadLogOptions">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cell">The cell to be saved.</param>
        void SaveGenericCell(LocalMemoryStorage storage, CellAccessOptions writeAheadLogOptions, ICell cell);

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// Note that the generic cell will be saved as a strongly typed cell. It can then be loaded into either a strongly-typed cell or a generic cell.
        /// The <paramref name="cellId"/> overrides the cell id in <paramref name="cell"/>.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> object.</param>
        /// <param name="writeAheadLogOptions">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellId">A 64-bit cell id.</param>
        /// <param name="cell">The cell to be saved.</param>
        void SaveGenericCell(LocalMemoryStorage storage, CellAccessOptions writeAheadLogOptions, long cellId, ICell cell);

        /// <summary>
        /// Loads the content of the cell with the specified cell Id.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> object.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns>An generic cell instance that implements <see cref="Trinity.Storage.ICell"/> interfaces.</returns>
        ICell LoadGenericCell(LocalMemoryStorage storage, long cellId);

        /// <summary>
        /// Instantiate a new generic cell with the specified type.
        /// </summary>
        /// <param name="cellType">The string representation of the cell type.</param>
        /// <returns>The allocated generic cell.</returns>
        ICell NewGenericCell(string cellType);

        /// <summary>
        /// Instantiate a new generic cell with the specified type and a cell ID.
        /// </summary>
        /// <param name="cellId">Cell Id.</param>
        /// <param name="cellType">The string representation of the cell type.</param>
        /// <returns>The allocated generic cell.</returns>
        ICell NewGenericCell(long cellId, string cellType);

        /// <summary>
        /// Instantiate a new generic cell with the specified type and a cell ID.
        /// </summary>
        /// <param name="cellType">The string representation of the cell type.</param>
        /// <param name="content">The json representation of the cell.</param>
        /// <returns>The allocated generic cell.</returns>
        ICell NewGenericCell(string cellType, string content);

        /// <summary>
        /// Allocate a generic cell accessor on the specified cell.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> object.</param>
        /// <param name="cellId">The id of the specified cell.</param>
        /// <returns>A <see cref="Trinity.Storage.ICellAccessor"/> instance.</returns>
        ICellAccessor UseGenericCell(LocalMemoryStorage storage, long cellId);

        /// <summary>
        /// Allocate a generic cell accessor on the specified cell.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> object.</param>
        /// <param name="cellId">The id of the specified cell.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>A <see cref="Trinity.Storage.ICellAccessor"/> instance.</returns>
        ICellAccessor UseGenericCell(LocalMemoryStorage storage, long cellId, CellAccessOptions options);

        /// <summary>
        /// Allocate a generic cell accessor on the specified cell.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> object.</param>
        /// <param name="cellId">The id of the specified cell.</param>
        /// <param name="options">Cell access options.</param>
        /// <param name="cellType">Specifies the type of cell to be created.</param>
        /// <returns>A <see cref="Trinity.Storage.ICellAccessor"/> instance.</returns>
        ICellAccessor UseGenericCell(LocalMemoryStorage storage, long cellId, CellAccessOptions options, string cellType);

        /// <summary>
        /// Enumerates all the typed cells within the local memory storage.
        /// The cells without a type (where CellType == 0) are skipped.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> object.</param>
        /// <returns>All the typed cells within the local memory storage.</returns>
        IEnumerable<ICell> EnumerateGenericCells(LocalMemoryStorage storage);

        /// <summary>
        /// Enumerates accessors of all the typed cells within the local memory storage.
        /// The cells without a type (where CellType == 0) are skipped.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> object.</param>
        /// <returns>The accessors of all the typed cells within the local memory storage.</returns>
        IEnumerable<ICellAccessor> EnumerateGenericCellAccessors(LocalMemoryStorage storage);

        /// <summary>
        /// Loads the content of the cell with the specified cell Id.
        /// </summary>
        /// <param name="storage">The cloud storage to load from.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns>An generic cell instance that implements <see cref="Trinity.Storage.ICell"/> interfaces.</returns>
        ICell LoadGenericCell(MemoryCloud storage, long cellId);

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// Note that the generic cell will be saved as a strongly typed cell. It can then be loaded into either a strongly-typed cell or a generic cell.
        /// </summary>
        /// <param name="storage">The cloud storage to save to.</param>
        /// <param name="cell">The cell to be saved.</param>
        void SaveGenericCell(MemoryCloud storage, ICell cell);
    }

    internal class DefaultStorageSchema : IStorageSchema
    {
        public IEnumerable<ICellDescriptor> CellDescriptors
        {
            get { yield break; }
        }

        public ushort GetCellType(string cellTypeString)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> CellTypeSignatures
        {
            get { yield break; }
        }
    }

    internal class DefaultGenericCellOperations : IGenericCellOperations
    {
        public void SaveGenericCell(LocalMemoryStorage storage, ICell cell)
        {
            throw new NotImplementedException();
        }

        public void SaveGenericCell(LocalMemoryStorage storage, long cellId, ICell cell)
        {
            throw new NotImplementedException();
        }

        public void SaveGenericCell(LocalMemoryStorage storage, CellAccessOptions writeAheadLogOptions, ICell cell)
        {
            throw new NotImplementedException();
        }

        public void SaveGenericCell(LocalMemoryStorage storage, CellAccessOptions writeAheadLogOptions, long cellId, ICell cell)
        {
            throw new NotImplementedException();
        }

        public ICell LoadGenericCell(LocalMemoryStorage storage, long cellId)
        {
            throw new NotImplementedException();
        }

        public ICell NewGenericCell(string cellType)
        {
            throw new NotImplementedException();
        }

        public ICell NewGenericCell(long cellId, string cellType)
        {
            throw new NotImplementedException();
        }

        public ICell NewGenericCell(string cellType, string content)
        {
            throw new NotImplementedException();
        }

        public ICellAccessor UseGenericCell(LocalMemoryStorage storage, long cellId)
        {
            throw new NotImplementedException();
        }

        public ICellAccessor UseGenericCell(LocalMemoryStorage storage, long cellId, CellAccessOptions options)
        {
            throw new NotImplementedException();
        }

        public ICellAccessor UseGenericCell(LocalMemoryStorage storage, long cellId, CellAccessOptions options, string cellType)
        {
            throw new NotImplementedException();
        }


        public ICell LoadGenericCell(MemoryCloud storage, long cellId)
        {
            throw new NotImplementedException();
        }

        public void SaveGenericCell(MemoryCloud storage, ICell cell)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICell> EnumerateGenericCells(LocalMemoryStorage storage)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICellAccessor> EnumerateGenericCellAccessors(LocalMemoryStorage storage)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trinity.Extension;
using Trinity.TSL.Lib;

namespace Trinity.Storage
{
    /// <summary>
    /// Exposes methods for generic cell manipulation.
    /// </summary>
    public interface IGenericCellOperations
    {
        /// <summary>
        /// Loads the content of the cell with the specified cell Id.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.IKeyValueStore"/> object to load from.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns>An generic cell instance that implements <see cref="Trinity.Storage.ICell"/> interfaces.</returns>
        Task<ICell> LoadGenericCellAsync(IKeyValueStore storage, long cellId);

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// Note that the generic cell will be saved as a strongly typed cell. It can then be loaded into either a strongly-typed cell or a generic cell.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.IKeyValueStore"/> object.</param>
        /// <param name="cell">The cell to be saved.</param>
        Task SaveGenericCellAsync(IKeyValueStore storage, ICell cell);

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// Note that the generic cell will be saved as a strongly typed cell. It can then be loaded into either a strongly-typed cell or a generic cell.
        /// The <paramref name="cellId"/> overrides the cell id in <paramref name="cell"/>.
        /// </summary>
        /// <param name="cellId">A 64-bit cell id.</param>
        /// <param name="storage">A <see cref="Trinity.Storage.IKeyValueStore"/> object.</param>
        /// <param name="cell">The cell to be saved.</param>
        Task SaveGenericCellAsync(IKeyValueStore storage, long cellId, ICell cell);

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

        #region LocalMemoryStorage specialized operations
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
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> object.</param>
        /// <param name="cellId">A 64-bit cell id.</param>
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
        #endregion

        /// <summary>
        /// Allocate a generic cell accessor on the specified cell.
        /// The cell should be already locked, with its cell id, buffer pointer type and entry index supplied.
        /// The cell lock will be released upon ICellAccessor disposal.
        /// </summary>
        /// <param name="cellId">The id of the specified cell.</param>
        /// <param name="cellPtr">Points to the content of the cell.</param>
        /// <param name="cellEntryIndex">The entry index of the cell, obtained from cell lock-acquiring methods.</param>
        /// <param name="cellType">The cell type Id.</param>
        /// <returns>A <see cref="Trinity.Storage.ICellAccessor"/> instance.</returns>
        unsafe ICellAccessor UseGenericCell(long cellId, byte* cellPtr, int cellEntryIndex, ushort cellType);

        /// <summary>
        /// Allocate a generic cell accessor on the specified cell.
        /// The cell should be already locked, with its cell id, buffer pointer type and entry index supplied.
        /// The cell lock will be released upon ICellAccessor disposal.
        /// </summary>
        /// <param name="cellId">The id of the specified cell.</param>
        /// <param name="cellPtr">Points to the content of the cell.</param>
        /// <param name="entryIndex">The entry index of the cell, obtained from cell lock-acquiring methods.</param>
        /// <param name="cellType">The cell type Id.</param>
        /// <param name="options">Cell access options.</param>
        /// <returns>A <see cref="Trinity.Storage.ICellAccessor"/> instance.</returns>
        unsafe ICellAccessor UseGenericCell(long cellId, byte* cellPtr, int entryIndex, ushort cellType, CellAccessOptions options);
    }

    [ExtensionPriority(int.MinValue)]
    internal class DefaultGenericCellOperations : IGenericCellOperations
    {
        public IEnumerable<ICellAccessor> EnumerateGenericCellAccessors(LocalMemoryStorage storage)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICell> EnumerateGenericCells(LocalMemoryStorage storage)
        {
            throw new NotImplementedException();
        }

        public Task<ICell> LoadGenericCellAsync(IKeyValueStore storage, long cellId)
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

        public Task SaveGenericCellAsync(IKeyValueStore storage, ICell cell)
        {
            throw new NotImplementedException();
        }

        public Task SaveGenericCellAsync(IKeyValueStore storage, long cellId, ICell cell)
        {
            throw new NotImplementedException();
        }

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

        public unsafe ICellAccessor UseGenericCell(long cellId, byte* cellBuffer, int cellEntryIndex, ushort cellType)
        {
            throw new NotImplementedException();
        }

        public unsafe ICellAccessor UseGenericCell(long cellId, byte* cellBuffer, int cellEntryIndex, ushort cellType, CellAccessOptions options)
        {
            throw new NotImplementedException();
        }
    }
}

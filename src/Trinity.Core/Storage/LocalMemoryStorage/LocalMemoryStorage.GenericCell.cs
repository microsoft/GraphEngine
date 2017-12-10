// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.TSL.Lib;

namespace Trinity.Storage
{
    partial class LocalMemoryStorage : Storage
    {
        private Action<LocalMemoryStorage, ICell>                                m_SaveGenericCell_ICell;
        private Action<LocalMemoryStorage, long, ICell>                    		 m_SaveGenericCell_long_ICell;
        private Action<LocalMemoryStorage, CellAccessOptions, ICell>       		 m_SaveGenericCell_CellAccessOptions_ICell;
        private Action<LocalMemoryStorage, CellAccessOptions, long, ICell> 		 m_SaveGenericCell_CellAccessOptions_long_ICell;
        private Func<LocalMemoryStorage, long, ICell>                      		 m_LoadGenericCell_long;
        private Func<string, ICell>                    		                     m_NewGenericCell_string;
        private Func<long, string, ICell>              		                     m_NewGenericCell_long_string;
        private Func<string, string, ICell>      		                         m_NewGenericCell_string_string;
        private Func<LocalMemoryStorage, long, ICellAccessor>              		 m_UseGenericCell_long;
        private Func<LocalMemoryStorage, long, CellAccessOptions, ICellAccessor> m_UseGenericCell_long_CellAccessOptions;
        private Func<LocalMemoryStorage, long, CellAccessOptions, string, ICellAccessor> m_UseGenericCell_long_CellAccessOptions_string;
        private Func<LocalMemoryStorage, IEnumerable<ICell>>                     m_EnumerateGenericCells;
        private Func<LocalMemoryStorage, IEnumerable<ICellAccessor>>             m_EnumerateGenericCellAccessors;

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// Note that the generic cell will be saved as a strongly typed cell. It can then be loaded into either a strongly-typed cell or a generic cell.
        /// </summary>
        /// <param name="cell">The cell to be saved.</param>
        public void SaveGenericCell(ICell cell)
        {
            m_SaveGenericCell_ICell(this, cell);
        }

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// Note that the generic cell will be saved as a strongly typed cell. It can then be loaded into either a strongly-typed cell or a generic cell.
        /// The <paramref name="cellId"/> overrides the cell id in <paramref name="cell"/>.
        /// </summary>
        /// <param name="cellId">A 64-bit cell id.</param>
        /// <param name="cell">The cell to be saved.</param>
        public void SaveGenericCell(long cellId, ICell cell)
        {
            m_SaveGenericCell_long_ICell(this, cellId, cell);
        }

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// Note that the generic cell will be saved as a strongly typed cell. It can then be loaded into either a strongly-typed cell or a generic cell.
        /// </summary>
        /// <param name="writeAheadLogOptions">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cell">The cell to be saved.</param>
        public void SaveGenericCell(CellAccessOptions writeAheadLogOptions, ICell cell)
        {
            m_SaveGenericCell_CellAccessOptions_ICell(this, writeAheadLogOptions, cell);
        }

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// Note that the generic cell will be saved as a strongly typed cell. It can then be loaded into either a strongly-typed cell or a generic cell.
        /// The <paramref name="cellId"/> overrides the cell id in <paramref name="cell"/>.
        /// </summary>
        /// <param name="writeAheadLogOptions">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellId">A 64-bit cell id.</param>
        /// <param name="cell">The cell to be saved.</param>
        public void SaveGenericCell(CellAccessOptions writeAheadLogOptions, long cellId, ICell cell)
        {
            m_SaveGenericCell_CellAccessOptions_long_ICell(this, writeAheadLogOptions, cellId, cell);
        }

        /// <summary>
        /// Loads the content of the cell with the specified cell Id.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns>An generic cell instance that implements <see cref="Trinity.Storage.ICell"/> interfaces.</returns>
        public ICell LoadGenericCell(long cellId)
        {
            return m_LoadGenericCell_long(this, cellId);
        }


        /// <summary>
        /// Instantiate a new generic cell with the specified type.
        /// </summary>
        /// <param name="cellType">The string representation of the cell type.</param>
        /// <returns>The allocated generic cell.</returns>
        public ICell NewGenericCell(string cellType)
        {
            return m_NewGenericCell_string(cellType);
        }

        /// <summary>
        /// Instantiate a new generic cell with the specified type and a cell ID.
        /// </summary>
        /// <param name="cellId">Cell Id.</param>
        /// <param name="cellType">The string representation of the cell type.</param>
        /// <returns>The allocated generic cell.</returns>
        public ICell NewGenericCell(long cellId, string cellType)
        {
            return m_NewGenericCell_long_string(cellId, cellType);
        }

        /// <summary>
        /// Instantiate a new generic cell with the specified type and a cell ID.
        /// </summary>
        /// <param name="cellType">The string representation of the cell type.</param>
        /// <param name="content">The json representation of the cell.</param>
        /// <returns>The allocated generic cell.</returns>
        public ICell NewGenericCell(string cellType, string content)
        {
            return m_NewGenericCell_string_string(cellType, content);
        }

        /// <summary>
        /// Allocate a generic cell accessor on the specified cell.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock.
        /// </summary>
        /// <param name="cellId">The id of the specified cell.</param>
        /// <returns>A <see cref="Trinity.Storage.ICellAccessor"/> instance.</returns>
        public ICellAccessor UseGenericCell(long cellId)
        {
            return m_UseGenericCell_long(this, cellId);
        }

        /// <summary>
        /// Allocate a generic cell accessor on the specified cell.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock.
        /// </summary>
        /// <param name="cellId">The id of the specified cell.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>A <see cref="Trinity.Storage.ICellAccessor"/> instance.</returns>
        public ICellAccessor UseGenericCell(long cellId, CellAccessOptions options)
        {
            return m_UseGenericCell_long_CellAccessOptions(this, cellId, options);
        }

        /// <summary>
        /// Allocate a generic cell accessor on the specified cell.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock.
        /// </summary>
        /// <param name="cellId">The id of the specified cell.</param>
        /// <param name="options">Cell access options.</param>
        /// <param name="cellType">Specifies the type of cell to be created.</param>
        /// <returns>A <see cref="Trinity.Storage.ICellAccessor"/> instance.</returns>
        public ICellAccessor UseGenericCell(long cellId, CellAccessOptions options, string cellType)
        {
            return m_UseGenericCell_long_CellAccessOptions_string(this, cellId, options, cellType);
        }

        /// <summary>
        /// Enumerates all the typed cells within the local memory storage.
        /// The cells without a type (where CellType == 0) are skipped.
        /// </summary>
        /// <returns>All the typed cells within the local memory storage.</returns>
        public IEnumerable<ICell> GenericCell_Selector()
        {
            return m_EnumerateGenericCells(this);
        }

        /// <summary>
        /// Enumerates accessors of all the typed cells within the local memory storage.
        /// The cells without a type (where CellType == 0) are skipped.
        /// </summary>
        /// <returns>The accessors of all the typed cells within the local memory storage.</returns>
        public IEnumerable<ICellAccessor> GenericCellAccessor_Selector()
        {
            return m_EnumerateGenericCellAccessors(this);
        }


        internal void RegisterGenericOperationsProvider(IGenericCellOperations operations)
        {
            m_SaveGenericCell_ICell                        = operations.SaveGenericCell;
            m_SaveGenericCell_long_ICell                   = operations.SaveGenericCell;
            m_SaveGenericCell_CellAccessOptions_ICell      = operations.SaveGenericCell;
            m_SaveGenericCell_CellAccessOptions_long_ICell = operations.SaveGenericCell;
            m_LoadGenericCell_long                         = operations.LoadGenericCell;
            m_NewGenericCell_string                        = operations.NewGenericCell;
            m_NewGenericCell_long_string                   = operations.NewGenericCell;
            m_NewGenericCell_string_string                 = operations.NewGenericCell;
            m_UseGenericCell_long                          = operations.UseGenericCell;
            m_UseGenericCell_long_CellAccessOptions        = operations.UseGenericCell;
            m_EnumerateGenericCells                        = operations.EnumerateGenericCells;
            m_EnumerateGenericCellAccessors                = operations.EnumerateGenericCellAccessors;
        }
    }
}

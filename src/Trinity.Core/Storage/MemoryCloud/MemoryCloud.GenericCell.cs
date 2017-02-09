// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Storage
{
    public partial class MemoryCloud
    {
        private Action<MemoryCloud, ICell>     m_SaveGenericCell_ICell;
        private Func<MemoryCloud, long, ICell> m_LoadGenericCell_long;
        private Func<string, ICell>            m_NewGenericCell_string;
        private Func<long, string, ICell>      m_NewGenericCell_long_string;
        private Func<string, string, ICell>    m_NewGenericCell_string_string;

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
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// Note that the generic cell will be saved as a strongly typed cell. It can then be loaded into either a strongly-typed cell or a generic cell.
        /// </summary>
        /// <param name="cell">The cell to be saved.</param>
        public void SaveGenericCell(ICell cell)
        {
            m_SaveGenericCell_ICell(this, cell);
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

        internal void RegisterGenericOperationsProvider(IGenericCellOperations cloud_operations)
        {
            m_SaveGenericCell_ICell        = cloud_operations.SaveGenericCell;
            m_LoadGenericCell_long         = cloud_operations.LoadGenericCell;
            m_NewGenericCell_string        = cloud_operations.NewGenericCell;
            m_NewGenericCell_long_string   = cloud_operations.NewGenericCell;
            m_NewGenericCell_string_string = cloud_operations.NewGenericCell;
        }
    }
}

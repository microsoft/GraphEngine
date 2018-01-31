// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Extension;

namespace Trinity.Storage
{
    /// <summary>
    /// Exposes methods for accessing the storage schema defined in a TSL storage extension assembly.
    /// </summary>
    public interface IStorageSchema
    {
        /// <summary>
        /// Enumerates descriptors for all cells defined in a TSL storage extension assembly.
        /// </summary>
        IEnumerable<ICellDescriptor> CellDescriptors { get; }
        /// <summary>
        /// Converts a type string to a cell type id.
        /// </summary>
        /// <param name="cellTypeString">The type string to be converted.</param>
        /// <returns>The converted cell type id.</returns>
        ushort GetCellType(string cellTypeString);
        /// <summary>
        /// Gets the type signature strings for all cell types defined in a TSL storage extension assembly.
        /// </summary>
        IEnumerable<string> CellTypeSignatures { get; }
    }

    [ExtensionPriority(-100)]
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
}

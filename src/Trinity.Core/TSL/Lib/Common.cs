// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.TSL.Lib
{
    /// <summary>
    /// Represents option flags for cell access.
    /// </summary>
    [Flags]
    public enum CellAccessOptions
    {
        /// <summary>
        /// Throws an exception when a cell is not found.
        /// </summary>
        ThrowExceptionOnCellNotFound = 0x1,
        /// <summary>
        /// Returns null when a cell is not found.
        /// </summary>
        ReturnNullOnCellNotFound     = 0x2,
        /// <summary>
        /// Creates a new cell when a cell is not found.
        /// </summary>
        CreateNewOnCellNotFound      = 0x4,
        /// <summary>
        /// Specifies that write-ahead-log should be performed with strong durability.
        /// </summary>
        StrongLogAhead               = 0x8,
        /// <summary>
        /// Specifies that write-ahead-log should be performed with weak durability. This option brings better performance,
        /// but the durability may be degraded when this option is used.
        /// </summary>
        WeakLogAhead                 = 0x10,
    }

    /// <summary>
    /// Represents a reference to a resize function, which will be called when the underlying storage is resized.
    /// </summary>
    /// <param name="ptr">A point pointing to the underlying storage of the current data structure.</param>
    /// <param name="offset">The offset starting which the underlying storage needs to expand or shrink.</param>
    /// <param name="delta">The size to expand or shrink, in bytes.</param>
    /// <returns>The pointer pointing to the underlying storage after resizing.</returns>
    public unsafe delegate byte* ResizeFunctionDelegate(byte* ptr, int offset, int delta);

}

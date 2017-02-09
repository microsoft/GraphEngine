// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Trinity;

namespace Trinity.Storage
{
    internal enum CellTransformerTypes
    {
        Undefined, CellTransformSlim, CellTransform
    }

    /// <summary>
    /// Represents a cell action that takes an unsafe cell pointer as parameter.
    /// </summary>
    /// <param name="cellPtr">An unsafe cell pointer.</param>
    internal unsafe delegate void CellAction(byte* cellPtr);

    /// <summary>
    /// Represents a cell action that takes an unsafe cell pointer and a cell size (int) as parameters.
    /// </summary>
    /// <param name="cellPtr">An unsafe cell pointer.</param>
    /// <param name="size">The size of current cell.</param>
    internal unsafe delegate void CellAction<in T>(byte* cellPtr, T size);

    /// <summary>
    /// Represents a cell action that takes an unsafe cell pointer, a cell Id (long), and a cell size (int) as parameters.
    /// </summary>
    /// <param name="cellPtr">An unsafe cell pointer.</param>
    /// <param name="cellId">The 64-bit cell Id of current cell.</param>
    /// <param name="size">The size of current cell.</param>
    internal unsafe delegate void CellAction<in T1, in T2>(byte* cellPtr, T1 cellId, T2 size);

    /// <summary>
    /// Represents a cell action that takes an unsafe cell pointer, a cell Id (long), a cell size (int), and a corresponding MTHash entry index (int) as parameters.
    /// </summary>
    /// <param name="cellPtr">An unsafe cell pointer.</param>
    /// <param name="cellId">The 64-bit cell id of current cell.</param>
    /// <param name="size">The size of current cell.</param>
    /// <param name="entryIndex">The corresponding MTHash entry index.</param>
    internal unsafe delegate void CellAction<in T1, in T2, in T3>(byte* cellPtr, T1 cellId, T2 size, T3 entryIndex);

    /// <summary>
    /// Represents a cell action that takes an unsafe cell pointer, a cell Id (long), a cell size (int), a corresponding MTHash entry index (int), and a cell type (ushort) as parameters.
    /// </summary>
    /// <param name="cellPtr">An unsafe cell pointer.</param>
    /// <param name="cellId">The 64-bit cell id of current cell.</param>
    /// <param name="size">The size of current cell.</param>
    /// <param name="entryIndex">The corresponding MTHash entry index.</param>
    /// <param name="cellType">The cell type of current cell.</param>
    internal unsafe delegate void CellAction<in T1, in T2, in T3, in T4>(byte* cellPtr, T1 cellId, T2 size, T3 entryIndex, T4 cellType);

    /// <summary>
    /// Represents a cell transformation action that takes an unsafe cell pointer as parameter.
    /// </summary>
    /// <param name="cellPtr">An unsafe cell pointer.</param>
    /// <returns>Transformed cell blob.</returns>
    internal unsafe delegate byte[] CellTransformAction(byte* cellPtr);

    /// <summary>
    /// Represents a cell transformation action that takes an unsafe cell pointer and a cell size (int) as parameters.
    /// </summary>
    /// <param name="cellPtr">An unsafe cell pointer.</param>
    /// <param name="size">The size of current cell.</param>
    /// <returns>Transformed cell blob.</returns>
    internal unsafe delegate byte[] CellTransformAction<T>(byte* cellPtr, T size);

    /// <summary>
    /// Represents a cell transformation action that takes an unsafe cell pointer, a cell Id (long), and a cell size (int) as parameters.
    /// </summary>
    /// <param name="cellPtr">An unsafe cell pointer.</param>
    /// <param name="cellId">The 64-bit cell id of current cell.</param>
    /// <param name="size">The size of current cell.</param>
    /// <returns>Transformed cell blob.</returns>
    internal unsafe delegate byte[] CellTransformAction<T1, T2>(byte* cellPtr, T1 cellId, T2 size);

    /// <summary>
    /// Represents a cell transformation action that takes an unsafe cell pointer, a cell Id (long), a cell size (int), and a cell type (ushort) as parameters.
    /// </summary>
    /// <param name="cellPtr">An unsafe cell pointer.</param>
    /// <param name="cellId">The 64-bit cell id of current cell.</param>
    /// <param name="size">The size of current cell.</param>
    /// <param name="cellType">The cell type of current cell.</param>
    /// <returns>Transformed cell blob.</returns>
    internal unsafe delegate byte[] CellTransformAction<T1, T2, T3>(byte* cellPtr, T1 cellId, T2 size, ref T3 cellType);

    /// <summary>
    /// Defines a transformation method which takes an unsafe cell pointer and a cell size as parameters.
    /// </summary>
    internal unsafe interface ICellTransformSlim
    {
        /// <summary>
        /// Transforms a cell from one form to another.
        /// </summary>
        /// <param name="cellPtr">An unsafe cell pointer.</param>
        /// <param name="size">The size of current cell.</param>
        /// <returns>Transformed cell bytes.</returns>
        byte[] Transform(byte* cellPtr, int size);
    }

    /// <summary>
    /// Defines a transformation method which takes an unsafe cell pointer, a cell Id, and a cell size as parameters.
    /// </summary>
    internal unsafe interface ICellTransform
    {
        /// <summary>
        /// Transforms a cell from one from to another.
        /// </summary>
        /// <param name="cellPtr">An unsafe cell pointer.</param>
        /// <param name="cellId">An 64-bit cell Id.</param>
        /// <param name="size">The size of current cell.</param>
        /// <returns>The transformed cell bytes.</returns>
        byte[] Transform(byte* cellPtr, long cellId, int size);
    }
}

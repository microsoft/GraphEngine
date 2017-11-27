// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Network
{
    /// <summary>
    /// Represents a reference to a method that gets the partition Id by the specified cell Id.
    /// </summary>
    /// <param name="cellId">A 64-bit Trinity cell Id.</param>
    /// <returns>The Trinity partition Id.</returns>
    public delegate int GetPartitionIdByCellIdDelegate(long cellId);
}

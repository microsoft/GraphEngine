// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

using Trinity;
using System.Runtime.CompilerServices;

namespace Trinity.Storage
{
    public partial class LocalMemoryStorage
    {
        /// <summary>
        /// The total amount of trunk memory, in bytes.
        /// </summary>
        public ulong CommittedTrunkMemory
        {
            get
            {
                return _CommitTrunkMemoryWrapper();
            }
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private ulong _CommitTrunkMemoryWrapper()
        {
            return CLocalMemoryStorage.CTrunkCommittedMemorySize();
        }

        /// <summary>
        /// The total amount of memory consumed by the trunk indexes, in bytes.
        /// </summary>
        public ulong CommittedIndexMemory
        {
            get
            {
                /**!
                 * Note, unlike the methods, properties with getters only seem to be heavily optimized
                 * that it needs an extra wrapper method before it reaches the internal call stub.
                 * Otherwise ECall security exception will be raised. Properties with getter/setter
                 * appear to be not affected.
                 */
                return _CommitIndexMemoryWrapper();
            }
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private ulong _CommitIndexMemoryWrapper()
        {
            return CLocalMemoryStorage.CMTHashCommittedMemorySize();
        }

        /// <summary>
        /// The total amount of memory consumed by memory trunks, in bytes.
        /// </summary>
        public ulong TotalCommittedMemory
        {
            get
            {
                return _TotalCommittedMemoryWrapper();
            }
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private ulong _TotalCommittedMemoryWrapper()
        {
            return CLocalMemoryStorage.CTotalCommittedMemorySize();
        }

        /// <summary>
        /// The total size of cells stored in the system.
        /// </summary>
        public ulong TotalCellSize
        {
            get
            {
                return _TotalCellSizeWrapper();
            }
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private ulong _TotalCellSizeWrapper()
        {
            return CLocalMemoryStorage.CTotalCellSize();
        }
    }
}

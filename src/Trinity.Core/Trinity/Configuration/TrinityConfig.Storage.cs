// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;

using Trinity;
using Trinity.Utilities;
using Trinity.Diagnostics;
using Trinity.Storage;
using Trinity.Core.Lib;
using System.Runtime.CompilerServices;
using Trinity.Configuration;

namespace Trinity
{
    public unsafe static partial class TrinityConfig
    {
        /// <summary>
        /// Set the number of memory trunks in the Trinity memory storage.
        /// Note:
        /// 1)	This property should be switched on at the very beginning of your program.
        /// 2)	The value will be consumed during the initialization of the local memory storage.
        ///     After the local memory storage initialized, this property cannot be changed.
        /// 3)  The value of the property should be power of two between [1,256]. 
        ///     Values other than these will be ignored.
        /// </summary>
        public static int TrunkCount
        {
            get { return StorageConfig.Instance.TrunkCount; }
            set { StorageConfig.Instance.TrunkCount = value; }
        }

        /// <summary>
        /// Set readonly flag of the Trinity memory storage. The caller hereby promise that it does not modify the trinity memory storage.
        /// Note:
        /// 1)	This flag should be switched on at the very beginning of your program.
        /// 2)	Once this flag is enabled, it cannot be disabled in the current process.
        /// 3)	The current process SHOULD NOT update/modify Trinity memory storage any more.  Operations, such as SaveCell(), AddCell(), cannot be called in this mode.
        /// </summary>
        public static bool ReadOnly
        {
            get { return StorageConfig.Instance.ReadOnly; }
            set { StorageConfig.Instance.ReadOnly = value; }
        }

        /// <summary>
        /// Gets and Sets the current storage capacity profile.
        /// </summary>
        public static StorageCapacityProfile StorageCapacity
        {
            get { return StorageConfig.Instance.StorageCapacity; }
            set { StorageConfig.Instance.StorageCapacity = value; }
        }

        /// <summary>
        /// Gets or sets the root data directory for the current Trinity instance.
        /// </summary>
        public static string StorageRoot
        {
            get { return StorageConfig.Instance.StorageRoot; }

            set { StorageConfig.Instance.StorageRoot = value; }
        }

        /// <summary>
        /// Default = 10 M
        /// </summary>
        internal static int LargeObjectThreshold
        {
            get { return StorageConfig.Instance.LargeObjectThreshold; }
            set { StorageConfig.Instance.LargeObjectThreshold = value; }
        }

        /// <summary>
        /// Defragmentation frequency, Default Value = 600
        /// </summary>
        public static int DefragInterval
        {
            get { return StorageConfig.Instance.DefragInterval; }
            set { StorageConfig.Instance.DefragInterval = value; }
        }
    }
}


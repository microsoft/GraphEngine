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
    /// <summary>
    /// Represents a collection of system capacity profiles.
    /// </summary>
    public enum StorageCapacityProfile: int
    {
        /// <summary>
        /// Maximum 256 million cells supported by each LocalStorage instance.
        /// </summary>
        Max256M,

        /// <summary>
        /// Maximum 512 million cells supported by each LocalStorage instance.
        /// </summary>
        Max512M,

        /// <summary>
        /// Maximum 1 billion cells supported by each LocalStorage instance.
        /// </summary>
        Max1G,

        /// <summary>
        /// Maximum 2 billion cells supported by each LocalStorage instance.
        /// </summary>
        Max2G,

        /// <summary>
        /// Maximum 4 billion cells supported by each LocalStorage instance.
        /// </summary>
        Max4G,

        /// <summary>
        /// Maximum 8 billion cells supported by each LocalStorage instance.
        /// </summary>
        Max8G,
        /// <summary>
        /// Maximum 16 billion cells supported by each LocalStorage instance.
        /// </summary>
        Max16G,
        /// <summary>
        /// Maximum 32 billion cells supported by each LocalStorage instance.
        /// </summary>
        Max32G
    }
}

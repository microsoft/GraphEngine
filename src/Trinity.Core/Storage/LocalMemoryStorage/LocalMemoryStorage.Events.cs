// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System;
using Trinity.Diagnostics;

namespace Trinity.Storage
{
    public unsafe partial class LocalMemoryStorage
    {
        /// <summary>
        /// A StorageEventHandler handles events related to storage disk I/O events.
        /// </summary>
        public delegate void StorageEventHandler();
        /// <summary>
        /// Raised before loading a storage image from the disk.
        /// !!Note, on this event, the primary storage slot points
        /// to the current storage image, and the secondary to
        /// the storage image to be loaded.
        /// </summary>
        public event StorageEventHandler StorageBeforeLoad = delegate { };
        /// <summary>
        /// Raised before saving a storage image to the disk.
        /// !!Note, on this event, the primary storage slot points
        /// to the current storage image, and the secondary to
        /// the storage image to be saved.
        /// </summary>
        public event StorageEventHandler StorageBeforeSave = delegate { };
        /// <summary>
        /// Raised before resetting the storage and clearing the
        /// storage directory.
        /// !!Note, on this event, the primary storage slot points
        /// to the current storage image, and the secondary to
        /// the storage image to be overwritten by the reset operation.
        /// </summary>
        public event StorageEventHandler StorageBeforeReset = delegate { };
        /// <summary>
        /// Raised after a storage image is loaded from the disk.
        /// !!Note, on this event, the primary storage slot points
        /// to the newly-loaded storage image, and the secondary to
        /// the previous version of storage image.
        /// </summary>
        public event StorageEventHandler StorageLoaded = delegate { };
        /// <summary>
        /// Raised after a storage image is saved to the disk.
        /// !!Note, on this event, the primary storage slot points
        /// to the newly-saved storage image, and the secondary to
        /// the previous version of storage image.
        /// </summary>
        public event StorageEventHandler StorageSaved = delegate { };
        /// <summary>
        /// Raised after resetting the storage and clearing the
        /// storage directory.
        /// !!Note, on this event, the primary storage slot points
        /// to the newly-reset and empty storage image, and the 
        /// secondary to the previous version of storage image.
        /// </summary>
        public event StorageEventHandler StorageReset = delegate { };


    }
}

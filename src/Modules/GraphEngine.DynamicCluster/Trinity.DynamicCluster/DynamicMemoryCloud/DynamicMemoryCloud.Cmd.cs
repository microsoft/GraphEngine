// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using Trinity;
using Trinity.Core.Lib;
using Trinity.Network.Messaging;
using Trinity.Network;

namespace Trinity.DynamicCluster.Storage
{
    public partial class DynamicMemoryCloud
    {
        /// <summary>
        /// Dumps memory storages to disk files on all Trinity servers.
        /// </summary>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public override async Task<bool> SaveStorageAsync()
        {
            await m_backupctl.Backup();
            return true;
        }

        /// <summary>
        /// Loads Trinity key-value store from disk to main memory on all Trinity servers.
        /// </summary>
        /// <returns>true if loading succeeds; otherwise, false.</returns>
        public override async Task<bool> LoadStorageAsync()
        {
            await m_backupctl.Restore();
            return true;
        }

        /// <summary>
        /// Resets local memory storage to the initial state on all Trinity servers. The content in the memory storage will be cleared. And the memory storage will be shrunk to the initial size.
        /// </summary>
        /// <returns>true if resetting succeeds; otherwise, false.</returns>
        public override Task<bool> ResetStorageAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<long> GetTotalMemoryUsageAsync()
        {
            throw new NotImplementedException();
        }
    }
}

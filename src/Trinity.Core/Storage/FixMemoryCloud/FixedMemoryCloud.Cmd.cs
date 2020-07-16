// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Network.Messaging;

namespace Trinity.Storage
{
    public partial class FixedMemoryCloud
    {
        /// <summary>
        /// Dumps memory storages to disk files on all Trinity servers.
        /// </summary>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public override async Task<bool> SaveStorageAsync()
        {
            try
            {
                TrinityMessage msg = new TrinityMessage(TrinityMessageType.PRESERVED_SYNC, (ushort)RequestType.SaveStorage, 0);
                await Task.WhenAll(StorageTable.Select(storage => storage.SendMessageAsync(msg)));
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Loads Trinity key-value store from disk to main memory on all Trinity servers.
        /// </summary>
        /// <returns>true if loading succeeds; otherwise, false.</returns>
        public override async Task<bool> LoadStorageAsync()
        {
            try
            {
                TrinityMessage msg = new TrinityMessage(TrinityMessageType.PRESERVED_SYNC, (ushort)RequestType.LoadStorage, 0);
                await Task.WhenAll(StorageTable.Select(storage => storage.SendMessageAsync(msg)));
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Resets local memory storage to the initial state on all Trinity servers. The content in the memory storage will be cleared. And the memory storage will be shrunk to the initial size.
        /// </summary>
        /// <returns>true if resetting succeeds; otherwise, false.</returns>
        public override async Task<bool> ResetStorageAsync()
        {
            try
            {
                TrinityMessage msg = new TrinityMessage(TrinityMessageType.PRESERVED_SYNC, (ushort)RequestType.ResetStorage, 0);
                await Task.WhenAll(StorageTable.Select(storage => storage.SendMessageAsync(msg)));
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}

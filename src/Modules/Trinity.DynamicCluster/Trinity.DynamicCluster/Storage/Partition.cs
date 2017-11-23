using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Trinity;
using Trinity.Network.Messaging;
using Trinity.Diagnostics;
using System.Runtime.CompilerServices;
using Trinity.Network;
using Trinity.Utilities;
using Trinity.Storage;
using Trinity.DynamicCluster;
using Trinity.Configuration;
using System.Collections;
using System.Collections.Concurrent;

namespace Trinity.DynamicCluster.Storage
{
    using Storage = Trinity.Storage.Storage;
    /// <summary>
    /// A Partition represent multiple storages scattered across the cluster,
    /// which add up to a complete availability group (aka partition). Hence, it
    /// is both a storage, and a list of storages.
    /// </summary>
    internal unsafe partial class Partition : Storage, IEnumerable<Storage>
    {
        private ConcurrentDictionary<Storage, IEnumerable<Chunk>> m_storages;

        internal TrinityErrorCode Mount(Storage storage, _QueryChunkedRemoteStorageInformationReusltReader info)
        {
            List<Chunk> chunks = new List<Chunk>();
            info.chunks.ForEach(ci => chunks.Add(new Chunk(ci.lowKey, ci.highKey, storage, ci.id)));
            return Mount(storage, chunks);
        }

        internal TrinityErrorCode Mount(Storage storage, IEnumerable<Chunk> cc)
        {
            //bool storage_exists = m_chunks.ContainsKey(storage);
            m_storages[storage] = cc;
            return TrinityErrorCode.E_SUCCESS;
        }

        internal TrinityErrorCode Unmount(Storage s)
        {
            try
            {
                m_storages.TryRemove(s, out var _);
                return TrinityErrorCode.E_SUCCESS;
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogLevel.Error, "ChunkedStorage: Errors occurred during Unmount.");
                Log.WriteLine(LogLevel.Error, ex.ToString());
                return TrinityErrorCode.E_FAILURE;
            }
        }

        public override void Dispose()
        {
            m_storages.ForEach(kvp => kvp.Key.Dispose());
        }

        internal bool IsLocal(long cellId)
        {
            return PickStorages(cellId).Any(s => s == Global.LocalStorage);
        }

        /// <summary>
        /// Returns the storages that cover the cellId
        /// </summary>
        /// <param name="cellId"></param>
        /// <returns></returns>
        private IEnumerable<Storage> PickStorages(long cellId)
        {
            return this.Where(s => m_storages[s].Any(c => c.Covers(cellId)));
        }

        #region IEnumerable
        public IEnumerator<Storage> GetEnumerator()
        {
            return m_storages.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}

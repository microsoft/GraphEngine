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
    /// <summary>
    /// A Partition represent multiple storages scattered across the cluster,
    /// which add up to a complete availability group (aka partition). Hence, it
    /// is both a storage, and a list of storages.
    /// </summary>
    internal unsafe partial class Partition : IStorage, IEnumerable<IStorage>
    {
        private ConcurrentDictionary<IStorage, IEnumerable<Chunk>> m_storages = new ConcurrentDictionary<IStorage, IEnumerable<Chunk>>();

        internal TrinityErrorCode Mount(IStorage storage, IEnumerable<Chunk> cc)
        {
            m_storages[storage] = cc;
            return TrinityErrorCode.E_SUCCESS;
        }

        internal TrinityErrorCode Unmount(IStorage s)
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

        internal bool IsLocal(long cellId)
        {
            return PickStorages(cellId).Any(s => s == Global.LocalStorage);
        }

        /// <summary>
        /// Returns the storages that cover the cellId
        /// </summary>
        /// <param name="cellId"></param>
        /// <returns></returns>
        private IEnumerable<IStorage> PickStorages(long cellId)
        {
            return this.Where(s => m_storages[s].Any(c => c.Covers(cellId)));
        }

        #region IEnumerable
        public IEnumerator<IStorage> GetEnumerator()
        {
            return m_storages.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    m_storages.ForEach(kvp => kvp.Key.Dispose());
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}

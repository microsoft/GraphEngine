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

namespace Trinity.DynamicCluster
{
    using Storage = Trinity.Storage.Storage;
    internal unsafe partial class ChunkedStorage : Storage, IEnumerable<Storage>
    {
        // need a protocol to communicate within an AG: QueryYourChunkIds
        // we also need a protocol to "discuss" about chunk allocation. (P2)
        // route messages to a specific storage
        private List<Storage> Storages; // <----
        private Dictionary<Storage, ChunkCollection> Chunks; //<-----ChunkCollection
        DynamicMemoryCloud m_MemoryCloud; // get ChunkIds from m_MemoryCloud and decide routing.???????

        internal TrinityErrorCode Mount(Storage s, _QueryChunkedRemoteStorageInformationReusltReader info) => _Mount_impl(s, info.chunks);

        internal TrinityErrorCode Mount(Storage s, IEnumerable<int> chunks) => _Mount_impl(s, chunks);

        private TrinityErrorCode _Mount_impl(Storage storage, IEnumerable<int> cc)
        {
            if (Chunks.ContainsKey(storage))
            {
                return Chunks[storage].AddSlice(cc);
            }
            else
            {
                Storages.Add(storage);
                Chunks[storage] = new ChunkCollection(cc);
                return TrinityErrorCode.E_SUCCESS;
            }
        }

        internal TrinityErrorCode Unmount(Storage s)
        {
            try
            {
                Chunks.Remove(s);
                Storages.Remove(s);
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
            throw new NotImplementedException();
        }

        public override void SendMessage(TrinityMessage message)
        {
            throw new NotImplementedException();
        }

        public override void SendMessage(byte* message, int size)
        {
            throw new NotImplementedException();
        }

        public override void SendMessage(TrinityMessage message, out TrinityResponse response)
        {
            throw new NotImplementedException();
        }

        public override void SendMessage(byte* message, int size, out TrinityResponse response)
        {
            throw new NotImplementedException();
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
            return this.Where(s => Chunks[s].Covers(cellId));
        }

        public ChunkCollection QueryChunkCollection(Storage storage)
        {
            return Chunks[storage];
        }

        public IEnumerable<Storage> QueryRemoteStorage(IEnumerable<int> cc)
        {
            return Chunks.Where(s => Enumerable.SequenceEqual(s.Value, cc)).Select(s => s.Key);
        }

        #region IEnumerable
        public IEnumerator<Storage> GetEnumerator()
        {
            return Storages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Storages.GetEnumerator();
        }
        #endregion
    }

    internal class ChunkCollection : IEnumerable<int>
    {
        private HashSet<int> m_list;

        public ChunkCollection(IEnumerable<int> chunks)
        {
            m_list = new HashSet<int>(chunks);
        }

        internal bool Covers(long cellId)
        {
            int chunk_id = DynamicMemoryCloud.Instance.GetChunkIdByCellId(cellId);
            return m_list.Contains(chunk_id);
        }

        internal TrinityErrorCode AddSlice(IEnumerable<int> collection)
        {
            foreach (var c in collection)
            {
                m_list.Add(c);
            }
            return TrinityErrorCode.E_SUCCESS;
        }

        /// <summary>
        /// A = A - B
        /// A is current collection
        /// B is collection
        /// </summary>
        internal TrinityErrorCode RemoveSlice(IEnumerable<int> collection)
        {
            foreach (var c in collection)
            {
                m_list.Remove(c);
            }
            return TrinityErrorCode.E_SUCCESS;
        }

        public IEnumerator<int> GetEnumerator()
        {
            return m_list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_list.GetEnumerator();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.DynamicCluster.Consensus;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Storage
{
    using Storage = Trinity.Storage.Storage;
    /// <summary>
    /// Provides information about chunks, storages and replicas.
    /// </summary>
    internal class CloudIndex : IDisposable
    {
        private DynamicMemoryCloud                     m_dmc;
        private Dictionary<Guid, Storage>              m_storagetab;
        private Dictionary<Guid, IEnumerable<Chunk>>   m_ctcache;
        private INameService                           m_nameservice;
        private IChunkTable                            m_chunktable;
        private CancellationToken                      m_cancel;
        private IEnumerable<ReplicaInformation>[]      m_replicaList;
        private Task                                   m_ctupdateproc;
        private Task                                   m_scanproc;
        private SemaphoreSlim                          m_ctupdate_sem = new SemaphoreSlim(0);

        public IEnumerable<Chunk> MyChunks => GetChunks(m_nameservice.InstanceId);
        public IEnumerable<ReplicaInformation> MyPartitionReplicas => m_replicaList[m_nameservice.PartitionId];
        public int MyPartitionId => m_nameservice.PartitionId;

        public IEnumerable<Chunk> GetChunks(Guid id)
        {
            lock (m_ctcache)
            {
                if (m_ctcache.TryGetValue(id, out var ret)) return ret;
                else return Enumerable.Empty<Chunk>();
            }
        }

        public void SetChunks(Guid id, IEnumerable<Chunk> chunks)
        {
            lock (m_ctcache)
            {
                if (chunks != null) { m_ctcache[id] = chunks; }
                else { m_ctcache.Remove(id); }
            }
        }

        public Storage GetStorage(Guid id)
        {
            lock (m_storagetab)
            {
                if (m_storagetab.TryGetValue(id, out var ret)) return ret;
                else return null;
            }
        }

        public void SetStorage(Guid id, Storage storage)
        {
            lock (m_storagetab)
            {
                if (storage != null) { m_storagetab[id] = storage; }
                else { m_storagetab.Remove(id); }
            }
        }

        public async Task<IEnumerable<(ReplicaInformation rep, IEnumerable<Chunk> cks)>> GetMyPartitionReplicaChunks()
        {
            await m_ctupdate_sem.WaitAsync();
            return MyPartitionReplicas.Join(m_ctcache, r => r.Id, kvp => kvp.Key, (r, kvp) => (r, kvp.Value));
        }

        public CloudIndex(CancellationToken token, INameService namesvc, IChunkTable chunktable)
        {
            m_dmc            = DynamicMemoryCloud.Instance;
            m_nameservice    = namesvc;
            m_chunktable     = chunktable;
            m_cancel         = token;
            m_storagetab     = new Dictionary<Guid, Storage>();
            m_ctcache        = new Dictionary<Guid, IEnumerable<Chunk>>();
            SetStorage(m_nameservice.InstanceId, Global.LocalStorage);
            m_replicaList    = Utils.Integers(m_nameservice.PartitionCount).Select(_ => Enumerable.Empty<ReplicaInformation>()).ToArray();
            m_ctupdateproc   = Utils.Daemon(m_cancel, "ChunkTableUpdaterProc", 10000, ChunkTableUpdaterProc);
            m_scanproc       = Utils.Daemon(m_cancel, "ScanNodesProc", 10000, ScanNodesProc);
        }

        private void UpdatePartition(IEnumerable<ReplicaInformation> rps)
        {
            if (!rps.Any()) return;
            var pid = rps.First().PartitionId;
            var oldset = m_replicaList[pid];
            var newset = rps.ToList();
            foreach (var r in newset.Except(oldset))
            {
                if (r.Address == m_nameservice.Address && r.Port == m_nameservice.Port) continue;
                Log.WriteLine("{0}", $"{nameof(CloudIndex)}: {r.Address}:{r.Port} ({r.Id}) added to partition {r.PartitionId}");
                DynamicRemoteStorage rs = new DynamicRemoteStorage(r, TrinityConfig.ClientMaxConn, m_dmc);
                SetStorage(r.Id, rs);
            }
            foreach (var r in oldset.Except(newset))
            {
                Log.WriteLine("{0}", $"{nameof(CloudIndex)}: {r.Address}:{r.Port} ({r.Id}) removed from partition {r.PartitionId}");
                SetStorage(r.Id, null);
                SetChunks(r.Id, null);
                if (m_nameservice.IsMaster && m_nameservice.PartitionId == r.PartitionId)
                { m_chunktable.DeleteEntry(r.Id); }
            }
            m_replicaList[pid] = newset;
        }

        /// <summary>
        /// ScanNodesProc periodically polls instances from the name service.
        /// ScanNodesProc runs on every replica.
        /// !Note, ScanNodesProc does not reports the current instance.
        /// Only remote instances are reported.
        /// </summary>
        private async Task ScanNodesProc()
        {
            await Utils.Integers(m_nameservice.PartitionCount)
                       .Select(m_nameservice.ResolvePartition)
                       .Then(UpdatePartition)
                       .WhenAll();
        }

        /// <summary>
        /// ChunkTableUpdaterProc periodically polls chunk tables passively.
        /// ChunkTableUpdaterProc runs on every replica.
        /// </summary>
        private async Task ChunkTableUpdaterProc()
        {
            bool updated = false;
            foreach (var r in m_replicaList.SelectMany(Utils.Identity))
            {
                var stg = GetStorage(r.Id);
                if (null == stg) { continue; }
                var ct = await m_chunktable.GetChunks(r);
                ct = ct.OrderBy(_ => _.LowKey);
                IEnumerable<Chunk> ctcache = GetChunks(r.Id);
                if (Enumerable.SequenceEqual(ctcache, ct)) { continue; }
                SetChunks(r.Id, ct);
                m_dmc.PartitionTable(r.PartitionId).Mount(stg, ct);
                var nickname = (stg as DynamicRemoteStorage)?.NickName ?? m_dmc.NickName;
                Log.WriteLine("{0}: {1}", nameof(ChunkTableUpdaterProc), $"Replica {nickname}: chunk table updated.");
                updated = true;
            }
            if (updated)
            {
                m_ctupdate_sem.Release();
            }
        }

        public void Dispose()
        {
            m_scanproc.Wait();
            m_ctupdateproc.Wait();
        }
    }
}

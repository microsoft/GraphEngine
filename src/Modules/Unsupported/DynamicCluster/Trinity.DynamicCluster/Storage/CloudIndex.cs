using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.DynamicCluster.Communication;
using Trinity.DynamicCluster.Consensus;
using Trinity.DynamicCluster.DynamicCluster;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Storage
{
    /// <summary>
    /// Provides information about chunks, storages and replicas.
    /// </summary>
    internal class CloudIndex : IDisposable
    {
        private Dictionary<Guid, IStorage>             m_storagetab;
        private Dictionary<Guid, IEnumerable<Chunk>>   m_ctcache;
        private INameService                           m_nameservice;
        private IChunkTable                            m_chunktable;
        private CancellationToken                      m_cancel;
        private IEnumerable<ReplicaInformation>[]      m_replicaList;
        private IStorage[]                             m_masters;
        private Task                                   m_ctupdateproc;
        private Task                                   m_masterproc;
        private Task                                   m_scanproc;
        private MemoryCloud                            m_mc;
        private string                                 m_myname;
        private Func<int, Partition>                   m_partitions;

        public IEnumerable<Chunk> MyChunks => GetChunks(m_nameservice.InstanceId);
        public IEnumerable<ReplicaInformation> MyPartitionReplicas => m_replicaList[m_nameservice.PartitionId];
        public int MyPartitionId => m_nameservice.PartitionId;

        public CloudIndex(CancellationToken token, INameService namesvc, IChunkTable ctable, MemoryCloud mc, string nickname, Func<int, Partition> ptable)
        {
            m_mc             = mc;
            m_myname         = nickname;
            m_partitions     = ptable;
            m_nameservice    = namesvc;
            m_chunktable     = ctable;
            m_cancel         = token;
            m_storagetab     = new Dictionary<Guid, IStorage>();
            m_ctcache        = new Dictionary<Guid, IEnumerable<Chunk>>();
            m_masters        = new IStorage[m_nameservice.PartitionCount];
            m_replicaList    = Utils.Integers(m_nameservice.PartitionCount).Select(_ => Enumerable.Empty<ReplicaInformation>()).ToArray();
            m_ctupdateproc   = Utils.Daemon(m_cancel, "ChunkTableUpdateProc", 10000, ChunkTableUpdateProc);
            m_masterproc     = Utils.Daemon(m_cancel, "MasterNotifyProc", 10000, MasterNotifyProc);
            m_scanproc       = Utils.Daemon(m_cancel, "ScanNodesProc", 10000, ScanNodesProc);
        }

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

        public IStorage GetStorage(Guid id)
        {
            lock (m_storagetab)
            {
                if (m_storagetab.TryGetValue(id, out var ret)) return ret;
                else return null;
            }
        }

        public void SetStorage(Guid id, IStorage storage)
        {
            lock (m_storagetab)
            {
                if (storage != null) { m_storagetab[id] = storage; }
                else { m_storagetab.Remove(id); }
            }
        }

        public IEnumerable<(ReplicaInformation rep, IEnumerable<Chunk> cks)> GetMyPartitionReplicaChunks()
        {
            return
            from rep in MyPartitionReplicas
            join cks in m_ctcache on rep.Id equals cks.Key into gj
            from subcks in gj.DefaultIfEmpty()
            select (rep, subcks.Value ?? Enumerable.Empty<Chunk>());
        }

        private void UpdatePartition(IEnumerable<ReplicaInformation> rps)
        {
            if (!rps.Any()) return;
            var pid = rps.First().PartitionId;
            var oldset = m_replicaList[pid];
            var newset = rps.ToList();
            foreach (var r in newset.Except(oldset))
            {
                Log.WriteLine("{0}", $"{nameof(CloudIndex)}: {r.Address}:{r.Port} ({r.Id}) added to partition {r.PartitionId}");
                IStorage storage = null;
                if (r.Id == m_nameservice.InstanceId)
                {
                    storage = Global.LocalStorage;
                }
                else
                {
                    storage = new DynamicRemoteStorage(r, TrinityConfig.ClientMaxConn, m_mc);
                }
                SetStorage(r.Id, storage);
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
        /// !Note, ScanNodesProc does not report the current instance.
        /// Only remote instances are reported.
        /// </summary>
        private async Task ScanNodesProc()
        {
            await Utils.Integers(m_nameservice.PartitionCount)
                       .Select(m_nameservice.ResolvePartition)
                       .Then(UpdatePartition)
                       .WhenAll();
        }

        private async Task MasterNotifyProc()
        {
            if (!m_nameservice.IsMaster) return;
            using(var req = new StorageInformationWriter(MyPartitionId, m_nameservice.InstanceId))
            {
                var rsps = await Utils.Integers(m_nameservice.PartitionCount)
                                .Select(i => m_partitions(i)
                                .Broadcast(p => p.AnnounceMaster(req)))
                                .Unwrap();
                rsps.SelectMany(_ => _).ForEach(_ => _.Dispose());
            }
        }

        /// <summary>
        /// ChunkTableUpdateProc periodically polls states (chunk tables, etc.) passively.
        /// ChunkTableUpdateProc runs on every replica.
        /// </summary>
        private async Task ChunkTableUpdateProc()
        {
            IEnumerable<Chunk> pulledct = null;
            IStorage stg = null;
            foreach (var r in m_replicaList.SelectMany(Utils.Identity))
            {
                stg = GetStorage(r.Id);
                if (null == stg) { continue; }
                CancellationTokenSource tsrc = new CancellationTokenSource(1000);
                try { pulledct = await _GetChunks(stg).WaitAsync(tsrc.Token); }
                catch (OperationCanceledException) { continue; }
                pulledct = pulledct.OrderBy(_ => _.LowKey);
                IEnumerable<Chunk> ctcache = GetChunks(r.Id);
                if (Enumerable.SequenceEqual(ctcache, pulledct)) { continue; }
                SetChunks(r.Id, pulledct);
                m_partitions(r.PartitionId).Mount(stg, pulledct);
                var nickname = (stg as DynamicRemoteStorage)?.NickName ?? m_myname;
                Log.WriteLine("{0}: {1}", nameof(ChunkTableUpdateProc), $"Replica {nickname}: chunk table updated.");
            }
        }

        internal void SetMaster(int partition, Guid replicaId)
        {
            if (replicaId == m_nameservice.InstanceId) m_masters[partition] = Global.LocalStorage;
            else m_masters[partition] = m_partitions(partition).OfType<DynamicRemoteStorage>().FirstOrDefault(_ => _.ReplicaInformation.Id == replicaId);
        }

        internal IStorage GetMaster(int partition)
        {
            return m_masters[partition];
        }

        internal IEnumerable<IStorage> GetMasters()
        {
            if (!m_nameservice.IsMaster) throw new NotMasterException();
            var masters = Utils.Integers(m_nameservice.PartitionCount).Select(GetMaster);
            if (masters.Any(_ => _ == null)) throw new NoSuitableReplicaException("One or more partition masters not found.");
            return masters;
        }

        private async Task<IEnumerable<Chunk>> _GetChunks(IStorage storage)
        {
            List<Chunk> chunks = new List<Chunk>();
            using (var rsp = await storage.GetChunks())
            {
                foreach(var ci in rsp.chunk_info)
                {
                    chunks.Add(new Chunk(ci.lowKey, ci.highKey, ci.id));
                }
            }
            return chunks;
        }

        public void Dispose()
        {
            m_masterproc.Wait();
            m_scanproc.Wait();
            m_ctupdateproc.Wait();
        }
    }
}

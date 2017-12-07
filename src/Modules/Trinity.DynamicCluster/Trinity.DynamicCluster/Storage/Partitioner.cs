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
    /// Partitioner handles replica information consumption,
    /// data replication and chunk management.
    /// </summary>
    internal class Partitioner : IDisposable
    {
        private CancellationToken                      m_cancel;
        private DynamicMemoryCloud                     m_dmc;
        private IChunkTable                            m_chunktable;
        private IEnumerable<ReplicaInformation>[]      m_replicaList;
        private INameService                           m_nameservice;
        private ITaskQueue                             m_taskqueue;
        private IHealthManager                         m_healthmgr;
        private ReplicationMode                        m_repmode;
        private int                                    m_redundancy;
        private Task                                   m_partitionproc;
        private Task                                   m_scanproc;
        private Task                                   m_replicatorproc;
        private Task                                   m_parthealthproc;
        private Func<Guid, IEnumerable<Chunk>>         m_ctcache_get;
        private Action<Guid, IEnumerable<Chunk>>       m_ctcache_set;
        private Func<Guid, Storage>                    m_stg_get;
        private Action<Guid, Storage>                  m_stg_set;

        public IEnumerable<Chunk> MyChunks => m_ctcache_get(m_nameservice.InstanceId);

        public Partitioner(CancellationToken token, IChunkTable chunktable, INameService nameservice, ITaskQueue taskqueue, IHealthManager healthmgr, ReplicationMode replicationMode, int minimalRedundancy)
        {
            m_dmc            = DynamicMemoryCloud.Instance;
            m_cancel         = token;
            m_nameservice    = nameservice;
            m_chunktable     = chunktable;
            m_taskqueue      = taskqueue;
            m_healthmgr      = healthmgr;
            m_repmode        = replicationMode;
            m_redundancy     = minimalRedundancy;

            var storagetab   = new Dictionary<Guid, Storage>();
            var ctcache      = new Dictionary<Guid, IEnumerable<Chunk>>();
            m_ctcache_get    = id => { lock (ctcache) { if (ctcache.TryGetValue(id, out var ret)) return ret; else return Enumerable.Empty<Chunk>(); } };
            m_ctcache_set    = (id, ct) => { lock (ctcache) { if (ct != null) { ctcache[id] = ct; } else { ctcache.Remove(id); } } };
            m_stg_get        = id => { lock (storagetab) { if (storagetab.TryGetValue(id, out var ret)) return ret; else return null; } };
            m_stg_set        = (id, ct) => { lock (storagetab) { if (ct != null) { storagetab[id] = ct; } else { storagetab.Remove(id); } } };
            m_stg_set(m_nameservice.InstanceId, Global.LocalStorage);

            m_replicaList    = Utils.Integers(m_nameservice.PartitionCount).Select(_ => Enumerable.Empty<ReplicaInformation>()).ToArray();
            m_partitionproc  = Utils.Daemon(m_cancel, "PartitionerProc", 10000, PartitionerProc);
            m_scanproc       = Utils.Daemon(m_cancel, "ScanNodesProc", 10000, ScanNodesProc);
            m_replicatorproc = Utils.Daemon(m_cancel, "ReplicatorProc", 10000, ReplicatorProc);
            m_parthealthproc = Utils.Daemon(m_cancel, "PartitionHealthMonitor", 20000, PartitionHealthMonitorProc);
        }

        private void UpdatePartition(int partitionId, Task<IEnumerable<ReplicaInformation>> resolveTask)
        {
            var oldset = m_replicaList[partitionId];
            var newset = resolveTask.Result;
            foreach (var r in newset.Except(oldset))
            {
                if (r.Address == m_nameservice.Address && r.Port == m_nameservice.Port) continue;
                Log.WriteLine("{0}", $"Partitioner: {r.Address}:{r.Port} ({r.Id}) added to partition {r.PartitionId}");
                DynamicRemoteStorage rs = new DynamicRemoteStorage(r, TrinityConfig.ClientMaxConn, m_dmc);
                m_stg_set(r.Id, rs);
            }
            foreach (var r in oldset.Except(newset))
            {
                Log.WriteLine("{0}", $"Partitioner: {r.Address}:{r.Port} ({r.Id}) removed from partition {r.PartitionId}");
                m_stg_set(r.Id, null);
                m_ctcache_set(r.Id, null);
                if (m_chunktable.IsMaster && m_nameservice.PartitionId == r.PartitionId)
                { m_chunktable.DeleteEntry(r.Id); }
            }
            m_replicaList[partitionId] = newset;
        }

        /// <summary>
        /// !Note, ScanNodesProc does not reports the current instance.
        /// Only remote instances are reported.
        /// </summary>
        private async Task ScanNodesProc()
        {
            var ids = Utils.Integers(m_nameservice.PartitionCount);
            var tasks = ids.Select(m_nameservice.ResolvePartition);
            await Task.WhenAll(tasks);
            tasks.ForEach(UpdatePartition);
        }

        /// <summary>
        /// PartitionerProc periodically polls chunk tables passively
        /// </summary>
        private async Task PartitionerProc()
        {
            foreach (var r in m_replicaList.SelectMany(Utils.Identity))
            {
                var stg = m_stg_get(r.Id);
                if (null == stg) { continue; }
                var ct = await m_chunktable.GetChunks(r);
                IEnumerable<Chunk> ctcache = m_ctcache_get(r.Id);
                if (Enumerable.SequenceEqual(
                    ctcache.OrderBy(_ => _.LowKey),
                    ct.OrderBy(_ => _.LowKey)))
                { continue; }
                m_ctcache_set(r.Id, ct);
                m_dmc.PartitionTable(r.PartitionId).Mount(stg, ct);
            }
        }

        /// <summary>
        /// ReplicatorProc runs on the leader of each partition, and
        /// initiates/monitors replication tasks. When replication is
        /// done, it updates the chunk table.
        /// </summary>
        private async Task ReplicatorProc()
        {
            if (!m_taskqueue.IsMaster) return;
            var rpg = m_replicaList[m_nameservice.PartitionId];
            var cks = rpg.Select(m_chunktable.GetChunks).ToArray();
            await Task.WhenAll(cks);
            var replica_chunks = rpg.Zip(cks, (r, t) => (r, t.Result));

            // If there's nothing in the chunk table yet, an initialization is needed.
            if (!cks.SelectMany(_ => _.Result).Any())
            {
                await InitChunkTableAsync();
            }

            //foreach (var (r, cs) in replica_chunks)
            //{

            //}

            // TODO find replication tasks based on replication mode.
            // TODO updates secondaries on replication completion
        }

        private async Task InitChunkTableAsync()
        {
            Log.WriteLine($"Replicator: Partition {m_nameservice.PartitionId}: Initializing chunk table.");
            switch (m_repmode)
            {
                case ReplicationMode.Mirroring:
                break;
                case ReplicationMode.Sharding:
                break;
                case ReplicationMode.MirroredSharding:
                break;
                case ReplicationMode.DistributedHashTable:
                break;
            }
        }

        private async Task PartitionHealthMonitorProc()
        {
            if (!m_healthmgr.IsMaster) return;
            var rpg = m_replicaList[m_nameservice.PartitionId];
            var cached_chunks = rpg.SelectMany(r => m_ctcache_get(r.Id)).OrderBy(c => c.LowKey);
            var cnt_dict = cached_chunks.Aggregate(new Dictionary<Chunk, int>(), (dict, c) =>
            {
                if (!dict.ContainsKey(c)) dict.Add(c, 1);
                else dict[c]++;
                return dict;
            });

            // 1. Range coverage check -- whether the whole address space is covered by the chunks.
            // 2. Chunk overlap check  -- overlapped chunks indicate corruptions.
            // 3. Redundancy check     -- whether the chunks meet the redundancy requirements.
            var chunks = cnt_dict.Keys.OrderBy(_ => _.LowKey).Distinct();
            if (!chunks.Any()) { return; }
            // By ensuring neighborhood between two chunks we achieve both 1) and 2).
            foreach(var (prev, cur) in chunks.Skip(1).Concat(chunks.Take(1)).Zip(chunks, (prev, cur) => (prev, cur)))
            {
                if(prev.HighKey + 1 != cur.LowKey)
                {
                    await m_healthmgr.ReportPartitionStatus(
                        Health.HealthStatus.Error, 
                        $"Partition {m_nameservice.PartitionId} chunk table misalignment between " + 
                        $"[{prev.LowKey}-{prev.HighKey}] and [{cur.LowKey}-{cur.HighKey}]");
                    return;
                }
                if(cur.HighKey < cur.LowKey)
                {
                    await m_healthmgr.ReportPartitionStatus(
                        Health.HealthStatus.Error, 
                        $"Partition {m_nameservice.PartitionId} chunk information corrupted:" + 
                        $"[{cur.LowKey}-{cur.HighKey}]");
                    return;
                }
            }
            if(cnt_dict.Values.Min() < m_redundancy)
            {
                await m_healthmgr.ReportPartitionStatus(Health.HealthStatus.Warning, $"Partition {m_nameservice.PartitionId} has not reached required redundancy.");
                return;
            }

            await m_healthmgr.ReportPartitionStatus(Health.HealthStatus.Healthy, $"Partition {m_nameservice.PartitionId} is healthy.");
        }

        public void Dispose()
        {
            m_scanproc.Wait();
            m_partitionproc.Wait();
            m_replicatorproc.Wait();
            m_parthealthproc.Wait();
        }
    }
}

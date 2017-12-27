using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.DynamicCluster.Consensus;
using Trinity.DynamicCluster.Tasks;
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
            Log.WriteLine($"{nameof(Partitioner)}: Initializing. ReplicationMode={replicationMode}, MinimumReplica={minimalRedundancy}");

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

        private void UpdatePartition(IEnumerable<ReplicaInformation> rps)
        {
            if (!rps.Any()) return;
            var pid = rps.First().PartitionId;
            var oldset = m_replicaList[pid];
            var newset = rps.ToList();
            foreach (var r in newset.Except(oldset))
            {
                if (r.Address == m_nameservice.Address && r.Port == m_nameservice.Port) continue;
                Log.WriteLine("{0}", $"{nameof(Partitioner)}: {r.Address}:{r.Port} ({r.Id}) added to partition {r.PartitionId}");
                DynamicRemoteStorage rs = new DynamicRemoteStorage(r, TrinityConfig.ClientMaxConn, m_dmc);
                m_stg_set(r.Id, rs);
            }
            foreach (var r in oldset.Except(newset))
            {
                Log.WriteLine("{0}", $"{nameof(Partitioner)}: {r.Address}:{r.Port} ({r.Id}) removed from partition {r.PartitionId}");
                m_stg_set(r.Id, null);
                m_ctcache_set(r.Id, null);
                if (m_chunktable.IsMaster && m_nameservice.PartitionId == r.PartitionId)
                { m_chunktable.DeleteEntry(r.Id); }
            }
            m_replicaList[pid] = newset;
        }

        /// <summary>
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
        /// PartitionerProc periodically polls chunk tables passively
        /// </summary>
        private async Task PartitionerProc()
        {
            foreach (var r in m_replicaList.SelectMany(Utils.Identity))
            {
                var stg = m_stg_get(r.Id);
                if (null == stg) { continue; }
                var ct = await m_chunktable.GetChunks(r);
                ct = ct.OrderBy(_ => _.LowKey);
                IEnumerable<Chunk> ctcache = m_ctcache_get(r.Id);
                if (Enumerable.SequenceEqual(ctcache, ct)) { continue; }
                m_ctcache_set(r.Id, ct);
                m_dmc.PartitionTable(r.PartitionId).Mount(stg, ct);
                var nickname = (stg as DynamicRemoteStorage)?.NickName ?? m_dmc.NickName;
                Log.WriteLine("{0}: {1}", nameof(PartitionerProc), $"Replica {nickname}: chunk table updated.");
            }
        }

        /// <summary>
        /// ReplicatorProc runs on the leader of each partition, and
        /// initiates/monitors replication/chunk init/persistency tasks. When replication is
        /// done, it updates the chunk table.
        /// </summary>
        private async Task ReplicatorProc()
        {
            if (!m_taskqueue.IsMaster) return;
            var rpg = m_replicaList[m_nameservice.PartitionId].ToList();
            if (rpg.Count == 0) return;
            var cks = rpg.Select(m_chunktable.GetChunks).Unwrap();

            await Task.WhenAll(
                m_taskqueue.Wait(ReplicatorTask.Guid),
                m_taskqueue.Wait(UpdateChunkTableTask.Guid),
                m_taskqueue.Wait(PersistedSaveTask.Guid),
                cks);

            var replica_chunks = rpg.ZipWith(cks.Result);
            var exists_unmounted_replicas = replica_chunks.Where(p => !p.Item2.Any()).Select(p => p.Item1).Any();
            if (exists_unmounted_replicas)
            {
                await _UpdateChunkTableAsync(replica_chunks);
                return;
            }

            //foreach (var (r, cs) in replica_chunks)
            //{

            //}

            // TODO find replication tasks based on replication mode.
            // TODO updates secondaries on replication completion
        }

        private async Task _UpdateChunkTableAsync(IEnumerable<(ReplicaInformation, IEnumerable<Chunk>)> rpg)
        {
            Log.WriteLine($"{nameof(ReplicatorProc)}: Partition {m_nameservice.PartitionId}: Submitting chunk table initialization task.");
            UpdateChunkTableTask task = new UpdateChunkTableTask(m_repmode, rpg);
            await m_taskqueue.PostTask(task);
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
                        m_nameservice.PartitionId,
                        $"Partition {m_nameservice.PartitionId} chunk table misalignment between " + 
                        $"[{prev.LowKey}-{prev.HighKey}] and [{cur.LowKey}-{cur.HighKey}]");
                    return;
                }
                if(cur.HighKey < cur.LowKey)
                {
                    await m_healthmgr.ReportPartitionStatus(
                        Health.HealthStatus.Error, 
                        m_nameservice.PartitionId,
                        $"Partition {m_nameservice.PartitionId} chunk information corrupted:" + 
                        $"[{cur.LowKey}-{cur.HighKey}]");
                    return;
                }
            }
            if(cnt_dict.Values.Min() < m_redundancy)
            {
                await m_healthmgr.ReportPartitionStatus(Health.HealthStatus.Warning, m_nameservice.PartitionId, $"Partition {m_nameservice.PartitionId} has not reached required redundancy.");
                return;
            }

            await m_healthmgr.ReportPartitionStatus(Health.HealthStatus.Healthy, m_nameservice.PartitionId, $"Partition {m_nameservice.PartitionId} is healthy.");
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

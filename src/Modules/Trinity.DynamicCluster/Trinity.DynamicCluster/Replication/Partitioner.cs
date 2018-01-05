using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.DynamicCluster.Consensus;
using Trinity.DynamicCluster.Storage;
using Trinity.DynamicCluster.Tasks;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Replication
{
    /// <summary>
    /// Partitioner handles replica information consumption,
    /// data replication and chunk management.
    /// </summary>
    internal class Partitioner : IDisposable
    {
        private static Dictionary<ReplicationMode, IReplicationPlanner> s_planners = new Dictionary<ReplicationMode, IReplicationPlanner>
        {
            { ReplicationMode.Mirroring, new MirroringPlanner() },
            { ReplicationMode.Sharding, new ShardingPlanner() },
            { ReplicationMode.MirroredSharding, new MirrorShardingPlanner() },
            { ReplicationMode.Unrestricted, new UnrestrictedPlanner() },
        };

        private CancellationToken m_cancel;
        private CloudIndex        m_idx;
        private ITaskQueue        m_taskqueue;
        private ReplicationMode   m_repmode;
        private int               m_minreplicas;
        private Task              m_partitionerproc;

        public Partitioner(CancellationToken token, CloudIndex idx, ITaskQueue taskqueue, ReplicationMode replicationMode, int minimalRedundancy)
        {
            Log.WriteLine($"{nameof(Partitioner)}: Initializing. ReplicationMode={replicationMode}, MinimumReplica={minimalRedundancy}");

            m_cancel          = token;
            m_idx             = idx;
            m_taskqueue       = taskqueue;
            m_repmode         = replicationMode;
            m_minreplicas     = minimalRedundancy;
            m_partitionerproc = Utils.Daemon(m_cancel, "PartitionerProc", 10000, PartitionerProc);
        }

        /// <summary>
        /// PartitionerProc runs on the leader of each partition.
        /// </summary>
        private async Task PartitionerProc()
        {
            if (!m_taskqueue.IsMaster) return;
            await Task.WhenAll(
                m_taskqueue.Wait(ReplicatorTask.Guid),
                m_taskqueue.Wait(UpdateChunkTableTask.Guid),
                m_taskqueue.Wait(PersistedSaveTask.Guid));

            var replica_chunks = await m_idx.GetMyPartitionReplicaChunks();
            if (replica_chunks.Count(p => p.cks.Any()) < m_minreplicas)
            {
                Log.WriteLine($"{nameof(PartitionerProc)}: waiting for {m_minreplicas} ready replicas to conduct partitioning");
                return;
            }

            IEnumerable<ReplicatorTask> plan = s_planners[m_repmode].Plan(m_minreplicas, replica_chunks);

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
            Log.WriteLine($"{nameof(PartitionerProc)}: Partition {m_idx.MyPartitionId}: Submitting chunk table initialization task.");
            UpdateChunkTableTask task = new UpdateChunkTableTask(m_repmode, rpg);
            await m_taskqueue.PostTask(task);
        }

        public void Dispose()
        {
            m_partitionerproc.Wait();
        }
    }
}

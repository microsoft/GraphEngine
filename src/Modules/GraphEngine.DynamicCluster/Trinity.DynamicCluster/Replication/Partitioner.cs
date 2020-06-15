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
        private INameService      m_namesvc;
        private ReplicationMode   m_repmode;
        private int               m_minreplicas;
        private Task              m_partitionerproc;

        public Partitioner(CancellationToken token, CloudIndex idx, INameService namesvc, ITaskQueue taskqueue, ReplicationMode replicationMode, int minimalRedundancy)
        {
            Log.WriteLine($"{nameof(Partitioner)}: Initializing. ReplicationMode={replicationMode}, MinimumReplica={minimalRedundancy}");

            m_cancel          = token;
            m_idx             = idx;
            m_namesvc         = namesvc;
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
            if (!m_namesvc.IsMaster) return;
            await Task.WhenAll(
                m_taskqueue.Wait(ReplicatorTask.Guid),
                m_taskqueue.Wait(ShrinkDataTask.Guid),
                m_taskqueue.Wait(PersistedSaveTask.Guid));

            var replica_chunks = m_idx.GetMyPartitionReplicaChunks().ToList();
            if (ClusterInitInProgress(replica_chunks))
            {
                Log.WriteLine($"{nameof(Partitioner)}: waiting for {m_minreplicas} ready replicas to conduct partitioning");
                return;
            }

            if (NeedRepartition(replica_chunks))
            {
                Log.WriteLine($"{nameof(Partitioner)}: Repartition initiated for partition #{m_namesvc.PartitionId}.");
                IEnumerable<ITask> plan = s_planners[m_repmode].Plan(m_minreplicas, replica_chunks);
                // Replication tasks can be done in parallel, and shrink tasks too.
                // However, together they form a multi-stage task -- no shrink task should happen
                // before all rep tasks are done.
                var rep_tasks = plan.OfType<ReplicatorTask>();
                var shr_tasks = plan.OfType<ShrinkDataTask>();
                var chain = new List<ITask>();

                var repTasks = rep_tasks.ToList();

                if (repTasks.Any())
                {
                    chain.Add(new GroupedTask(repTasks, ReplicatorTask.Guid));
                }

                var shrinkDataTasks = shr_tasks.ToList();

                if (shrinkDataTasks.Any())
                {
                    chain.Add(new GroupedTask(shrinkDataTasks, ShrinkDataTask.Guid));
                }

                bool any = chain.Any();

                if (!any) return;

                var ctask = new ChainedTasks(chain, ReplicatorTask.Guid);

                await m_taskqueue.PostTask(ctask);

                return;
            }

            //TODO load balance
        }

        private bool ClusterInitInProgress(List<(ReplicaInformation rep, IEnumerable<Chunk> cks)> replica_chunks)
        {
            // no rep is initialized, and the required minimum rep count is not reached.
            bool any = replica_chunks.Any(tup => tup.cks.Any());

            return !any && replica_chunks.Count < m_minreplicas;
        }

        private bool NeedRepartition(IEnumerable<(ReplicaInformation rep, IEnumerable<Chunk> cks)> replica_chunks)
        {
            return replica_chunks.Any(tup => !tup.cks.Any());
        }

        public void Dispose()
        {
            m_partitionerproc.Wait(m_cancel);
        }
    }
}

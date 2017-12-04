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
    /// <summary>
    /// Partitioner handles replica information consumption,
    /// data replication and chunk management.
    /// </summary>
    internal class Partitioner : IDisposable
    {
        private CancellationToken                 m_cancel;
        private DynamicMemoryCloud                m_dmc;
        private IChunkTable                       m_chunktable;
        private IEnumerable<ReplicaInformation>[] m_replicaList;
        private INameService                      m_nameservice;
        private ITaskQueue                        m_taskqueue;
        private ReplicationMode                   m_repmode;
        private Task                              m_partitionproc;
        private Task                              m_scanproc;

        public IEnumerable<Chunk> MyChunks => throw new NotImplementedException();

        public Partitioner(CancellationToken token, IChunkTable chunktable, INameService nameservice, ITaskQueue taskqueue, ReplicationMode replicationMode)
        {
            m_dmc           = DynamicMemoryCloud.Instance;
            m_cancel        = token;
            m_nameservice   = nameservice;
            m_chunktable    = chunktable;
            m_taskqueue     = taskqueue;
            m_replicaList   = Utils.Integers(m_nameservice.PartitionCount).Select(_ => Enumerable.Empty<ReplicaInformation>()).ToArray();
            m_repmode       = replicationMode;
            m_partitionproc = Utils.Daemon(m_cancel, "PartitionerProc", 10000, PartitionerProc);
            m_scanproc      = Utils.Daemon(m_cancel, "ScanNodesProc", 10000, ScanNodesProc);
        }

        private void UpdatePartition(int partitionId, Task<IEnumerable<ReplicaInformation>> resolveTask)
        {
            var oldset = m_replicaList[partitionId];
            var newset = resolveTask.Result;
            foreach (var r in newset.Where(rs => !oldset.Any(rs_ => rs_.Id == rs.Id)))
            {
                if (r.Address == m_nameservice.Address && r.Port == m_nameservice.Port) continue;
                Log.WriteLine("{0}", $"Partitioner: {r.Address}:{r.Port} ({r.Id}) added to partition {r.PartitionId}");
                DynamicRemoteStorage rs = new DynamicRemoteStorage(r, TrinityConfig.ClientMaxConn, m_dmc);
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

        private async Task PartitionerProc()
        {
            // TODO mount self: PartitionTable(MyPartitionId).Mount(Global.LocalStorage, MyChunks);
            // TODO mount secondaries on replication completion
            // TODO mount interface Partitioner-->dmc
            await Task.Delay(1000, m_cancel);
        }

        public void Dispose()
        {
            m_scanproc.Wait();
            m_partitionproc.Wait();
        }
    }
}

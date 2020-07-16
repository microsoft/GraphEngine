using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Storage;
using Trinity.Storage;
using Trinity.Diagnostics;
using Trinity.DynamicCluster.Persistency;
using Trinity.DynamicCluster.Communication;

namespace Trinity.DynamicCluster.Tasks
{
    [Serializable]
    internal class ReplicatorTask : ITask
    {
        public static readonly Guid Guid = new Guid("0ADA7058-AAD6-4383-95C3-3022E4DBBD50");
        private Guid m_guid = Guid.NewGuid();
        private ReplicaInformation m_from;
        private ReplicaInformation m_to;
        private List<Chunk> m_range;
        [NonSerialized]
        private DynamicMemoryCloud m_dmc;
        public ReplicatorTask(ReplicaInformation from, ReplicaInformation to, IEnumerable<Chunk> range)
        {
            m_from = from;
            m_to = to;
            m_range = range.ToList();
        }

        public Guid Id => m_guid;

        public Guid Tag => Guid;

        public async Task Execute(CancellationToken cancel)
        {
            m_dmc = DynamicMemoryCloud.Instance;
            if (m_from != null) await DoReplication();
            await DoUpdateChunkTable();
            // At this stage, the data has been safely placed into target replicas.
        }

        private async Task DoUpdateChunkTable()
        {
            var ct  = m_dmc.m_chunktable;
            //TODO race condition?
            var cks = await ct.GetChunks(m_to.Id);
            await m_dmc.m_chunktable.SetChunks(m_to.Id, cks.Concat(m_range).Distinct());
        }

        private async Task DoReplication()
        {
            var from_id = m_dmc.GetInstanceId(m_from.Id);
            var mod = m_dmc.GetCommunicationModule<DynamicClusterCommModule>();
            using (var msg = new ReplicationTaskInformationWriter(
                task_id: m_guid,
                to: new StorageInformation { id = m_to.Id, partition = m_to.PartitionId },
                range: m_range.Select(_ => new ChunkInformation { id = _.Id, highKey = _.HighKey, lowKey = _.LowKey }).ToList()))
            using (var rsp = await mod.ReplicationAsync(from_id, msg))
            {
                if (rsp.errno != Errno.E_OK) throw new Exception();
            }
        }

        public override string ToString()
        {
            var chunks = string.Join(",", m_range.Select(ChunkSerialization.ToString));
            return $"{nameof(ReplicatorTask)}: {m_from.Id} -> {m_to.Id} : [{chunks}] ";
        }
    }
}

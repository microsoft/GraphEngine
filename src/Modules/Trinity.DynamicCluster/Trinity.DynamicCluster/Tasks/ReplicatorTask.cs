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
        private DynamicMemoryCloud m_dmc;

        public ReplicatorTask(ReplicationTaskDescriptor taskdesc)
        {
            m_from = taskdesc.From;
            m_to = taskdesc.To;
            m_range = taskdesc.Range.ToList();
            m_dmc = DynamicMemoryCloud.Instance;
        }

        public Guid Id => m_guid;

        public Guid Tag => Guid;

        public async Task Execute(CancellationToken cancel)
        {
            var from_id = m_dmc.GetInstanceId(m_from.Id);

            var mod = m_dmc.GetCommunicationModule<DynamicClusterCommModule>();
            using (var msg = new ReplicationTaskInformationWriter(
                task_id: m_guid,
                to: new StorageInformation { id = m_to.Id, partition = m_to.PartitionId },
                range: m_range.Select(_ => new ChunkInformation { id = _.Id, highKey = _.HighKey, lowKey = _.LowKey }).ToList()))
            using (var rsp = await mod.Replication(from_id, msg))
            {
                if (rsp.errno != Errno.E_OK) throw new Exception();
            }
        }

        public override string ToString()
        {
            var chunks = string.Join(",", m_range.Select(ChunkSerialization.ToString));
            return $"{nameof(ReplicatorTask)}: {m_from.Id} -> {m_to.Id} : [{chunks}] ";
        }

        private Func<Trinity.Storage.Storage, bool> Is(ReplicaInformation replicaInfo) =>
            storage => (storage == Global.LocalStorage && m_dmc.InstanceGuid == replicaInfo.Id) ||
                       (storage is DynamicRemoteStorage rs && rs.ReplicaInformation == replicaInfo);
    }
}

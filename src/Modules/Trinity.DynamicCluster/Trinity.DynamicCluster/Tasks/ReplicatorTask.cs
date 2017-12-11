using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Storage;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Tasks
{
    [Serializable]
    internal class ReplicatorTask: ITask
    {
        public static readonly Guid Guid = new Guid("0ADA7058-AAD6-4383-95C3-3022E4DBBD50");
        private Guid m_guid = Guid.NewGuid();
        private ReplicaInformation m_from;
        private ReplicaInformation m_to;
        private List<Chunk> m_range;

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
        }
    }
}

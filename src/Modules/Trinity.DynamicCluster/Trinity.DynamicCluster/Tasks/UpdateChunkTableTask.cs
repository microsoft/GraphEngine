using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.DynamicCluster.Consensus;
using Trinity.DynamicCluster.Storage;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Tasks
{
    /// <summary>
    /// An UpdateChunkTableTask receives a replication mode, and a list of (replica,chunks) tuples.
    /// For the chunks with empty chunk list, it updates them according to the replication mode.
    /// </summary>
    [Serializable]
    internal class UpdateChunkTableTask : ITask
    {
        public static readonly Guid Guid = new Guid("143A2C01-939C-4A2B-92A6-3A26F9FCD38C");
        private Guid m_guid = Guid.NewGuid();
        private ReplicationMode m_repmode;
        private List<(ReplicaInformation, IEnumerable<Chunk>)> m_nonemptyreplicas;
        private List<ReplicaInformation> m_emptyreplicas;
        [NonSerialized]
        private IChunkTable m_ctable;
        [NonSerialized]
        private CancellationToken m_cancel;
        [NonSerialized]
        private DynamicMemoryCloud m_dmc;

        public UpdateChunkTableTask(ReplicationMode replicationMode, IEnumerable<(ReplicaInformation, IEnumerable<Chunk>)> replicas)
        {
            m_repmode          = replicationMode;
            m_emptyreplicas    = replicas.Where(p => !p.Item2.Any()).Select(p => p.Item1).ToList();
            m_nonemptyreplicas = replicas.Where(p => p.Item2.Any()).ToList();
        }

        public Guid Id => m_guid;

        public Guid Tag => Guid;

        public Task Execute(CancellationToken cancel)
        {
            Log.WriteLine($"{nameof(UpdateChunkTableTask)}: Initializing with mode {m_repmode}");
            m_dmc = DynamicMemoryCloud.Instance;
            m_ctable = DynamicMemoryCloud.Instance.m_chunktable;
            m_cancel = cancel;
            switch (m_repmode)
            {
                case ReplicationMode.Mirroring:
                return mirror_init();
                case ReplicationMode.Sharding:
                return sharding_init();
                case ReplicationMode.MirroredSharding:
                return mirrorshard_init();
                case ReplicationMode.Unrestricted:
                return dht_init();
                default:
                throw new ArgumentException("Invalid replication mode");
            }
        }

        private async Task mirror_init()
        {
            var frc = new Chunk[]{ Chunk.FullRangeChunk };
            await m_emptyreplicas.Select(r => m_ctable.SetChunks(r.Id, frc)).WhenAll();
        }

        private async Task sharding_init()
        {
            List<Chunk> chunks = new List<Chunk>();
            long step = (long)(ulong.MaxValue / (ulong)m_nonemptyreplicas.Count);
            long head = long.MinValue;
            for(int i = 1; i<=m_nonemptyreplicas.Count; ++i)
            {
                long tail = head + step;
                if (i == m_nonemptyreplicas.Count) tail = long.MaxValue;
                chunks.Add(new Chunk(head, tail));
                head = tail + 1;
            }
            // TODO if 
            await m_emptyreplicas.Select(r => m_ctable.SetChunks(r.Id, chunks)).WhenAll();
        }

        private async Task mirrorshard_init()
        {
            throw new NotImplementedException();
        }

        private async Task dht_init()
        {
            throw new NotImplementedException();
        }
    }
}

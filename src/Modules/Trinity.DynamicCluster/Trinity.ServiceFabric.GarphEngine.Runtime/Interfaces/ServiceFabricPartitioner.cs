using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using Trinity.DynamicCluster.Consensus;
using Trinity.Network;
using Trinity.Storage;

namespace Trinity.ServiceFabric.GarphEngine.Infrastructure.Interfaces
{
    public class ServiceFabricPartitioner : IPartitioner
    {
        // TODO chunking scheme conforming SF availability semantics
        private List<Chunk> m_chunkList = new List<Chunk> { Chunk.FullRangeChunk };
        private CancellationToken m_canceltkn;

        // TODO configurable chunking
        public int ChunkCount => m_chunkList.Count;

        public int PartitionCount => GraphEngineStatefulServiceRuntime.Instance.PartitionCount;

        public int PartitionId => GraphEngineStatefulServiceRuntime.Instance.PartitionId;

        public IEnumerable<Chunk> MyChunkList => m_chunkList;
        public IEnumerable<Chunk> GlobalChunkList => m_chunkList;

        //  Uses Trinity default partitioning method.
        public GetPartitionIdByCellIdDelegate PartitionerProc => null;

        public bool IsMaster => GraphEngineStatefulServiceRuntime.Instance?.Role == ReplicaRole.Primary;

        public event EventHandler<int> ChunkCountUpdated = delegate { };
#pragma warning disable
        public event EventHandler<int> PartitionCountUpdated = null; // Not going to happen. SF has fixed partitioning scheme.
#pragma warning restore

        public TrinityErrorCode Start(CancellationToken cancellationToken)
        {
            //TODO initialize partition metadata if necessary
            //TODO start task to watch the metadata
            m_canceltkn = cancellationToken;
            //TODO DHT
            return TrinityErrorCode.E_SUCCESS;
        }

        public void Dispose() { }

        public (Chunk c1, Chunk c2) SplitChunk(Chunk c, long splitKey)
        {
            throw new NotImplementedException();
        }
    }
}

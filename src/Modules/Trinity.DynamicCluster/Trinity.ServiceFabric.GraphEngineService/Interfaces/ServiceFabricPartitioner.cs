using Microsoft.ServiceFabric.Services.Communication.Client;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Daemon;
using Trinity.DynamicCluster.Consensus;
using Trinity.Storage;
using System.Threading;
using Trinity.Network;

namespace Trinity.ServiceFabric.Interfaces
{
    class ServiceFabricPartitioner : IPartitioner
    {
        // TODO chunking scheme conforming SF availability semantics
        private List<Chunk> m_chunkList = new List<Chunk> { Chunk.FullRangeChunk };
        private CancellationToken m_canceltkn;

        // TODO configurable chunking
        public int ChunkCount => m_chunkList.Count;

        public int PartitionCount => GraphEngineService.Instance.PartitionCount;

        public int PartitionId => GraphEngineService.Instance.PartitionId;

        public IEnumerable<Chunk> MyChunkList => m_chunkList;
        public IEnumerable<Chunk> GlobalChunkList => m_chunkList;

        //  Uses Trinity default partitioning method.
        public GetPartitionIdByCellIdDelegate PartitionerProc => null;

        public bool IsMaster => throw new NotImplementedException();

        public event EventHandler<int> ChunkCountUpdated = delegate { };
        public event EventHandler<int> PartitionCountUpdated = null; // Not going to happen. SF has fixed partitioning scheme.

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

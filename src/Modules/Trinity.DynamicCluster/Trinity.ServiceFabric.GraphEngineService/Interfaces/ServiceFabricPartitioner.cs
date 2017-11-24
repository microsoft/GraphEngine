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

namespace Trinity.ServiceFabric.Interfaces
{
    class ServiceFabricPartitioner : IPartitioner
    {
        // TODO chunking scheme conforming SF availability semantics
        private static readonly List<Chunk> s_chunkList = new List<Chunk> { Chunk.FullRangeChunk };

        // TODO configurable chunking
        public int ChunkCount => 1;

        public int PartitionCount => GraphEngineService.Instance.PartitionCount;

        public int PartitionId => GraphEngineService.Instance.PartitionId;

        public IEnumerable<Chunk> MyChunkList => s_chunkList;

        public event EventHandler<int> ChunkCountUpdated = delegate { };
        public event EventHandler<int> PartitionCountUpdated = delegate { };

        public int GetPartitionIdByCellId(long cellId)
        {
            throw new NotImplementedException();
        }

        public TrinityErrorCode Start(CancellationToken cancellationToken)
        {
            //TODO DHT
            return TrinityErrorCode.E_SUCCESS;
        }

        public void Dispose() { }
    }
}

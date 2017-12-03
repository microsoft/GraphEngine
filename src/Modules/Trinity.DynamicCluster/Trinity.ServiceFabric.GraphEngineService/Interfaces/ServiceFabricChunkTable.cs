using System;
using System.Collections.Generic;
using System.Fabric;
using Trinity.DynamicCluster.Consensus;
using Trinity.Storage;
using System.Threading;
using Trinity.Network;
using System.Threading.Tasks;

namespace Trinity.ServiceFabric.Interfaces
{
    class ServiceFabricChunkTable : IChunkTable
    {
        private CancellationToken m_canceltkn;

        public bool IsMaster => throw new NotImplementedException();

        public Task DeleteEntry(Guid replicaId)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Chunk>> GetChunks(Guid replicaId)
        {
            throw new NotImplementedException();
        }

        public Task SetChunks(Guid replicaId, IEnumerable<Chunk> chunks)
        {
            throw new NotImplementedException();
        }

        public void Start(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

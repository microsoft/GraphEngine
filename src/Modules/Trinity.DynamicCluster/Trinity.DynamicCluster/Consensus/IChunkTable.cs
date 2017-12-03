using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trinity.Network;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Consensus
{
    /// <summary>
    /// Fault-tolerant Chunk metadata storage.
    /// Each instance represents the chunk table of a partition.
    /// Only the partition leader will update the chunk table for
    /// this partition. The secondary partitions passively receive updates
    /// from the master.
    /// One partition should not access the chunk
    /// partition of another partition.
    /// </summary>
    public interface IChunkTable: IService
    {
        Task<IEnumerable<Chunk>> GetChunks(Guid replicaId);
        Task SetChunks(Guid replicaId, IEnumerable<Chunk> chunks);
        Task DeleteEntry(Guid replicaId);
    }
}

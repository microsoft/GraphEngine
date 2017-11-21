using System;
using System.Collections.Generic;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Consensus
{
    public interface IPartitioner: IDisposable
    {
        TrinityErrorCode Start();
        int ChunkCount { get; }
        int PartitionCount { get; }
        /// <summary>
        /// My partition Id.
        /// </summary>
        int PartitionId { get; }
        IEnumerable<Chunk> MyChunkList { get; }
        /// <summary>
        /// An adapter should implement its own
        /// partitioning scheme here.
        /// </summary>
        int GetPartitionIdByCellId(long cellId);

        event EventHandler<int> ChunkCountUpdated;
        event EventHandler<int> PartitionCountUpdated;
    }
}

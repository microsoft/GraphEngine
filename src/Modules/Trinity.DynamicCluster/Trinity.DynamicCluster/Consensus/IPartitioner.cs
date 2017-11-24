using System;
using System.Collections.Generic;
using Trinity.Network;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Consensus
{
    public interface IPartitioner: IService
    {
        int ChunkCount { get; }
        int PartitionCount { get; }
        /// <summary>
        /// My partition Id.
        /// </summary>
        int PartitionId { get; }
        IEnumerable<Chunk> MyChunkList { get; }
        IEnumerable<Chunk> GlobalChunkList { get; }
        (Chunk c1, Chunk c2) SplitChunk(Chunk c, long splitKey);
        /// <summary>
        /// An adapter should implement its own
        /// partitioning scheme here. If it is
        /// intended to use the default partitioning
        /// method, return null.
        /// </summary>
        GetPartitionIdByCellIdDelegate PartitionerProc { get; }

        bool IsMaster { get; }

        event EventHandler<int> ChunkCountUpdated;
        event EventHandler<int> PartitionCountUpdated;
    }
}

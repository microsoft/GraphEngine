using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Replication
{
    [Serializable]
    public enum ReplicationMode
    {
        /// <summary>
        /// Share-nothing, no replication. Maximum capacity but not fault tolerant.
        /// !Note, a partition running in <see cref="Sharding"/> mode will not reach
        /// a health status of <see cref="HealthStatus.Healthy"/>.
        /// </summary>
        Sharding,
        /// <summary>
        /// Full replication. Capacity is limited to that of the smallest replica.
        /// </summary>
        Mirroring,
        /// <summary>
        /// Replicas are divided into shards, and each shard is replicated.
        /// </summary>
        MirroredSharding,
        /// <summary>
        /// Using DHT algorithms to distribute chunks. Guarantees availability
        /// given a minimal number of operational replicas.
        /// </summary>
        Unrestricted,
    }
}

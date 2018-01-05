using System.Collections.Generic;
using Trinity.DynamicCluster.Storage;
using Trinity.DynamicCluster.Tasks;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Replication
{
    internal interface IReplicationPlanner
    {
        IEnumerable<ITask> Plan(int min_replicas, IEnumerable<(ReplicaInformation rp, IEnumerable<Chunk> cks)> current_ct);
    }
}
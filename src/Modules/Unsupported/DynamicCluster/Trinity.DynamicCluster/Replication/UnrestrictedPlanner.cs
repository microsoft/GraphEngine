using System.Collections.Generic;
using Trinity.DynamicCluster.Storage;
using Trinity.DynamicCluster.Tasks;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Replication
{
    internal class UnrestrictedPlanner : IReplicationPlanner
    {
        public IEnumerable<ITask> Plan(int min_replicas, IEnumerable<(ReplicaInformation rp, IEnumerable<Chunk> cks)> current_ct)
        {
            throw new System.NotImplementedException();
        }
    }
}
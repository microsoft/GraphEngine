using System.Collections.Generic;
using System.Linq;
using Trinity.DynamicCluster.Storage;
using Trinity.DynamicCluster.Tasks;
using Trinity.Storage;
using static Trinity.DynamicCluster.Utils;

namespace Trinity.DynamicCluster.Replication
{
    internal class MirroringPlanner : IReplicationPlanner
    {
        public IEnumerable<ITask> Plan(int min_replicas, IEnumerable<(ReplicaInformation rp, IEnumerable<Chunk> cks)> current_ct)
        {
            var frc = new Chunk[]{ Chunk.FullRangeChunk };
            var filled_rps = current_ct.Where(_ => _.cks.Any()).Select(_ => _.rp).Schedule(SchedulePolicy.RoundRobin);
            return current_ct.Where(_ => !_.cks.Any()).Zip(filled_rps, (p, from) => new ReplicatorTask(from, p.rp, frc));
        }
    }
}
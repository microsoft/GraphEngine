using System.Collections.Generic;
using System.Linq;
using Trinity.DynamicCluster.Storage;
using Trinity.DynamicCluster.Tasks;
using Trinity.Storage;
using static Trinity.DynamicCluster.Utils;

namespace Trinity.DynamicCluster.Replication
{
    internal class ShardingPlanner : IReplicationPlanner
    {
        public IEnumerable<ReplicatorTask> Plan(int min_replicas, IEnumerable<(ReplicaInformation rp, IEnumerable<Chunk> cks)> current_ct)
        {
            var allrps = current_ct.Select(_ => _.rp).ToList();
            var filled = current_ct.Where(_ => _.cks.Any());
            var existing_cks = filled.SelectMany(_ => _.cks).Distinct().ToList();
            var filledrps = filled.Select(_ => _.rp).ToList();
            if (existing_cks.Count == 0)
            {
                long step = (long)(ulong.MaxValue / (ulong)filledrps.Count);
                long head = long.MinValue;
                foreach(var (rp, i) in allrps.ZipWith(Enumerable.Range(1, allrps.Count)))
                {
                    long tail = head + step;
                    if (i == allrps.Count) tail = long.MaxValue;
                    yield return new ReplicatorTask(null, rp, new[] { new Chunk(head, tail) });
                    head = tail + 1;
                }
            }
            else
            {
                //TODO move data from filled rps to empty ones
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Trinity.DynamicCluster.Storage;
using Trinity.DynamicCluster.Tasks;
using Trinity.Storage;
using static Trinity.DynamicCluster.Utils;

namespace Trinity.DynamicCluster.Replication
{
    internal class ShardingPlanner : IReplicationPlanner
    {
        public IEnumerable<ITask> Plan(int min_replicas, IEnumerable<(ReplicaInformation rp, IEnumerable<Chunk> cks)> current_ct)
        {
            var filled = current_ct.Where(_ => _.cks.Any());
            var existing_cks = filled.SelectMany(_ => _.cks).Distinct().ToList();
            var filledrps = filled.Select(_ => _.rp).ToList();
            var emptyrps  = current_ct.Where(_ => !_.cks.Any()).Select(_ => _.rp).ToList();

            //  Each of n filled rps moves a part of data to an empty one.
            //  Each existing chunk get split into parts:
            //  [-----------Original chunk--------]
            //                  ||
            //                  \/
            //  [-Shrinked to %p-][NS1][NS2]..[NSk]
            //  Each NSi will be replicated to one of k empty replicas.
            //  p = n / (k+n)
            //  When n = 0, that there're no filled replicas, it degenerates
            //  to splitting the full range into k steps, as p = 0.

            int n = filledrps.Count;
            int k = emptyrps.Count;

            if (n == 0)
            {
                long step = (long)(ulong.MaxValue / (ulong)k);
                long head = long.MinValue;
                foreach (var (rp, i) in emptyrps.ZipWith(Enumerable.Range(1, k)))
                {
                    long tail = head + step;
                    if (i == k) tail = long.MaxValue;
                    yield return new ReplicatorTask(null, rp, new[] { new Chunk(head, tail) });
                    head = tail + 1;
                }
            }
            else
            {
                foreach (var eck in existing_cks)
                {
                    var (shrinked, moved) = _split(eck, n, k);
                    int i=0;
                    var from = current_ct.First(_ => _.cks.Contains(eck)).rp;
                    foreach (var (rp, ck) in emptyrps.ZipWith(moved))
                    {
                        yield return new ReplicatorTask(from, emptyrps[i], new[] { ck });
                        ++i;
                    }
                    yield return new ShrinkDataTask(from, new[] { (eck, shrinked) });
                }
            }
        }

        private (Chunk shrinked, IEnumerable<Chunk> moved) _split(Chunk eck, int n, int k)
        {
            var len = new BigInteger(eck.HighKey) - new BigInteger(eck.LowKey);
            var shrinked_len = (len * n)/(n+k);
            var shrinked_high = (long)(eck.LowKey + shrinked_len);
            Chunk shrinked = new Chunk(eck.LowKey, shrinked_high);
            List<Chunk> moved = new List<Chunk>();
            var step = (len - shrinked_len) / k;
            var head = new BigInteger(shrinked_high + 1);
            for (int i = 1; i<=k; ++i)
            {
                var tail = head + step;
                if (i == k) tail = eck.HighKey;
                moved.Add(new Chunk((long)head, (long)tail));
                head = tail + 1;
            }
            return (shrinked, moved);
        }
    }
}

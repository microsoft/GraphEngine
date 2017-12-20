using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Storage;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Tasks
{
    class ReplicationTaskDescriptor
    {
        public ReplicaInformation From { get; }
        public ReplicaInformation To { get; }
        public IEnumerable<Chunk> Range { get; }

        public ReplicationTaskDescriptor(ReplicaInformation from, ReplicaInformation to, IEnumerable<Chunk> range)
        {
            From = from;
            To = to;
            Range = range;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Availability
{
    public enum ClusterHealthStatus
    {
        /// <summary>
        /// At least one partition is offline.
        /// </summary>
        Red,
        /// <summary>
        /// All partitions online, but there are potential single-point-failures.
        /// </summary>
        Yellow,
        /// <summary>
        /// All partitions online, and fault tolerance is guaranteed.
        /// </summary>
        Green
    }
}

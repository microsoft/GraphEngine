using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Storage
{
    /// <summary>
    /// Represents the health status of the partition.
    /// </summary>
    public enum PartitionHealth
    {
        /// <summary>
        /// The partition is unavailable.
        /// </summary>
        Red,
        /// <summary>
        /// The partition is available, but not fault tolerant.
        /// </summary>
        Yellow,
        /// <summary>
        /// The partition is available and fault tolerant.
        /// </summary>
        Green,
    }
}

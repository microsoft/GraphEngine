using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Health
{
    /// <summary>
    /// Represents the health status of various components.
    /// </summary>
    public enum HealthStatus
    {
        /// <summary>
        /// The partition is unavailable.
        /// </summary>
        Error,
        /// <summary>
        /// The partition is available, but not fault tolerant.
        /// </summary>
        Warning,
        /// <summary>
        /// The partition is available and fault tolerant.
        /// </summary>
        Healthy,
    }
}

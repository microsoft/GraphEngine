using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Consensus
{
    public interface ILeaderElectionService: IService
    {
        /// <summary>
        /// Fired when a new instance is elected as the leader.
        /// </summary>
        event EventHandler<Guid> NewLeaderElected;
    }
}

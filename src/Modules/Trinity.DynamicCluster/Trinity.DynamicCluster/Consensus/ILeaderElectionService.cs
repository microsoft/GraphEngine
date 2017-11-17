using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Consensus
{
    public interface ILeaderElectionService
    {
        TrinityErrorCode Start(INameService service);
        TrinityErrorCode Stop();
        /// <summary>
        /// Fired when the local instance is elected as the leader.
        /// </summary>
        event EventHandler Elected;
    }
}

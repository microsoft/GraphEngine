using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Consensus
{
    interface ILeaderElectionService
    {
        TrinityErrorCode Start();
        TrinityErrorCode Stop();
    }
}

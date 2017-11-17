using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Consensus;

namespace Trinity.ServiceFabric
{
    class LeaderElectionService : ILeaderElectionService
    {
        private INameService m_namesvc;
        public TrinityErrorCode Start(INameService service)
        {
            m_namesvc = service;
            return TrinityErrorCode.E_SUCCESS;
        }

        public TrinityErrorCode Stop()
        {
            return TrinityErrorCode.E_SUCCESS;
        }

        public event EventHandler Elected = delegate { };
    }
}

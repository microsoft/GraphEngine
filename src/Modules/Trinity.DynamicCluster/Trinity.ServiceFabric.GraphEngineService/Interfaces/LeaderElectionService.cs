using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Consensus;

namespace Trinity.ServiceFabric
{
    public class LeaderElectionService : ILeaderElectionService
    {
        private INameService m_namesvc;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public event EventHandler Elected = delegate { };
    }
}

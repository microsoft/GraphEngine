using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Consensus;
using Trinity.Network;

namespace Trinity.ServiceFabric
{
    class NameService : INameService
    {
        public event EventHandler<Tuple<NameDescriptor, ServerInfo>> NewServerInfoPublished;

        public TrinityErrorCode PublishServerInfo(NameDescriptor name, ServerInfo serverInfo)
        {
            throw new NotImplementedException();
        }

        public TrinityErrorCode Start()
        {
            throw new NotImplementedException();
        }

        public TrinityErrorCode Stop()
        {
            throw new NotImplementedException();
        }
    }
}

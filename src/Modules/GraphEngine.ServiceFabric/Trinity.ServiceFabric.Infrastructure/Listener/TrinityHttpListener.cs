using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.ServiceFabric.Infrastructure;

namespace Trinity.ServiceFabric.Listener
{
    class TrinityHttpListener : GraphEngineListenerBase
    {
        public override string ListenerName => GraphEngineConstants.GraphEngineHttpListenerName;

        public override bool ListenOnSecondaries => false;

        public override string EndpointName => GraphEngineConstants.TrinityHttpProtocolEndpoint;

        public override void Configure(int port)
        {
            // Set the Http port
            TrinityConfig.HttpPort = port;
        }
    }
}

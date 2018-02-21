using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.Network;
using Trinity.ServiceFabric.Infrastructure;

namespace Trinity.ServiceFabric.Listener
{
    class NativeTrinityListener : GraphEngineListenerBase
    {
        public override string ListenerName => GraphEngineConstants.GraphEngineListenerName;

        public override bool ListenOnSecondaries => true;

        public override string EndpointName => GraphEngineConstants.TrinityProtocolEndpoint;

        public override void Configure(int port, StatefulServiceContext _)
        {
            // Let's configure the Trinity Server Configuration gotten from the Service Fabric Runtime Stateful-Service contexte
            var groupOfAvailabilityServers = TrinityConfig.CurrentClusterConfig.Servers;

            // Clear out any default configure in place!
            groupOfAvailabilityServers.Clear();
            // Now load and configure TrinityServer via dynamically acquired SF Cluster information
            groupOfAvailabilityServers.Add(new AvailabilityGroup(GraphEngineConstants.LocalAvailabilityGroup, 
                                           new ServerInfo(GraphEngineConstants.AvailabilityGroupLocalHost,
                                           port, null, LogLevel.Info)));

        }
    }
}

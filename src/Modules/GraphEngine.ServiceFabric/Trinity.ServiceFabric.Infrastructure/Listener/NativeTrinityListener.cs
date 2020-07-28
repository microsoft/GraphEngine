using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.Network;
using Trinity.ServiceFabric.Infrastructure;
using Trinity.ServiceFabric.NativeClient.Remoting;

namespace Trinity.ServiceFabric.Listener
{
    class NativeTrinityListener : GraphEngineListenerBase
    {
        public override string ListenerName => GraphEngineConstants.GraphEngineListenerName;

        public override bool ListenOnSecondaries => true;

        public override string EndpointName => GraphEngineConstants.TrinityProtocolEndpoint;

        private TrinityOverNativeTCPCommunicationService m_trinityProxy = null;

        public override void Configure(int port, StatefulServiceContext _)
        {
            // Let's configure the Trinity Server Configuration gotten from the Service Fabric Runtime Stateful-Service context
            var groupOfAvailabilityServers = TrinityConfig.CurrentClusterConfig.Servers;

            // Clear out any default configure in place!
            groupOfAvailabilityServers.Clear();
            // Now load and configure TrinityServer via dynamically acquired SF Cluster information
            groupOfAvailabilityServers.Add(new AvailabilityGroup(GraphEngineConstants.LocalAvailabilityGroup, 
                                           new ServerInfo(GraphEngineConstants.AvailabilityGroupLocalHost,
                                           port, null, LogLevel.Info)));

            Log.WriteLine(LogLevel.Info, $"{nameof(NativeTrinityListener)}: Listening on port {port}.");

            m_trinityProxy = new TrinityOverNativeTCPCommunicationService();

        }

        public override void Abort()
        {
            //m_fabListener.Abort();
            base.Abort();
        }

        public override async Task CloseAsync(CancellationToken cancellationToken)
        {
            //await m_fabListener.CloseAsync(cancellationToken);
            await base.CloseAsync(cancellationToken);
        }

        public override async Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            return await base.OpenAsync(cancellationToken);
            //return await m_fabListener.OpenAsync(cancellationToken);
        }
    }
}

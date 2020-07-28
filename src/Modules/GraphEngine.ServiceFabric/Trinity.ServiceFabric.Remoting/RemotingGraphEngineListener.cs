using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.Extension;
using Trinity.ServiceFabric.Listener;

namespace Trinity.ServiceFabric.Remoting
{
    [ExtensionPriority(100)]
    internal class RemotingGraphEngineListener : GraphEngineListenerBase
    {
        //TODO configurable endpoint name         +
        public override string EndpointName => Constants.c_RemotingEndpointName;

        public override string ListenerName => Constants.c_RemotingListenerName;

        public override bool ListenOnSecondaries => false;

        private FabricTransportServiceRemotingListener m_fabListener = null;
        private TrinityOverRemotingService m_trinityProxy = null;

        public override void Configure(int port, StatefulServiceContext ctx)
        {
            Log.WriteLine(LogLevel.Info, $"{nameof(RemotingGraphEngineListener)}: Listening on port {port}.");

            m_trinityProxy = new TrinityOverRemotingService();
            m_fabListener = new FabricTransportServiceRemotingListener(
                ctx,
                m_trinityProxy,
                new FabricTransportRemotingListenerSettings
                {
                    EndpointResourceName = EndpointName,
                    //TODO security stuff
                });
        }

        public override void Abort()
        {
            m_fabListener.Abort();
            base.Abort();
        }

        public override async Task CloseAsync(CancellationToken cancellationToken)
        {
            await m_fabListener.CloseAsync(cancellationToken);
            await base.CloseAsync(cancellationToken);
        }

        public override async Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            await base.OpenAsync(cancellationToken);
            return await m_fabListener.OpenAsync(cancellationToken);
        }
    }
}

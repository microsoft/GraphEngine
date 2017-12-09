using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Trinity.ServiceFabric.GarphEngine.Infrastructure;
using System;

namespace Trinity.ServiceFabric.GraphEngine.Listeners
{
    public class GraphEngineHttpListener : ICommunicationListener
    {
        private readonly GraphEngineStatefulServiceRuntime m_graphEngineRuntime;

        private GraphEngineStatefulServiceRuntime GraphEngineRuntime => m_graphEngineRuntime;

        public GraphEngineHttpListener(GraphEngineStatefulServiceRuntime graphEngineRuntime)
        {
            this.m_graphEngineRuntime = graphEngineRuntime;
        }

        public void Abort()
        {
            Debug.Assert(GraphEngineRuntime != null, nameof(GraphEngineRuntime) + " != null");
            GraphEngineRuntime.TrinityServerRuntime.Stop();
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            Debug.Assert(GraphEngineRuntime != null, nameof(GraphEngineRuntime) + " != null");
            GraphEngineRuntime.TrinityServerRuntime.Stop();
            return Task.FromResult(0);
        }

        public async Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            //  !Note, although the http listener does speak the http protocol, it does
            //  not leverage the Http infrastructure provided by SF. Hence we tell SF
            //  that this is a tcp port and ask for complete control of the port.
            Debug.Assert(GraphEngineRuntime != null, nameof(GraphEngineRuntime) + " != null");
            await Task.Run(() => GraphEngineRuntime.TrinityServerRuntime.Start());
            return $"tcp://{GraphEngineRuntime.TrinityServerRuntime.Address}:{GraphEngineRuntime.TrinityServerRuntime.HttpPort}";
        }
    }
}
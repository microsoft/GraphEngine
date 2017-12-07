using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Trinity.ServiceFabric.GarphEngine.Infrastructure;

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

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            GraphEngineRuntime.TrinityServerRuntime.Stop();
            return Task.FromResult($"http://{GraphEngineRuntime.TrinityServerRuntime.Address}:{GraphEngineRuntime.TrinityServerRuntime.HttpPort}");
        }
    }
}
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Trinity.ServiceFabric.GarphEngine.Infrastructure;

namespace Trinity.ServiceFabric.GraphEngine.Listeners
{
    public class GraphEngineListener : ICommunicationListener
    {
        private readonly GraphEngineStatefulServiceRuntime m_graphEngineRuntime;

        public GraphEngineListener(GraphEngineStatefulServiceRuntime graphEngineRuntime)
        {
            m_graphEngineRuntime = graphEngineRuntime;
        }

        private GraphEngineStatefulServiceRuntime GraphEngineRuntime => m_graphEngineRuntime;

        /// <inheritdoc />
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
            Debug.Assert(GraphEngineRuntime != null, nameof(GraphEngineRuntime) + " != null");
            GraphEngineRuntime.TrinityServerRuntime.Start();
            return Task.FromResult($"tcp://{GraphEngineRuntime.TrinityServerRuntime.Address}:{GraphEngineRuntime.TrinityServerRuntime.Port}");
        }
    }
}
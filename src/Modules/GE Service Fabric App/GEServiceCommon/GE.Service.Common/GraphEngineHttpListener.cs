using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Trinity.GraphEngine.ServiceFabric.Core;

namespace Trinity.GraphEngine.ServiceFabric.Listeners
{
    public class GraphEngineHttpListener : ICommunicationListener
    {
        private StatefulServiceContext m_ctx;
        private GraphEngineStatefulServiceCore geEngineStatefulServiceCore;

        public GraphEngineHttpListener(StatefulServiceContext ctx, GraphEngineStatefulServiceCore geEngineStatefulServiceCore)
        {
            this.m_ctx = ctx;
            this.geEngineStatefulServiceCore = geEngineStatefulServiceCore;
        }

        public void Abort()
        {
            //GraphEngineService.Instance?.Stop();
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            //GraphEngineService.Instance?.Stop();
            return Task.FromResult(0);
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            //GraphEngineService.Instance?.Start();
            return Task.FromResult($"tcp://{GraphEngineStatefulServiceCore.Instance.Address}:{GraphEngineStatefulServiceCore.Instance.HttpPort}");
        }
    }
}
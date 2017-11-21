using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Trinity.Network;

namespace Trinity.ServiceFabric
{
    internal class GraphEngineHttpListener : ICommunicationListener
    {
        private StatefulServiceContext m_ctx;

        public GraphEngineHttpListener(StatefulServiceContext ctx)
        {
            this.m_ctx = ctx;
        }

        public void Abort()
        {
            GraphEngineService.Instance.Stop();
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            GraphEngineService.Instance.Stop();
            return Task.FromResult(0);
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            GraphEngineService.Instance.Start();
            return Task.FromResult($"trinity-http://{GraphEngineService.Instance.Address}:{GraphEngineService.Instance.HttpPort}");
        }
    }
}
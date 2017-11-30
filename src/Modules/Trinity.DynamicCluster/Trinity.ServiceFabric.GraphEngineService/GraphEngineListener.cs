using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace Trinity.ServiceFabric
{
    internal class GraphEngineListener : ICommunicationListener
    {
        private StatefulServiceContext m_svcctx;

        public GraphEngineListener(StatefulServiceContext ctx)
        {
            m_svcctx = ctx;
        }

        public void Abort()
        {
            GraphEngineService.Instance?.Stop();
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            GraphEngineService.Instance?.Stop();
            return Task.FromResult(0);
        }

        public async Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            retry:
            var ge_svc = GraphEngineService.Instance;
            if (ge_svc == null)
            {
                await Task.Delay(1000);
                goto retry;
            }
            await Task.Run(() => ge_svc.Start());
            return $"tcp://{GraphEngineService.Instance.Address}:{GraphEngineService.Instance.Port}";
        }
    }
}
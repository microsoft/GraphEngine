using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Trinity.Network;
using Trinity.Utilities;
using System;
using System.Linq;
using Trinity.Diagnostics;

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
            return Task.FromResult($"trinity-tcp://{GraphEngineService.Instance.Address}:{GraphEngineService.Instance.Port}");
        }
    }
}
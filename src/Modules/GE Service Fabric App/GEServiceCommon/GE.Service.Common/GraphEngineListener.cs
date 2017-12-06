using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Trinity.GraphEngine.ServiceFabric.Core;

namespace Trinity.GraphEngine.ServiceFabric.Listeners
{
    public class GraphEngineListener : GraphEngineListenerBase, ICommunicationListener
    {
        private StatefulServiceContext m_svcctx;
        private GraphEngineStatefulServiceCore geEngineStatefulServiceCore;

        public GraphEngineListener(StatefulServiceContext ctx, GraphEngineStatefulServiceCore geEngineStatefulServiceCore)
        {
            m_svcctx = ctx;
            this.geEngineStatefulServiceCore = geEngineStatefulServiceCore;
        }

        /// <inheritdoc />
        public new void Abort()
        {
            geEngineStatefulServiceCore.Stop();
        }

        public new Task CloseAsync(CancellationToken cancellationToken)
        {
            geEngineStatefulServiceCore?.Stop();
            return Task.FromResult(0);
        }

        public new Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            geEngineStatefulServiceCore?.Start();
            Debug.Assert(geEngineStatefulServiceCore != null, "GraphEngineService.Instance != null");
            return Task.FromResult($"tcp://{geEngineStatefulServiceCore?.Address}:{geEngineStatefulServiceCore.Port}");
        }
    }
}

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Trinity.ServiceFabric.Infrastructure;

namespace Trinity.ServiceFabric.Listeners
{
    public class GraphEngineListener : IGraphEngineCommunicationListener
    {
        public string ListenerName => GraphEngineConstants.GraphEngineListenerName;

        public bool ListenOnSecondaries => true;

        /// <inheritdoc />
        public void Abort()
        {
            GraphEngineStatefulServiceRuntime.Instance.TrinityServerRuntime.Stop();
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            GraphEngineStatefulServiceRuntime.Instance.TrinityServerRuntime.Stop();
            return Task.FromResult(0);
        }

        public async Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            var Runtime = GraphEngineStatefulServiceRuntime.Instance;
            await Task.Run(() => Runtime.TrinityServerRuntime.Start());
            return $"tcp://{Runtime.TrinityServerRuntime.Address}:{Runtime.TrinityServerRuntime.HttpPort}";
        }
    }
}
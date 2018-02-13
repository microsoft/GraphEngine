using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Trinity.ServiceFabric.Infrastructure;

namespace Trinity.ServiceFabric.Listeners
{
    public class GraphEngineHttpListener : IGraphEngineCommunicationListener
    {
        public string ListenerName => GraphEngineConstants.GraphEngineListenerName;

        public bool ListenOnSecondaries => false;

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
            //  !Note, although the http listener does speak the http protocol, it does
            //  not leverage the Http infrastructure provided by SF. Hence we tell SF
            //  that this is a tcp port and ask for complete control of the port.
            var Runtime = GraphEngineStatefulServiceRuntime.Instance;
            await Task.Run(() => Runtime.TrinityServerRuntime.Start());
            return $"tcp://{Runtime.TrinityServerRuntime.Address}:{Runtime.TrinityServerRuntime.HttpPort}";
        }
    }
}
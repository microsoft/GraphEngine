using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Trinity.ServiceFabric.Infrastructure;

namespace Trinity.ServiceFabric.Listeners
{
    public class GraphEngineHttpListener : IGraphEngineCommunicationListener
    {
        public GraphEngineStatefulServiceRuntime Runtime {get;set;}

        public string ListenerName => GraphEngineConstants.GraphEngineListenerName;

        public bool ListenOnSecondaries => false;

        public void Abort()
        {
            Debug.Assert(Runtime != null, nameof(Runtime) + " != null");
            Runtime.TrinityServerRuntime.Stop();
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            Debug.Assert(Runtime != null, nameof(Runtime) + " != null");
            Runtime.TrinityServerRuntime.Stop();
            return Task.FromResult(0);
        }

        public async Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            //  !Note, although the http listener does speak the http protocol, it does
            //  not leverage the Http infrastructure provided by SF. Hence we tell SF
            //  that this is a tcp port and ask for complete control of the port.
            Debug.Assert(Runtime != null, nameof(Runtime) + " != null");
            await Task.Run(() => Runtime.TrinityServerRuntime.Start());
            return $"tcp://{Runtime.TrinityServerRuntime.Address}:{Runtime.TrinityServerRuntime.HttpPort}";
        }

        public void SetGraphEngineRuntime(GraphEngineStatefulServiceRuntime runtime)
        {
            this.Runtime = runtime;
        }
    }
}
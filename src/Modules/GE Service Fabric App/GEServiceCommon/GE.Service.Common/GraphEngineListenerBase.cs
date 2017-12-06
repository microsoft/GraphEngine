using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Trinity.Network;

namespace Trinity.GraphEngine.ServiceFabric.Listeners
{
    public class GraphEngineListenerBase : TrinityServer, ICommunicationListener
    {
        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public void Abort()
        {
            throw new System.NotImplementedException();
        }
    }
}
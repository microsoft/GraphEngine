using System.Fabric;
using System.Net;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Trinity;
using Trinity.Network;

namespace GE.ServiceFabric.Services.Communiction.Trinity.Runtime
{

    /// <summary>
    /// A Microsoft Trinity Communiactions based listener for Service Fabric based stateless and stateful
    /// service.
    /// </summary>
    public class TrinityCommunictionListener<TServiceContract, TServer> : ICommunicationListener where TServer: TrinityServer
    {
        private TServer m_server;
        public TrinityCommunictionListener(TServer server, ServiceContext serviceContext, TServiceContract trinityServiceObject)
        {
            m_server = server;
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => m_server.Start()) as Task<string>;
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => m_server.Stop());
        }

        public void Abort()
        {
            ;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}

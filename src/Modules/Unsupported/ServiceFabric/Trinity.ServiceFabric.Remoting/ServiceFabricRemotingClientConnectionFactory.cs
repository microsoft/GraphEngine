using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Trinity.Client;
using Trinity.Extension;
using Trinity.Network;
using Trinity.Storage;

namespace Trinity.ServiceFabric.Remoting
{
    [ExtensionPriority(100)]
    class ServiceFabricRemotingConnectionClientFactory : IClientConnectionFactory
    {
        public Task<IMessagePassingEndpoint> ConnectAsync(string connectionString, ICommunicationModuleRegistry modules)
        {
            return Task.FromResult<IMessagePassingEndpoint>(new ServiceFabricRemotingClientConnection(connectionString, modules));
        }

        public Task DisconnectAsync(IMessagePassingEndpoint endpoint)
        {
            return Task.FromResult(true);
        }
    }
}

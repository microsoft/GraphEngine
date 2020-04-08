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
    using Microsoft.ServiceFabric.Services.Client;

    [ExtensionPriority(100)]
    public class ServiceFabricRemotingConnectionClientFactory : IClientConnectionFactory
    {
        public Task<IMessagePassingEndpoint> ConnectAsync(string connectionString, ICommunicationModuleRegistry modules)
        {
            return Task.FromResult<IMessagePassingEndpoint>(new ServiceFabricRemotingClientConnection(connectionString, modules));
        }

        public Task<IMessagePassingEndpoint> ConnectAsync(string connectionString, ICommunicationModuleRegistry modules, ServicePartitionKey userServicePartitionKey)
        {
            return Task.FromResult<IMessagePassingEndpoint>(new ServiceFabricRemotingClientConnection(connectionString, modules, userServicePartitionKey));
        }

        public Task DisconnectAsync(IMessagePassingEndpoint endpoint)
        {
            return Task.FromResult(true);
        }
    }
}

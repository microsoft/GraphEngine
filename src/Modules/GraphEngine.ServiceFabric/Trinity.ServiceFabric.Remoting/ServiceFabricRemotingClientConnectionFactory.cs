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

    /// <summary>
    /// ServiceFabricRemotingConnectionClientFactory is dynamically discovered and it methods called via reflection
    /// </summary>
    [ExtensionPriority(100)]
    public class ServiceFabricRemotingConnectionClientFactory : IClientConnectionFactory
    {
        /// <summary>
        /// ConnectAsync into Service Fabric Cluster and return an Trinity Messaging endpoint. This call makes it
        /// possible to connect from within the service fabric cluster
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="modules"></param>
        /// <returns></returns>
        public Task<IMessagePassingEndpoint> ConnectAsync(string connectionString, ICommunicationModuleRegistry modules)
        {
            // Make a service-to-server in a node-to-node cluster environment
            return Task.FromResult<IMessagePassingEndpoint>(new ServiceFabricRemotingClientConnection(connectionString, modules));
        }

        /// <summary>
        /// ConnectAsync into Service Fabric Cluster and return an Trinity Messaging endpoint. This call makes it
        /// possible to connect from a remote client outside of the service fabric cluster
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="modules"></param>
        /// <param name="userServicePartitionKey"></param>
        /// <returns></returns>
        public Task<IMessagePassingEndpoint> ConnectAsync(string connectionString, ICommunicationModuleRegistry modules, ServicePartitionKey userServicePartitionKey)
        {
            // this is a down-level call from the TrinityClient object making a call into the service fabric runtime to connect to a service cluster node
            return Task.FromResult<IMessagePassingEndpoint>(new ServiceFabricRemotingClientConnection(connectionString, modules, userServicePartitionKey));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public Task DisconnectAsync(IMessagePassingEndpoint endpoint)
        {
            return Task.FromResult(true);
        }
    }
}

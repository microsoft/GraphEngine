using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Client;
using Trinity.Client;
using Trinity.Extension;
using Trinity.Network;
using Trinity.Storage;

namespace Trinity.ServiceFabric.NativeClient.Remoting
{
    /// <summary>
    /// TrinityOverNativeTCPClientConnectionFactory is dynamically discovered and it methods called via reflection
    /// </summary>
    [ExtensionPriority(90)]
    public class TrinityOverNativeTCPClientConnectionFactory : IClientConnectionFactory
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
            return Task.FromResult<IMessagePassingEndpoint>(new TrinityOverNativeTCPClientConnection<TrinityOverNativeTCPCommunicationClient>(connectionString, modules));
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
            // this is a down-level call from the TrinityClient object making a call into the service fabric runtime to connect to a remote service cluster
            return Task.FromResult<IMessagePassingEndpoint>(new TrinityOverNativeTCPClientConnection<TrinityOverNativeTCPCommunicationClient>(connectionString, modules, userServicePartitionKey));
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

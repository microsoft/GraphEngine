using System.Threading.Tasks;
using Trinity.Network;
using Trinity.Storage;

namespace Trinity.Client
{
    using Microsoft.ServiceFabric.Services.Client;

    /// <summary>
    /// Provides methods for connecting/disconnecting client connections.
    /// </summary>
    public interface IClientConnectionFactory
    {
        /// <summary>
        /// Connects to a server asynchronously
        /// </summary>
        /// <param name="connectionString">The connection string of the server.</param>
        Task<IMessagePassingEndpoint> ConnectAsync(string connectionString, ICommunicationModuleRegistry modules);
        Task<IMessagePassingEndpoint> ConnectAsync(string connectionString, ICommunicationModuleRegistry modules, ServicePartitionKey userSuppliedPartitionKey = null);

        /// <summary>
        /// Disconnects a client endpoint from the server.
        /// </summary>
        /// <param name="endpoint">The client endpoint</param>
        Task DisconnectAsync(IMessagePassingEndpoint endpoint);
    }
}

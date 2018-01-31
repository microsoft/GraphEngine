using System.Threading.Tasks;
using Trinity.Storage;

namespace Trinity.Client
{
    /// <summary>
    /// Provides methods for connecting/disconnecting client connections.
    /// </summary>
    public interface IClientConnectionFactory
    {
        /// <summary>
        /// Connects to a server asynchronously
        /// </summary>
        /// <param name="connectionString">The connection string of the server.</param>
        Task<IMessagePassingEndpoint> ConnectAsync(string connectionString);
        /// <summary>
        /// Disconnects a client endpoint from the server.
        /// </summary>
        /// <param name="endpoint">The client endpoint</param>
        Task DisconnectAsync(IMessagePassingEndpoint endpoint);
    }
}

using System.Threading.Tasks;
using Trinity.Configuration;
using Trinity.Diagnostics;
using Trinity.Network;
using Trinity.Storage;

namespace Trinity.Client
{
    /// <summary>
    /// Default Client Connection object - supports native TCP/HTTP service endpoints.
    /// Implements Trinity RemoteStorage environment / runtime on the client-side
    /// </summary>
    internal class DefaultClientConnection : RemoteStorage
    {
        private ICommunicationModuleRegistry m_modules;

        /// <summary>
        /// Default constructor for Default Client Connection 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="modules"></param>
        protected internal DefaultClientConnection(ServerInfo server, ICommunicationModuleRegistry modules)
            : base(new[] { server }, NetworkConfig.Instance.ClientMaxConn, null, -1, nonblocking: true)
        {
            m_modules = modules;
        }

        /// <summary>
        /// New method creates a new instance of the DefaultClientConnection
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="moudles"></param>
        /// <returns></returns>
        internal static IMessagePassingEndpoint New(string host, int port, ICommunicationModuleRegistry moudles)
        {
            return new DefaultClientConnection(new ServerInfo(host, port, null, LogLevel.Info), moudles);
        }

        /// <summary>
        /// Generic method to Get and inject a Communications module of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override T GetCommunicationModule<T>() => m_modules.GetCommunicationModule<T>();
    }
}
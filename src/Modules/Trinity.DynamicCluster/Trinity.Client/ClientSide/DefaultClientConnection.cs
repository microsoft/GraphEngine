using System.Threading.Tasks;
using Trinity.Configuration;
using Trinity.Diagnostics;
using Trinity.Network;
using Trinity.Storage;

namespace Trinity.Client
{
    internal class DefaultClientConnection : RemoteStorage, IMessagePassingEndpoint
    {
        protected internal DefaultClientConnection(ServerInfo server)
            : base(new[] { server }, NetworkConfig.Instance.ClientMaxConn, null, -1, nonblocking: true) { }

        internal static IMessagePassingEndpoint New(string host, int port)
        {
            return new DefaultClientConnection(new ServerInfo(host, port, null, LogLevel.Info));
        }
    }
}
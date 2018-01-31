using System;
using System.Threading.Tasks;
using Trinity.Extension;
using Trinity.Storage;

namespace Trinity.Client
{
    [ExtensionPriority(-100)]
    class DefaultClientConnectionFactory : IClientConnectionFactory
    {
        public Task<IMessagePassingEndpoint> ConnectAsync(string endpoint)
        {
            string[] ep = endpoint.Split(new[]{':' }, StringSplitOptions.RemoveEmptyEntries);
            int port = int.Parse(ep[1]);
            return Task.FromResult(DefaultClientConnection.New(ep[0], port));
        }

        public Task DisconnectAsync(IMessagePassingEndpoint endpoint)
        {
            var cc = endpoint as DefaultClientConnection;
            return Task.Run(() => cc.Dispose());
        }
    }
}

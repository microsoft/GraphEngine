using System;
using System.Threading.Tasks;
using Trinity.Extension;
using Trinity.Network;
using Trinity.Storage;

namespace Trinity.Client
{
    [ExtensionPriority(-100)]
    public class DefaultClientConnectionFactory : IClientConnectionFactory
    {
        public Task<IMessagePassingEndpoint> ConnectAsync(string endpoint, ICommunicationModuleRegistry modules)
        {
            string[] ep = endpoint.Split(new[]{':' }, StringSplitOptions.RemoveEmptyEntries);
            int port = int.Parse(ep[1]);
            return Task.FromResult(DefaultClientConnection.New(ep[0], port, modules));
        }

        public Task DisconnectAsync(IMessagePassingEndpoint endpoint)
        {
            var cc = endpoint as DefaultClientConnection;
            return Task.Run(() => cc.Dispose());
        }
    }
}

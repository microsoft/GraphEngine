using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Storage;

namespace Trinity.Client
{
    class DefaultClientConnectionFactory : IClientConnectionFactory
    {
        public Task<IMessagePassingEndpoint> ConnectAsync(string endpoing)
        {
            throw new NotImplementedException();
        }

        public Task DisconnectAsync(IMessagePassingEndpoint endpoint)
        {
            throw new NotImplementedException();
        }
    }
}

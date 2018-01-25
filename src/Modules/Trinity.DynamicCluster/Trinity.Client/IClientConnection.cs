using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Storage;

namespace Trinity.Client
{
    interface IClientConnectionFactory
    {
        Task<IMessagePassingEndpoint> ConnectAsync(string endpoing);
        Task DisconnectAsync(IMessagePassingEndpoint endpoint);
    }
}

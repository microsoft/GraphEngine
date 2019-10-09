using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network.Messaging;

[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace Trinity.ServiceFabric.Remoting
{
    public interface ITrinityOverRemotingService : IService
    {
        Task SendMessageAsync(byte[] message);
        Task<byte[]> SendMessageWithResponseAsync(byte[] message);
    }
}

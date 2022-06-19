using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network.Messaging;

[assembly: FabricTransportServiceRemotingProvider(RemotingListener = RemotingListener.V2Listener, RemotingClient = RemotingClient.V2Client)]
namespace Trinity.ServiceFabric.Remoting
{
    public interface ITrinityOverRemotingService : IService
    {
        Task SendMessageAsync(byte[] message);
        Task<byte[]> SendMessageWithResponseAsync(byte[] message);
    }
}

using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

//[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2, RemotingClientVersion = RemotingClientVersion.V2)]
namespace Trinity.ServiceFabric.NativeClient.Remoting.Interfaces
{
    public interface ITrinityOverNativeTCPCommunicationService // IService
    {
        Task SendMessageAsync(byte[] message);
        Task<byte[]> SendMessageWithResponseAsync(byte[] message);
    }
}

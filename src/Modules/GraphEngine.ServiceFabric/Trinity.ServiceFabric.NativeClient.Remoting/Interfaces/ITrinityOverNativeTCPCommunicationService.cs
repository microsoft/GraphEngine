using System.Threading.Tasks;

namespace Trinity.ServiceFabric.NativeClient.Remoting.Interfaces
{
    public interface ITrinityOverNativeTCPCommunicationService
    {
        Task SendMessageAsync(byte[] message);
        Task<byte[]> SendMessageWithResponseAsync(byte[] message);
    }
}

using System;
using System.Threading.Tasks;
using Trinity.Network;
using Trinity.Network.Messaging;

namespace Trinity.Storage
{
    /// <summary>
    /// Represents an endpoint that can receive messages.
    /// </summary>
    public unsafe interface IMessagePassingEndpoint : ICommunicationModuleRegistry
    {
        Task SendMessageAsync(byte* message, int size);
        Task<TrinityResponse> SendRecvMessageAsync(byte* message, int size);
        Task SendMessageAsync(byte** message, int* sizes, int count);
        Task<TrinityResponse> SendRecvMessageAsync(byte** message, int* sizes, int count);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        void SendMessage(byte* message, int size);
        void SendMessage(byte* message, int size, out TrinityResponse response);
        void SendMessage(byte** message, int* sizes, int count);
        void SendMessage(byte** message, int* sizes, int count, out TrinityResponse response);
    }
}

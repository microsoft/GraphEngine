using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network.Messaging;

namespace Trinity.Storage
{
    public static unsafe class MessagePassingExtensionMethods
    {
        public static void SendMessage(this IMessagePassingEndpoint storage, TrinityMessage message)
        {
            storage.SendMessage(message.Buffer, message.Size);
        }

        public static void SendMessage(this IMessagePassingEndpoint storage, TrinityMessage message, out TrinityResponse response)
        {
            storage.SendMessage(message.Buffer, message.Size, out response);
        }
    }
}

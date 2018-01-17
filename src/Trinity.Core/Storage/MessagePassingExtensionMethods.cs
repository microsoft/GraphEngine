using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network;
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

        public static void SendMessage<T>(this IMessagePassingEndpoint storage, byte* message, int size)
            where T: CommunicationModule
        {
            storage.GetModule<T>().SendMessage(storage, message, size);
        }

        public static void SendMessage<T>(this IMessagePassingEndpoint storage, byte* message, int size, out TrinityResponse response)
            where T: CommunicationModule
        {
            storage.GetModule<T>().SendMessage(storage, message, size, out response);
        }

        public static void SendMessage<T>(this IMessagePassingEndpoint storage, byte** message, int* sizes, int count)
            where T: CommunicationModule
        {
            storage.GetModule<T>().SendMessage(storage, message, sizes, count);
        }

        public static void SendMessage<T>(this IMessagePassingEndpoint storage, byte** message, int* sizes, int count, out TrinityResponse response)
            where T: CommunicationModule
        {
            storage.GetModule<T>().SendMessage(storage, message, sizes, count, out response);
        }
    }
}

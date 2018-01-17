using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Storage
{
    internal unsafe partial class Partition : IStorage
    {
        public void SendMessage(TrinityMessage message)
        {
            m_storages.First().Key.SendMessage(message);
        }

        public void SendMessage(byte* message, int size)
        {
            m_storages.First().Key.SendMessage(message, size);
        }

        public void SendMessage(TrinityMessage message, out TrinityResponse response)
        {
            m_storages.First().Key.SendMessage(message, out response);
        }

        public void SendMessage(byte* message, int size, out TrinityResponse response)
        {
            m_storages.First().Key.SendMessage(message, size, out response);
        }

        public unsafe void SendMessage(byte** message, int* sizes, int count)
        {
            m_storages.First().Key.SendMessage(message, sizes, count);
        }

        public unsafe void SendMessage(byte** message, int* sizes, int count, out TrinityResponse response)
        {
            m_storages.First().Key.SendMessage(message, sizes, count, out response);
        }
    }
}

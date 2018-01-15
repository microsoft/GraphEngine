using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network.Messaging;

namespace Trinity.DynamicCluster.Storage
{
    using Storage = Trinity.Storage.Storage;
    internal unsafe partial class Partition : Storage
    {
        public override void SendMessage(TrinityMessage message)
        {
            m_storages.First().Key.SendMessage(message);
        }

        public override void SendMessage(byte* message, int size)
        {
            m_storages.First().Key.SendMessage(message, size);
        }

        public override void SendMessage(TrinityMessage message, out TrinityResponse response)
        {
            m_storages.First().Key.SendMessage(message, out response);
        }

        public override void SendMessage(byte* message, int size, out TrinityResponse response)
        {
            m_storages.First().Key.SendMessage(message, size, out response);
        }

        public override unsafe void SendMessage(byte** message, int* sizes, int count)
        {
            m_storages.First().Key.SendMessage(message, sizes, count);
        }

        public override unsafe void SendMessage(byte** message, int* sizes, int count, out TrinityResponse response)
        {
            m_storages.First().Key.SendMessage(message, sizes, count, out response);
        }
    }
}

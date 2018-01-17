using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Storage
{
    internal unsafe partial class Partition : IStorage
    {
        public T GetModule<T>() where T : CommunicationModule
            => Global.CommunicationInstance.GetCommunicationModule<T>();

        public void SendMessage(byte* message, int size)
        {
            m_storages.First().Key.SendMessage(message, size);
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

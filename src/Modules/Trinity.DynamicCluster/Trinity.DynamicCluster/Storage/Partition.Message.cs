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
        // TODO HA semantics should be implemented here
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

        public void Broadcast(TrinityMessage message)
        {
            m_storages.Keys.ForEach(s => s.SendMessage(message));
        }

        public void Broadcast(byte* message, int size)
        {
            m_storages.Keys.ForEach(s => s.SendMessage(message, size));
        }

        public void Broadcast(TrinityMessage message, out TrinityResponse response)
        {
            // TODO what does it mean to broadcast with response?
            throw new NotImplementedException();
        }

        public void Broadcast(byte* message, int size, out TrinityResponse response)
        {
            // TODO
            throw new NotImplementedException();
        }

        // TODO Round-robin

        // TODO First-available

        // TODO partition-aware dispatch
    }
}

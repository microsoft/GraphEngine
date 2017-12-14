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
        // We should provide a mechanism to annotate protocols with HA semantics,
        // and interfaces for override such semantics.
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

        public void Broadcast(TrinityMessage message)
        {
            m_storages.Keys.ForEach(s => s.SendMessage(message));
        }

        public void Broadcast(byte* message, int size)
        {
            m_storages.Keys.ForEach(s => s.SendMessage(message, size));
        }

        public void Broadcast(TrinityMessage message, out TrinityResponse[] response)
        {
            response = m_storages.Keys.Select(s => { s.SendMessage(message, out var rsp); return rsp; }).ToArray();
        }

        public void Broadcast(byte* message, int size, out TrinityResponse[] response)
        {
            response = m_storages.Keys.Select(s => { s.SendMessage(message, size, out var rsp); return rsp; }).ToArray();
        }

        // TODO Round-robin

        // TODO First-available

        // TODO chunk-aware dispatch and message grouping. Some protocols (like FanoutSearch)
        // combines multiple cellIds into a single message. In this case we should provide a
        // mechanism to allocate a group of messages, each representing a chunk set. On dispatch,
        // these messages will be sent to the correct replica.

        // TODO send message to a specific storage, identified by a GUID. This works for situations
        // where a temporary state is attached to a specific storage.
    }
}

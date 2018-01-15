using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network.Messaging;

namespace Trinity.DynamicCluster.Storage
{
    internal unsafe partial class Partition
    {
        public void Broadcast(byte* message, int size)
        {
            m_storages.Keys.ForEach(s => s.SendMessage(message, size));
        }

        // TODO we may consider add an interface: TrinityMessage -> IEnumerable<TrinityResponse>
        // And thus in the message passing interface: IEnumerable<TrinityMessage>, IEnumerable<int> -> IEnumerable<TrinityResponse>,
        // where IEnumerable<int> holds partition identifiers.

        public void Broadcast(byte* message, int size, out TrinityResponse[] response)
        {
            response = m_storages.Keys.Select(s => { s.SendMessage(message, size, out var rsp); return rsp; }).ToArray();
        }
        
        // TODO First-available

        // TODO chunk-aware dispatch and message grouping. Some protocols (like FanoutSearch)
        // combines multiple cellIds into a single message. In this case we should provide a
        // mechanism to allocate a group of messages, each representing a chunk set. On dispatch,
        // these messages will be sent to the correct replica.

        // TODO send message to a specific storage, identified by a GUID. This works for situations
        // where a temporary state is attached to a specific storage.
    }
}

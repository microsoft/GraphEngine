using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Storage
{
    internal unsafe partial class Partition
    {
        public void Broadcast(Action<IMessagePassingEndpoint> sendFunc)
            => m_storages.Keys.ForEach(s => sendFunc(s));

        public IEnumerable<TResponse> Broadcast<TResponse>(Func<IMessagePassingEndpoint, TResponse> sendFunc)
            => m_storages.Keys.Select(sendFunc);

        public Task<TResponse[]> Broadcast<TResponse>(Func<IMessagePassingEndpoint, Task<TResponse>> sendFunc)
            => m_storages.Keys.Select(sendFunc).Unwrap();

        // TODO First-available

        // TODO chunk-aware dispatch and message grouping. Some protocols (like FanoutSearch)
        // combines multiple cellIds into a single message. In this case we should provide a
        // mechanism to allocate a group of messages, each representing a chunk set. On dispatch,
        // these messages will be sent to the correct replica.

        // TODO send message to a specific storage, identified by a GUID. This works for situations
        // where a temporary state is attached to a specific storage.
    }
}

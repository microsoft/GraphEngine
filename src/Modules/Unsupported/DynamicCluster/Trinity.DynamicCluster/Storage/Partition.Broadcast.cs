using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Storage
{
    public partial class Partition
    {
        /// <summary>
        /// Broadcasts a message to all message passing endpoints in the partition.
        /// The delivery of the message to a particular endpoint will not be affected
        /// if the delivery to another results in an exception.
        /// </summary>
        public void Broadcast(Action<IMessagePassingEndpoint> sendFunc)
        {
            var stgs = this.ToList();
            var exs  = new Exception[stgs.Count];
            Task.WhenAll(stgs.Select((stg, i) =>
            {
                try { sendFunc(stg); }
                catch (Exception ex) { exs[i] = ex; }
                return Task.FromResult(0);
            })).Wait();
            if (!exs.Any(_ => _!= null)) return;
            else throw new BroadcastException(stgs.ZipWith(exs));
        }

        public IEnumerable<TResponse> Broadcast<TResponse>(Func<IMessagePassingEndpoint, TResponse> sendFunc)
            where TResponse : IDisposable
        {
            var stgs = this.ToList();
            var exs  = new Exception[stgs.Count];
            var vals = new TResponse[stgs.Count];
            Task.WhenAll(stgs.Select((stg, i) =>
            {
                try { vals[i] = sendFunc(stg); }
                catch (Exception ex) { exs[i] = ex; }
                return Task.FromResult(0);
            })).Wait();
            if (!exs.Any(_ => _!= null)) return vals;
            else throw new BroadcastException<TResponse>(stgs.ZipWith(exs), stgs.ZipWith(vals));
        }

        public async Task<TResponse[]> Broadcast<TResponse>(Func<IMessagePassingEndpoint, Task<TResponse>> sendFunc)
            where TResponse : IDisposable
        {
            var stgs = this.ToList();
            var exs  = new Exception[stgs.Count];
            var vals = new TResponse[stgs.Count];

            await Task.WhenAll(stgs.Select(async (stg, i) =>
            {
                try { vals[i] = await sendFunc(stg); }
                catch (Exception ex) { exs[i] = ex; }
            }));

            if (!exs.Any(_ => _!= null)) return vals;
            else throw new BroadcastException<TResponse>(stgs.ZipWith(exs), stgs.ZipWith(vals));
        }

        // TODO chunk-aware dispatch and message grouping. Some protocols (like FanoutSearch)
        // combines multiple cellIds into a single message. In this case we should provide a
        // mechanism to allocate a group of messages, each representing a chunk set. On dispatch,
        // these messages will be sent to the correct replica.
    }
}

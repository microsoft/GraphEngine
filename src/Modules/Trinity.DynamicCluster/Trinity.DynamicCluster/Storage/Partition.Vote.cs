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
        public void Vote(Action<IMessagePassingEndpoint> sendFunc, double ratio = 0.5)
        {
            //TODO
            //var reps = this.ToList();
            //int succeeded_tasks = 0;
            //var tasks = reps.Select(rep => _Run(rep, sendFunc));
            //for(int cnt = 0; cnt < )
            //Task.WhenAny(tasks, )
            //var current = m_roundrobin_getter();
            //if (current == null) throw new NoSuitableReplicaException();
            //sendFunc(current);
        }

        //private Task<bool> _Run(IStorage rep, Action<IMessagePassingEndpoint> sendFunc)
        //{
        //}

        public TResponse Vote<TResponse>(Func<IMessagePassingEndpoint, TResponse> sendFunc)
        {
            var current = m_roundrobin_getter();
            if (current == null) throw new NoSuitableReplicaException();
            return sendFunc(current);
        }

        public Task<TResponse> Vote<TResponse>(Func<IMessagePassingEndpoint, Task<TResponse>> sendFunc)
        {
            var current = m_roundrobin_getter();
            if (current == null) throw new NoSuitableReplicaException();
            return sendFunc(current);
        }
    }
}

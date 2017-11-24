using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Consensus;
using Trinity.DynamicCluster.Tasks;

namespace Trinity.ServiceFabric.Interfaces
{
    class ReliableTaskQueue : ITaskQueue
    {
        public bool IsMaster => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public ITask GetTask(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public TrinityErrorCode Start(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void TaskCompleted(ITask task)
        {
            throw new NotImplementedException();
        }

        public void TaskFailed(ITask task)
        {
            throw new NotImplementedException();
        }
    }
}

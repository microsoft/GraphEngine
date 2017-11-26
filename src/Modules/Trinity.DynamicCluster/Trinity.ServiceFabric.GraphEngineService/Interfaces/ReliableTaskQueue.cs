using System;
using System.Collections.Generic;
using System.Fabric;
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
        public bool IsMaster => GraphEngineService.Instance.Role == ReplicaRole.Primary;

        public void Dispose()
        {
        }

        public ITask GetTask(CancellationToken token)
        {
            return null;
        }

        public TrinityErrorCode Start(CancellationToken cancellationToken)
        {
            return TrinityErrorCode.E_SUCCESS;
        }

        public void TaskCompleted(ITask task)
        {
            return;
        }

        public void TaskFailed(ITask task)
        {
            return;
        }
    }
}

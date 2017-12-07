using System.Fabric;
using System.Threading;
using Trinity.DynamicCluster.Consensus;
using Trinity.DynamicCluster.Tasks;

namespace Trinity.ServiceFabric.GarphEngine.Infrastructure.Interfaces
{
    internal class ReliableTaskQueue : ITaskQueue
    {
        public bool IsMaster => GraphEngineStatefulServiceRuntime.Instance.TrinityServerRuntime.Role == ReplicaRole.Primary;

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

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Health;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Consensus;
using Trinity.DynamicCluster.Health;
using Trinity.Network;
using Trinity.ServiceFabric.GarphEngine.Infrastructure;

namespace Trinity.ServiceFabric
{
    public class ServiceFabricHealthManager : IHealthManager
    {
        public bool IsMaster => false;

        public void Dispose()
        {
        }

        public void Start(CancellationToken cancellationToken)
        {
        }

        public Task ReportMemoryCloudStatus(HealthStatus health, string message)
        {
            throw new NotImplementedException();
        }

        public Task ReportModuleStatus(HealthStatus health, string moduleName, string message)
        {
            throw new NotImplementedException();
        }

        public Task ReportReplicaStatus(HealthStatus health, Guid id, string message)
        {
            throw new NotImplementedException();
        }

        public Task ReportPartitionStatus(HealthStatus health, int id, string message)
        {
            var guid = GraphEngineStatefulServiceRuntime.Instance.Partitions[id].PartitionInformation.Id;
            throw new NotImplementedException();
        }
    }
}

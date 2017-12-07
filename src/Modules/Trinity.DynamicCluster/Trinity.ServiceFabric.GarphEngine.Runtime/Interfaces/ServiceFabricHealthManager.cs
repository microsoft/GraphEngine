using System;
using System.Threading;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Consensus;
using Trinity.DynamicCluster.Health;
using Trinity.Network;

namespace Trinity.ServiceFabric.GarphEngine.Infrastructure.Interfaces
{
    public class ServiceFabricHealthManager : IHealthManager
    {
        public bool IsMaster => false;

        public void Dispose()
        {
        }

        public Task ReportMemoryCloudStatus(HealthStatus health, string message)
        {
            throw new NotImplementedException();
        }

        public Task ReportModuleStatus<T>(HealthStatus health, string message) where T : CommunicationModule
        {
            throw new NotImplementedException();
        }

        public Task ReportPartitionStatus(HealthStatus health, string message)
        {
            throw new NotImplementedException();
        }

        public Task ReportTrinityServerStatus<T>(HealthStatus health, string message) where T : CommunicationInstance
        {
            throw new NotImplementedException();
        }

        public void Start(CancellationToken cancellationToken)
        {
        }
    }
}

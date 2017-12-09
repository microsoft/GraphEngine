using System;
using System.Fabric.Health;
using System.Threading;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Consensus;
using Trinity.DynamicCluster.Health;
using Trinity.ServiceFabric.GarphEngine.Infrastructure;

namespace Trinity.ServiceFabric
{
    public class ServiceFabricHealthManager : IHealthManager
    {
        public bool IsMaster => GraphEngineStatefulServiceRuntime.Instance?.Role == System.Fabric.ReplicaRole.Primary;
        private CancellationToken m_token;
        private System.Fabric.FabricClient m_fclient;
        private const string sourceId = "Trinity.ServiceFabric.HealthManager";

        public ServiceFabricHealthManager(string sourceId)
        {
        }

        private HealthState _hstate(HealthStatus hstatus)
        {
            switch (hstatus)
            {
                case HealthStatus.Error: return HealthState.Error;
                case HealthStatus.Warning: return HealthState.Warning;
                case HealthStatus.Healthy: return HealthState.Ok;
                default: return HealthState.Unknown;
            }
        }

        public void Dispose()
        {
        }

        public void Start(CancellationToken cancellationToken)
        {
            m_token = cancellationToken;
            m_fclient = new System.Fabric.FabricClient();
        }

        public async Task ReportMemoryCloudStatus(HealthStatus health, string message)
        {
            var rt = GraphEngineStatefulServiceRuntime.Instance;
            await rt.GetRoleAsync();
            var m_svruri = rt.Context.ServiceName;
            var hi = new HealthInformation(sourceId, "ReportMemoryCloudStatus", _hstate(health))
            {
                Description = message
            };
            m_fclient.HealthManager.ReportHealth(
                new ServiceHealthReport(m_svruri, hi));
        }

        public async Task ReportModuleStatus(HealthStatus health, string moduleName, string message)
        {
            // Errors of a module lead to the crash of the total replica, so this report is of the same type as that of `ReportReplicaStatus`. 
            var rt = GraphEngineStatefulServiceRuntime.Instance;
            await rt.GetRoleAsync();
            var replicaId = rt.Context.ReplicaId;
            var guid = rt.Partitions[rt.PartitionId].PartitionInformation.Id; // I have to get guid from 
            var hi = new HealthInformation(sourceId, $"ReportModuleStatus::{moduleName}", _hstate(health))
            {
                Description = message
            };
            m_fclient.HealthManager.ReportHealth(
                new StatefulServiceReplicaHealthReport(guid, replicaId, hi)
            );
        }

        public async Task ReportReplicaStatus(HealthStatus health, Guid id, string message)
        {
            var rt = GraphEngineStatefulServiceRuntime.Instance;
            await rt.GetRoleAsync();
            var replicaId = rt.Context.ReplicaId;
            var hi = new HealthInformation(sourceId, "ReportReplicaStatus", _hstate(health))
            {
                Description = message
            };
            m_fclient.HealthManager.ReportHealth(
                new StatefulServiceReplicaHealthReport(id, replicaId, hi));
        }

        public async Task ReportPartitionStatus(HealthStatus health, int id, string message)
        {
            var rt = GraphEngineStatefulServiceRuntime.Instance;
            await rt.GetRoleAsync();
            var guid = rt.Partitions[id].PartitionInformation.Id;
            var hi = new HealthInformation(sourceId, "ReportPartitionStatus", _hstate(health))
            {
                Description = message
            };
            m_fclient.HealthManager.ReportHealth(
                new PartitionHealthReport(guid, hi));
        }
    }
}

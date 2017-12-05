using System.Threading.Tasks;
using Trinity.DynamicCluster.Health;
using Trinity.Network;

namespace Trinity.DynamicCluster.Consensus
{
    public interface IHealthManager : IService
    {
        Task ReportModuleStatus<T>(HealthStatus health, string message) where T: CommunicationModule;
        Task ReportTrinityServerStatus<T>(HealthStatus health, string message) where T: CommunicationInstance;
        Task ReportPartitionStatus(HealthStatus health, string message);
        Task ReportMemoryCloudStatus(HealthStatus health, string message);
    }
}

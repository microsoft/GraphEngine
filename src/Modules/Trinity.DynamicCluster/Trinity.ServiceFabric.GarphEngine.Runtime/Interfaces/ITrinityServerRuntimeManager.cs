using System.Fabric;

namespace Trinity.ServiceFabric.GarphEngine.Infrastructure.Interfaces
{
    public interface ITrinityServerRuntimeManager: IGraphEngineStatefulServiceRuntime
    {
        TrinityErrorCode Start();
        TrinityErrorCode Stop();

    }
}
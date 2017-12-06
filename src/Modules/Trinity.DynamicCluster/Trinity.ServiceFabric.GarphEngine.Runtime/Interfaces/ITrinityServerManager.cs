namespace Trinity.ServiceFabric.GarphEngine.Infrastructure.Interfaces
{
    public interface ITrinityServerManager
    {
        TrinityErrorCode Start();
        TrinityErrorCode Stop();

    }
}
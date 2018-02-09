using Trinity.Storage;

namespace Trinity.Client.ServerSide
{
    /// <summary>
    /// Should be implemented by a hosting memory cloud.
    /// </summary>
    public interface IClientRegistry
    {
        int RegisterClient(IStorage client);
        void UnregisterClient(int instanceId);
    }
}

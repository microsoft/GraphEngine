using System;
using System.Fabric;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;

namespace Trinity.ServiceFabric.Infrastructure
{
    public interface IGraphEngineStatefulService
    {
        StatefulServiceContext Context { get; }
        IReliableStateManager StateManager { get; }
        Task BackupAsync(BackupDescription backupDescription);
        event EventHandler<RestoreEventArgs> RequestRestore;
    }
}
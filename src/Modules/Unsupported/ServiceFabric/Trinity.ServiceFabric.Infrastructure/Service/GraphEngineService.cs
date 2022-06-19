using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Trinity.Diagnostics;
using Trinity.DynamicCluster.Storage;
using Trinity.ServiceFabric.Infrastructure;
using Trinity.ServiceFabric.Listener;

namespace Trinity.ServiceFabric
{
    /// <inheritdoc />
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    public sealed class GraphEngineService : StatefulService, IGraphEngineStatefulService
    {
        //  Passive Singleton
        private GraphEngineService(StatefulServiceContext context)
            : base(context)
        {
            GraphEngineStatefulServiceRuntime.Initialize(this);
        }

        public static async Task StartServiceAsync(string serviceTypeName)
        {
            var ready = DynamicMemoryCloud.WaitReadyAsync();
            var start = ServiceRuntime.RegisterServiceAsync(serviceTypeName, context => new GraphEngineService(context));

            await start.ConfigureAwait(false);
            await ready.ConfigureAwait(false);

            var my_initial_role = GraphEngineStatefulServiceRuntime.Instance.Role;
            if (my_initial_role != ReplicaRole.Primary) await Task.Delay(Timeout.Infinite);
        }

        public event EventHandler<RestoreEventArgs> RequestRestore = delegate{ };

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
            => GraphEngineCommunicationListenerLoader.CreateServiceReplicaListeners(this.Context);

        protected override Task OnChangeRoleAsync(ReplicaRole newRole, CancellationToken cancellationToken)
        {
            GraphEngineStatefulServiceRuntime.Instance.Role = newRole;
            Log.WriteLine("{0}", $"Replica {Context.ReplicaOrInstanceId} changed role to {newRole}");
            return base.OnChangeRoleAsync(newRole, cancellationToken);
        }

        protected override async Task<bool> OnDataLossAsync(RestoreContext restoreCtx, CancellationToken cancellationToken)
        {
            Log.WriteLine("{0}", $"Partition {GraphEngineStatefulServiceRuntime.Instance.PartitionId} received OnDataLossAsync. Triggering data restore.");
            RestoreEventArgs rstArgs = new RestoreEventArgs(restoreCtx, cancellationToken);
            RequestRestore(this, rstArgs);
            await rstArgs.Wait();
            return true;
        }
    }
}

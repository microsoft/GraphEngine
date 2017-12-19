using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Network;
using Trinity.ServiceFabric.Communication;
using Trinity.ServiceFabric.Diagnostics;
using Trinity.ServiceFabric.Storage;
using Trinity.ServiceFabric.Storage.External;

namespace Trinity.ServiceFabric.Stateless
{
    public sealed class TrinityStatelessService : StatelessService
    {
        public TrinityServer TrinityServer { get; private set; }
        public string TrinityServerEndpointName { get; private set; }

        public TrinityStatelessService(StatelessServiceContext serviceContext,
            TrinityServer trinityServer, string trinityServerEndpointName, ITrinityStorageImage externalStroage) : base(serviceContext)
        {
            TrinityServer = trinityServer;
            TrinityServerEndpointName = trinityServerEndpointName;
            Global.CreateLocalMemoryStorage = () => new LocalMemoryStorageBackedByExternal(externalStroage);
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[]
            {
                new ServiceInstanceListener(_ => new TrinityServerCommunicationListener(this), TrinityServerCommunicationListener.Name)
            };
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await OpenMemoryCloudAsync(cancellationToken);

            if (Global.LocalStorage.LoadStorage() == false)
                throw new Exception("Failed to load local storage");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }

        private async Task OpenMemoryCloudAsync(CancellationToken cancellationToken)
        {
            var instances = (await Utilities.ResolveTrinityClusterConfigAsync(Context.ServiceName, Context.PartitionId, cancellationToken)).ToList();
            var myServerId = Enumerable.Range(0, instances.Count).First(idx => instances[idx].InstanceId == Context.InstanceId);

            TrinityConfig.CurrentRunningMode = RunningMode.Server;
            TrinityConfig.CurrentClusterConfig.ListeningPort = instances[myServerId].Port;

            TrinityServer.InitializeCloudMemory();
            TrinityServer.InitializeModules();

            Global.CloudStorage.ServerDisconnected += (sender, e) =>
            {
                Log.Info("Disconnected with Server {0}", e.ServerId);
            };

            Log.Info("Memory cloud opened. ServerCount={0}, MyServerId={1}, Address={2}:{3}",
                Global.ServerCount, Global.MyServerId, Global.MyIPEndPoint.Address, Global.MyIPEndPoint.Port);
        }
    }
}

using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
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
    public class TrinityStatelessService : StatelessService
    {
        public StatelessClusterConfig ClusterConfig { get; private set; }
        public TrinityServer TrinityServer { get; private set; }
        public string TrinityServerEndpointName { get; private set; }

        public TrinityStatelessService(StatelessServiceContext serviceContext,
            TrinityServer trinityServer, string trinityServerEndpointName,
            Func<TrinityStatelessService, ITrinityStorageImage> externalStroageFactory) : base(serviceContext)
        {
            TrinityServer = trinityServer;
            TrinityServerEndpointName = trinityServerEndpointName;

            TrinityConfig.CreateClusterConfig = () => ClusterConfig;
            Global.CreateLocalMemoryStorage = () => new LocalMemoryStorageBackedByExternal(this, externalStroageFactory(this));
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            ClusterConfig = StatelessClusterConfig.Resolve(Context.ServiceName, Context.PartitionId, RunningMode.Server);
            ClusterConfig.MyServerId = Enumerable.Range(0, ClusterConfig.Servers.Count).First(idx => ClusterConfig.GetServerInfo(idx).InstanceId == Context.InstanceId);

            if (Global.LocalStorage.LoadStorage() == false)
                throw new Exception("Failed to load local storage");

            return new[]
            {
                new ServiceInstanceListener(_ => new TrinityServerCommunicationListener(this), TrinityServerCommunicationListener.Name)
            };
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await OpenMemoryCloudAsync(cancellationToken);

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }

        protected async Task OpenMemoryCloudAsync(CancellationToken cancellationToken)
        {
            await ClusterConfig.ResolveTrinityEndpointsAsync(cancellationToken);

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

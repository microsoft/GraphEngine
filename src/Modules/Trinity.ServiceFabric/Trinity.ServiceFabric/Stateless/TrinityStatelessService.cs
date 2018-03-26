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

        private Func<TrinityStatelessService, ITrinityStorageImage> externalStroageFactory;

        public TrinityStatelessService(StatelessServiceContext serviceContext,
            TrinityServer trinityServer, string trinityServerEndpointName,
            Func<TrinityStatelessService, ITrinityStorageImage> externalStroageFactory) : base(serviceContext)
        {
            TrinityServer = trinityServer;
            TrinityServerEndpointName = trinityServerEndpointName;
            this.externalStroageFactory = externalStroageFactory;
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            ClusterConfig = StatelessClusterConfig.Resolve(Context.ServiceName, Context.PartitionId, RunningMode.Server);
            ClusterConfig.MyServerId = Enumerable.Range(0, ClusterConfig.Servers.Count).First(idx => ClusterConfig.GetServerInfo(idx).InstanceId == Context.InstanceId);

            TrinityConfig.CreateClusterConfig = () => ClusterConfig;
            Global.CreateLocalMemoryStorage = () => new LocalMemoryStorageBackedByExternal(ClusterConfig, externalStroageFactory(this));

            Trinity.Storage.MemoryCloud.StaticGetPartitionIdByCellId = StatelessClusterConfig.GetPartitionIdByCellId;

            if (Global.LocalStorage.LoadStorage() == false)
                throw new Exception("Failed to load local storage");

            return new[]
            {
                new ServiceInstanceListener(_ => new TrinityServerCommunicationListener(this), TrinityServerCommunicationListener.Name)
            };
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await ClusterConfig.ResolveTrinityEndpointsAsync(cancellationToken);

            TrinityServer.Started += () => Log.Info("Trinity server {0}/{1} started, listening on {2}:{3}",
                Global.MyServerId, Global.ServerCount, Global.MyIPEndPoint.Address, Global.MyIPEndPoint.Port);

            TrinityServer.Start();

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}

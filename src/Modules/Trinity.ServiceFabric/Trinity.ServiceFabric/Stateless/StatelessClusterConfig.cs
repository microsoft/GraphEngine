using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using System.Fabric.Query;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Network;
using Trinity.ServiceFabric.Communication;
using Trinity.ServiceFabric.Diagnostics;

namespace Trinity.ServiceFabric.Stateless
{
    public partial class StatelessClusterConfig
    {
        private static FabricClient fabricClient;
        private Guid clusterPartitionId;

        public static StatelessClusterConfig Resolve(Uri trinityServiceName, Guid partitionId, RunningMode runningMode)
        {
            var client = UseFabricClient();
            var desc = client.ServiceManager.GetServiceDescriptionAsync(trinityServiceName).Result as StatelessServiceDescription;
            while (true)
            {
                var instList = client.QueryManager.GetReplicaListAsync(partitionId).Result;
                if (instList.Count == desc.InstanceCount)
                {
                    var config = new StatelessClusterConfig()
                    {
                        RunningMode = runningMode,
                        clusterPartitionId = partitionId,
                        Servers = instList.Select(inst => inst.Id)
                                .OrderBy(id => id)
                                .Select(id => new AvailabilityGroup(id.ToString(), new StatelessServiceInfo(id)))
                                .ToList()
                    };

                    if (runningMode != RunningMode.Server)
                        config.ResolveTrinityEndpointsAsync(CancellationToken.None).Wait();

                    return config;
                }

                Task.Delay(TimeSpan.FromSeconds(1)).Wait();
            }
        }

        public static StatelessClusterConfig Resolve(Uri trinityServiceName, RunningMode runningMode)
        {
            var client = UseFabricClient();
            var partitions = client.QueryManager.GetPartitionListAsync(trinityServiceName).Result;

            if (partitions == null || partitions.Count == 0)
                return null;

            var partitionId = partitions[0].PartitionInformation.Id;
            if (partitions.Count > 1)
                Log.Warn("Multiple trinity service partitions were found. The partition {0} will be used.", partitionId);

            return Resolve(trinityServiceName, partitionId, runningMode);
        }

        public async Task ResolveTrinityEndpointsAsync(CancellationToken cancellationToken)
        {
            var client = UseFabricClient();
            while (!cancellationToken.IsCancellationRequested)
            {
                var instances = await client.QueryManager.GetReplicaListAsync(clusterPartitionId);

                if (UpdateServerInfo(instances))
                    return;

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private bool UpdateServerInfo(ServiceReplicaList instances)
        {
            if (!instances.All(inst => inst.ReplicaStatus == ServiceReplicaStatus.Ready))
                return false;

            if (instances.Count != Servers.Count)
            {
                var e = new ArgumentException("instance count doesn't match");
                Log.Fatal(e.Message);
                throw e;
            }

            for (var i = 0; i < ServerCount; i++)
            {
                var serviceInfo = GetServerInfo(i);
                var inst = instances.FirstOrDefault(item => item.Id == serviceInfo.InstanceId);
                if (inst == null)
                {
                    var e = new Exception($"Failed to resolve trinity endpoints in cluster config: server#{i}'s instance not found");
                    Log.Fatal(e.Message);
                    throw e;
                }

                var json = JsonConvert.DeserializeObject(inst.ReplicaAddress) as JObject;
                var epAddr = (json["Endpoints"] as JObject).Value<string>(TrinityServerCommunicationListener.Name).Split(new char[] { ':' });
                serviceInfo.HostName = epAddr[0];
                serviceInfo.Port = int.Parse(epAddr[1]);
            }

            return true;
        }

        private static FabricClient UseFabricClient()
        {
            if (fabricClient == null)
                fabricClient = new FabricClient();
            return fabricClient;
        }
    }

    public partial class StatelessClusterConfig : IClusterConfig
    {
        public static int TotalStoragePartitionCount { get; set; } = 256;
        public static Func<long, int> GetPartitionIdByCellId { get; set; } = (cellId) => (int)((cellId & 0xFF00) >> 8);
        public static int StorageImageSlots { get; set; } = 2;

        public RunningMode RunningMode { get; set; } = RunningMode.Server;

        public List<AvailabilityGroup> Servers { get; private set; }
        public List<AvailabilityGroup> Proxies => null;
        public int ServerCount => Servers.Count;
        public int MyServerId { get; internal set; } = -1;
        public int MyProxyId => -1;
        public int MyInstanceId => MyServerId;

        public int ListeningPort => MyServerId == -1 ? -1 : GetServerInfo(MyServerId).Port;
        public int ServerPort => ListeningPort;
        public int ProxyPort => -1;

        internal StatelessServiceInfo GetServerInfo(int serverId) => Servers[serverId].Instances[0] as StatelessServiceInfo;
        public ServerInfo GetMyServerInfo() => MyServerId == -1 ? null : GetServerInfo(MyServerId);
        public ServerInfo GetMyProxyInfo() => null;
        public string OutputCurrentConfig() => "TODO: Output StatelessClusterConfig";
    }
}

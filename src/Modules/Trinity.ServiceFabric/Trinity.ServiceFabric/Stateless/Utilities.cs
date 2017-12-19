using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Trinity.ServiceFabric.Communication;

namespace Trinity.ServiceFabric.Stateless
{
    public static class Utilities
    {
        public static async Task<IEnumerable<StatelessServiceInfo>> ResolveTrinityClusterConfigAsync(Uri trinityServiceName, Guid partitionId, CancellationToken cancellationToken)
        {
            var instances = await ResolveTrinityInstancesAsync(trinityServiceName, partitionId, cancellationToken);

            foreach (var inst in instances)
            {
                TrinityConfig.AddServer(inst);
            }

            return instances;
        }

        public static async Task<IEnumerable<StatelessServiceInfo>> ResolveTrinityInstancesAsync(Uri serviceName, Guid partitionId, CancellationToken cancellationToken)
        {
            using (var client = new FabricClient())
            {
                var instances = new List<StatelessServiceInfo>();
                var desc = await client.ServiceManager.GetServiceDescriptionAsync(serviceName, TimeSpan.FromSeconds(10), cancellationToken) as StatelessServiceDescription;
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // Wait until all trinity service instances are ready
                    var instList = await client.QueryManager.GetReplicaListAsync(partitionId);
                    if (instList.Count == desc.InstanceCount && instList.All(inst => inst.ReplicaStatus == System.Fabric.Query.ServiceReplicaStatus.Ready))
                    {
                        foreach (var inst in instList)
                        {
                            var json = JsonConvert.DeserializeObject(inst.ReplicaAddress) as JObject;
                            var epAddr = (json["Endpoints"] as JObject).Value<string>(TrinityServerCommunicationListener.Name).Split(new char[] { ':' });
                            instances.Add(new StatelessServiceInfo(inst.Id, epAddr[0], int.Parse(epAddr[1])));
                        }

                        return instances.OrderBy(inst => inst.InstanceId);
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
        }
    }
}

using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Runtime;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Daemon;
using Trinity.Diagnostics;
using Trinity.DynamicCluster;
using Trinity.DynamicCluster.Consensus;
using Trinity.Network;

namespace Trinity.ServiceFabric
{
    public class NameService : INameService
    {
        private const int c_bgtaskInterval = 10000;
        private Task m_bgtask;
        private CancellationToken m_token;
        private Dictionary<Guid, HashSet<string>> m_replicaList;
        private List<Guid> m_partitionIds;
        private ServicePartitionResolver m_resolver;

        public string Address => GraphEngineService.Instance.Address;

        public int Port => GraphEngineService.Instance.Port;

        public int HttpPort => GraphEngineService.Instance.HttpPort;

        public Guid InstanceId { get; private set; }

        public bool IsMaster => GraphEngineService.Instance.Role == ReplicaRole.Primary;


        public NameService()
        {
            InstanceId = GetInstanceId();
            m_resolver = ServicePartitionResolver.GetDefault();
            m_partitionIds = GraphEngineService.Instance.Partitions.Select(_ => _.PartitionInformation.Id).ToList();
            m_replicaList = Enumerable.ToDictionary(m_partitionIds, _ => _, _ => new HashSet<string>());
        }

        private static Guid GetInstanceId()
        {
            return new Guid(Enumerable.Concat(
                                         GraphEngineService.Instance.NodeContext.NodeInstanceId.ToByteArray(),
                                         Enumerable.Repeat<byte>(0x0, 16))
                                        .Take(16).ToArray());
        }

        public TrinityErrorCode Start(CancellationToken token)
        {
            m_token = token;
            ServerInfo my_si = new ServerInfo(Address, Port, Global.MyAssemblyPath, TrinityConfig.LoggingLevel);
            m_bgtask = ScanNodesProc();

            return TrinityErrorCode.E_SUCCESS;
        }

        public void Dispose()
        {
            m_bgtask.Wait();
        }

        private async Task ScanNodesProc()
        {
            ResolvedServicePartition[] resrsp = Enumerable.Repeat<ResolvedServicePartition>(null, GraphEngineService.Instance.PartitionCount).ToArray();
            while (true)
            {
                if (m_token.IsCancellationRequested) return;
                try
                {
                    var tasks = Enumerable.Range(0, resrsp.Length).Zip(resrsp, ResolvePartition);
                    await Task.WhenAll(tasks);
                    resrsp = tasks.Select(_ => _.Result.Item1).ToArray();
                    m_partitionIds.Zip(tasks.Select(_ => _.Result.Item2), UpdatePartition).ToList();
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, $"ScanNodesProc: {ex.ToString()}");
                }

                await Task.Delay(c_bgtaskInterval);
            }
        }

        private int UpdatePartition(Guid id, HashSet<string> newset)
        {
            var oldset = m_replicaList[id];
            var tmp = new HashSet<string>(newset);
            newset.ExceptWith(oldset);
            foreach (var addr in newset)
            {
                var ents = addr.Substring("tcp://".Length).Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if ($"{ents[0]}:{ents[1]}" == $"{this.Address}:{this.Port}") continue;
                Log.WriteLine("{0}", $"NameService: {addr} added to partition {id}");
                NewServerInfoPublished(this, new ServerInfo(ents[0], int.Parse(ents[1]), null, LogLevel.Info));
            }
            m_replicaList[id] = tmp;
            return 0;
        }

        private async Task<(ResolvedServicePartition, HashSet<string>)> ResolvePartition(int key, ResolvedServicePartition resrsp)
        {
            if (resrsp == null)
            {
                resrsp = await m_resolver.ResolveAsync(GraphEngineService.Instance.Context.ServiceName, new ServicePartitionKey(key), m_token);
            }
            else
            {
                resrsp = await m_resolver.ResolveAsync(resrsp, m_token);
            }
            var addrs = resrsp.Endpoints
                .Select(ep => JObject.Parse(ep.Address)["Endpoints"]["GraphEngineTrinityProtocolListener"].ToString());
            return (resrsp, new HashSet<string>(addrs));
        }

        public event EventHandler<ServerInfo> NewServerInfoPublished = delegate { };
    }
}

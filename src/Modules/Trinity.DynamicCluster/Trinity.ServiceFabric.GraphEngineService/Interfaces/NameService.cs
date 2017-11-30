using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Runtime;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Fabric.Query;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
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
        private Dictionary<Guid, HashSet<ReplicaInformation>> m_replicaList;
        private List<Guid> m_partitionIds;
        private System.Fabric.FabricClient m_fclient;
        private Uri m_svcuri;

        public string Address => GraphEngineService.Instance.Address;

        public int Port => GraphEngineService.Instance.Port;

        public int HttpPort => GraphEngineService.Instance.HttpPort;

        public Guid InstanceId { get; private set; }

        public bool IsMaster => GraphEngineService.Instance?.Role == System.Fabric.ReplicaRole.Primary;


        public NameService()
        {
            InstanceId = GetInstanceId(GraphEngineService.Instance.Context.ReplicaId, GraphEngineService.Instance.PartitionId);
            m_partitionIds = GraphEngineService.Instance.Partitions.Select(_ => _.PartitionInformation.Id).ToList();
            m_replicaList = Enumerable.ToDictionary(m_partitionIds, Utils.Identity, _ => new HashSet<ReplicaInformation>());
            m_svcuri = GraphEngineService.Instance.Context.ServiceName;
            m_fclient = new System.Fabric.FabricClient();
        }

        internal static Guid GetInstanceId(long replicaId, int partitionId)
        {
            BigInteger low = new BigInteger(replicaId);
            BigInteger high = new BigInteger(partitionId) << 64;
            return new Guid(Enumerable.Concat((low + high).ToByteArray(),
                                        Enumerable.Repeat<byte>(0x0, 16))
                                       .Take(16).ToArray());
        }

        private int GetPartitionId(Guid partitionGuid) => m_partitionIds.FindIndex(_ => _ == partitionGuid);

        public void Start(CancellationToken token)
        {
            m_token = token;
            m_bgtask = ScanNodesProc();
        }

        public void Dispose()
        {
            m_bgtask.Wait();
        }

        private async Task ScanNodesProc()
        {
            while (true)
            {
                if (m_token.IsCancellationRequested) return;
                try
                {
                    var tasks = m_partitionIds.Select(ResolvePartition);
                    await Task.WhenAll(tasks);
                    m_partitionIds.Zip(tasks.Select(_ => _.Result), UpdatePartition).ToList();
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, $"ScanNodesProc: {ex.ToString()}");
                }

                await Task.Delay(c_bgtaskInterval);
            }
        }

        private int UpdatePartition(Guid partitionGuid, HashSet<ReplicaInformation> newset)
        {
            var oldset = m_replicaList[partitionGuid];
            foreach (var r in newset.Except(oldset))
            {
                if (r.Address == this.Address && r.Port == this.Port) continue;
                Log.WriteLine("{0}", $"NameService: {r} added to partition {r.PartitionId} ({partitionGuid})");
                NewReplicaInformationPublished(this, r);
            }
            m_replicaList[partitionGuid] = newset;
            return 0;
        }


        private async Task<HashSet<ReplicaInformation>> ResolvePartition(Guid partId)
        {
            var rs = await m_fclient.QueryManager.GetReplicaListAsync(partId);
            var ris = rs.Select(r => GetReplicaInformation(partId, r)).Where(_ => _ != null);
            return new HashSet<ReplicaInformation>(ris);
        }

        private ReplicaInformation GetReplicaInformation(Guid partitionGuid, Replica r)
        {
            try
            {
                var partitionId = GetPartitionId(partitionGuid);
                var rid = GetInstanceId(r.Id, partitionId);
                var (addr, port) = JObject.Parse(r.ReplicaAddress)["Endpoints"]["GraphEngineTrinityProtocolListener"].ToString()
                                  .Substring("tcp://".Length).Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                return new ReplicaInformation(addr, int.Parse(port), rid, partitionId);
            }
            catch { return null; }
        }

        public event EventHandler<ReplicaInformation> NewReplicaInformationPublished = delegate { };
    }
}

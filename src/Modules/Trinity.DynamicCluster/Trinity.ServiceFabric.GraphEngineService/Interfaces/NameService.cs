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
        private CancellationToken m_token;
        private System.Fabric.FabricClient m_fclient;
        private Uri m_svcuri;
        private List<Guid> m_partitionIds;

        public string Address => GraphEngineService.Instance.Address;

        public int Port => GraphEngineService.Instance.Port;

        public int HttpPort => GraphEngineService.Instance.HttpPort;

        public Guid InstanceId { get; private set; }

        public bool IsMaster => GraphEngineService.Instance?.Role == System.Fabric.ReplicaRole.Primary;


        public NameService()
        {
            InstanceId = GetInstanceId(GraphEngineService.Instance.Context.ReplicaId, GraphEngineService.Instance.PartitionId);
            m_svcuri = GraphEngineService.Instance.Context.ServiceName;
            m_fclient = new System.Fabric.FabricClient();
            m_partitionIds = GraphEngineService.Instance.Partitions.Select(_ => _.PartitionInformation.Id).ToList();
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

        public int PartitionCount => GraphEngineService.Instance.PartitionCount;

        public int PartitionId => GraphEngineService.Instance.PartitionId;

        public void Start(CancellationToken token)
        {
            m_token = token;
        }

        public void Dispose() { }

        public async Task<IEnumerable<ReplicaInformation>> ResolvePartition(int partId)
        {
            var partGuid = m_partitionIds[partId];
            var rs = await m_fclient.QueryManager.GetReplicaListAsync(partGuid);
            return rs.Select(r => GetReplicaInformation(partGuid, r)).Where(_ => _ != null);
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
    }
}

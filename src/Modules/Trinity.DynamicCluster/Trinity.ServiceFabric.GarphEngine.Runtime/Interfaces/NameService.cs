using Microsoft.ServiceFabric.Data.Collections;
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
using Trinity.DynamicCluster.Storage;
using Trinity.Network;
using Trinity.ServiceFabric.GarphEngine.Infrastructure;

namespace Trinity.ServiceFabric.GarphEngine.Infrastructure.Interfaces
{
    public class NameService : INameService
    {
        private CancellationToken          m_token;
        private System.Fabric.FabricClient m_fclient;
        private List<Guid>                 m_partitionIds;
        private Task m_init;
        private long m_handle;

        public string Address => GraphEngineStatefulServiceRuntime.Instance.Address;

        public int Port => GraphEngineStatefulServiceRuntime.Instance.Port;

        public int HttpPort => GraphEngineStatefulServiceRuntime.Instance.HttpPort;

        public Guid InstanceId { get; private set; }

        public bool IsMaster => GraphEngineStatefulServiceRuntime.Instance?.Role == System.Fabric.ReplicaRole.Primary;

        internal static Guid GetInstanceId(long replicaId, int partitionId)
        {
            BigInteger low = new BigInteger(replicaId);
            BigInteger high = new BigInteger(partitionId) << 64;
            return new Guid(Enumerable.Concat((low + high).ToByteArray(),
                                        Enumerable.Repeat<byte>(0x0, 16))
                                       .Take(16).ToArray());
        }

        private int GetPartitionId(Guid partitionGuid) => m_partitionIds.FindIndex(_ => _ == partitionGuid);

        public int PartitionCount => GraphEngineStatefulServiceRuntime.Instance.PartitionCount;

        public int PartitionId => GraphEngineStatefulServiceRuntime.Instance.PartitionId;

        public void Start(CancellationToken token)
        {
            InstanceId = GetInstanceId(GraphEngineStatefulServiceRuntime.Instance.Context.ReplicaId, GraphEngineStatefulServiceRuntime.Instance.PartitionId);
            m_token = token;
            m_fclient = new System.Fabric.FabricClient();
            m_partitionIds = GraphEngineStatefulServiceRuntime.Instance.Partitions.Select(_ => _.PartitionInformation.Id).ToList();
            var listen_to = new System.Fabric.Description.ServiceNotificationFilterDescription();
            m_handle = m_fclient.ServiceManager.RegisterServiceNotificationFilterAsync(listen_to).Result;
        }

        private async Task PollMaster()
        {
            await m_init;
            if (!IsMaster) return;
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
                var (addr, port) = JObject.Parse(r.ReplicaAddress)["Endpoints"][GraphEngineConstants.GraphEngineListenerName].ToString()
                                  .Substring("tcp://".Length).Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                return new ReplicaInformation(addr, int.Parse(port), rid, partitionId);
            }
            catch { return null; }
        }
    }
}

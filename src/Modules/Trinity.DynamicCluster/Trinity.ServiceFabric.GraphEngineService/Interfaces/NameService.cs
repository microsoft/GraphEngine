using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Runtime;
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
using Trinity.DynamicCluster.Consensus;
using Trinity.Network;

namespace Trinity.ServiceFabric
{
    public class NameService : INameService
    {
        private const int c_bgtaskInterval = 10000;
        private Task m_bgtask;
        private CancellationToken m_token;
        private Dictionary<Guid, int> m_replicaList;
        private ServicePartitionResolver m_resolver;

        public string Address => GraphEngineService.Instance.Address;

        public int Port => GraphEngineService.Instance.Port;

        public int HttpPort => GraphEngineService.Instance.HttpPort;

        public Guid InstanceId { get; private set; }

        public bool IsMaster => GraphEngineService.Instance.Role == ReplicaRole.Primary;

        public NameService()
        {
            InstanceId = new Guid(Enumerable.Concat(
                             GraphEngineService.Instance.NodeContext.NodeInstanceId.ToByteArray(),
                             Enumerable.Repeat<byte>(0x0, 16))
                            .Take(16).ToArray());
            m_replicaList = new Dictionary<Guid, int>();
            m_resolver = ServicePartitionResolver.GetDefault();
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
            while (true)
            {
                if (m_token.IsCancellationRequested) return;
                if (IsMaster)
                {
                    var rsp = await m_resolver.ResolveAsync(GraphEngineService.Instance.Context.ServiceName, new ServicePartitionKey(), m_token);
                    Log.WriteLine(rsp.ToString());
                }

                await Task.Delay(c_bgtaskInterval);
            }
        }

        public event EventHandler<(Guid, ServerInfo)> NewServerInfoPublished = delegate { };
    }
}

using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Trinity.Daemon;
using Trinity.DynamicCluster.Consensus;
using Trinity.Network;

namespace Trinity.ServiceFabric
{
    public class NameService : INameService
    {
        private BackgroundTask m_bgtask;
        private const int c_bgtaskInterval = 10000;

        public string Address => GraphEngineService.Instance.Address;

        public int Port => GraphEngineService.Instance.Port;

        public int HttpPort => GraphEngineService.Instance.HttpPort;

        public NameService()
        {
            m_bgtask = new BackgroundTask(ScanNodesProc, c_bgtaskInterval);
        }

        public TrinityErrorCode Start()
        {
            BackgroundThread.AddBackgroundTask(m_bgtask);
            return TrinityErrorCode.E_SUCCESS;
        }

        public void Dispose()
        {
            BackgroundThread.RemoveBackgroundTask(m_bgtask);
        }

        private int ScanNodesProc()
        {
            return c_bgtaskInterval;
        }

        public event EventHandler<Tuple<NameDescriptor, ServerInfo>> NewServerInfoPublished;

        public TrinityErrorCode PublishServerInfo(NameDescriptor name, ServerInfo serverInfo)
        {
            return TrinityErrorCode.E_SUCCESS;
        }
    }
}

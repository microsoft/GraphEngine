using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.Network;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.Networking.WCFService.Client
{
    internal class WCFRemoteStorage : RemoteStorage
    {
        private WCFClientMemoryCloud m_memorycloud;
        private TrinityWCFAdapterClient m_wcfclient;

        protected internal WCFRemoteStorage(WCFClientMemoryCloud mc, int partitionId, TrinityWCFAdapterClient trinityWCFAdapterClient)
            : base(Enumerable.Empty<ServerInfo>(), connPerServer: 0, mc: null, partitionId: partitionId, nonblocking: true)
        {
            m_memorycloud = mc;
            m_wcfclient = trinityWCFAdapterClient;
        }
    }
}

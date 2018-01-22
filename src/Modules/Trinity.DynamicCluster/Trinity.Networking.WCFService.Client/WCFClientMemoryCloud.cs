using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.Networking.WCFService.Client
{
    class WCFClientMemoryCloud : MemoryCloud
    {
        private IStorage[] m_storagetable;

        public override int MyPartitionId => throw new NotImplementedException();

        public override int MyProxyId => throw new NotImplementedException();

        public override IEnumerable<Chunk> MyChunks => throw new NotImplementedException();

        public override int PartitionCount => throw new NotImplementedException();

        public override int ProxyCount => throw new NotImplementedException();

        public override IList<RemoteStorage> ProxyList => throw new NotImplementedException();

        protected override IList<IStorage> StorageTable => throw new NotImplementedException();

        public override int MyInstanceId => throw new NotImplementedException();

        public override bool IsLocalCell(long cellId)
        {
            throw new NotImplementedException();
        }

        public override bool LoadStorage()
        {
            throw new NotImplementedException();
        }

        public override bool ResetStorage()
        {
            throw new NotImplementedException();
        }

        public override bool SaveStorage()
        {
            throw new NotImplementedException();
        }

        public override bool Open(ClusterConfig config, bool nonblocking)
        {
            this.m_storagetable = new IStorage[]
            {
                new WCFRemoteStorage(this, 0, new TrinityWCFAdapterClient(
                    new WcfCommunicationClientFactory(),
                    //TODO url pull
                    null, 
            // [uniform: ]
            // [MinValue, -1]
            // [0, MaxValue]
                    partitionKey: new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(0)
                    ))
            };

            return true;
        }

        public override long GetTotalMemoryUsage()
        {
            throw new NotImplementedException();
        }
    }
}

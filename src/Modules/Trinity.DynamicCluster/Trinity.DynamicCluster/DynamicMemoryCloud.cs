using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.DynamicCluster
{
    public class DynamicMemoryCloud : MemoryCloud
    {
        public override IEnumerable<int> MyChunkIds
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int MyPartitionId
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int MyProxyId
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int ProxyCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override IList<RemoteStorage> ProxyList
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int ServerCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool IsLocalCell(long cellId)
        {
            throw new NotImplementedException();
        }

        public override bool LoadStorage()
        {
            throw new NotImplementedException();
        }

        public override bool Open(ClusterConfig config, bool nonblocking)
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

        public override unsafe void SendMessageToProxy(int proxyId, byte* buffer, int size)
        {
            throw new NotImplementedException();
        }

        public override unsafe void SendMessageToProxy(int proxyId, byte* buffer, int size, out TrinityResponse response)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Storage;

namespace Trinity.ServiceFabric.Client
{
    public class ClientMemoryCloud : MemoryCloud
    {
        public override int MyInstanceId => throw new NotImplementedException();

        public override int MyPartitionId => throw new NotImplementedException();

        public override int MyProxyId => throw new NotImplementedException();

        public override IEnumerable<Chunk> MyChunks => throw new NotImplementedException();

        public override int PartitionCount => throw new NotImplementedException();

        public override int ProxyCount => throw new NotImplementedException();

        public override IList<RemoteStorage> ProxyList => throw new NotImplementedException();

        protected override IList<IStorage> StorageTable => throw new NotImplementedException();

        public override long GetTotalMemoryUsage()
        {
            throw new NotImplementedException();
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
    }
}

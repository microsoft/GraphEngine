using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Client;
using Trinity.Extension;
using Trinity.Storage;

namespace Trinity.Client
{
    [ExtensionPriority(-200)]
    internal partial class ClientMemoryCloud : MemoryCloud
    {
        private static int s_instanceId;
        private static int s_partitionCount;
        private static IMessagePassingEndpoint s_ep;
        private static TrinityClient s_client;
        private static List<RedirectedIStorage> s_redir_storages = new List<RedirectedIStorage>();

        internal static void Initialize(IMessagePassingEndpoint ep, TrinityClient tc)
        {
            s_ep = ep;
            s_redir_storages.Clear();
            s_redir_storages.Add(new RedirectedIStorage(ep, tc));
        }

        public override int MyInstanceId => s_instanceId;

        public override int MyPartitionId => -1;

        public override int MyProxyId => throw new NotSupportedException();

        public override IEnumerable<Chunk> MyChunks => Enumerable.Empty<Chunk>();

        //TODO query server about partition count
        public override int PartitionCount => s_partitionCount;

        public override int ProxyCount => throw new NotSupportedException();

        public override IList<RemoteStorage> ProxyList => throw new NotSupportedException();

        //TODO redirected storage
        protected override IList<IStorage> StorageTable => s_redir_storages as IList<IStorage>;

        public override long GetTotalMemoryUsage()
        {
            throw new NotImplementedException();
        }

        public override bool IsLocalCell(long cellId)
            => false;

        public override bool LoadStorage()
        {
            throw new NotImplementedException();
        }

        public override bool Open(ClusterConfig config, bool nonblocking) => true;

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

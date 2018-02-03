using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Client;
using Trinity.Extension;
using Trinity.Storage;
using Trinity.Client.TrinityClientModule;

namespace Trinity.Client
{
    [ExtensionPriority(-200)]
    internal partial class ClientMemoryCloud : MemoryCloud
    {
        private static int s_instanceId = -1;
        private static int s_cookie = -1;
        private static IMessagePassingEndpoint s_ep = null;
        private static TrinityClient s_client = null;
        private static IList<IStorage> s_redir_storages = null;
        private static TrinityClientModule.TrinityClientModule s_cmod = null;

        internal static void BeginInitialize(IMessagePassingEndpoint ep, TrinityClient tc)
        {
            s_ep = ep;
            s_client = tc;
            s_redir_storages = new List<IStorage> { new PassThroughIStorage(ep) };
        }

        internal static void EndInitialize()
        {
            s_cmod = s_client.GetCommunicationModule<TrinityClientModule.TrinityClientModule>();
            s_cookie = s_cmod.MyCookie;
            int partitionCount = 0;
            
            using(var req = new RegisterClientRequestWriter(s_cmod.MyCookie))
            using(var rsp = s_ep.RegisterClient(req))
            {
                partitionCount = rsp.PartitionCount;
                s_instanceId = rsp.InstanceId;
            }
            s_redir_storages = Enumerable.Range(0, partitionCount).Select(p => new RedirectedIStorage(s_ep, s_client, p)).ToList() as IList<IStorage>;
        }

        public override int MyInstanceId => s_instanceId;

        public override int MyPartitionId => -1;

        public override int MyProxyId => -1;

        public override IEnumerable<Chunk> MyChunks => Enumerable.Empty<Chunk>();

        //TODO query server about partition count
        public override int PartitionCount => s_redir_storages.Count;

        public override int ProxyCount => throw new NotSupportedException();

        public override IList<RemoteStorage> ProxyList => throw new NotSupportedException();

        //TODO redirected storage
        protected override IList<IStorage> StorageTable => s_redir_storages;

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

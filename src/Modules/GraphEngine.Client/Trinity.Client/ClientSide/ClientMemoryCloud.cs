using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Trinity.Extension;
using Trinity.Storage;

namespace Trinity.Client.ClientSide
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

            s_redir_storages = null;

            using (var req = new RegisterClientRequestWriter(s_cmod.MyCookie))
            using (var rsp = s_ep.RegisterClient(req))
            {
                partitionCount = rsp.PartitionCount;
                s_instanceId = rsp.InstanceId;
            }
            s_redir_storages = Enumerable.Range(0, partitionCount).Select(p => new RedirectedIStorage(s_ep, s_client, p) as IStorage).ToList() as IList<IStorage>;
        }

        public override int MyInstanceId => s_instanceId;

        public override int MyPartitionId => -1;

        public override int MyProxyId => -1;

        public override IEnumerable<Chunk> MyChunks => Enumerable.Empty<Chunk>();

        public override int PartitionCount => _GetRedirStorages().Count;

        private IList<IStorage> _GetRedirStorages()
        {
            IList<IStorage> ret = null;
            while(null == (ret = s_redir_storages))
            {
                Thread.Sleep(1);
            }
            return ret;
        }

        public override int ProxyCount => throw new NotSupportedException();

        public override IList<RemoteStorage> ProxyList => throw new NotSupportedException();

        protected override IList<IStorage> StorageTable => _GetRedirStorages();

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

using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Client;
using Trinity.Extension;
using Trinity.Storage;
using Trinity.Client.TrinityClientModule;
using System.Threading;
using System.Runtime.CompilerServices;

namespace Trinity.Client
{
    [ExtensionPriority(-200)]
    internal partial class ClientMemoryCloud : MemoryCloud
    {
        private static IMessagePassingEndpoint s_ep = null;
        private static TrinityClient s_client = null;
        private static IList<IStorage> s_redir_storages = null;

        private int m_partitionCount = -1;
        private int m_instanceId = -1;
        private int m_cookie = -1;
        private IMessagePassingEndpoint m_ep = null;
        private TrinityClient m_client = null;
        private IList<IStorage> m_redir_storages = null;
        private TrinityClientModule.TrinityClientModule m_cmod = null;

        internal static void Initialize(IMessagePassingEndpoint ep, TrinityClient tc)
        {
            s_ep = ep;
            s_client = tc;

            //  during initialization, there's a single 
            //  PassThroughIStorage in our storage table.
            s_redir_storages = new List<IStorage> { new PassThroughIStorage(ep) };
        }

        public void RegisterClient()
        {
            //  copy from initialization result
            m_client = s_client;
            m_ep = s_ep;

            m_cmod = m_client.GetCommunicationModule<TrinityClientModule.TrinityClientModule>();
            m_cookie = m_cmod.MyCookie;

            using (var req = new RegisterClientRequestWriter(m_cmod.MyCookie))
            using (var rsp = m_ep.RegisterClient(req))
            {
                m_partitionCount = rsp.PartitionCount;
                m_instanceId = rsp.InstanceId;
            }

            //  after initialization, we switch from pass-through storage
            //  to redirecting storage.
            SetPartitionMethod(GetServerIdByCellIdDefault);

            m_redir_storages = Enumerable.Range(0, m_partitionCount).Select(p => new RedirectedIStorage(m_ep, m_client, p)).Cast<IStorage>().ToList();
        }

        // TODO: we should negotiate with the server about partitioning scheme
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe int GetServerIdByCellIdDefault(long cellId)
        {
            return (*(((byte*)&cellId) + 1)) % m_partitionCount;
        }

        public override int MyInstanceId => m_instanceId;

        public override int MyPartitionId => -1;

        public override int MyProxyId => -1;

        public override IEnumerable<Chunk> MyChunks => Enumerable.Empty<Chunk>();

        public override int PartitionCount => _GetRedirStorages().Count;

        /// <summary>
        /// The memory cloud is first accessed when querying remote host for module alignment.
        /// This should happen after the client module has set up the passthrough storage.
        /// Afterwards, when the memory cloud is started and we register the client, we
        /// switch to redirected storages.
        /// </summary>
        private IList<IStorage> _GetRedirStorages()
        {
            return m_redir_storages ?? s_redir_storages;
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
            try
            {
                m_ep.LoadStorage();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool Open(ClusterConfig config, bool nonblocking) => true;

        public override bool ResetStorage()
        {
            try
            {
                m_ep.ResetStorage();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool SaveStorage()
        {
            try
            {
                m_ep.SaveStorage();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Trinity;
using Trinity.Network.Messaging;
using Trinity.Diagnostics;
using System.Runtime.CompilerServices;
using Trinity.Network;
using Trinity.Utilities;
using Trinity.Storage;
using Trinity.DynamicCluster;
using static Trinity.DynamicCluster.Utils;

namespace Trinity.Storage
{
    public unsafe partial class DynamicMemoryCloud : MemoryCloud
    {
        //private int server_count = -1;
        private int partition_count = -1;    
        private int my_partition_id = -1;
        private int my_proxy_id = -1;
        private Random m_rng = new Random();
        private ChunkCollection m_chunks = new ChunkCollection(new int[] { });
        //private ChunkedStorage[] ChunkedStorageTable;//better to integrate in memorycloud.cs? 

        private static List<long> chunk_range = new List<long>();
        public static List<long> ChunkRange
        {
            get { return chunk_range; }

            set { chunk_range = value; }
        }

        internal ClusterConfig cluster_config;//TODO will be substituted
        internal DynamicClusterConfig dynamic_cluster_config;
        internal INameService m_nameservice; 
        internal NameDescriptor m_namedescriptor = new NameDescriptor();
        internal ChunkedStorage ChunkedStorageTable(int id) => StorageTable[id] as ChunkedStorage;
        private Dictionary<int, DynamicRemoteStorage> temporaryRemoteStorageRepo = new Dictionary<int, DynamicRemoteStorage>();

        private int _PutTempStorage(DynamicRemoteStorage tmpstorage)
        {
            lock (this)
            {
                int temp_id = Infinity(() => m_rng.Next(-10000000, -1))
                         .SkipWhile(temporaryRemoteStorageRepo.ContainsKey)
                         .First();
                temporaryRemoteStorageRepo[temp_id] = tmpstorage;
                return temp_id;
            }
        }

        internal TrinityErrorCode OnStorageJoin(DynamicRemoteStorage remoteStorage)
        {
            Random r = new Random();
            int temp_id = _PutTempStorage(remoteStorage);
            var module = GetCommunicationModule<DynamicClusterCommModule>();
            using (var remotestorage_info = module.QueryChunkedRemoteStorageInformation(temp_id))
            {
                ChunkedStorageTable(remotestorage_info.partitionid).Mount(remoteStorage, remotestorage_info);
            }

            lock (this) { temporaryRemoteStorageRepo.Remove(temp_id); }
            return TrinityErrorCode.E_SUCCESS;
        }

        /// <summary>
        /// It is guaranteed that OnStorageLeave will only be called
        /// on those storages that has been previously sent to
        /// [OnStorageJoin]
        /// </summary>
        /// <param name="remoteStorage"></param>
        internal TrinityErrorCode OnStorageLeave(int partitionid, string name)
        {
            var module         = GetCommunicationModule<DynamicClusterCommModule>();
            var remote_storage = ChunkedStorageTable(partitionid)
                                .OfType<DynamicRemoteStorage>()
                                .FirstOrDefault(_ => _.Iscalled(name));
            if (remote_storage == null) return TrinityErrorCode.E_FAILURE;
            int temp_id        = _PutTempStorage(remote_storage);
            TrinityErrorCode errno = ChunkedStorageTable(partitionid).Unmount(remote_storage);
            lock (this) { temporaryRemoteStorageRepo.Remove(temp_id); }
            return errno;
        }

        public override IEnumerable<int> MyChunkIds//better if named MyChunkCollection, return type is list?
        {
            get
            {
                return m_chunks;
            }
        }

        public override int MyPartitionId
        {
            get
            {
                return dynamic_cluster_config.LocalPartitionId;
            }
        }

        public override int MyProxyId
        {
            get
            {
                return my_proxy_id;
            }
        }

        public override int ProxyCount
        {
            get
            {
                return cluster_config.Proxies.Count;
            }
        }

        public override int PartitionCount
        {
            get
            {
                return DynamicClusterConfig.Instance.PartitionCount;
            }
        }

        public int ChunkCount
        {
            get
            {
                return m_chunks.Count();
            }
        }

        public string MyNickname
        {
            get
            {
                return m_namedescriptor.Nickname;
            }
        }

        public override bool IsLocalCell(long cellId)
        {
            return (GetStorageByCellId(cellId) as ChunkedStorage).IsLocal(cellId);
        }

        public override bool Open(ClusterConfig config, bool nonblocking)
        {
            Log.WriteLine($"DynamicMemoryCloud: server {m_namedescriptor.Nickname} starting.");
            TrinityErrorCode errno = TrinityErrorCode.E_SUCCESS;

            this.cluster_config = config;
            my_partition_id = DynamicClusterConfig.Instance.LocalPartitionId;
            my_proxy_id = -1;
            partition_count = DynamicClusterConfig.Instance.PartitionCount;
            StorageTable = Infinity(() => new ChunkedStorage())
                          .Take(partition_count)
                          .ToArray();

            if (partition_count == 0)
                goto server_not_found;

            if (TrinityErrorCode.E_SUCCESS != (errno = ChunkedStorageTable(my_partition_id).Mount(Global.LocalStorage, MyChunkIds)))
            {
                //TODO 
            }

            m_nameservice = AssemblyUtility.GetAllClassInstances(t => t.GetConstructor(new Type[] { }).Invoke(new object[] { }) as INameService).First();
            m_nameservice.NewServerInfoPublished += (o, e) =>
            {
                var name = e.Item1;
                var si = e.Item2;
                Log.WriteLine($"DynamicCluster: New server info published: {name.Nickname}, {si.ToString()}");
                Task t = new Task(() => Connect(name, si));
                t.Start();
            };
            ServerInfo my_si = new ServerInfo(Global.MyIPAddress.ToString(), Global.MyIPEndPoint.Port, Global.MyAssemblyPath, TrinityConfig.LoggingLevel);
            m_nameservice.PublishServerInfo(m_namedescriptor, my_si);
            ServerConnected += DynamicMemoryCloud_ServerConnected;
            ServerDisconnected += DynamicMemoryCloud_ServerDisconnected;

            bool server_found = true;
            if (cluster_config.RunningMode == RunningMode.Server && !server_found)
            {
                goto server_not_found;
            }

            if (!nonblocking) { CheckServerProtocolSignatures(); } // TODO should also check in nonblocking setup when any remote storage is connected
            // else this.ServerConnected += (_, __) => {};

            return true;

        server_not_found:
            if (cluster_config.RunningMode == RunningMode.Server || cluster_config.RunningMode == RunningMode.Client)
                Log.WriteLine(LogLevel.Warning, "Incorrect server configuration. Message passing via CloudStorage not possible.");
            else if (cluster_config.RunningMode == RunningMode.Proxy)
                Log.WriteLine(LogLevel.Warning, "No servers are found. Message passing to servers via CloudStorage not possible.");
            return false;

        }

        public TrinityErrorCode Shutdown()
        {
            Random r = new Random();
            var module = GetCommunicationModule<DynamicClusterCommModule>();
            var rs = Enumerable
                    .Range(0, PartitionCount)
                    .SelectMany(ChunkedStorageTable)
                    .OfType<DynamicRemoteStorage>();
            foreach(var s in rs)
            {
                int temp_id = _PutTempStorage(s);
                using (var request = new _MotivateRemoteStorageOnLeavingRequestWriter(MyPartitionId, MyNickname))
                { module.MotivateRemoteStorageOnLeaving(temp_id, request); }
                lock (this) { temporaryRemoteStorageRepo.Remove(temp_id); }
            }

            //then?
            return TrinityErrorCode.E_SUCCESS;
        }

         private void DynamicMemoryCloud_ServerDisconnected(object sender, ServerStatusEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DynamicMemoryCloud_ServerConnected(object sender, ServerStatusEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Connect(NameDescriptor name, ServerInfo si)
        {
            Log.WriteLine($"DynamicCluster: connecting to {name} at {si.HostName}:{si.Port}");
            DynamicRemoteStorage rs = new DynamicRemoteStorage(si, name, TrinityConfig.ClientMaxConn);
            OnStorageJoin(rs);
        }


        private void CheckServerProtocolSignatures()
        {
            Log.WriteLine("Checking {0}-Server protocol signatures...", cluster_config.RunningMode);
            int my_server_id = (cluster_config.RunningMode == RunningMode.Server) ? MyPartitionId : -1;
            var storage = StorageTable.Where((_, idx) => idx != my_server_id).FirstOrDefault() as RemoteStorage;

            CheckProtocolSignatures_impl(storage, cluster_config.RunningMode, RunningMode.Server);
        }

        private void CheckProxySignatures(IEnumerable<RemoteStorage> proxy_list)
        {
            Log.WriteLine("Checking {0}-Proxy protocol signatures...", cluster_config.RunningMode);
            int my_proxy_id = (cluster_config.RunningMode == RunningMode.Proxy) ? MyProxyId : -1;
            var storage = proxy_list.Where((_, idx) => idx != my_proxy_id).FirstOrDefault();

            CheckProtocolSignatures_impl(storage, cluster_config.RunningMode, RunningMode.Proxy);
        }

        internal int GetServerIdByIPE(IPEndPoint ipe)
        {
            for (int i = 0; i < cluster_config.Servers.Count; i++)
            {
                if (cluster_config.Servers[i].Has(ipe))
                    return i;
            }
            return -1;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using Trinity.Diagnostics;
using Trinity.Network;
using Trinity.Storage;
using Trinity.Utilities;
using Trinity.DynamicCluster.Consensus;
using Trinity.DynamicCluster.Configuration;

using static Trinity.DynamicCluster.Utils;
using Trinity.DynamicCluster.Communication;

namespace Trinity.DynamicCluster.Storage
{
    public unsafe partial class DynamicMemoryCloud : MemoryCloud
    {
        private int partition_count = -1;    
        private int my_partition_id = -1;
        private int my_proxy_id = -1;
        private Random m_rng = new Random();

        internal ClusterConfig cluster_config;//TODO will be substituted
        internal INameService m_nameservice; 
        internal NameDescriptor m_namedescriptor = new NameDescriptor();
        internal ChunkedStorage ChunkedStorageTable(int id) => StorageTable[id] as ChunkedStorage;
        private ConcurrentDictionary<int, DynamicRemoteStorage> temporaryRemoteStorageRepo = new ConcurrentDictionary<int, DynamicRemoteStorage>();

        internal static DynamicMemoryCloud Instance => Global.CloudStorage as DynamicMemoryCloud;

        private int _PutTempStorage(DynamicRemoteStorage tmpstorage)
        {
            return Infinity(() => m_rng.Next(-10000000, -1))
                  .SkipWhile(_ => !temporaryRemoteStorageRepo.TryAdd(_, tmpstorage))
                  .First();
        }

        internal TrinityErrorCode OnStorageJoin(DynamicRemoteStorage remoteStorage)
        {
            // TODO refactor: DoWithTempoStorage
            int temp_id = _PutTempStorage(remoteStorage);
            var module = GetCommunicationModule<DynamicClusterCommModule>();
            using (var rsp = module.QueryChunkedRemoteStorageInformation(temp_id))
            {
                ChunkedStorageTable(rsp.info.partition).Mount(remoteStorage, rsp);
            }

            temporaryRemoteStorageRepo.TryRemove(temp_id, out var _);
            return TrinityErrorCode.E_SUCCESS;
        }

        /// <summary>
        /// It is guaranteed that OnStorageLeave will only be called
        /// on those storages that has been previously sent to
        /// <see cref="OnStorageJoin"/>. However, OnStorageLeave may
        /// be called multiple times (disconnected, then reconnected,
        /// or remote <see cref="Shutdown"/> is called).
        /// </summary>
        /// <param name="remoteStorage"></param>
        internal TrinityErrorCode OnStorageLeave(int partition, NameDescriptor name)
        {
            var module         = GetCommunicationModule<DynamicClusterCommModule>();
            var remote_storage = ChunkedStorageTable(partition)
                                .OfType<DynamicRemoteStorage>()
                                .FirstOrDefault(_ => _.Iscalled(name));
            if (remote_storage == null) return TrinityErrorCode.E_FAILURE;
            return ChunkedStorageTable(partition).Unmount(remote_storage);
        }

        public override IEnumerable<int> MyChunkIds
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public NameDescriptor MyName
        {
            get
            {
                return m_namedescriptor;
            }
        }


        public override int MyPartitionId
        {
            get
            {
                return DynamicClusterConfig.Instance.LocalPartitionId;
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
                throw new NotImplementedException();
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
                string errmsg = $"DynamicMemoryCloud: server {m_namedescriptor.Nickname} failed to mount itself.";
                Log.WriteLine(LogLevel.Error, errmsg);
                throw new Exception(errmsg);
            }

            bool server_found = true;
            if (cluster_config.RunningMode == RunningMode.Server && !server_found)
            {
                goto server_not_found;
            }

            InitializeNameService();

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

        private void InitializeNameService()
        {
            m_nameservice = AssemblyUtility.GetAllClassInstances<INameService>().First();
            m_nameservice.NewServerInfoPublished += (o, e) =>
            {
                var name = e.Item1;
                var si = e.Item2;
                Log.WriteLine($"DynamicCluster: New server info published: {name.Nickname}, {si.ToString()}");

                DynamicRemoteStorage rs = new DynamicRemoteStorage(si, name, TrinityConfig.ClientMaxConn, this, 0, nonblocking: true);
                OnStorageJoin(rs);
            };
            m_nameservice.Start();
            ServerInfo my_si = new ServerInfo(Global.MyIPAddress.ToString(), Global.MyIPEndPoint.Port, Global.MyAssemblyPath, TrinityConfig.LoggingLevel);
            m_nameservice.PublishServerInfo(m_namedescriptor, my_si);
        }

        public TrinityErrorCode Shutdown()
        {
            var module = GetCommunicationModule<DynamicClusterCommModule>();
            var rs = Enumerable
                    .Range(0, PartitionCount)
                    .SelectMany(ChunkedStorageTable)
                    .OfType<DynamicRemoteStorage>();
            foreach(var s in rs)
            {
                int temp_id = _PutTempStorage(s);
                using (var request = new StorageInformationWriter(MyPartitionId, MyName.ServerId, MyName.Nickname))
                { module.NotifyRemoteStorageOnLeaving(temp_id, request); }
                temporaryRemoteStorageRepo.TryRemove(temp_id, out var _);
            }

            return TrinityErrorCode.E_SUCCESS;
        }

        protected override void OnConnected(RemoteStorageEventArgs e)
        {
            base.OnConnected(e);
            OnStorageJoin(e.RemoteStorage as DynamicRemoteStorage);
        }

        protected override void OnDisconnected(RemoteStorageEventArgs e)
        {
            base.OnDisconnected(e);
            var rs = e.RemoteStorage as DynamicRemoteStorage;
            OnStorageLeave(rs.PartitionId, rs.Name);
        }

        internal int GetChunkIdByCellId(long cellId)
        {
            throw new NotImplementedException();
            //return ChunkRange.FindIndex(upbound => cellId < upbound);
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
    }
}
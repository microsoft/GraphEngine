using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using Trinity.Diagnostics;
using Trinity.Network;
using Trinity.Storage;
using Trinity.Utilities;
using Trinity.DynamicCluster.Consensus;

using static Trinity.DynamicCluster.Utils;
using Trinity.DynamicCluster.Communication;

namespace Trinity.DynamicCluster.Storage
{
    public unsafe partial class DynamicMemoryCloud : MemoryCloud
    {
        private Random m_rng = new Random();

        internal ClusterConfig cluster_config;//TODO will be substituted
        internal IPartitioner m_partitioner;
        internal ILeaderElectionService m_leaderElectionService;
        internal INameService m_nameservice;
        internal NameDescriptor m_namedescriptor = new NameDescriptor();
        internal Partition ChunkedStorageTable(int id) => StorageTable[id] as Partition;
        private ConcurrentDictionary<int, DynamicRemoteStorage> temporaryRemoteStorageRepo = new ConcurrentDictionary<int, DynamicRemoteStorage>();

        internal static DynamicMemoryCloud Instance => Global.CloudStorage as DynamicMemoryCloud;

        private void _DoWithTempStorage(DynamicRemoteStorage remoteStorage, Action<int> action)
        {
            int temp_id = Infinity(() => m_rng.Next(-10000000, -1))
                         .SkipWhile(_ => !temporaryRemoteStorageRepo.TryAdd(_, remoteStorage))
                         .First();
            action(temp_id);
            temporaryRemoteStorageRepo.TryRemove(temp_id, out var _);
        }

        internal TrinityErrorCode OnStorageJoin(DynamicRemoteStorage remoteStorage)
        {
            _DoWithTempStorage(remoteStorage, id =>
            {
                var module = GetCommunicationModule<DynamicClusterCommModule>();
                using (var rsp = module.QueryChunkedRemoteStorageInformation(id))
                {
                    ChunkedStorageTable(rsp.info.partition).Mount(remoteStorage, rsp);
                }
            });
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
            var module = GetCommunicationModule<DynamicClusterCommModule>();
            var remote_storage = ChunkedStorageTable(partition)
                                .OfType<DynamicRemoteStorage>()
                                .FirstOrDefault(_ => _.Iscalled(name));
            if (remote_storage == null) return TrinityErrorCode.E_FAILURE;
            return ChunkedStorageTable(partition).Unmount(remote_storage);
        }

        public NameDescriptor MyName => m_namedescriptor;

        public override IEnumerable<Chunk> MyChunks => m_partitioner.MyChunkList;
        public override int MyPartitionId => m_partitioner.PartitionId;
        public override int PartitionCount => m_partitioner.PartitionCount;
        public int ChunkCount => m_partitioner.ChunkCount;
        public override bool IsLocalCell(long cellId)
        {
            return (GetStorageByCellId(cellId) as Partition).IsLocal(cellId);
        }
        public override int MyProxyId => -1;
        public override int ProxyCount => 0;
        public override bool Open(ClusterConfig config, bool nonblocking)
        {
            InitializeServices();

            Log.WriteLine($"DynamicMemoryCloud: server {m_namedescriptor.Nickname} starting.");
            TrinityErrorCode errno = TrinityErrorCode.E_SUCCESS;

            this.cluster_config = config;
            StorageTable = Infinity(() => new Partition())
                          .Take(PartitionCount)
                          .ToArray();

            if (PartitionCount == 0)
                goto server_not_found;

            if (TrinityErrorCode.E_SUCCESS != (errno = ChunkedStorageTable(MyPartitionId).Mount(Global.LocalStorage, MyChunks)))
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

            if (!nonblocking) { CheckServerProtocolSignatures(); } // TODO should also check in nonblocking setup when any remote storage is connected
            // else this.ServerConnected += (_, __) => {};

            return true;

            server_not_found:
            Log.WriteLine(LogLevel.Error, "Incorrect server configuration. Message passing via CloudStorage not possible.");
            //TODO should wait for partitioner events
            throw new Exception();

        }

        internal void InitializeServices()
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
            ServerInfo my_si = new ServerInfo(Global.MyIPAddress.ToString(), Global.MyIPEndPoint.Port, Global.MyAssemblyPath, TrinityConfig.LoggingLevel);
            m_nameservice.PublishServerInfo(m_namedescriptor, my_si);
        }

        public TrinityErrorCode Shutdown()
        {
            var module = GetCommunicationModule<DynamicClusterCommModule>();
            Enumerable
           .Range(0, PartitionCount)
           .SelectMany(ChunkedStorageTable)
           .OfType<DynamicRemoteStorage>()
           .ForEach(s => _DoWithTempStorage(s, id =>
           {
               using (var request = new StorageInformationWriter(MyPartitionId, MyName.ServerId, MyName.Nickname))
               { module.NotifyRemoteStorageOnLeaving(id, request); }
           }));
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

        internal Chunk GetChunkByCellId(long cellId)
        {
            return MyChunks.FirstOrDefault(c => c.LowKey <= cellId && cellId <= c.HighKey);
        }

        private void CheckServerProtocolSignatures()
        {
            Log.WriteLine("Checking {0}-Server protocol signatures...", cluster_config.RunningMode);
            int my_server_id = (cluster_config.RunningMode == RunningMode.Server) ? MyPartitionId : -1;
            var storage = StorageTable.Where((_, idx) => idx != my_server_id).FirstOrDefault() as RemoteStorage;

            CheckProtocolSignatures_impl(storage, cluster_config.RunningMode, RunningMode.Server);
        }
    }
}
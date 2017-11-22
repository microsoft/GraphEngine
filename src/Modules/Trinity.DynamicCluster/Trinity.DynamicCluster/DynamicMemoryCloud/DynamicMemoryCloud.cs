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
        internal TrinityErrorCode OnStorageLeave(int partition, Guid Id)
        {
            var module = GetCommunicationModule<DynamicClusterCommModule>();
            var remote_storage = ChunkedStorageTable(partition)
                                .OfType<DynamicRemoteStorage>()
                                .FirstOrDefault(_ => _.Id == Id);
            return ChunkedStorageTable(partition).Unmount(remote_storage);
        }

        public string NickName { get; private set; }

        public override IEnumerable<Chunk> MyChunks => m_partitioner.MyChunkList;
        public override int MyPartitionId => m_partitioner.PartitionId;
        public override int PartitionCount => m_partitioner.PartitionCount;
        public int ChunkCount => m_partitioner.ChunkCount;
        public Guid InstanceId => m_nameservice.InstanceId;
        public override bool IsLocalCell(long cellId)
        {
            return (GetStorageByCellId(cellId) as Partition).IsLocal(cellId);
        }
        public override int MyProxyId => -1;
        public override int ProxyCount => 0;
        public override bool Open(ClusterConfig config, bool nonblocking)
        {
            Initialize();

            Log.WriteLine($"DynamicMemoryCloud: server {NickName} starting.");
            TrinityErrorCode errno = TrinityErrorCode.E_SUCCESS;

            this.cluster_config = config;
            StorageTable = Infinity(() => new Partition())
                          .Take(PartitionCount)
                          .ToArray();

            if (TrinityErrorCode.E_SUCCESS != (errno = ChunkedStorageTable(MyPartitionId).Mount(Global.LocalStorage, MyChunks)))
            {
                string errmsg = $"DynamicMemoryCloud: server {NickName} failed to mount itself.";
                Log.WriteLine(LogLevel.Error, errmsg);
                throw new Exception(errmsg);
            }

            // TODO check protocol signatures for every new connection.

            return true;
        }

        internal void Initialize()
        {
            m_nameservice = AssemblyUtility.GetAllClassInstances<INameService>().First();
            m_nameservice.NewServerInfoPublished += (o, e) =>
            {
                var id = e.Item1;
                var si = e.Item2;
                Log.WriteLine($"DynamicCluster: New server info published: {GenerateNickName(id)}, {si.ToString()}");

                DynamicRemoteStorage rs = new DynamicRemoteStorage(si, id, TrinityConfig.ClientMaxConn, this, 0, nonblocking: true);
                OnStorageJoin(rs);
            };
            m_nameservice.Start();

            m_leaderElectionService = AssemblyUtility.GetAllClassInstances<ILeaderElectionService>().First();
            m_partitioner = AssemblyUtility.GetAllClassInstances<IPartitioner>().First();
            m_partitioner.Start();
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
               using (var request = new StorageInformationWriter(MyPartitionId, m_nameservice.InstanceId))
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
            OnStorageLeave(rs.PartitionId, rs.Id);
        }

        internal Chunk GetChunkByCellId(long cellId)
        {
            return MyChunks.FirstOrDefault(c => c.Covers(cellId));
        }

        private void CheckServerProtocolSignatures()
        {
            // TODO
            Log.WriteLine("Checking {0}-Server protocol signatures...", cluster_config.RunningMode);
            int my_server_id = (cluster_config.RunningMode == RunningMode.Server) ? MyPartitionId : -1;
            var storage = StorageTable.Where((_, idx) => idx != my_server_id).FirstOrDefault() as RemoteStorage;

            CheckProtocolSignatures_impl(storage, cluster_config.RunningMode, RunningMode.Server);
        }
    }
}
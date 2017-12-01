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
using System.Threading;
using Trinity.DynamicCluster.Tasks;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Storage
{
    public partial class DynamicMemoryCloud : MemoryCloud
    {
        private Random m_rng = new Random();

        internal ClusterConfig cluster_config;//TODO will be substituted
        internal IPartitioner m_partitioner;
        internal INameService m_nameservice;
        internal ITaskQueue m_taskqueue;
        internal Executor m_taskexec;
        private DynamicClusterCommModule m_module;
        internal CancellationTokenSource m_cancelSrc;
        internal Partition PartitionTable(int id) => StorageTable[id] as Partition;
        private ConcurrentDictionary<int, DynamicRemoteStorage> temporaryRemoteStorageRepo = new ConcurrentDictionary<int, DynamicRemoteStorage>();
        private volatile bool disposed = false;


        internal static DynamicMemoryCloud Instance => Global.CloudStorage as DynamicMemoryCloud;

        private void _DoWithTempStorage(DynamicRemoteStorage remoteStorage, Action<int> action)
        {
            int temp_id = Infinity(() => m_rng.Next(-10000000, -1))
                         .SkipWhile(_ => !temporaryRemoteStorageRepo.TryAdd(_, remoteStorage))
                         .First();
            action(temp_id);
            temporaryRemoteStorageRepo.TryRemove(temp_id, out var _);
        }

        private void CheckServerProtocolSignatures(DynamicRemoteStorage rs)
        {
            Log.WriteLine(LogLevel.Debug, $"Checking protocol signatures with '{rs.NickName}' ({rs.Id})...");

            CheckProtocolSignatures_impl(rs, cluster_config.RunningMode, RunningMode.Server);
        }

        internal TrinityErrorCode OnStorageJoin(DynamicRemoteStorage remoteStorage)
        {
            _DoWithTempStorage(remoteStorage, id =>
            {
                using (var rsp = m_module.QueryChunkedRemoteStorageInformation(id))
                {
                    PartitionTable(rsp.info.partition).Mount(remoteStorage, rsp);
                }
                CheckServerProtocolSignatures(remoteStorage);
                Log.WriteLine($"DynamicCluster: Connected to '{remoteStorage.NickName}' ({remoteStorage.Id})");
            });
            return TrinityErrorCode.E_SUCCESS;
        }

        /// <summary>
        /// It is guaranteed that OnStorageLeave will only be called
        /// on those storages that has been previously sent to
        /// <see cref="OnStorageJoin"/>. However, OnStorageLeave may
        /// be called multiple times (disconnected, then reconnected,
        /// or remote <see cref="Close"/> is called).
        /// </summary>
        /// <param name="remoteStorage"></param>
        internal TrinityErrorCode OnStorageLeave(int partition, Guid Id)
        {
            var remote_storage = PartitionTable(partition)
                                .OfType<DynamicRemoteStorage>()
                                .FirstOrDefault(_ => _.Id == Id);
            return PartitionTable(partition).Unmount(remote_storage);
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
            this.cluster_config = config;
            InitializeComponents();

            m_partitioner.Start(m_cancelSrc.Token);
            StorageTable = Infinity<Partition>()
                          .Take(PartitionCount)
                          .ToArray();

            m_nameservice.NewReplicaInformationPublished += (o, e) =>
            {
                Log.WriteLine($"DynamicCluster: New server info published: {e.Address}:{e.Port}");
                DynamicRemoteStorage rs = new DynamicRemoteStorage(e, TrinityConfig.ClientMaxConn, this);
            };
            m_nameservice.Start(m_cancelSrc.Token);
            NickName = GenerateNickName(InstanceId);

            m_taskqueue.Start(m_cancelSrc.Token);
            m_taskexec = new Executor(m_taskqueue, m_cancelSrc.Token);

            Log.WriteLine($"DynamicMemoryCloud: Partition {MyPartitionId}: Instance '{NickName}' {InstanceId} opened.");

            return true;
        }

        internal void InitializeComponents()
        {
            m_module = GetCommunicationModule<DynamicClusterCommModule>();
            m_cancelSrc = new CancellationTokenSource();
            m_nameservice = AssemblyUtility.GetAllClassInstances<INameService>().First();
            m_partitioner = AssemblyUtility.GetAllClassInstances<IPartitioner>().First();
            m_taskqueue = AssemblyUtility.GetAllClassInstances<ITaskQueue>().First();
            PartitionTable(MyPartitionId).Mount(Global.LocalStorage, MyChunks);
            var partition_proc = m_partitioner.PartitionerProc;
            if (partition_proc != null)
            {
                Log.WriteLine($"Partitioner [{m_partitioner.GetType().Name}] governs partitioning scheme.");
                SetPartitionMethod(partition_proc);
            }
            else
            {
                Log.WriteLine($"Partitioner [{m_partitioner.GetType().Name}] does not implement partitioning scheme, falling back to default.");
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (this.disposed) return;
            Enumerable
           .Range(0, PartitionCount)
           .SelectMany(PartitionTable)
           .OfType<DynamicRemoteStorage>()
           .ForEach(s => _DoWithTempStorage(s, id =>
            {
                try
                {
                    using (var request = new StorageInformationWriter(MyPartitionId, m_nameservice.InstanceId))
                    { m_module.NotifyRemoteStorageOnLeaving(id, request); }
                }
                catch { }
            }));

            m_cancelSrc.Cancel();
            m_nameservice.Dispose();
            m_partitioner.Dispose();
            m_taskqueue.Dispose();
            m_taskexec.Dispose();

            base.Dispose(disposing);
            this.disposed = true;
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
    }
}
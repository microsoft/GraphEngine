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
        #region Fields
        internal ClusterConfig           m_cluster_config;
        internal IChunkTable             m_chunktable;
        internal INameService            m_nameservice;
        internal ITaskQueue              m_taskqueue;
        internal IHealthManager          m_healthmanager;
        internal Executor                m_taskexec;
        internal Partitioner             m_partitioner;
        internal CancellationTokenSource m_cancelSrc;

        private Random                   m_rng = new Random();
        private volatile bool            m_disposed = false;
        private DynamicClusterCommModule m_module;
        // !Note can also be achieve by extending Storage[] StorageTable beyond PartitionCount,
        // and use interlocked increment to obtain index
        private ConcurrentDictionary<int, DynamicRemoteStorage> m_tmp_rs_repo = new ConcurrentDictionary<int, DynamicRemoteStorage>();
        #endregion

        internal Partition PartitionTable(int id) => StorageTable[id] as Partition;
        internal static DynamicMemoryCloud Instance => Global.CloudStorage as DynamicMemoryCloud;

        private void _DoWithTempStorage(DynamicRemoteStorage remoteStorage, Action<int> action)
        {
            int temp_id = Infinity(() => m_rng.Next(-10000000, -1))
                         .SkipWhile(_ => !m_tmp_rs_repo.TryAdd(_, remoteStorage))
                         .First();
            action(temp_id);
            m_tmp_rs_repo.TryRemove(temp_id, out var _);
        }

        private void CheckServerProtocolSignatures(DynamicRemoteStorage rs)
        {
            Log.WriteLine(LogLevel.Debug, $"Checking protocol signatures with '{rs.NickName}' ({rs.ReplicaInformation})...");

            CheckProtocolSignatures_impl(rs, m_cluster_config.RunningMode, RunningMode.Server);
        }

        internal TrinityErrorCode OnStorageJoin(DynamicRemoteStorage remoteStorage)
        {
            _DoWithTempStorage(remoteStorage, id =>
            {
                CheckServerProtocolSignatures(remoteStorage);
                Log.WriteLine($"DynamicCluster: Connected to '{remoteStorage.NickName}' ({remoteStorage.ReplicaInformation.Id})");
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
                                .FirstOrDefault(_ => _.ReplicaInformation.Id == Id);
            return PartitionTable(partition).Unmount(remote_storage);
        }

        public string NickName { get; private set; }
        public override IEnumerable<Chunk> MyChunks => m_partitioner.MyChunks;
        public override int MyPartitionId => m_nameservice.PartitionId;
        public override int PartitionCount => m_nameservice.PartitionCount;
        public Guid InstanceId => m_nameservice.InstanceId;
        public override bool IsLocalCell(long cellId)
        {
            return (GetStorageByCellId(cellId) as Partition).IsLocal(cellId);
        }
        public override int MyProxyId => -1;
        public override int ProxyCount => 0;
        public override bool Open(ClusterConfig config, bool nonblocking)
        {
            this.m_cluster_config = config;
            InitializeComponents();

            StorageTable = Infinity<Partition>()
                          .Take(PartitionCount)
                          .ToArray();
            NickName = GenerateNickName(InstanceId);

            //TODO repmode is hard-coded Mirroring
            //TODO redundancy hard-coded 2
            m_partitioner = new Partitioner(m_cancelSrc.Token, m_chunktable, m_nameservice, m_taskqueue, m_healthmanager, ReplicationMode.Mirroring, minimalRedundancy: 2);
            m_taskexec = new Executor(m_taskqueue, m_cancelSrc.Token);

            Log.WriteLine($"DynamicMemoryCloud: Partition {MyPartitionId}: Instance '{NickName}' {InstanceId} opened.");
            Global.CommunicationInstance.Started += InitModule;

            return true;
        }

        private void InitModule()
        {
            m_module = GetCommunicationModule<DynamicClusterCommModule>();
        }

        internal void InitializeComponents()
        {
            m_cancelSrc = new CancellationTokenSource();
            m_nameservice = AssemblyUtility.GetAllClassInstances<INameService>().First();
            m_chunktable = AssemblyUtility.GetAllClassInstances<IChunkTable>().First();
            m_taskqueue = AssemblyUtility.GetAllClassInstances<ITaskQueue>().First();
            m_healthmanager = AssemblyUtility.GetAllClassInstances<IHealthManager>().First();

            m_nameservice.Start(m_cancelSrc.Token);
            m_taskqueue.Start(m_cancelSrc.Token);
            m_chunktable.Start(m_cancelSrc.Token);
            m_healthmanager.Start(m_cancelSrc.Token);
        }

        protected override void Dispose(bool disposing)
        {
            if (this.m_disposed) return;
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

            m_taskexec.Dispose();
            m_partitioner.Dispose();

            m_nameservice.Dispose();
            m_chunktable.Dispose();
            m_taskqueue.Dispose();
            m_healthmanager.Dispose();

            base.Dispose(disposing);
            this.m_disposed = true;
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
            OnStorageLeave(rs.PartitionId, rs.ReplicaInformation.Id);
        }

        internal Chunk GetChunkByCellId(long cellId)
        {
            return MyChunks.FirstOrDefault(c => c.Covers(cellId));
        }
    }
}
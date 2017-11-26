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
    public unsafe partial class DynamicMemoryCloud : MemoryCloud
    {
        private Random m_rng = new Random();

        internal ClusterConfig cluster_config;//TODO will be substituted
        internal IPartitioner m_partitioner;
        internal INameService m_nameservice;
        internal ITaskQueue m_taskqueue;
        internal Thread m_tasktrd;
        internal CancellationTokenSource m_cancelSrc;
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

        private void CheckServerProtocolSignatures(DynamicRemoteStorage rs)
        {
            Log.WriteLine(LogLevel.Debug, $"Incoming connection from {rs.Id}, checking protocol signatures...");

            CheckProtocolSignatures_impl(rs, cluster_config.RunningMode, RunningMode.Server);
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
        /// or remote <see cref="Close"/> is called).
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
            InitializeComponents();

            m_partitioner.Start(m_cancelSrc.Token);
            this.cluster_config = config;
            StorageTable = Infinity(() => new Partition())
                          .Take(PartitionCount)
                          .ToArray();

            m_nameservice.Start(m_cancelSrc.Token);
            m_taskqueue.Start(m_cancelSrc.Token);
            m_tasktrd = new Thread(TaskExecutionProc);
            m_tasktrd.Start();

            Log.WriteLine($"DynamicMemoryCloud: Partition {MyPartitionId}: Instance {InstanceId} opened.");
            // TODO check protocol signatures for every new connection.

            return true;
        }

        internal void InitializeComponents()
        {
            m_cancelSrc = new CancellationTokenSource();

            m_nameservice = AssemblyUtility.GetAllClassInstances<INameService>().First();
            m_nameservice.NewServerInfoPublished += (o, e) =>
            {
                var id = e.Item1;
                var si = e.Item2;
                Log.WriteLine($"DynamicCluster: New server info published: {GenerateNickName(id)}, {si.ToString()}");

                DynamicRemoteStorage rs = new DynamicRemoteStorage(si, id, TrinityConfig.ClientMaxConn, this, 0, nonblocking: true);
                OnStorageJoin(rs);
            };

            m_partitioner = AssemblyUtility.GetAllClassInstances<IPartitioner>().First();
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

            m_taskqueue = AssemblyUtility.GetAllClassInstances<ITaskQueue>().First();
        }

        public TrinityErrorCode Close()
        {
            var module = GetCommunicationModule<DynamicClusterCommModule>();
            Enumerable
           .Range(0, PartitionCount)
           .SelectMany(ChunkedStorageTable)
           .OfType<DynamicRemoteStorage>()
           .ForEach(s => _DoWithTempStorage(s, id =>
            {
                try
                {
                    using (var request = new StorageInformationWriter(MyPartitionId, m_nameservice.InstanceId))
                    { module.NotifyRemoteStorageOnLeaving(id, request); }
                }
                catch { }
            }));

            m_cancelSrc.Cancel();
            m_nameservice.Dispose();
            m_partitioner.Dispose();
            m_taskqueue.Dispose();
            m_tasktrd.Join();

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

        private void TaskExecutionProc()
        {
            while (true)
            {
                try
                {
                    if (!m_taskqueue.IsMaster)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    var task = m_taskqueue.GetTask(m_cancelSrc.Token);
                    if (task == null) continue;
                    var errno = task.Execute();
                    if (errno == TrinityErrorCode.E_SUCCESS)
                    {
                        m_taskqueue.TaskCompleted(task);
                    }
                    else
                    {
                        Log.WriteLine(LogLevel.Error, $"TaskExecutionProc: task {task.Id} failed with code {errno}");
                        m_taskqueue.TaskFailed(task);
                    }
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, $"TaskExecutionProc: {ex.ToString()}");
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
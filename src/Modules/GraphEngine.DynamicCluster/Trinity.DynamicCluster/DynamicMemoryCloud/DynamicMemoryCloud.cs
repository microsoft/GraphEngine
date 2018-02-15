// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Trinity.Diagnostics;
using Trinity.DynamicCluster.Communication;
using Trinity.DynamicCluster.Config;
using Trinity.DynamicCluster.Consensus;
using Trinity.DynamicCluster.Health;
using Trinity.DynamicCluster.Persistency;
using Trinity.DynamicCluster.Replication;
using Trinity.DynamicCluster.Tasks;
using Trinity.Storage;
using Trinity.Utilities;
using static Trinity.DynamicCluster.Utils;

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
        internal IPersistentStorage      m_persistent_storage;
        internal IBackupManager          m_backupmgr;
        internal BackupController        m_backupctl;
        internal Executor                m_taskexec;
        internal CloudIndex              m_cloudidx;
        internal HealthMonitor           m_healthmon;
        internal Partitioner             m_partitioner;
        internal CancellationTokenSource m_cancelSrc;
        internal DynamicStorageTable     m_storageTable;

        private Random                   m_rng = new Random();
        private DynamicClusterCommModule m_module;
        private int                      m_myid;
        #endregion

        internal Partition PartitionTable(int id) => StorageTable[id] as Partition;
        internal Partition MyPartition => PartitionTable(MyPartitionId);
        internal static DynamicMemoryCloud Instance => Global.CloudStorage as DynamicMemoryCloud;

        private void CheckServerProtocolSignatures(DynamicRemoteStorage rs)
        {
            Log.WriteLine(LogLevel.Debug, $"Checking protocol signatures with '{rs.NickName}' ({rs.ReplicaInformation})...");

            CheckProtocolSignatures_impl(rs, m_cluster_config.RunningMode, RunningMode.Server);
        }

        internal void OnStorageJoin(DynamicRemoteStorage remoteStorage)
        {
            CheckServerProtocolSignatures(remoteStorage);
            m_storageTable.AddInstance(GetInstanceId(remoteStorage.ReplicaInformation.Id), remoteStorage);
            Log.WriteLine($"{nameof(DynamicCluster)}: Connected to '{remoteStorage.NickName}' ({remoteStorage.ReplicaInformation.Id})");
        }

        /// <summary>
        /// It is guaranteed that OnStorageLeave will only be called
        /// on those storages that has been previously sent to
        /// <see cref="OnStorageJoin"/>. However, OnStorageLeave may
        /// be called multiple times (disconnected, then reconnected,
        /// or remote <see cref="Close"/> is called).
        /// </summary>
        /// <param name="remoteStorage"></param>
        internal void OnStorageLeave(int partition, Guid Id)
        {
            var remote_storage = PartitionTable(partition)
                                .OfType<DynamicRemoteStorage>()
                                .FirstOrDefault(_ => _.ReplicaInformation.Id == Id);
            m_storageTable.RemoveInstance(GetInstanceId(Id));
            PartitionTable(partition).Unmount(remote_storage);
        }

        public string NickName { get; private set; }

        public override int MyInstanceId => m_myid;

        internal int GetInstanceId(Guid instanceId)
        {
            int id = instanceId.GetHashCode();
            if (id >= 0 && id < PartitionCount) id += PartitionCount;
            return id;
        }

        protected override IList<IStorage> StorageTable => m_storageTable;
        public override IEnumerable<Chunk> MyChunks => m_cloudidx.MyChunks;
        public override int MyPartitionId => m_nameservice.PartitionId;
        public override int PartitionCount => m_nameservice.PartitionCount;
        public Guid InstanceGuid => m_nameservice.InstanceId;
        public override bool IsLocalCell(long cellId)
        {
            return (GetStorageByCellId(cellId) as Partition).IsLocal(cellId);
        }
        public override int MyProxyId => -1;
        public override int ProxyCount => 0;
        public override bool Open(ClusterConfig config, bool nonblocking)
        {
            this.m_cluster_config = config;
            m_cancelSrc = new CancellationTokenSource();
            m_nameservice = AssemblyUtility.GetAllClassInstances<INameService>().First();
            Log.WriteLine(LogLevel.Info, $"{nameof(DynamicMemoryCloud)}: using name service provided by '{m_nameservice.GetType().FullName}'");
            m_chunktable = AssemblyUtility.GetAllClassInstances<IChunkTable>().First();
            Log.WriteLine(LogLevel.Info, $"{nameof(DynamicMemoryCloud)}: using chunk table provided by '{m_chunktable.GetType().FullName}'");
            m_taskqueue = AssemblyUtility.GetAllClassInstances<ITaskQueue>().First();
            Log.WriteLine(LogLevel.Info, $"{nameof(DynamicMemoryCloud)}: using task queue provided by '{m_taskqueue.GetType().FullName}'");
            m_healthmanager = AssemblyUtility.GetAllClassInstances<IHealthManager>().First();
            Log.WriteLine(LogLevel.Info, $"{nameof(DynamicMemoryCloud)}: using health manager provided by '{m_healthmanager.GetType().FullName}'");
            m_backupmgr = AssemblyUtility.GetAllClassInstances<IBackupManager>().First();
            Log.WriteLine(LogLevel.Info, $"{nameof(DynamicMemoryCloud)}: using backup manager provided by '{m_backupmgr.GetType().FullName}'");
            m_persistent_storage = AssemblyUtility.GetAllClassInstances<IPersistentStorage>().First();
            Log.WriteLine(LogLevel.Info, $"{nameof(DynamicMemoryCloud)}: using persistent storage provided by '{m_persistent_storage.GetType().FullName}'");

            m_nameservice.Start(m_cancelSrc.Token);
            m_taskqueue.Start(m_cancelSrc.Token);
            m_chunktable.Start(m_cancelSrc.Token);
            m_healthmanager.Start(m_cancelSrc.Token);
            m_backupmgr.Start(m_cancelSrc.Token);

            m_myid = GetInstanceId(InstanceGuid);
            m_storageTable = new DynamicStorageTable(PartitionCount);
            m_storageTable[m_myid] = Global.LocalStorage;
            NickName = GenerateNickName(InstanceGuid);

            int redundancy = DynamicClusterConfig.Instance.MinimumReplica;
            m_cloudidx = new CloudIndex(m_cancelSrc.Token, m_nameservice, m_chunktable, this, NickName, PartitionTable);
            m_healthmon= new HealthMonitor(m_cancelSrc.Token, m_nameservice, m_cloudidx, m_healthmanager, redundancy);
            m_partitioner = new Partitioner(m_cancelSrc.Token, m_cloudidx, m_nameservice, m_taskqueue, DynamicClusterConfig.Instance.ReplicationMode, redundancy);
            m_taskexec = new Executor(m_cancelSrc.Token, m_nameservice, m_taskqueue);
            m_backupctl = new BackupController(m_cancelSrc.Token, m_backupmgr, m_nameservice, m_persistent_storage, m_taskqueue);

            Log.WriteLine($"{nameof(DynamicMemoryCloud)}: Partition {MyPartitionId}: Instance '{NickName}' {InstanceGuid} opened.");
            Global.CommunicationInstance.Started += InitModule;

            return true;
        }

        private void InitModule()
        {
            m_module = GetCommunicationModule<DynamicClusterCommModule>();
        }

        public void Close()
        {
            Enumerable
           .Range(0, PartitionCount)
           .SelectMany(PartitionTable)
           .OfType<DynamicRemoteStorage>()
           .ForEach(s =>
           {
               try
               {
                   int id = GetInstanceId(s.ReplicaInformation.Id);
                   using (var request = new StorageInformationWriter(MyPartitionId, m_nameservice.InstanceId))
                   { m_module.NotifyRemoteStorageOnLeaving(id, request); }
               }
               catch { }
           });

            m_cancelSrc.Cancel();

            m_backupctl.Dispose();
            m_taskexec.Dispose();
            m_partitioner.Dispose();
            m_healthmon.Dispose();
            m_cloudidx.Dispose();
            m_persistent_storage.Dispose();

            m_nameservice.Dispose();
            m_chunktable.Dispose();
            m_taskqueue.Dispose();
            m_healthmanager.Dispose();
            m_backupmgr.Dispose();
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
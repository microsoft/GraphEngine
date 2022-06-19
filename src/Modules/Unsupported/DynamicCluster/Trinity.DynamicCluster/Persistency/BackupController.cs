using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Consensus;
using Trinity.DynamicCluster.DynamicCluster;
using Trinity.DynamicCluster.Persistency;
using Trinity.DynamicCluster.Tasks;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Storage
{
    class BackupController : IDisposable
    {
        private IBackupManager m_backupmgr;
        private CancellationToken m_cancel;
        private DynamicMemoryCloud m_dmc;
        private INameService m_namesvc;
        private IPersistentStorage m_pstore;
        private ITaskQueue m_taskqueue;

        public BackupController(CancellationToken cancel, IBackupManager backupmgr, INameService namesvc, IPersistentStorage pstore, ITaskQueue taskqueue)
        {
            this.m_backupmgr = backupmgr;
            this.m_cancel = cancel;
            this.m_dmc = Global.CloudStorage as DynamicMemoryCloud;
            this.m_namesvc = namesvc;
            this.m_pstore = pstore;
            this.m_taskqueue = taskqueue;

            m_backupmgr.RequestPartitionBackup  += OnBackupManagerRequestBackup;
            m_backupmgr.RequestPartitionRestore += OnBackupManagerRequestRestore;
        }

        internal async Task RestoreCurrentPartition(Guid task_id, Guid version, EventArgs e)
        {
            var downloader = await m_pstore.Download(version, m_namesvc.PartitionId, 0, 0);
            await m_backupmgr.Restore(downloader, e);

            // After the backup manager finishes state restoration, chunk tables,
            // name service, pending tasks should be all restored. Thus we load
            // chunks into replicas as per described by the chunk table.

            List<PersistedLoadTask> passive_tasks = new List<PersistedLoadTask>();
            var cts = m_dmc.m_cloudidx.GetMyPartitionReplicaChunks();
            foreach (var (rep, cks) in cts)
            {
                passive_tasks.Add(new PersistedLoadTask(cks
                    .Select(ck => (rep, new PersistedSlice(version, ck.LowKey, ck.HighKey)))));
            }
            GroupedTask gt = new GroupedTask(passive_tasks, task_id);
            await m_taskqueue.PostTask(gt);
            await m_taskqueue.Wait(task_id);
        }

        internal async Task BackupCurrentPartition(Guid task_id, Guid version, EventArgs e)
        {
            List<PersistedSaveTask> passive_tasks = new List<PersistedSaveTask>();
            var cts = m_dmc.m_cloudidx.GetMyPartitionReplicaChunks();
            // Backup strategy: back up each chunk only ONCE, by the first replica that holds it.
            // XXX this would burden the first replica if it has the most chunks.
            // TODO load balance
            HashSet<Guid> planned = new HashSet<Guid>();
            foreach (var (rep, cks) in cts)
            {
                passive_tasks.Add(new PersistedSaveTask(cks
                    .Where(_ => !planned.Contains(_.Id))
                    .Select(ck => (rep, new PersistedSlice(version, ck.LowKey, ck.HighKey)))));
                cks.ForEach(ck => planned.Add(ck.Id));
            }
            GroupedTask gt = new GroupedTask(passive_tasks, task_id);
            await m_taskqueue.PostTask(gt);
            await m_taskqueue.Wait(task_id);

            // After we successfully dump all partials of the partition to the persisted storage,
            // we then proceed to backup the cluster state, including chunk table, name service,
            // pending tasks, etc.

            var uploader = await m_pstore.Upload(version, m_namesvc.PartitionId, 0, 0);
            await m_backupmgr.Backup(uploader, e);
            await uploader.FinishUploading();
        }


        private async Task RestoreAllPartitions()
        {
            var version = await m_pstore.GetLatestVersion();
            await RestoreAllPartitions(version);
        }

        private async Task RestoreAllPartitions(Guid version)
        {
            // Leader, distribute restore tasks to masters.
            var task_id = Guid.NewGuid();
            var masters = m_dmc.m_cloudidx.GetMasters();

            using (var req = new BackupTaskInformationWriter(task_id, version))
            {
                var rsps = await masters.Select(m => m.PersistedLoadPartition(req)).Unwrap();
                bool fail = false;
                foreach (var rsp in rsps)
                {
                    if (rsp.errno != Errno.E_OK) { fail = true; }
                    rsp.Dispose();
                }
                if (fail) throw new RestoreException();
            }
        }

        private async Task BackupAllPartitions()
        {
            var version = await m_pstore.CreateNewVersion();
            // Leader, distribute backup tasks to masters.
            var task_id = Guid.NewGuid();
            var masters = m_dmc.m_cloudidx.GetMasters();
            try
            {
                using (var req = new BackupTaskInformationWriter(task_id, version))
                {
                    var rsps = await masters.Select(m => m.PersistedSavePartition(req)).Unwrap();
                    bool fail = false;
                    foreach (var rsp in rsps)
                    {
                        if (rsp.errno != Errno.E_OK) { fail = true; }
                        rsp.Dispose();
                    }
                    if (fail) throw new Exception();
                }
                await m_pstore.CommitVersion(version);
            }
            catch
            {
                await m_pstore.DeleteVersion(version);
                throw new BackupException();
            }
        }

        public Task Backup() => BackupAllPartitions();

        public Task Restore() => RestoreAllPartitions();

        public Task Restore(Guid version) => RestoreAllPartitions(version);

        private void OnBackupManagerRequestBackup(object sender, EventArgs e)
            => BackupCurrentPartition(Guid.NewGuid(), m_pstore.CreateNewVersion().Result, e).Wait();

        private void OnBackupManagerRequestRestore(object sender, EventArgs e)
            => RestoreCurrentPartition(Guid.NewGuid(), m_pstore.GetLatestVersion().Result, e).Wait();

        public void Dispose() { }
    }
}

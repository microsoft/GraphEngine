using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Consensus;
using Trinity.DynamicCluster.Persistency;
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

        public BackupController(CancellationToken cancel, IBackupManager backupmgr, INameService namesvc, IPersistentStorage pstore)
        {
            this.m_backupmgr = backupmgr;
            this.m_cancel = cancel;
            this.m_dmc = Global.CloudStorage as DynamicMemoryCloud;
            this.m_namesvc = namesvc;
            this.m_pstore = pstore;

            m_backupmgr.RequestPartitionBackup  += OnBackupManagerRequestBackup;
            m_backupmgr.RequestPartitionRestore += OnBackupManagerRequestRestore;
        }

        private async Task RestoreAllPartitions()
        {
            var version = await m_pstore.GetLatestVersion();
            await RestoreAllPartitions(version);
        }

        private async Task RestoreAllPartitions(Guid version)
        {
            var masters = _GetMasters();

            // Leader, distribute restore tasks to masters.
            var task_id = Guid.NewGuid();
        }

        private IEnumerable<IStorage> _GetMasters()
        {
            if (!m_namesvc.IsMaster) throw new InvalidOperationException();
            var masters = Utils.Integers(m_namesvc.PartitionCount).Select(m_dmc.m_cloudidx.GetMaster);
            if (masters.Any(_ => _ == null)) throw new NoSuitableReplicaException("One or more partition masters not found.");
            return masters;
        }

        private async Task BackupAllPartitions()
        {
            var version = await m_pstore.CreateNewVersion();
            // Leader, distribute backup tasks to masters.
            var task_id = Guid.NewGuid();
            var masters = _GetMasters();
            try
            {
                await m_pstore.CommitVersion(version);
            }
            catch
            {
                await m_pstore.DeleteVersion(version);
            }
        }

        public Task Backup() => BackupAllPartitions();

        public Task Restore() => RestoreAllPartitions();

        public Task Restore(Guid version) => RestoreAllPartitions(version);

        private void OnBackupManagerRequestRestore(object sender, EventArgs e)
            => RestoreCurrentPartition(e);

        private void RestoreCurrentPartition(EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BackupCurrentPartition(EventArgs e)
        {
            // generate tasks, to slaves...
        }


        private void OnBackupManagerRequestBackup(object sender, EventArgs e)
            => BackupCurrentPartition(e);

        public void Dispose() { }
    }
}

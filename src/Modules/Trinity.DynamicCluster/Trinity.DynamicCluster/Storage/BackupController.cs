using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Consensus;
using Trinity.DynamicCluster.Persistency;

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

            m_backupmgr.RequestBackup  += OnBackupManagerRequestBackup;
            m_backupmgr.RequestRestore += OnBackupManagerRequestRestore;
        }

        private async Task _RestoreImpl(Guid version, EventArgs e)
        {
            if (!m_namesvc.IsMaster) throw new InvalidOperationException();
        }

        private async Task _RestoreImpl(EventArgs e)
        {
            var version = await m_pstore.GetLatestVersion();
            await _RestoreImpl(version, e);
        }

        private async Task _BackupImpl(EventArgs e)
        {
            var version = await m_pstore.CreateNewVersion();
        }

        public Task Backup() => _BackupImpl(EventArgs.Empty);

        public Task Restore() => _RestoreImpl(EventArgs.Empty);

        private void OnBackupManagerRequestRestore(object sender, EventArgs e)
            => _RestoreImpl(e);

        private void OnBackupManagerRequestBackup(object sender, EventArgs e)
            => _BackupImpl(e);

        public void Dispose()
        {
        }
    }
}

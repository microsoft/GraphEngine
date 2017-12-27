using System;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Persistency;

namespace Trinity.DynamicCluster.Consensus
{
    /// <summary>
    /// Represents a set of custom actions to perform during backup/restoration on persistent
    /// storage, and also, provides backup and restore events, if the infrastructure is able to initiate
    /// such actions.
    /// </summary>
    public interface IBackupManager : IService
    {
        /// <summary>
        /// Performs additional backup operations, apart from saving the in-memory storage.
        /// </summary>
        /// <param name="uploader">
        /// The persistent storage uploader, which can be used to upload additional metadata in a snapshot.
        /// </param>
        /// <param name="eventArgs">
        /// If the backup is triggered from within the backup manager with <see cref="RequestBackup"/>, the event arguments will be forwarded.
        /// Otherwise, the backup is triggered via GraphEngine management interface, and eventArgs is set to <see cref="EventArgs.Empty"/>.
        /// </param>
        /// <returns></returns>
        Task Backup(IPersistentUploader uploader, EventArgs eventArgs);
        /// <summary>
        /// Performs additional restore operations, apart from loading the in-memory storage.
        /// </summary>
        /// <param name="downloader">
        /// The persistent storage downloader, which can be used to download additional metadata in a snapshot.
        /// </param>
        /// <param name="eventArgs">
        /// If the restore is triggered from within the backup manager with <see cref="RequestRestore"/>, the event arguments will be forwarded.
        /// Otherwise, the restore is triggered via GraphEngine management interface, and eventArgs is set to <see cref="EventArgs.Empty"/>.
        /// </param>
        /// <returns></returns>
        Task Restore(IPersistentDownloader downloader, EventArgs eventArgs);
        /// <summary>
        /// Requests Graph Engine to conduct a backup. Note,
        /// when this event is raised, the event arguments will be loopbacked to <see cref="Backup"/> .
        /// </summary>
        event EventHandler RequestBackup;
        /// <summary>
        /// Requests Graph Engine to conduct a restoration. Note,
        /// when this event is raised, the event arguments will be loopbacked to <see cref="Restore"/> .
        /// </summary>
        event EventHandler RequestRestore;
    }
}

using System;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Persistency
{
    public interface IPersistentStorage : IDisposable
    {
        /// <summary>
        /// Gets information about the capabilities of the implementation.
        /// The mode description consists of flags covering Locality(LO), 
        /// Storage Medium(ST) and Accessibility(AC), and can be 
        /// bitwise-or'ed together.
        /// </summary>
        Task<PersistentStorageMode> QueryPersistentStorageMode();
        /// <summary>
        /// Gets the version Id of the latest snapshot.
        /// </summary>
        /// <exception cref="NoDataException">Nothing in the persistent storage yet.</exception>
        Task<Guid> GetLatestVersion();
        /// <summary>
        /// Deletes a snapshot from the persistent storage.
        /// </summary>
        /// <param name="version">The version Id of the snapshot.</param>
        /// <exception cref="SnapshotNotFoundException">The specified snapshot is not found.</exception>
        Task DeleteVersion(Guid version);
        /// <summary>
        /// Creates a new snapshot, and prepare for data upload.
        /// </summary>
        Task<Guid> CreateNewVersion();
        /// <summary>
        /// Called only once when all the data related to a snapshot, are uploaded,
        /// and the version is visible to <![CDATA[GetLatestVersion]]>, and ready to be downloaded and restored.
        /// </summary>
        /// <param name="version">The version Id of the snapshot.</param>
        /// <exception cref="SnapshotNotFoundException">The specified snapshot is not found.</exception>
        /// <exception cref="SnapshotUploadUnfinishedException">
        /// The implementation has detected a premature call to CommitVersion, before some 
        /// of the uploaders could finish the uploading tasks.
        /// </exception>
        Task CommitVersion(Guid version);
        /// <summary>
        /// Downloads a range of a partition from a snapshot.
        /// </summary>
        /// <param name="version">The version Id of the snapshot.</param>
        /// <param name="partitionId">The id of the partition in this snapshot.</param>
        /// <param name="lowKey">The inclusive lower bound of the range.</param>
        /// <param name="highKey">The inclusive upper bound of the range.</param>
        /// <returns>An instance of <see cref="IPersistentDownloader"/> to pull data from.</returns>
        /// <exception cref="SnapshotNotFoundException">The specified snapshot is not found.</exception>
        /// <exception cref="SnapshotUploadUnfinishedException">The specified snapshot is not marked as finished.</exception>
        Task<IPersistentDownloader> Download(Guid version, int partitionId, long lowKey, long highKey);
        /// <summary>
        /// Uploads a range of a partition to a snapshot.
        /// </summary>
        /// <param name="version">The version Id of the snapshot.</param>
        /// <param name="partitionId">The id of the partition in this snapshot.</param>
        /// <param name="lowKey">The inclusive lower bound of the range.</param>
        /// <param name="highKey">The inclusive upper bound of the range.</param>
        /// <returns>An instance of <see cref="IPersistentUploader"/> to push data to.</returns>
        /// <exception cref="SnapshotNotFoundException">The specified snapshot is not found.</exception>
        Task<IPersistentUploader> Upload(Guid version, int partitionId, long lowKey, long highKey);
    }
}

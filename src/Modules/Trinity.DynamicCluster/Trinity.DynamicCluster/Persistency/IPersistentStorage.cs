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
        /// Downloads a range from a snapshot.
        /// </summary>
        /// <param name="version">The version Id of the snapshot.</param>
        /// <param name="lowKey">The inclusive lower bound of the range.</param>
        /// <param name="highKey">The inclusive upper bound of the range.</param>
        /// <returns>An instance of <see cref="IPersistentDownloader"/> to pull data from.</returns>
        /// <exception cref="SnapshotNotFoundException">The specified snapshot is not found.</exception>
        /// <exception cref="SnapshotUploadUnfinishedException">The specified snapshot is not marked as finished.</exception>
        Task<IPersistentDownloader> Download(Guid version, long lowKey, long highKey);
        /// <summary>
        /// Uploads a range to a snapshot.
        /// </summary>
        /// <param name="version">The version Id of the snapshot.</param>
        /// <param name="lowKey">The inclusive lower bound of the range.</param>
        /// <param name="highKey">The inclusive upper bound of the range.</param>
        /// <returns>An instance of <see cref="IPersistentUploader"/> to push data to.</returns>
        /// <exception cref="SnapshotNotFoundException">The specified snapshot is not found.</exception>
        Task<IPersistentUploader> Upload(Guid version, long lowKey, long highKey);
    }
}

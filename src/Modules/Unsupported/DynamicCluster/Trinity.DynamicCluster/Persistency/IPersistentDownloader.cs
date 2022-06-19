using System;
using System.IO;
using System.Threading.Tasks;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Persistency
{
    public interface IPersistentDownloader : IDisposable
    {
        /// <summary>
        /// Pulls down a chunk asynchronously. Returns null when the downloader
        /// has reached EOF.
        /// </summary>
        Task<IPersistentDataChunk> DownloadAsync();
        /// <summary>
        /// Asynchronously download metadata identified by a key. The key is unique
        /// within the snapshot. Usually the key will be of the format PARTITIO_ID-key.
        /// When the data is not found in the snapshot, throws <see cref="NoDataException"/>.
        /// </summary>
        /// <param name="key">The identifier of the metadata.</param>
        /// <param name="output">The stream where the metadata is to be downloaded.</param>
        Task DownloadMetadataAsync(string key, Stream output);
        /// <summary>
        /// Seeks to the <see cref="IPersistentDataChunk"/> after the given range.
        /// </summary>
        /// <param name="progress">
        /// Indicates the range that should be skipped. A typical usage is to resume
        /// downloading after interruption caused by network errors etc.
        /// </param>
        Task SeekAsync(Chunk progress);
    }
}

using System;
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
        /// Seeks to the <see cref="IPersistentDataChunk"/> after the given range.
        /// </summary>
        /// <param name="progress">
        /// Indicates the range that should be skipped. A typical usage is to resume
        /// downloading after interruption caused by network errors etc.
        /// </param>
        Task SeekAsync(Chunk progress);
    }
}

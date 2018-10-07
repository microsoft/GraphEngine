using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Newtonsoft.Json;
using Trinity.DynamicCluster.Persistency;
using Trinity.Storage;
using System.Threading;
using Trinity.Diagnostics;
using Trinity.DynamicCluster.Config;

namespace Trinity.Azure.Storage
{
    class BlobUploader : IPersistentUploader
    {
        private Guid version;
        private CloudBlobContainer m_container;
        private CancellationTokenSource m_tokenSource;
        private CloudBlobDirectory m_dir;
        private SemaphoreSlim m_sem;
        private StorageHelper m_helper;

        public BlobUploader(Guid version, int partitionId, CloudBlobContainer m_container)
        {
            this.version = version;
            this.m_container = m_container;
            this.m_tokenSource = new CancellationTokenSource();
            this.m_dir = m_container.GetDirectoryReference(version.ToString()).GetDirectoryReference(partitionId.ToString());
            this.m_sem = new SemaphoreSlim(DynamicClusterConfig.Instance.ConcurrentUploads);
            this.m_helper = new StorageHelper(m_tokenSource.Token);
        }

        public void Dispose()
        {
            try
            {
                m_sem.Dispose();
                m_tokenSource.Cancel();
                m_tokenSource.Dispose();
            }
            catch(Exception ex)
            {
                Log.WriteLine(LogLevel.Error, "{0}: an error occured during disposal: {1}", nameof(BlobUploader), ex.ToString());
            }
        }

        /// <summary>
        /// FinishUploading will only be called once, when all uploaders have finished.
        /// Hence we safely conclude that all partial indices are in position.
        /// </summary>
        public async Task FinishUploading()
        {
            var blobs = await m_helper.ListBlobsAsync(m_dir, flat: false);
            var partial_idxs = blobs
                .OfType<CloudBlockBlob>()
                .Where(f => f.Name.Contains(Constants.c_partition_index));
            var contents = await Task.WhenAll(
                partial_idxs.Select(m_helper.DownloadTextAsync));
            await Task.WhenAll(partial_idxs.Select(f => f.DeleteIfExistsAsync()));
            var full_idx = string.Join("\n", contents);
            await m_dir.GetBlockBlobReference(Constants.c_partition_index)
                             .UploadTextAsync(full_idx);
            Log.WriteLine(LogLevel.Info, $"{nameof(BlobUploader)}: Version {version} finished uploading.");
        }

        /// <summary>
        /// !Note UploadAsync must not report a complete status when the actual uploading
        /// is not finished -- otherwise <code>FinishUploading</code> will be called prematurely.
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public async Task UploadAsync(IPersistentDataChunk payload)
        {
            try
            {
                await m_sem.WaitAsync();
                //TODO make sure everything in IPersistentDataChunk are in range
                var partial_idx = ChunkSerialization.ToString(payload.DataChunkRange);
                Log.WriteLine(LogLevel.Info, $"{nameof(BlobUploader)}: uploading {partial_idx}.");

                var buf = payload.GetBuffer();

                await Task.WhenAll(
                    m_dir.GetBlockBlobReference($"{Constants.c_partition_index}_{payload.DataChunkRange.Id}") // TODO(maybe): index_<chunk id> should be _index. Append `parse(chunk)` to the tail of `_index`.
                   .Then(_ => m_helper.UploadTextAsync(_, partial_idx)),
                    m_dir.GetBlockBlobReference(payload.DataChunkRange.Id.ToString())
                   .Then(_ => m_helper.UploadDataAsync(_, buf)));
                Log.WriteLine(LogLevel.Info, $"{nameof(BlobUploader)}: finished uploading {partial_idx}.");
            }
            finally
            {
                m_sem.Release();
            }
        }

        public async Task UploadMetadataAsync(string key, Stream input)
        {
            var blob = m_dir.GetBlockBlobReference(key);
            await blob.UploadFromStreamAsync(input);
        }
    }
}
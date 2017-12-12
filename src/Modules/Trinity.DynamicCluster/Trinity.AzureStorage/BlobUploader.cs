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

namespace Trinity.Azure.Storage
{
    class BlobUploader : IPersistentUploader
    {
        private Guid version;
        private long lowKey;
        private long highKey;
        private CloudBlobContainer m_container;
        private CancellationTokenSource m_tokenSource;

        public BlobUploader(Guid version, long lowKey, long highKey, CloudBlobContainer m_container)
        {
            this.version = version;
            this.lowKey = lowKey;
            this.highKey = highKey;
            this.m_container = m_container;
            this.m_tokenSource = new CancellationTokenSource();
        }

        public void Dispose()
        {
            m_tokenSource.Cancel();
            m_tokenSource.Dispose();
        }

        /// <summary>
        /// UploadAsync will only be called once, when all uploaders have finished.
        /// Hence we safely conclude that all partial indices are in position.
        /// </summary>
        public async Task FinishUploading()
        {
            var partial_idxs = m_container.GetDirectoryReference(version.ToString())
                .ListBlobs(useFlatBlobListing: false)
                .OfType<CloudBlockBlob>()
                .Where(f => f.Name.Contains(Constants.c_index));
            var contents = await Task.WhenAll(
                partial_idxs.Select(f => f.DownloadTextAsync(m_tokenSource.Token)));
            await Task.WhenAll(partial_idxs.Select(f => f.DeleteIfExistsAsync()));
            var full_idx = string.Join("\n", contents);
            await m_container.GetBlockBlobReference(Constants.c_index)
                             .UploadTextAsync(full_idx);
            await m_container.GetBlockBlobReference(Constants.c_finished)
                             .UploadTextAsync("finished");
        }

        /// <summary>
        /// !Note UploadAsync must not report a complete status when the actual uploading
        /// is not finished -- otherwise <code>FinishUploading</code> will be called prematurally.
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public async Task UploadAsync(IPersistentDataChunk payload)
        {
            //TODO make sure everything in IPersistentDataChunk are in range
            var partial_idx = payload.DataChunkRange.ToString();
            var dir         = m_container.GetDirectoryReference(version.ToString());
            var buf         = payload.GetBuffer();
            await Task.WhenAll(
                dir.GetBlockBlobReference($"{Constants.c_index}_{payload.DataChunkRange.Id}")
               .UploadTextAsync(partial_idx, m_tokenSource.Token),
                dir.GetBlockBlobReference(payload.DataChunkRange.Id.ToString())
               .UploadFromByteArrayAsync(buf, 0, buf.Length, m_tokenSource.Token));
        }
    }
}
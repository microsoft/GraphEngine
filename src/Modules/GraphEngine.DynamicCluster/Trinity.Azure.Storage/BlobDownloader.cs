using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Trinity.DynamicCluster.Persistency;
using Trinity.Storage;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using Newtonsoft.Json;
using System.IO;
using Trinity.Diagnostics;
using Trinity.DynamicCluster.Config;

namespace Trinity.Azure.Storage
{
    class BlobDownloader : IPersistentDownloader
    {
        private Guid                             m_version;
        private long                             m_lowKey;
        private long                             m_highKey;
        private CloudBlobContainer               m_container;
        private CloudBlobDirectory               m_dir;
        private CancellationTokenSource          m_tokenSource;
        private BufferBlock<Task<InMemoryDataChunk>> m_buffer;
        private Task                             m_download;

        public BlobDownloader(Guid version, int partitionId, long lowKey, long highKey, CloudBlobContainer container)
        {
            this.m_version   = version;
            this.m_lowKey    = lowKey;
            this.m_highKey   = highKey;
            this.m_container = container;
            this.m_dir       = container.GetDirectoryReference(version.ToString()).GetDirectoryReference(partitionId.ToString());
            m_download       = _Download();
        }

        private async Task _Download(Chunk skip = null)
        {
            m_tokenSource = new CancellationTokenSource();
            bool finished = await m_dir.GetBlockBlobReference(Constants.c_finished).ExistsAsync();
            if (!finished) throw new SnapshotUploadUnfinishedException();
            m_buffer = new BufferBlock<Task<InMemoryDataChunk>>(new DataflowBlockOptions()
            {
                EnsureOrdered= true,
                CancellationToken=m_tokenSource.Token,
                BoundedCapacity = DynamicClusterConfig.Instance.ConcurrentDownloads
            });
            Log.WriteLine(LogLevel.Info, $"{nameof(BlobDownloader)}: Begin downloading {m_version} [{m_lowKey}-{m_highKey}].");
            if (skip != null)
            {
                Log.WriteLine(LogLevel.Info, $"{nameof(BlobDownloader)}: Version {m_version}: skipping {ChunkSerialization.ToString(skip)}.");
            }

            var idx = m_dir.GetBlockBlobReference(Constants.c_partition_index);
            var idx_content = await idx.DownloadTextAsync();
            var chunks = idx_content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                         .Select(ChunkSerialization.Parse)
                         .OrderBy(c => c.LowKey)
                         .Where(c => skip == null || c.LowKey > skip.HighKey)
                         .Where(InRange);
            foreach (var chunk in chunks)
            {
                m_buffer.Post(_Download_impl(chunk));
                if (m_tokenSource.IsCancellationRequested) break;
            }
        }

        private bool InRange(Chunk c) => c.LowKey <= m_highKey || c.HighKey >= m_lowKey;

        private async Task<InMemoryDataChunk> _Download_impl(Chunk chunk)
        {
            var file = m_dir.GetBlockBlobReference(chunk.Id.ToString());
            using (var ms = new MemoryStream())
            {
                Log.WriteLine(LogLevel.Info, $"{nameof(BlobDownloader)}: Version {m_version}: downloading {ChunkSerialization.ToString(chunk)}.");
                await file.DownloadToStreamAsync(ms);
                var buf = ms.ToArray();
                Log.WriteLine(LogLevel.Info, $"{nameof(BlobDownloader)}: Version {m_version}: finished {ChunkSerialization.ToString(chunk)}.");
                return new InMemoryDataChunk(chunk, buf, m_lowKey, m_highKey);
            }
        }

        public void Dispose()
        {
            m_tokenSource?.Cancel();
            m_tokenSource?.Dispose();
        }

        public async Task<IPersistentDataChunk> DownloadAsync()
        {
            await m_download;
            if (await m_buffer.OutputAvailableAsync(m_tokenSource.Token))
            {
                return await m_buffer.ReceiveAsync().Unwrap();
            }
            else
            {
                Log.WriteLine(LogLevel.Info, $"{nameof(BlobDownloader)}: Finish downloading {m_version} [{m_lowKey}-{m_highKey}].");
                return null;
            }
        }

        public async Task SeekAsync(Chunk progress)
        {
            await m_download;
            try { Dispose(); } catch { }
            m_download = _Download(progress);
        }

        public async Task DownloadMetadataAsync(string key, Stream output)
        {
            var blob = m_dir.GetBlockBlobReference(key);
            if (!await blob.ExistsAsync())
            {
                throw new NoDataException($"metadata {key} not found in snapshot {m_version}");
            }
            await blob.DownloadToStreamAsync(output);
        }
    }
}

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

namespace Trinity.Azure.Storage
{
    class BlobDownloader : IPersistentDownloader
    {
        private Guid version;
        private long lowKey;
        private long highKey;
        private CloudBlobContainer m_container;
        private CloudBlobDirectory m_dir;
        private CancellationTokenSource m_tokenSource;
        private BufferBlock<Task<BlobDataChunk>> m_buffer;
        private Task m_download;

        public BlobDownloader(Guid version, long lowKey, long highKey, CloudBlobContainer m_container)
        {
            this.version=version;
            this.lowKey=lowKey;
            this.highKey=highKey;
            this.m_container=m_container;
            this.m_dir = m_container.GetDirectoryReference(version.ToString());
            m_download = _Download();
        }

        private async Task _Download(Chunk skip = null)
        {
            m_tokenSource = new CancellationTokenSource();
            m_buffer = new BufferBlock<Task<BlobDataChunk>>(new DataflowBlockOptions()
            {
                EnsureOrdered= true,
                CancellationToken=m_tokenSource.Token,
                BoundedCapacity = BlobStorageConfig.Instance.ConcurrentDownloads
            });

            var idx = m_dir.GetBlockBlobReference(Constants.c_index);
            var idx_content = await idx.DownloadTextAsync();
            var chunks = idx_content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                         .Select(ParseChunk)
                         .OrderBy(c => c.LowKey)
                         .Where(c => skip == null || c.LowKey > skip.HighKey)
                         .Where(InRange);

            foreach(var chunk in chunks)
            {
                m_buffer.Post(_Download_impl(chunk));
            }
        }

        private bool InRange(Chunk c) => c.LowKey <= highKey || c.HighKey >= lowKey;

        private async Task<BlobDataChunk> _Download_impl(Chunk chunk)
        {
            var file = m_dir.GetBlockBlobReference(chunk.Id.ToString());
            var ms = new MemoryStream();
            await file.DownloadToStreamAsync(ms);
            var buf = ms.GetBuffer();
            ms.Dispose();
            return new BlobDataChunk(chunk, buf, lowKey, highKey);
        }

        private Chunk ParseChunk(string content) => JsonConvert.DeserializeObject<Chunk>(content);

        public void Dispose()
        {
            m_tokenSource?.Cancel();
            m_tokenSource?.Dispose();
        }

        public async Task<IPersistentDataChunk> DownloadAsync()
        {
            await m_download;
            if (await m_buffer.OutputAvailableAsync(m_tokenSource.Token))
                return await m_buffer.ReceiveAsync().Unwrap();
            else return null;
        }

        public async Task SeekAsync(Chunk progress)
        {
            await m_download;
            try { Dispose(); } catch { }
        }
    }
}

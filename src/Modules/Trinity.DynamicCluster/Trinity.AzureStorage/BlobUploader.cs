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

namespace Trinity.Azure.Storage
{
    class BlobUploader : IPersistentUploader
    {
        private Guid version;
        private long lowKey;
        private long highKey;
        private CloudBlobContainer m_container;
        
        public BlobUploader(Guid version, long lowKey, long highKey, CloudBlobContainer m_container)
        {
            this.version = version;
            this.lowKey = lowKey;
            this.highKey = highKey;
            this.m_container = m_container;
        }

        public void Dispose()
        {
            
        }

        private static string Encode(Chunk chunk) => JsonConvert.SerializeObject(chunk);
        public Task FinishUploading()
        {
            throw new NotImplementedException();

        }

        public async Task UploadAsync(IPersistentDataChunk payload)
        {
            var index = Encode(payload.DataChunkRange);
            var remote_index = m_container.GetDirectoryReference(version.ToString())
                .GetBlockBlobReference(Constants.c_index);

            await remote_index
                .UploadFromStreamAsync(new MemoryStream(Encoding.UTF8.GetBytes(index)))
                .ContinueWith(fst =>
                {
                    m_container.GetBlockBlobReference(payload.DataChunkRange.Id.ToString()).UploadFromStreamAsync(
                        new MemoryStream(payload.GetBuffer())
                    );

                });
        }
    }
}
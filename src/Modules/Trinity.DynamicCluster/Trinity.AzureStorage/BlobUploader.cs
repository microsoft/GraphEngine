using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Persistency;

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
            this.version     = version;
            this.lowKey      = lowKey;
            this.highKey     = highKey;
            this.m_container = m_container;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task FinishUploading()
        {
            throw new NotImplementedException();
        }

        public Task UploadAsync(IPersistentDataChunk payload)
        {
            throw new NotImplementedException();
        }
    }
}

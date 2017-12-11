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

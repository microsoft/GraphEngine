using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Persistency;
using Trinity.Storage;

namespace Trinity.Azure.Storage
{
    class BlobDownloader : IPersistentDownloader
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<IPersistentDataChunk> DownloadAsync()
        {
            throw new NotImplementedException();
        }

        public Task SeekAsync(Chunk progress)
        {
            throw new NotImplementedException();
        }
    }
}

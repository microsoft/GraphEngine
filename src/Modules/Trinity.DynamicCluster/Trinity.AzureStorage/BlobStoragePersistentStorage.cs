using System;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Persistency;

namespace Trinity.Azure.Storage
{
    public class BlobStoragePersistentStorage : IPersistentStorage
    {
        public BlobStoragePersistentStorage()
        {
        }

        public Task<Guid> CreateNewVersion()
        {
            throw new NotImplementedException();
        }

        public Task DeleteVersion(Guid version)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<IPersistentDownloader> Download(Guid version, long lowKey, long highKey)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> GetLatestVersion()
        {
            throw new NotImplementedException();
        }

        public Task<PersistentStorageMode> QueryPersistentStorageMode()
        {
            throw new NotImplementedException();
        }

        public Task<IPersistentUploader> Upload(Guid version, long lowKey, long highKey)
        {
            throw new NotImplementedException();
        }
    }
}

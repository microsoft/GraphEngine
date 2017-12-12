using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Trinity.DynamicCluster.Persistency;
using System.Threading;

namespace Trinity.Azure.Storage
{
    public class BlobStoragePersistentStorage : IPersistentStorage
    {
        private CloudStorageAccount m_storageAccount;
        private CloudBlobClient m_client;
        private CloudBlobContainer m_container;
        private CancellationTokenSource m_cancellationTokenSource;
        private CancellationToken m_cancel;

        public BlobStoragePersistentStorage()
        {
            m_storageAccount = CloudStorageAccount.Parse(BlobStorageConfig.Instance.ConnectionString);
            m_client = m_storageAccount.CreateCloudBlobClient();
            m_container = m_client.GetContainerReference(BlobStorageConfig.Instance.ContainerName);
            m_cancellationTokenSource = new CancellationTokenSource();
            m_cancel = m_cancellationTokenSource.Token;
        }

        private async Task EnsureContainer()
        {
            await m_container.CreateIfNotExistsAsync(cancellationToken: m_cancel);
        }

        public async Task<Guid> CreateNewVersion()
        {
            await EnsureContainer();
retry:
            var guid = Guid.NewGuid();
            var dir  = m_container.GetDirectoryReference(guid.ToString());
            if (dir.ListBlobs(useFlatBlobListing: true).Any()) goto retry;
            try
            {
                var blob = dir.GetBlockBlobReference(Constants.c_uploading);
                await blob.UploadFromByteArrayAsync(new byte[1], 0, 1, cancellationToken: m_cancel);
            }
            catch
            {
                // cleanup
                await DeleteVersion(guid);
                throw;
            }
            return guid;
        }

        public async Task DeleteVersion(Guid version)
        {
            await EnsureContainer();
            var blobs = m_container.GetDirectoryReference(version.ToString())
                       .ListBlobs(useFlatBlobListing:true)
                       .OfType<CloudBlob>()
                       .Select(_ => _.DeleteIfExistsAsync(cancellationToken:m_cancel));
            await Task.WhenAll(blobs);
        }

        public void Dispose()
        {
            m_cancellationTokenSource.Cancel();
            m_cancellationTokenSource.Dispose();
        }

        public async Task<Guid> GetLatestVersion()
        {
            await EnsureContainer();
            var files = m_container.ListBlobs(useFlatBlobListing: false)
                                      .OfType<CloudBlobDirectory>()
                                      .Select(dir => dir.GetBlockBlobReference(Constants.c_finished))
                                      .ToDictionary(f => f, f => f.ExistsAsync(m_cancel));
            await Task.WhenAll(files.Values.ToArray());
            var latest = files.Where(kvp => kvp.Value.Result)
                              .Select(kvp => kvp.Key)
                              .OrderByDescending(f => f.Properties.LastModified.Value)
                              .FirstOrDefault();
            if (latest == null) throw new NoDataException();
            return new Guid(latest.Parent.Uri.Segments.Last());
        }

        public Task<PersistentStorageMode> QueryPersistentStorageMode()
        {
            return Task.FromResult(PersistentStorageMode.AC_FileSystem | PersistentStorageMode.LO_External | PersistentStorageMode.ST_MechanicalDrive);
        }

        public async Task<IPersistentUploader> Upload(Guid version, long lowKey, long highKey)
        {
            await EnsureContainer();
            return new BlobUploader(version, lowKey, highKey, m_container);
        }

        public async Task<IPersistentDownloader> Download(Guid version, long lowKey, long highKey)
        {
            await EnsureContainer();
            return new BlobDownloader(version, lowKey, highKey, m_container);
        }

    }
}

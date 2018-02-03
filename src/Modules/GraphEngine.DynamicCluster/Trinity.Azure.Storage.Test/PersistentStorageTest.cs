using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Trinity.DynamicCluster.Persistency;

namespace Trinity.Azure.Storage.Test
{
    [TestClass]
    public class PersistentStorageTest
    {
        [TestInitialize]
        public void Init()
        {
            ConfigInit.Init();
        }

        [TestMethod]
        public void GetsAzureClientCorrectly()
        {
            BlobStoragePersistentStorage storage = new BlobStoragePersistentStorage();
            var client = storage._test_getclient();
            Assert.IsNotNull(client);
        }

        [TestMethod]
        [ExpectedException(typeof(NoDataException))]
        public async Task ThrowsNoDataCorrectly()
        {
            BlobStoragePersistentStorage storage = new BlobStoragePersistentStorage();
            var client = storage._test_getclient();
            var container = client.GetContainerReference(BlobStorageConfig.Instance.ContainerName);
retry:
            try
            {
                await DeleteContainer(container);
            }catch(StorageException ex) when (ex.Message.Contains("Conflict"))
            {
                await Task.Delay(1000);
                goto retry;
            }
            container = client.GetContainerReference(BlobStorageConfig.Instance.ContainerName);

retry2:
            try
            {
                await storage.GetLatestVersion();
            }catch(StorageException ex) when (ex.Message.Contains("Conflict"))
            {
                await Task.Delay(1000);
                goto retry2;
            }
        }

        private static async Task DeleteContainer(Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer container)
        {
            await container.DeleteIfExistsAsync();
            do
            {
                await Task.Delay(1000);
            } while (await container.ExistsAsync());
        }

        [TestMethod]
        public async Task CreatesContainerCorrectlyOnInit()
        {
            BlobStoragePersistentStorage storage = new BlobStoragePersistentStorage();
            var client = storage._test_getclient();
            var container = client.GetContainerReference(BlobStorageConfig.Instance.ContainerName);
            await container.DeleteIfExistsAsync();
            container = client.GetContainerReference(BlobStorageConfig.Instance.ContainerName);

retry2:
            try
            {
                await storage.GetLatestVersion();
            }
            catch (StorageException ex) when (ex.Message.Contains("Conflict"))
            {
                await Task.Delay(1000);
                goto retry2;
            }
            catch (NoDataException)
            {
            }

            var now_exists = await container.ExistsAsync();
            Assert.IsTrue(now_exists);
        }
    }
}
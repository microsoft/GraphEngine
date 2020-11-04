using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var container = client.GetBlobContainerClient(BlobStorageConfig.Instance.ContainerName);
retry:
            try
            {
                await DeleteContainer(container);
            }catch(RequestFailedException ex) when (ex.Message.Contains("Conflict"))
            {
                await Task.Delay(1000);
                goto retry;
            }

            container = client.GetBlobContainerClient(BlobStorageConfig.Instance.ContainerName);

            //container = client.GetContainerReference(BlobStorageConfig.Instance.ContainerName);

retry2:
            try
            {
                await storage.GetLatestVersion();
            }catch(RequestFailedException ex) when (ex.Message.Contains("Conflict"))
            {
                await Task.Delay(1000);
                goto retry2;
            }
        }

        private static async Task DeleteContainer(BlobContainerClient container)
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
            var container = client.GetBlobContainerClient(BlobStorageConfig.Instance.ContainerName);
            await container.DeleteIfExistsAsync();
            container = client.GetBlobContainerClient(BlobStorageConfig.Instance.ContainerName);

retry2:
            try
            {
                await storage.GetLatestVersion();
            }
            catch (RequestFailedException ex) when (ex.Message.Contains("Conflict"))
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
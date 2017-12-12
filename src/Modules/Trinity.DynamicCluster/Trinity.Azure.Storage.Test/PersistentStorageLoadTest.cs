
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trinity.DynamicCluster.Persistency;
using Trinity.Utilities;

namespace Trinity.Azure.Storage.Test
{
    [TestClass]
    public class PersistentStorageLoadTest
    {
        [TestMethod]
        public void DynamicClusterAssemblyCorrectlyLoaded()
        {
            Assert.IsTrue(typeof(DynamicCluster.Storage.DynamicMemoryCloud).Assembly.FullName.Contains("DynamicCluster"));
        }
        [TestMethod]
        public void DynamicClusterAssemblyIsFound()
        {
            Assert.IsTrue(AssemblyUtility.AnyAssembly(a => a.FullName.Contains("DynamicCluster")));
        }

        [TestMethod]
        public void LoadsDynamically()
        {
            BlobStorageConfig.Instance.ConnectionString =
                "DefaultEndpointsProtocol=https;AccountName=redtest;AccountKey=cJF7OVo5NluWtotOAE5ZA362UlXKvHPEE7khssSzamQfs8b3KMvL8CTMskD4Fa491AuLbVA0NffuCwiJj6crDA==;EndpointSuffix=core.windows.net";
            BlobStorageConfig.Instance.ContainerName = "TestContainer";
            var azure_store = AssemblyUtility.GetAllClassInstances<IPersistentStorage>().First();
            Assert.AreEqual(typeof(BlobStoragePersistentStorage), azure_store.GetType());
        }

        [TestMethod]
        public void LoadsBlobStoragePersistentStorage()
        {
            BlobStorageConfig.Instance.ConnectionString =
                "DefaultEndpointsProtocol=https;AccountName=redtest;AccountKey=cJF7OVo5NluWtotOAE5ZA362UlXKvHPEE7khssSzamQfs8b3KMvL8CTMskD4Fa491AuLbVA0NffuCwiJj6crDA==;EndpointSuffix=core.windows.net";
            BlobStorageConfig.Instance.ContainerName = "TestContainer";
            BlobStoragePersistentStorage storage = new BlobStoragePersistentStorage();
            //var azure_store = AssemblyUtility.GetAllClassInstances<BlobStoragePersistentStorage>().First();
            //Assert.AreEqual(typeof(BlobStoragePersistentStorage), azure_store.GetType());
        }
    }
}

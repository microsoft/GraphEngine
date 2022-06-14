using System;
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
        [ExpectedException(typeof(FormatException))]
        public void LoadsDynamically3()
        {
            BlobStorageConfig.Instance.ConnectionString =
                "not-a-connection-string";
            BlobStorageConfig.Instance.ContainerName = "TestContainer";
            var azure_store = AssemblyUtility.GetAllClassInstances<IPersistentStorage>().First();
            Assert.AreEqual(typeof(BlobStoragePersistentStorage), azure_store.GetType());
            try
            {
                azure_store.CreateNewVersion().Wait();
            }
            catch (AggregateException aex)
            {
                throw aex.InnerException;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LoadsDynamically4()
        {
            BlobStorageConfig.Instance.ConnectionString = null;
            BlobStorageConfig.Instance.ContainerName = "TestContainer";
            var azure_store = AssemblyUtility.GetAllClassInstances<IPersistentStorage>().First();
            Assert.AreEqual(typeof(BlobStoragePersistentStorage), azure_store.GetType());
            try
            {
                azure_store.CreateNewVersion().Wait();
            }
            catch (AggregateException aex)
            {
                throw aex.InnerException;
            }
        }

        [TestMethod]
        public void LoadsDynamically2()
        {
            BlobStorageConfig.Instance.ConnectionString =
                "not-a-connection-string";
            BlobStorageConfig.Instance.ContainerName = "TestContainer";
            var azure_store = AssemblyUtility.GetAllClassInstances<IPersistentStorage>().First();
            Assert.AreEqual(typeof(BlobStoragePersistentStorage), azure_store.GetType());
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Trinity.DynamicCluster;
using Trinity.Storage;

namespace Trinity.Azure.Storage.Test
{
    [TestClass]
    public class DownloadTest
    {
        private BlobStoragePersistentStorage m_storage;
        private CloudBlobClient m_client;
        private readonly Guid m_version = new Guid("0939a250-e41e-48b2-bce9-f3195b0388ae");
        public byte[][] Items = new[] {new byte[14], new byte[16]};


        [TestInitialize]
        public void Init()
        {
            ConfigInit.Init();
            m_storage = new BlobStoragePersistentStorage();
            m_client = m_storage._test_getclient();
        }

        [TestMethod]
        public async Task GetLatestVersion()
        {
            var container = m_client.GetContainerReference(BlobStorageConfig.Instance.ContainerName);
            await container.CreateIfNotExistsAsync();
            var dir = container.GetDirectoryReference(m_version.ToString());
            Chunk c1 = new Chunk(0, 10);
            Chunk c2 = new Chunk(100, 110);
            string idx = string.Join("\n", new[] {c1, c2}.Select(JsonConvert.SerializeObject));


            var f1 = Items[0].Clone() as byte[];
            var f2 = Items[1].Clone() as byte[];

            f2[0] = 0xEB;
            f2[10] = 0x02;
            f2[14] = 0xDE;
            f2[15] = 0xAD;
            await dir.GetDirectoryReference("0").GetBlockBlobReference(c1.Id.ToString()).UploadFromByteArrayAsync(f1, 0, f1.Length);
            await dir.GetDirectoryReference("0").GetBlockBlobReference(c2.Id.ToString()).UploadFromByteArrayAsync(f2, 0, f2.Length);
            await dir.GetDirectoryReference("0").GetBlockBlobReference(Constants.c_partition_index).UploadTextAsync(idx);
            await dir.GetBlockBlobReference(Constants.c_finished).UploadTextAsync("");

            var v = await m_storage.GetLatestVersion();

            Assert.AreEqual(m_version, v);
        }

        [TestMethod]
        public async Task Download()
        {
            await GetLatestVersion();
            var v = await m_storage.Download(m_version, 0, 0, 110);
            var src = await v.DownloadAsync();


            Assert.IsTrue(src.Take(Items.Length)
                .Select((it, i) =>
                    it.Buffer
                        .Take(Items[i].Length)
                        .SequenceEqual(Items[i]))
                .All(Utils.Identity));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Blob;
using Trinity.Diagnostics;
using Trinity.DynamicCluster.Persistency;
using Trinity.Storage;

namespace Trinity.Azure.Storage.Test
{
    [TestClass]
    public class UploadTest
    {
        private BlobStoragePersistentStorage m_storage;
        private CloudBlobClient m_client;
        private readonly Guid m_version = new Guid("0939a250-e41e-48b2-bce9-f3195b0388ae");

        [TestInitialize]
        public void Init()
        {
            ConfigInit.Init();
            m_storage = new BlobStoragePersistentStorage();
        }

        [TestMethod]
        public async Task Upload()
        {   // Equal.
            byte[] data = Encoding.UTF8.GetBytes("I am the bone of my sword.");

            var LowKey = 0;
            var HighKey = data.Length;

            Console.WriteLine($"HighKey: {HighKey}");

            var uploader = await m_storage.Upload(m_version, LowKey, HighKey);

            Chunk myChunk = new Chunk(LowKey, HighKey, new Guid("68d7c5be-beac-43a6-abf8-4daa4dce9090"));
            uploader.UploadAsync(new InMemoryDataChunk(myChunk, data, LowKey, HighKey)).Wait();
            var v = await m_storage.Download(m_version, LowKey, HighKey);
            var src = await v.DownloadAsync();
            var src_seq = src.GetEnumerator();
            
            var buffer = src_seq.Current.Buffer;
            Console.WriteLine(Encoding.UTF8.GetString(buffer.Take(data.Length).ToArray()));

            Assert.IsTrue(buffer.Take(data.Length).SequenceEqual(data.Take(data.Length)));
            
        }
    }
}
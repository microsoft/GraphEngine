using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Storage;
using Trinity.DynamicCluster.Test.Mocks;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Test
{
    [TestClass]
    public class PartitionTests
    {
        List<Chunk> cks = new List<Chunk>{ Chunk.FullRangeChunk };
        byte[] buf = new byte[16];
        unsafe byte* bp;
        private TrinityMessage tm;
        private GCHandle gchandle;

        [TestInitialize]
        public unsafe void Init()
        {
            gchandle = GCHandle.Alloc(buf, GCHandleType.Pinned);
            bp = (byte*)gchandle.AddrOfPinnedObject().ToPointer();
            tm = new TrinityMessage(bp, 16);
        }

        [TestCleanup]
        public void Cleanup()
        {
            gchandle.Free();
        }

        [TestMethod]
        public void PartitionInit()
        {
            var p = new Partition();
            p.Dispose();
        }

        [TestMethod]
        public unsafe void PartitionMount()
        {
            var stg = new IStorage1();
            using (var p = new Partition())
            {
                p.Mount(stg, cks);
                p.SendMessageAsync(tm).Wait();
            }
            Assert.IsTrue(stg.SendMessageCalledOnce);
        }

        [TestMethod]
        public unsafe void PartitionRR()
        {
            var stgs = Utils.Infinity<IStorage1>().Take(5).ToList();
            using (var p = new Partition())
            {
                foreach (var s in stgs) p.Mount(s, cks);
                for (int i = 0; i<5; ++i)
                {
                    p.RoundRobin(_ => _.SendMessageAsync(tm)).Wait();
                }
            }
            Assert.IsTrue(stgs.All(_ => _.SendMessageCalledOnce));
        }

        [TestMethod]
        public unsafe void PartitionRR2()
        {
            var stgs = Utils.Infinity<IStorage1>().Take(5).ToList();
            using (var p = new Partition())
            {
                foreach (var s in stgs) p.Mount(s, cks);
                for (int i = 0; i<5; ++i)
                {
                    p.RoundRobin(_ => _.SendRecvMessageAsync(tm)).Wait();
                }
            }
            Assert.IsTrue(stgs.All(_ => _.SendMessageCalledOnce));
        }

        [TestMethod]
        public unsafe void PartitionRR3()
        {
            var stgs = Utils.Infinity<IStorage1>().Take(5).ToList();
            using (var p = new Partition())
            {
                foreach (var s in stgs) p.Mount(s, cks);
                for (int i = 0; i<5; ++i)
                {
                    p.RoundRobin(_ => _.SendRecvMessageAsync(tm)).Wait();
                }
            }
            Assert.IsTrue(stgs.All(_ => _.SendMessageCalledOnce));
        }

        [TestMethod]
        public unsafe void PartitionFirstAvailable1()
        {
            var stgs = Utils.Infinity<IStorage1>().Take(5).ToList();
            using (var p = new Partition())
            {
                foreach (var s in stgs) p.Mount(s, cks);
                for (int i = 0; i < 5; ++i)
                {
                    p.FirstAvailable(_ => _.SendRecvMessageAsync(tm)).Wait();
                }
            }
            Assert.IsTrue(stgs.Any(_ => _.cnt == 5));
        }

        [TestMethod]
        public unsafe void PartitionFirstAvailable4()
        {
            var stgs = Utils.Infinity<IStorage1>().Take(5).ToList();
            using (var p = new Partition())
            {
                foreach (var s in stgs) p.Mount(s, cks);
                for (int i = 0; i < 3; ++i)
                {
                    p.FirstAvailable(_ => _.SendRecvMessageAsync(tm)).Wait();
                }
                int idx = stgs.FindIndex(_ => _.cnt == 3);
                p.Unmount(stgs[idx]);
                for (int i = 0; i < 2; ++i)
                {
                    p.FirstAvailable(_ => _.SendRecvMessageAsync(tm)).Wait();
                }

            }
            Assert.IsTrue(stgs.Any(_ => _.cnt == 3));
            Assert.IsTrue(stgs.Any(_ => _.cnt == 2));
        }

        [TestMethod]
        public unsafe void PartitionFirstAvailable5()
        {
            var stgs = Utils.Infinity<IStorage1>().Take(5).ToList();
            using (var p = new Partition())
            {
                foreach (var s in stgs) p.Mount(s, cks);
                for (int i = 0; i < 3; ++i)
                {
                    p.FirstAvailable(_ => _.SendRecvMessageAsync(tm)).Wait();
                }
                int idx = stgs.FindIndex(_ => _.cnt != 3);
                p.Unmount(stgs[idx]);
                for (int i = 0; i < 2; ++i)
                {
                    p.FirstAvailable(_ => _.SendRecvMessageAsync(tm)).Wait();
                }

            }
            Assert.IsTrue(stgs.Any(_ => _.cnt == 5));
        }

        [TestMethod]
        public unsafe void PartitionFirstAvailable2()
        {
            var stgs = Utils.Infinity<IStorage1>().Take(5).ToList();
            using (var p = new Partition())
            {
                foreach (var s in stgs) p.Mount(s, cks);
                for (int i = 0; i < 5; ++i)
                {
                    p.FirstAvailable(_ => _.SendRecvMessageAsync(tm)).Wait();
                }
            }
            Assert.IsTrue(stgs.Any(_ => _.cnt == 5));
        }

        [TestMethod]
        public unsafe void PartitionFirstAvailable3()
        {
            var stgs = Utils.Infinity<IStorage1>().Take(5).ToList();
            using (var p = new Partition())
            {
                foreach (var s in stgs) p.Mount(s, cks);
                for (int i = 0; i < 5; ++i)
                {
                    p.FirstAvailable(_ => _.SendMessageAsync(tm)).Wait();
                }
            }
            Assert.IsTrue(stgs.Any(_ => _.cnt == 5));
        }

        [TestMethod]
        public unsafe void PartitionUniformRandom1()
        {
            var stgs = Utils.Infinity<IStorage1>().Take(5).ToList();
            using (var p = new Partition())
            {
                foreach (var s in stgs) p.Mount(s, cks);
                for (int i = 0; i < 1024; ++i)
                {
                    p.UniformRandom(_ => _.SendMessageAsync(tm)).Wait();
                }
            }
            Assert.AreEqual(204.8, stgs.Average(_ => (double)_.cnt));
        }

        [TestMethod]
        public unsafe void PartitionUniformRandom2()
        {
            var stgs = Utils.Infinity<IStorage1>().Take(5).ToList();
            using (var p = new Partition())
            {
                foreach (var s in stgs) p.Mount(s, cks);
                for (int i = 0; i < 1024; ++i)
                {
                    p.UniformRandom(_ => _.SendRecvMessageAsync(tm)).Wait();
                }
            }
            Assert.AreEqual(204.8, stgs.Average(_ => (double)_.cnt));
        }

        [TestMethod]
        public unsafe void PartitionUniformRandom3()
        {
            var stgs = Utils.Infinity<IStorage1>().Take(5).ToList();
            using (var p = new Partition())
            {
                foreach (var s in stgs) p.Mount(s, cks);
                for (int i = 0; i < 1024; ++i)
                {
                    p.UniformRandom(_ => _.SendRecvMessageAsync(tm)).Wait();
                }
            }
            Assert.AreEqual(204.8, stgs.Average(_ => (double)_.cnt));
        }

        [TestMethod]
        public unsafe void Broadcast1()
        {
            var stg1s = Utils.Infinity<IStorage1>().Take(5).ToList();
            var stg2s = Utils.Infinity<IStorage2>().Take(5).ToList();
            using (var p = new Partition())
            {
                foreach (var s in stg1s) p.Mount(s, cks);
                p.Broadcast(_ => _.SendMessageAsync(tm)).Wait();
                Assert.IsTrue(stg1s.All(_ => _.cnt == 1));
                foreach (var s in stg2s) p.Mount(s, cks);
                try
                {
                    p.Broadcast(_ => _.SendMessageAsync(tm)).Wait();
                    Assert.Fail();
                }
                catch (BroadcastException ex) { }
                Assert.IsTrue(stg1s.All(_ => _.cnt == 2));
            }
        }

        [TestMethod]
        public unsafe void Broadcast2()
        {
            var stg1s = Utils.Infinity<IStorage1>().Take(5).ToList();
            var stg2s = Utils.Infinity<IStorage2>().Take(5).ToList();
            using (var p = new Partition())
            {
                foreach (var s in stg1s) p.Mount(s, cks);
                p.Broadcast(_ => _.SendRecvMessageAsync(tm)).Wait();
                Assert.IsTrue(stg1s.All(_ => _.cnt == 1));
                foreach (var s in stg2s) p.Mount(s, cks);
                try
                {
                    p.Broadcast(_ => _.SendRecvMessageAsync(tm)).Wait();
                    Assert.Fail();
                }
                catch (BroadcastException<TrinityResponse> bex)
                {
                    Assert.AreEqual(5, bex.Exceptions.Count());
                    Assert.AreEqual(5, bex.Results.Count());
                    bex.Dispose();
                }
                catch (BroadcastException ex) { throw; }
                Assert.IsTrue(stg1s.All(_ => _.cnt == 2));
            }
        }

        [TestMethod]
        public async Task Broadcast3()
        {
            var stg1s = Utils.Infinity<IStorage1>().Take(5).ToList();
            var stg2s = Utils.Infinity<IStorage2>().Take(5).ToList();
            using (var p = new Partition())
            {
                foreach (var s in stg1s) p.Mount(s, cks);
                await p.Broadcast(_ => _.SendRecvMessageAsync(tm));
                Assert.IsTrue(stg1s.All(_ => _.cnt == 1));
                foreach (var s in stg2s) p.Mount(s, cks);
                try
                {
                    await p.Broadcast(_ => _.SendRecvMessageAsync(tm));
                    Assert.Fail();
                }
                catch (BroadcastException<TrinityResponse> bex)
                {
                    Assert.AreEqual(5, bex.Exceptions.Count());
                    Assert.AreEqual(5, bex.Results.Count());
                    bex.Dispose();
                }
                catch (BroadcastException ex) { throw; }
                Assert.IsTrue(stg1s.All(_ => _.cnt == 2));
            }
        }
    }
}

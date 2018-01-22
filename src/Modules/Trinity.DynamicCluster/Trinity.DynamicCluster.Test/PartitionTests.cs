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
    public unsafe class PartitionTests
    {
        List<Chunk> cks = new List<Chunk>{ Chunk.FullRangeChunk };
        byte[] buf = new byte[16];
        byte* bp;
        private TrinityMessage tm;
        private GCHandle gchandle;

        [TestInitialize]
        public void Init()
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
                p.SendMessage(tm);
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
                    p.RoundRobin(_ => _.SendMessage(tm));
                }
            }
            Assert.IsTrue(stgs.All(_ => _.SendMessageCalledOnce));
        }
    }
}

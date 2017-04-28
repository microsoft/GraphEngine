using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using NUnit.Framework;
using Trinity;
using Trinity.Storage;

namespace storage1
{
    public class cell_resize_1
    {
        [TestCase(1, 10)]
        [TestCase(10, 100)]
        [TestCase(100, 1000)]
        [TestCase(1000, 10000)]
        public void TestCellResize(int iterations, int count)
        {
            TrinityConfig.CurrentRunningMode = RunningMode.Embedded;
            for (int c = 0; c < count; c++)
                Global.LocalStorage.SaveGraphNode(c, "MyCell");

            int thread_count = 50;

            Thread[] threads = new Thread[thread_count];

            for (int i = 0; i < thread_count; i++)
            {
                Thread t = new Thread(() => WorkerThread(iterations, count));
                threads[i] = t;
                t.Start();
            }
            for (int i = 0; i < thread_count; i++)
            {
                threads[i].Join();
            }

            for (int i=0;i<count; ++i)
            {
                using (var cell = Global.LocalStorage.UseGraphNode(i))
                {
                    Assert.Equal(iterations, cell.outLinks.Count);
                    Assert.Equal(0, cell.inLinks.Count);
                    Assert.Equal("MyCell", cell.value);
                }
            }
        }

        void WorkerThread(int iterations, int count)
        {
            for (int c = 0; c < count; c++)
                using (var cell = Global.LocalStorage.UseGraphNode(c))
                {
                    for (int i = 0; i < iterations; i++)
                        if (!cell.outLinks.Contains(i))
                            cell.outLinks.Add(i);
                }
        }
    }
}

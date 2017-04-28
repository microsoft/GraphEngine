using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Trinity;
using Trinity.Storage;

namespace storage6
{
    public class storage6
    {
        [TestCase(1<<10, 8<<20, 4)]
        [TestCase(1<<20, 8<<10, 4)]
        [TestCase(1<<11, 8<<16, 24)]
        [TestCase(1<<21, 8<<8, 24)]
        public void T1(int cellSize, int count, int tCount)
        {
            byte[] content = new byte[cellSize];

            TrinityConfig.CurrentRunningMode = RunningMode.Embedded;
            Global.LocalStorage.ResetStorage();

            Stopwatch sw = new Stopwatch();

            sw.Restart();
            Parallel.For(0, tCount, pid =>
            {
                for (int i = 0; i < count / tCount; ++i)
                {
                    long id = (long)i + (pid * count / tCount);
                    Assert.Equal(TrinityErrorCode.E_SUCCESS, Global.LocalStorage.SaveCell(id, content));
                }
            });

            sw.Stop();

            Console.WriteLine(sw.ElapsedMilliseconds);
            content = new byte[cellSize * 2];//double size

            sw.Restart();
            Parallel.For(0, tCount, pid =>
            {
                for (int i = count / tCount - 1; i >= 0; --i)
                {
                    long id = (long)i + (pid * count / tCount);
                    Assert.Equal(TrinityErrorCode.E_SUCCESS, Global.LocalStorage.SaveCell(id, content));
                }
            });

            sw.Stop();

            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}

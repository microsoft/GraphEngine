using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using NUnit.Framework;
using Trinity;
using Trinity.Storage;

namespace storage5
{
    public class storage5
    {
        [TestCase(100, 314, 7)]
        [TestCase(1000, 6736, 65)]
        [TestCase(100000, 23, 255)]
        public unsafe void T1(int cellCount, int cellSize, byte content)
        {
            Global.LocalStorage.ResetStorage();

            byte[] buff = new byte[cellSize];
            for (int i = 0; i < buff.Length; i++)
                buff[i] = content;

            Stopwatch sw = Stopwatch.StartNew();

            for (int i = 0; i < cellCount; i++)
                Global.LocalStorage.SaveCell(i, buff);
            sw.Stop();
            Console.WriteLine("Save cells done (cold start), took {0}.", sw.ElapsedMilliseconds);

            sw.Restart();
            for (int i = 0; i < cellCount; i++)
            {
                Global.LocalStorage.SaveCell(i, buff, 0, cellSize);
            }
            sw.Stop();
            Console.WriteLine("Save cells done (after warm-up), took {0}", sw.ElapsedMilliseconds);

            byte[] cell;
            Assert.Equal(TrinityErrorCode.E_SUCCESS, Global.LocalStorage.LoadCell(cellCount - 1, out cell));

            Console.WriteLine("cell size: {0}\n", cell.Length);
            Assert.Equal(cellSize, cell.Length);

            for (int i = 0; i < cellSize; i++)
            {
                Assert.Equal(content, cell[i]);
            }

            sw.Restart();
            ulong garbage = 0;
            ulong garbage_expected = 0;
            int id = 0;
            sw.Restart();
            foreach (var cellInfo in Global.LocalStorage)
            {
                garbage += (ulong)cellInfo.CellId + (ulong)cellInfo.CellSize + (ulong)cellInfo.CellType;
                garbage_expected += (ulong)id + (ulong)cellSize + 0;
                ++id;
            }
            sw.Stop();
            Console.WriteLine("Iteration (cold): {0} \t {1}", garbage, sw.ElapsedMilliseconds);
            Assert.Equal(garbage_expected, garbage);

            sw.Restart();
            id = 0;
            garbage = 0;
            garbage_expected = 0;
            foreach (var cellInfo in Global.LocalStorage)
            {
                garbage += (ulong)cellInfo.CellId + (ulong)cellInfo.CellSize + (ulong)cellInfo.CellType;
                garbage_expected += (ulong)id + (ulong)cellSize + 0;
                ++id;
            }
            sw.Stop();
            Console.WriteLine("Iteration (warm): {0} \t {1}", garbage, sw.ElapsedMilliseconds);
            Assert.Equal(garbage_expected, garbage);


            var cells = from c in Global.LocalStorage where c.CellId < cellCount / 2 select c;

            foreach (var c in cells)
            {
                Assert.True(c.CellId < cellCount / 2);
            }

            long __count = 0;
            id = 0;
            garbage = 0;
            garbage_expected = 0;
            using (var it = Global.LocalStorage.GetEnumerator())
            {
                while (it.MoveNext())
                {
                    var cellInfo = it.Current;
                    garbage += (ulong)cellInfo.CellId + (ulong)cellInfo.CellSize + (ulong)cellInfo.CellType;
                    garbage_expected += (ulong)id + (ulong)cellSize + 0;
                    ++__count;
                    ++id;
                }
            }
            Assert.Equal(garbage_expected, garbage);
            Assert.Equal((ulong)__count, (ulong)Global.LocalStorage.CellCount);
        }
    }
}

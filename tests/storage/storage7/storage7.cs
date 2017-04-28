using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Trinity;
using Trinity.Storage;

namespace storage7
{
    public class storage7
    {
        [TestCase(1027, 1<<20)]
        [TestCase(2049, 1575984)]
        public unsafe void T1(int buff_len, int count)
        {
            Global.LocalStorage.ResetStorage();

            byte[] buf = new byte[buff_len];


            for (int i = 0; i < count; ++i)
            {
                fixed (byte* b = buf)
                {
                    long* p = (long*)b;
                    for (int j = 0; j < buf.Length / sizeof(long); ++j)
                    {
                        p[j] = (long)(i + j);
                    }
                }
                Global.LocalStorage.SaveCell(i, buf);
            }

            var before = Global.LocalStorage.CellCount;

            Global.LocalStorage.SaveStorage();

            Console.WriteLine("Saved storage");

            Global.LocalStorage.LoadStorage();

            var after = Global.LocalStorage.CellCount;

            Assert.AreEqual(before, after);

            Console.WriteLine("Begin testing ...");

            for (int i = 0; i < count; ++i)
            {
                Assert.AreEqual(TrinityErrorCode.E_SUCCESS, Global.LocalStorage.LoadCell(i, out buf));
                fixed (byte* b = buf)
                {
                    long* p = (long*)b;
                    Assert.AreEqual(buff_len, buf.Length);

                    for (int j = 0; j < buf.Length / sizeof(long); ++j)
                    {
                        Assert.AreEqual(p[j], (long)(i + j));
                    }
                }
            }
        }
    }
}

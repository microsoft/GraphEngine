using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using Trinity;
using Trinity.Storage;
using System.Threading;

namespace storage2
{
    public class storage2
    {
        [Theory]
        [InlineData(10, 314, 333, 7)]
        [InlineData(100, 2, 65, 34)]
        [InlineData(1000, 8124, 16, 1)]
        [InlineData(10000, 999, 0, 233)]
        [InlineData(100000, 1, 1560, 12)]
        public void T1(int cellCount, int s1, int s2, byte v)
        {
            byte[] buff = new byte[s1];
            for (int i = 0; i < buff.Length; i++)
                buff[i] = v;

            for (int i = 0; i < cellCount; i++)
                Global.LocalStorage.SaveCell(i, buff);

            for (int i = 0; i < cellCount; i++)
            {
                byte[] out_buf = null;
                Global.LocalStorage.LoadCell(i, out out_buf);
                Assert.NotNull(out_buf);
                Assert.Equal(s1, out_buf.Length);
                foreach(var b in out_buf)
                    Assert.Equal(v, b);
            }

            Random rand = new Random();
            buff = new byte[s2];
            for (int i=0; i<s2; ++i)
            {
                buff[i] = (byte)i;
            }

            for (int i = 0; i < cellCount; i++)
                Global.LocalStorage.UpdateCell(i, buff, 0, rand.Next(Math.Min(1, s2), s2));

            for (int i = 0; i < cellCount; i++)
            {
                byte[] out_buf = null;
                Global.LocalStorage.LoadCell(i, out out_buf);
                Assert.NotNull(out_buf);
                Assert.InRange(out_buf.Length, Math.Min(1, s2), Math.Max(1, s2));
                for(int j=0;j<out_buf.Length;++j)
                {
                    Assert.Equal((byte)j, out_buf[j]);
                }
            }

            for (int i = 0; i < cellCount; i++)
                Global.LocalStorage.RemoveCell(i);

            ulong garbage = 0;
            foreach(var cellInfo in Global.LocalStorage)
            {
                garbage += (ulong)cellInfo.CellId + (ulong)cellInfo.CellSize + (ulong)cellInfo.CellType;
            }

            Assert.Equal(0UL, garbage);
        }
    }
}

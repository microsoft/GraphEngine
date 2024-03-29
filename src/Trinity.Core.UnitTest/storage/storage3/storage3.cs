using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Trinity;
using Trinity.Storage;

namespace storage3
{
    [CollectionDefinition("storage3", DisableParallelization = true)]
    public class storage3
    {
        [Theory]
        [InlineData(1)]
        [InlineData(256)]
        [InlineData(512)]
        [InlineData(3065)]
        [InlineData(131072)]
        [InlineData(16 * 1024 * 1024)]
        [InlineData(128 * 1024 * 1024)]
        //[InlineData(256 * 1024 * 1024)]
        //[InlineData(1024 * 1024 * 1024)]
        public void storage_empty_after_save_then_remove_one_byte_cells(int nr_cell)
        {
            for (int i = 0; i < nr_cell; i++)
            {
                Global.LocalStorage.SaveCell(i, new byte[] { 0 });
            }
            for (int i = 0; i < nr_cell; i++)
            {
                Global.LocalStorage.RemoveCell(i);
            }

            Assert.Equal(0, (int)Global.LocalStorage.CellCount);
        }

        [Fact]
        public void T2()
        {
            for (int i = 0; i < 256; i++)
            {
                Global.LocalStorage.SaveCell(i, new byte[] { 0 });
            }
            for (int i = 0; i < 256; i++)
            {
                Global.LocalStorage.RemoveCell(i);
            }

            Assert.Equal(0, (int)Global.LocalStorage.CellCount);
        }
    }
}

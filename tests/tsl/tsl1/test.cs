using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.Storage;
using Xunit;

namespace tsl1
{
    public class simple_tsl_tests
    {
        [Fact]
        public void T1()
        {
            MyCell cell = new MyCell();
            Assert.NotNull(cell);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void T2(int i)
        {
            MyCell cell = new MyCell(i);
            Assert.Equal(i, cell.A);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void T3(long cell_id)
        {
            MyCell cell = new MyCell(cell_id);
            Assert.Equal(cell_id, cell.CellID);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        [InlineData(3, 4)]
        [InlineData(1, 5)]
        [InlineData(3, 2)]
        [InlineData(6, 1)]
        public void T4(long cell_id, int x)
        {
            MyCell cell = new MyCell(cell_id, x);
            Global.LocalStorage.SaveMyCell(cell);

            var load_cell = Global.LocalStorage.LoadMyCell(cell_id);
            Assert.Equal(cell_id, load_cell.CellID);
            Assert.Equal(x, load_cell.A);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        [InlineData(3, 4)]
        [InlineData(1, 5)]
        [InlineData(3, 2)]
        [InlineData(6, 1)]
        public void T5(long cell_id, int x)
        {
            MyCell cell = new MyCell(cell_id, x);
            Global.LocalStorage.SaveMyCell(cell);

            using (var accessor = Global.LocalStorage.UseMyCell(cell_id))
            {
                Assert.Equal(cell_id, accessor.CellID);
                Assert.Equal(x, accessor.A);
            }
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        [InlineData(3, 4)]
        [InlineData(1, 5)]
        [InlineData(3, 2)]
        [InlineData(6, 1)]
        public void T6(long cell_id, int x)
        {
            Global.LocalStorage.RemoveCell(cell_id);
            using (var accessor = Global.LocalStorage.UseMyCell(cell_id, Trinity.TSL.Lib.CellAccessOptions.CreateNewOnCellNotFound))
            {
                accessor.A = x;
            }

            using (var accessor = Global.LocalStorage.UseMyCell(cell_id, Trinity.TSL.Lib.CellAccessOptions.CreateNewOnCellNotFound))
            {
                Assert.Equal(cell_id, accessor.CellID);
                Assert.Equal(x, accessor.A);
            }
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        [InlineData(3, 4)]
        [InlineData(1, 5)]
        [InlineData(3, 2)]
        [InlineData(6, 1)]
        public void T7(long cell_id, int x)
        {
            Global.LocalStorage.RemoveCell(cell_id);

            var ex = Assert.Throws<CellNotFoundException>(() =>
            Global.LocalStorage.UseMyCell(cell_id, Trinity.TSL.Lib.CellAccessOptions.ThrowExceptionOnCellNotFound));
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        [InlineData(3, 4)]
        [InlineData(1, 5)]
        [InlineData(3, 2)]
        [InlineData(6, 1)]
        public void T8(long cell_id, int x)
        {
            var cell = Global.LocalStorage.NewGenericCell("MyCell");
            cell.CellID = cell_id;
            cell.SetField("A", x);
            Global.LocalStorage.SaveGenericCell(cell);

            Assert.Equal(x, Global.LocalStorage.LoadMyCell(cell_id).A);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        [InlineData(3, 4)]
        [InlineData(1, 5)]
        [InlineData(3, 2)]
        [InlineData(6, 1)]
        public void T9(long cell_id, int x)
        {
            Global.LocalStorage.SaveMyCell(cell_id, x);

            Assert.Equal(x, Global.LocalStorage.LoadGenericCell(cell_id).GetField<int>("A"));
        }
    }
}

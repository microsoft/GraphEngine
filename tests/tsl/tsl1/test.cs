using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.Storage;
using NUnit.Framework;

namespace tsl1
{
    public class simple_tsl_tests
    {
        [Test]
        public void T1()
        {
            MyCell cell = new MyCell();
            Assert.NotNull(cell);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void T2(int i)
        {
            MyCell cell = new MyCell(i);
            Assert.Equal(i, cell.A);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void T3(long cell_id)
        {
            MyCell cell = new MyCell(cell_id);
            Assert.Equal(cell_id, cell.CellID);
        }

        [TestCase(1, 2)]
        [TestCase(2, 3)]
        [TestCase(3, 4)]
        [TestCase(1, 5)]
        [TestCase(3, 2)]
        [TestCase(6, 1)]
        public void T4(long cell_id, int x)
        {
            MyCell cell = new MyCell(cell_id, x);
            Global.LocalStorage.SaveMyCell(cell);

            var load_cell = Global.LocalStorage.LoadMyCell(cell_id);
            Assert.Equal(cell_id, load_cell.CellID);
            Assert.Equal(x, load_cell.A);
        }

        [TestCase(1, 2)]
        [TestCase(2, 3)]
        [TestCase(3, 4)]
        [TestCase(1, 5)]
        [TestCase(3, 2)]
        [TestCase(6, 1)]
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

        [TestCase(1, 2)]
        [TestCase(2, 3)]
        [TestCase(3, 4)]
        [TestCase(1, 5)]
        [TestCase(3, 2)]
        [TestCase(6, 1)]
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

        [TestCase(1, 2)]
        [TestCase(2, 3)]
        [TestCase(3, 4)]
        [TestCase(1, 5)]
        [TestCase(3, 2)]
        [TestCase(6, 1)]
        public void T7(long cell_id, int x)
        {
            Global.LocalStorage.RemoveCell(cell_id);

            var ex = Assert.Throws<CellNotFoundException>(() =>
            Global.LocalStorage.UseMyCell(cell_id, Trinity.TSL.Lib.CellAccessOptions.ThrowExceptionOnCellNotFound));
        }

        [TestCase(1, 2)]
        [TestCase(2, 3)]
        [TestCase(3, 4)]
        [TestCase(1, 5)]
        [TestCase(3, 2)]
        [TestCase(6, 1)]
        public void T8(long cell_id, int x)
        {
            var cell = Global.LocalStorage.NewGenericCell("MyCell");
            cell.CellID = cell_id;
            cell.SetField("A", x);
            Global.LocalStorage.SaveGenericCell(cell);

            Assert.Equal(x, Global.LocalStorage.LoadMyCell(cell_id).A);
        }

        [TestCase(1, 2)]
        [TestCase(2, 3)]
        [TestCase(3, 4)]
        [TestCase(1, 5)]
        [TestCase(3, 2)]
        [TestCase(6, 1)]
        public void T9(long cell_id, int x)
        {
            Global.LocalStorage.SaveMyCell(cell_id, x);

            Assert.Equal(x, Global.LocalStorage.LoadGenericCell(cell_id).GetField<int>("A"));
        }
    }
}

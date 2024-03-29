using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Trinity;
using Trinity.Storage;

namespace issue15
{
    public class issue15
    {
        [Fact]
        public void StorageEnumerationInterruption()
        {
            Enumerable.Range(0, 1024).ToList().ForEach(_ => Global.LocalStorage.SaveMyCell(_, _));
            int cnt = 0;
            foreach(var cell in Global.LocalStorage.MyCell_Selector())
            {
                ++cnt;
                if(cell.CellId == 256)
                {
                    Global.LocalStorage.RemoveCell(cell.CellId+1);
                }
                if(cell.CellId == 233)
                {
                    Global.LocalStorage.RemoveCell(cell.CellId+1);
                }
            }
            Assert.Equal(cnt, 1022);
        }
    }
}

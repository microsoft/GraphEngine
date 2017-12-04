using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Trinity;
using Trinity.Storage;

namespace icall5
{
    public class icall5
    {
        [Test]
        public void icall_addcell()
        {
            var val = Global.LocalStorage.AddCell(0, new byte[128], 0, 128, 0);
        }
    }
}

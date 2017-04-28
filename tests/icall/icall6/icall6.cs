using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Trinity;
using Trinity.Storage;

namespace icall6
{
    public class icall6
    {
        [Test]
        public void icall_loadcell()
        {
            byte[] content;
            ushort type;
            var val = Global.LocalStorage.LoadCell(0, out content, out type);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Trinity;
using Trinity.Storage;

namespace icall4
{
    public class icall4
    {
        [Test]
        public void icall_totalcommittedmemory()
        {
            var val = Global.LocalStorage.TotalCommittedMemory;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Trinity;
using Trinity.Storage;

namespace icall1
{
    public class icall1
    {
        [Test]
        public void icall_committedindex()
        {
            var val = Global.LocalStorage.CommittedIndexMemory;
        }
    }
}

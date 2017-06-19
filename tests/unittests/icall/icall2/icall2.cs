using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Trinity;
using Trinity.Storage;

namespace icall2
{
    public class icall2
    {
        [Test]
        public void icall_committedtrunk()
        {
            var val = Global.LocalStorage.CommittedTrunkMemory;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Trinity;
using Trinity.Storage;

namespace icall1
{
    public class icall1
    {
        [Fact]
        public void icall_committedindex()
        {
            var val = Global.LocalStorage.CommittedIndexMemory;
        }
    }
}

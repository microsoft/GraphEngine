using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Trinity;
using Trinity.Storage;

namespace cleanup4
{
    public class cleanup4
    {
        [Fact]
        public void AllocateLocalStorageTwice()
        {
            Global.LocalStorage.ResetStorage();
            Global.Uninitialize();
            Global.Initialize();
            Global.LocalStorage.ResetStorage();
            Global.Uninitialize();
        }
    }
}

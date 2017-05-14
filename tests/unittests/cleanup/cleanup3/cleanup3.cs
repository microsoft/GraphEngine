using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Trinity;
using Trinity.Storage;

namespace cleanup3
{
    public class cleanup3
    {
        [Test]
        public void T1()
        {
            Global.LocalStorage.ResetStorage();
            Global.Uninitialize();
        }
    }
}

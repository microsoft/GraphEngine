using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Trinity;
using Trinity.Storage;
using Trinity.Network;

namespace cleanup1
{
    public class cleanup1
    {
        [Test]
        public void T1()
        {
            TrinityServer server = new TrinityServer();
            server.Start();
            Global.Uninitialize();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Trinity;
using Trinity.Storage;

namespace cleanup2
{
    public class cleanup2
    {
        [Test]
        public void StartServerTwice()
        {
            TrinityServer server = new TrinityServer();
            server.Start();

            Global.Uninitialize();

            server.Start();
            Global.Uninitialize();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Client.TestProtocols.Impl;
using Trinity.Network;

namespace Trinity.Client.TestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            TrinityServer server = new TrinityServer();
            server.RegisterCommunicationModule<TrinityClientModule.TrinityClientModule>();
            server.RegisterCommunicationModule<TrinityClientTestModule>();
            server.Start();
        }
    }
}

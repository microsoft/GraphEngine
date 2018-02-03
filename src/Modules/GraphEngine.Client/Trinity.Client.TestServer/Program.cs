using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Client.ServerSide;
using Trinity.Client.TestProtocols;
using Trinity.Client.TestProtocols.Impl;
using Trinity.Client.TestProtocols.TrinityClientTestModule;
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

            var cmod = server.GetCommunicationModule<TrinityClientModule.TrinityClientModule>();
            var tmod = server.GetCommunicationModule<TrinityClientTestModule>();
            int i = 0;
            while (true)
            {
                var client = cmod.Clients.FirstOrDefault();
                if(client != null)
                {
                    using (var msg = new P1RequestWriter("foo", i++))
                        client.P1(msg);
                }
                Thread.Sleep(1000);
            }
        }
    }
}

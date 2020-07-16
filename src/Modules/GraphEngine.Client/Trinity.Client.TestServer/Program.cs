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
            TrinityConfig.LoggingLevel = Diagnostics.LogLevel.Debug;
            TrinityServer server = new TrinityServer();
            server.RegisterCommunicationModule<TrinityClientModule.TrinityClientModule>();
            server.RegisterCommunicationModule<TrinityClientTestModule>();
            server.Start();

            Global.LocalStorage.SaveC1(0, "foo", 123);

            var cmod = server.GetCommunicationModule<TrinityClientModule.TrinityClientModule>();
            var tmod = server.GetCommunicationModule<TrinityClientTestModule>();
            int i = 0;
            while (true)
            {
                var client = cmod.Clients.FirstOrDefault();
                Console.WriteLine($"{cmod.Clients.Count()} clients");
                if (client != null)
                {
                    try
                    {
                        using (var msg = new S1Writer("foo", i++))
                            client.P1Async(msg).ContinueWith(t =>
                            {
                                using (var rsp = t.Result)
                                {
                                    Console.WriteLine($"Client responded: {rsp.foo}, {rsp.bar}");
                                }
                            }, TaskContinuationOptions.RunContinuationsAsynchronously);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        --i;
                    }
                }
                Thread.Sleep(1000);
            }
        }
    }
}

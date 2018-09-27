using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Client.TestProtocols;
using Trinity.Client.TestProtocols.Impl;

namespace Trinity.Client.TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            TrinityConfig.LoggingLevel = Diagnostics.LogLevel.Debug;
            TrinityClient client = new TrinityClient("localhost:5304");
            client.RegisterCommunicationModule<TrinityClientTestModule>();
            client.Start();

            while (true) Thread.Sleep(1000000);
        }
    }
}

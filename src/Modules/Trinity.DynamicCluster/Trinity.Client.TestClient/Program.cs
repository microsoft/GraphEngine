using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Client.TestProtocols.Impl;

namespace Trinity.Client.TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            TrinityClient client = new TrinityClient("localhost:5304");
            client.RegisterCommunicationModule<TrinityClientTestModule>();
            client.Start();
        }
    }
}

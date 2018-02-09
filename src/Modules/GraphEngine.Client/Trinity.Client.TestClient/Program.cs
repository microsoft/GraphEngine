using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trinity.Client.TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            TrinityClient client = new TrinityClient("localhost:5304");
            client.RegisterCommunicationModule<TrinityClientTestModule>();
            client.Start();

            while (true) Thread.Sleep(1000000);
        }
    }
}

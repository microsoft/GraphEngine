using System;
using Trinity.FFI.Python;
using Trinity.Network;

namespace Trinity.FFI.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(typeof(PythonRuntimeProvider).Name);
            TrinityServer server = new TrinityServer();
            server.Start();
        }
    }
}

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
            FFIConfig.Instance.ProgramDirectory = Environment.CurrentDirectory;
            Console.WriteLine(Environment.CurrentDirectory);
            TrinityServer server = new TrinityServer();
            server.Start();
        }
    }
}

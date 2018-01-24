using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;
using Trinity.ServiceFabric.Infrastructure;

namespace Trinity.ServiceFabric
{
    internal static class Program
    {
        private static void Main()
        {
            Console.WriteLine(Trinity.Azure.Storage.BlobStorageConfig.Instance.ConnectionString);
            ServiceRuntime.RegisterServiceAsync("Trinity.ServiceFabric.GraphEngineServiceType",
                context => new GraphEngineService(context)).GetAwaiter().GetResult();

            // Prevents this host process from terminating so services keep running.
            Thread.Sleep(Timeout.Infinite);
        }
    }
}

using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;
using Trinity.Network;
using Trinity.ServiceFabric.Stateless;
using Trinity.ServiceFabric.Storage.External;
using Microsoft.Azure;

namespace DemoTrinityStateless
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.

                ServiceRuntime.RegisterServiceAsync("DemoTrinityStatelessType",
                    context => new TrinityStatelessService(context, new DemoTrinityServer(), "TrinityServerEndpoint", CreateTrinityStorageImage))
                    .GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(TrinityStatelessService).Name);

                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }

        private static ITrinityStorageImage CreateTrinityStorageImage(TrinityStatelessService service)
        {
            var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            return new PartitionedImage(service, (slotIndex) => new AzureBlobPartitionedImageStorage(connectionString, "storage", $"slot-{slotIndex}"));
        }
    }
}

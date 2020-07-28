using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;
using Trinity;
using Trinity.Client;
using Trinity.Client.TestProtocols;
using Trinity.Client.TrinityClientModule;
using Trinity.Diagnostics;
using Trinity.Network;
using Trinity.ServiceFabric;

namespace SFCluster.Stateful.TripleStore.GraphDataClient
{
    internal static class Program
    {
        private static TripleModule tripleServerModuleImpl = null;

        private static TripleModule GraphEngineTripleModuleImpl { get; set; }

        //private static TripleStoreDemoServerModule GraphEngineTripleStoreDemoServerImpl { get; set; }
        private static TrinityClientModule TrinityClientModuleRuntime { get; set; }
        private static TrinityServer TripleStoreServer { get; set; }

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

                //ServiceRuntime.RegisterServiceAsync("SFCluster.Stateful.TripleStore.GraphDataClientType",
                //    context => new GraphDataClient(context)).GetAwaiter().GetResult();

                TrinityConfig.LogEchoOnConsole = false;
                TrinityConfig.LoggingLevel = LogLevel.Debug;
                //TrinityConfig.StorageRoot = @"C:\GE-TripleStore-Storage";

                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.

                GraphEngineService.StartServiceAsync("SFCluster.Stateful.TripleStore.GraphDataClientType").GetAwaiter()
                                  .GetResult();

                Log.WriteLine(nameof(TripleModule) + " SFCluster Client is Up, Ready and Active");

                var m_trinity = new TrinityClient("fabric:/GraphEngine.ServiceFabric.TripleStoreApplication/Stateful.TripleStore.GraphDataService");

                m_trinity.Start();

                //ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(GraphDataClient).Name);

                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                //ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}

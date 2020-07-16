using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FanoutSearch;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.ServiceFabric.Services.Runtime;
using Trinity;
using Trinity.Azure.Storage;
using Trinity.Client.TestProtocols;
using Trinity.Client.TestProtocols.TripleServer;
using Trinity.Client.TrinityClientModule;
using Trinity.Diagnostics;
using Trinity.Extension;
using Trinity.Network;
using Trinity.ServiceFabric;
using Trinity.ServiceFabric.Remoting;

namespace Stateful.TripleStore.GraphDataService
{
    // Workaround: extension assembly will be removed by the
    // compiler if it is not explicitly used in the code.
    [UseExtension(typeof(BlobStoragePersistentStorage))]
    [UseExtension(typeof(ITrinityOverRemotingService))]
    [UseExtension(typeof(FanoutSearchModule))]
    [UseExtension(typeof(TripleModule))]
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
        private static async Task Main()
        {
            try
            {
                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.

                GraphEngineService.StartServiceAsync("Stateful.TripleStore.GraphDataServiceType").GetAwaiter()
                    .GetResult();

                Log.WriteLine(nameof(TripleModule) + " is Up, Ready and Active");

                TrinityConfig.LogEchoOnConsole = false;
                TrinityConfig.LoggingLevel = LogLevel.Debug;
                TrinityConfig.StorageRoot = @"C:\GE-TripleStore-Storage";

                // Global.Communications runtime is ready

                TripleStoreServer = Global.CommunicationInstance as TrinityServer;

                //Log.LogsWritten += Log_LogsWritten;

                if (TripleStoreServer != null)
                {
                    TripleStoreServer.RegisterCommunicationModule<TrinityClientModule>();
                    TripleStoreServer.RegisterCommunicationModule<TripleModule>();

                    TrinityClientModuleRuntime = TripleStoreServer.GetCommunicationModule<TrinityClientModule>();
                }

                while (true)
                {
                    // Each time we pass through the look check to see how many active clients are connected

                    if (TrinityClientModuleRuntime.Clients != null)
                    {
                        var tripleStoreClients = TrinityClientModuleRuntime.Clients.ToList();

                        Log.WriteLine(
                            $"The number of real-time Connected TripleStore Client: {tripleStoreClients.Count}.");

                        foreach (var connectedTripleStoreClient in tripleStoreClients.Where(
                            connectedTripleStoreClient => connectedTripleStoreClient != null))
                        {
                            try
                            {
                                List<Triple> triples = new List<Triple>
                                    {new Triple {Subject = "GraphEngineServer", Predicate = "is", Object = "Running"}};

                                // New-up the Request Message!

                                using var message = new TripleStreamWriter(triples);

                                // Push a triple to the Client

                                using var rsp = await connectedTripleStoreClient.StreamTriplesAsync(message)
                                    .ConfigureAwait(false);

                                //var writeMessage = new TripleWriter()
                                //                   {
                                //                       Namespace = "http://www.inknowworks.com/semanticweb/graphengine.io",
                                //                       Subject   = "Dr. Barry Smith",
                                //                       Predicate = "BFO-Creator",
                                //                       Object    = "Ph.D Advisory"
                                //                   };

                                //using var rpcOp = await connectedTripleStoreClient
                                //                       .WriteTripleAsync(writeMessage).ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                Log.WriteLine(ex.ToString());
                            }
                        }
                    }

                    await Task.Delay(1000).ConfigureAwait(false);

                    // Prevents this host process from terminating so services keep running.
                    //Thread.Sleep(Timeout.Infinite);
                }

                //catch (Exception ex)
                //{
                //}
            }
            catch (Exception ex)
            {
            }
        }

        private static async void Log_LogsWritten(IList<LOG_ENTRY> trinityLogCollection)
        {
            const string logLevelFormat = "{0,-8}";

            using var trinityLog = Task.Factory.StartNew(async () =>
                {
                    await Task.Yield(); // Get off the caller's stack 

                    foreach (var logEntry in trinityLogCollection)
                    {
                        var formatLogLevel = string.Format(logLevelFormat, logEntry.logLevel);

                        Log.WriteLine($"TrinityServer LOG: {logEntry.logTime}, {formatLogLevel}, {logEntry.logMessage}");
                    }
                }, cancellationToken: CancellationToken.None,
                creationOptions: TaskCreationOptions.HideScheduler,
                scheduler: TaskScheduler.Current).Unwrap();

            var ioTask = trinityLog.ConfigureAwait(false);

            await ioTask;
        }
    }
}

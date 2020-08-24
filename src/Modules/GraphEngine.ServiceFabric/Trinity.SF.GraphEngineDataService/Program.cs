using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using FanoutSearch;
using Microsoft.ServiceFabric.Services.Runtime;
using Trinity.Azure.Storage;
using Trinity.Client;
using Trinity.Client.TestProtocols;
using Trinity.Client.TrinityClientModule;
using Trinity.Diagnostics;
using Trinity.Network;
using Trinity.ServiceFabric;
using Trinity.ServiceFabric.NativeClient.Remoting.Interfaces;
using Trinity.ServiceFabric.Remoting;

namespace Trinity.SF.GraphEngineDataService
{
    // Workaround: extension assembly will be removed by the
    // compiler if it is not explicitly used in the code.
    [UseExtension(typeof(BlobStoragePersistentStorage))]
    [UseExtension(typeof(ITrinityOverRemotingService))]
    //[UseExtension(typeof(ITrinityOverNativeTCPCommunicationService))]
    [UseExtension(typeof(FanoutSearchModule))]
    [UseExtension(typeof(TripleModule))]
    internal static class Program
    {
        private static TripleModule GraphEngineTripleServerAPI { get; set; }

        //private static TripleStoreDemoServerModule GraphEngineTripleStoreDemoServerImpl { get; set; }
        private static TrinityClientModule GraphEngineTripleClientAPI { get; set; }
        private static TrinityServer TripleStoreServer { get; set; }
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static async Task Main()
        {
            try
            {
                TrinityConfig.LogEchoOnConsole = false;
                TrinityConfig.LoggingLevel = LogLevel.Debug;

                //TrinityConfig.StorageRoot = @"C:\GE-TripleStore-Storage";

                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.

                GraphEngineService.StartServiceAsync("Trinity.SF.GraphEngineDataServiceType").GetAwaiter()
                    .GetResult();

                // Global.Communications runtime is ready

                TripleStoreServer = Global.CommunicationInstance as TrinityServer;

                if (TripleStoreServer != null)
                {
                    Log.WriteLine(nameof(TripleModule) + " is Up, Ready and Active");

                    TripleStoreServer.Started += TripleStoreServer_Started;
                    TripleStoreServer_Started();
                }

                //Console.ReadLine();

                await Task.Delay(0).ConfigureAwait(false);

                Thread.Sleep(Timeout.Infinite);

                //GraphEngineTripleStoreDemoServerImpl =
                //    TripleStoreServer.GetCommunicationModule<TripleStoreDemoServerModule>();

                //while (true)
                //{
                //    // Each time we pass through the look check to see how many active clients are connected

                //    using var processingLoopTask = Task.Factory.StartNew(async () =>
                //    {
                //        if (GraphEngineTripleClientAPI.Clients != null)
                //        {
                //            var tripleStoreClients = GraphEngineTripleClientAPI.Clients.ToList();

                //            Console.WriteLine($"The number of real-time Connected TripleStore Client: {tripleStoreClients.Count}.");

                //            foreach (var connectedTripleStoreClient in tripleStoreClients.Where(connectedTripleStoreClient => connectedTripleStoreClient != null))
                //            {
                //                try
                //                {
                //                    List<Triple> triples = new List<Triple> { new Triple { Subject = "GraphEngineServer", Predicate = "is", Object = "Running" } };

                //                    // New-up the Request Message!

                //                    using var message = new TripleStreamWriter(triples);

                //                    // Push a triple to the Client

                //                    await connectedTripleStoreClient.StreamTriplesAsync(message).ConfigureAwait(false);
                //                }
                //                catch (Exception ex)
                //                {
                //                    Log.WriteLine(ex.ToString());
                //                }
                //            }
                //        }

                //        await Task.Delay(2000).ConfigureAwait(false);

                //    }, cancellationToken: CancellationToken.None,
                //        creationOptions: TaskCreationOptions.HideScheduler,
                //        scheduler: TaskScheduler.Current).Unwrap();

                //    var mainLoopTask = processingLoopTask;

                //    await mainLoopTask;
                //}
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void TripleStoreServer_Started()
        {
            if (TripleStoreServer != null)
            {
                //Global.LocalStorage.LoadStorage();

                TripleStoreServer.RegisterCommunicationModule<TrinityClientModule>();
                TripleStoreServer.RegisterCommunicationModule<TripleModule>();

                GraphEngineTripleClientAPI = TripleStoreServer.GetCommunicationModule<TrinityClientModule>();

                // We inject an instance of the TripleModule class object so that we hook-up to our custom sever-side code

                GraphEngineTripleServerAPI = TripleStoreServer.GetCommunicationModule<TripleModule>();
            }

            if (GraphEngineTripleServerAPI != null)
            {
                Log.WriteLine("Setup Reactive Event Stream Processing!");

                GraphEngineTripleServerAPI.TriplePostedToServerReceivedAction
                    .ObserveOn(GraphEngineTripleServerAPI.ObserverOnNewThreadScheduler)
                    .Do(onNext: subscriberSource =>
                    {
                        var msg = "TriplePostedToServerReceivedAction-1";
                        Log.WriteLine("{0} Subscription happened on this Thread: {1}", msg, Thread.CurrentThread.ManagedThreadId);

                    })
                    .SubscribeOn(GraphEngineTripleServerAPI.SubscribeOnEventLoopScheduler)
                    .Subscribe(onNext: async tripleFromClient =>
                    {
                        using var graphEngineResponseTask = Task.Factory.StartNew(async () =>
                        {
                                //await Task.Yield();

                                await Task.Delay(0).ConfigureAwait(false);

                            Log.WriteLine(
                            $"Processing Timestamp: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
                            Log.WriteLine($"Triple Received from Client has been saved Received.");
                            Log.WriteLine($"Triple Subject Node: {tripleFromClient.Subject}");
                            Log.WriteLine($"Triple Predicate Node: {tripleFromClient.Predicate}");
                            Log.WriteLine($"Triple Object Node: {tripleFromClient.Object}");
                        }, cancellationToken: CancellationToken.None,
                            creationOptions: TaskCreationOptions.HideScheduler,
                            scheduler: TaskScheduler.Current).Unwrap().ContinueWith(async _ =>
                            {
                                await Task.Delay(0);

                                var msg = "TriplePostedToServerReceivedAction-1";
                                Log.WriteLine("{0} Subscription happened on this Thread: {1}", msg,
                                Thread.CurrentThread.ManagedThreadId);

                                Log.WriteLine("Task TriplePostedToServerReceivedAction Complete...");
                            }, cancellationToken: CancellationToken.None);

                        var writeToConsoleTask = graphEngineResponseTask;

                        await writeToConsoleTask;
                    });

                GraphEngineTripleServerAPI.ServerStreamedTripleSavedToMemoryCloudAction
                    .ObserveOn(GraphEngineTripleServerAPI.ObserverOnNewThreadScheduler)
                    .Do(onNext: subscriberSource =>
                    {
                        var msg = "ServerStreamedTripleSavedToMemoryCloudAction-1";
                        Log.WriteLine("{0} Subscription happened on this Thread: {1}", msg,
                            Thread.CurrentThread.ManagedThreadId);
                    })
                    .SubscribeOn(GraphEngineTripleServerAPI.SubscribeOnEventLoopScheduler)
                    .Subscribe(onNext: async savedTriple =>
                    {
                        using var graphEngineResponseTask = Task.Factory.StartNew(async () =>
                        {
                            //await Task.Yield();
                        
                            await Task.Delay(0).ConfigureAwait(false);

                            Log.WriteLine(
                            $"Processing Timestamp: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
                            Log.WriteLine($"Triple Streamed to Pushed Client has been saved to MemoryCloud.");
                            Log.WriteLine($"TripleStore CellID: {savedTriple.NewTripleStore.CellId}");
                            Log.WriteLine($"Triple Subject Node: {savedTriple.NewTripleStore.TripleCell.Subject}");
                            Log.WriteLine(
                            $"Triple Predicate Node: {savedTriple.NewTripleStore.TripleCell.Predicate}");
                        }, cancellationToken: CancellationToken.None,
                            creationOptions: TaskCreationOptions.HideScheduler,
                            scheduler: TaskScheduler.Current).Unwrap().ContinueWith(async _ =>
                            {
                                await Task.Delay(0);

                                Log.WriteLine("Task ServerStreamedTripleSavedToMemoryCloudAction Complete...");
                            }, cancellationToken: CancellationToken.None);

                        var writeToConsoleTask = graphEngineResponseTask;

                        await writeToConsoleTask;
                    });

                // ClientPostedTripleStoreReadyInMemoryCloudAction

                GraphEngineTripleServerAPI.ClientPostedTripleStoreReadyInMemoryCloudAction
                    .ObserveOn(GraphEngineTripleServerAPI.ObserverOnNewThreadScheduler)
                    .Do(onNext: subscriberSource =>
                    {
                        var msg = "ClientPostedTripleStoreReadyInMemoryCloudAction-1";
                        Log.WriteLine("{0} Subscription happened on this Thread: {1}", msg,
                            Thread.CurrentThread.ManagedThreadId);
                    })
                    .SubscribeOn(GraphEngineTripleServerAPI.SubscribeOnEventLoopScheduler)
                    .Synchronize()
                    .Subscribe(onNext: async tripleObject =>
                    {
                        using var getTripleByCellIdTask = Task.Factory.StartNew(async () =>
                        {
                            //var t = Global.CloudStorage.IsLocalCell(tripleObject.CellId);

                            // await Task.Yield();
                            await Task.Delay(0).ConfigureAwait(false);

                            using var getRequest = new TripleGetRequestWriter()
                            {
                                TripleCellId = tripleObject.CellId,
                                Subject = tripleObject.TripleCell.Subject,
                                Predicate = tripleObject.TripleCell.Predicate,
                                Namespace = tripleObject.TripleCell.Namespace,
                                Object = tripleObject.TripleCell.Object
                            };

                            await GraphEngineTripleServerAPI.GetTripleByCellId(0, getRequest).ConfigureAwait(false);
                        }, cancellationToken: CancellationToken.None,
                            creationOptions: TaskCreationOptions.HideScheduler,
                            scheduler: TaskScheduler.Current).Unwrap().ContinueWith(async _ =>
                            {
                                await Task.Delay(0);

                                Log.WriteLine("Task ClientPostedTripleStoreReadyInMemoryCloudAction Complete...");
                            }, cancellationToken: CancellationToken.None);

                        var writeToConsoleTask = getTripleByCellIdTask;

                        await writeToConsoleTask;
                    });

                // Reactive: ClientPostedTripleStoreReadyInMemoryCloudHotAction

                GraphEngineTripleServerAPI.ClientPostedTripleStoreReadyInMemoryCloudHotAction
                    .ObserveOn(GraphEngineTripleServerAPI.HotObservableSchedulerContext)
                    .Do(onNext: subscriberSource =>
                    {
                        var msg = "ClientPostedTripleStoreReadyInMemoryCloudHotAction-1";
                        Log.WriteLine("{0} Subscription happened on this Thread: {1}", msg,
                            Thread.CurrentThread.ManagedThreadId);
                    })
                    .SubscribeOn(GraphEngineTripleServerAPI.HotObservableSchedulerContext)
                    .Synchronize()
                    .Subscribe(onNext: async tripleObject =>
                    {
                        using var getTripleByCellIdTask = Task.Factory.StartNew(async () =>
                        {
                           // await Task.Yield();
                           await Task.Delay(0).ConfigureAwait(false);

                            using var getRequest = new TripleGetRequestWriter()
                            {
                                TripleCellId = tripleObject.CellId,
                                Subject = tripleObject.TripleCell.Subject,
                                Predicate = tripleObject.TripleCell.Predicate,
                                Namespace = tripleObject.TripleCell.Namespace,
                                Object = tripleObject.TripleCell.Object
                            };

                            await GraphEngineTripleServerAPI.GetTripleByCellId(0, getRequest).ConfigureAwait(false);

                        }, cancellationToken: CancellationToken.None,
                           creationOptions: TaskCreationOptions.HideScheduler,
                           scheduler: TaskScheduler.Current).Unwrap().ContinueWith(async _ =>
                           {
                               await Task.Delay(0);

                                Log.WriteLine("Task ClientPostedTripleStoreReadyInMemoryCloudAction Complete...");
                           }, cancellationToken: CancellationToken.None);

                        var writeToConsoleTask = getTripleByCellIdTask;

                        await writeToConsoleTask;
                    });

                // Let's get hooked-up to the host observable 

                GraphEngineTripleServerAPI.ClientPostedTripleStoreReadyInMemoryCloudHotAction.Connect();

                GraphEngineTripleServerAPI.TripleByCellIdReceivedAction
                    .ObserveOn(GraphEngineTripleServerAPI.ObserverOnNewThreadScheduler)
                    .Do(onNext: subscriberSource =>
                    {
                        var msg = "TripleByCellIdReceivedAction-1";
                        Log.WriteLine("{0} Subscription happened on this Thread: {1}", msg,
                            Thread.CurrentThread.ManagedThreadId);
                    })
                    .SubscribeOn(GraphEngineTripleServerAPI.SubscribeOnEventLoopScheduler)
                    .Synchronize()
                    .Subscribe(onNext: async tripleObjectFromGetRequest =>
                    {
                        using var getTripleBySubjectTask = Task.Factory.StartNew(async () =>
                        {
                            //await Task.Yield();

                            await Task.Delay(0).ConfigureAwait(false);

                            Log.WriteLine(
                            "Reactive Async - Parallel-Tasking return from Server-Side Get Request on behalf of the Client.");
                            Log.WriteLine(
                            $"Processing Timestamp: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
                            Log.WriteLine($"Triple Object CellID: {tripleObjectFromGetRequest.CellId}.");
                            Log.WriteLine(
                            $"Triple Subject Node: {tripleObjectFromGetRequest.TripleCell.Subject}");
                            Log.WriteLine(
                            $"Triple Predicate Node: {tripleObjectFromGetRequest.TripleCell.Predicate}");
                            Log.WriteLine(
                            $"Triple Object Node: {tripleObjectFromGetRequest.TripleCell.Object}.");
                        }, cancellationToken: CancellationToken.None,
                            creationOptions: TaskCreationOptions.HideScheduler,
                            scheduler: TaskScheduler.Current).Unwrap().ContinueWith(async _ =>
                            {
                                await Task.Delay(0);

                                Console.WriteLine("Task TripleByCellIdReceivedAction Complete...");
                            }, cancellationToken: CancellationToken.None);

                        var writeToConsoleTask = getTripleBySubjectTask;

                        await writeToConsoleTask;
                    });

                GraphEngineTripleServerAPI.ClientPostedTripleSavedToMemoryCloudAction
                    .ObserveOn(GraphEngineTripleServerAPI.ObserverOnNewThreadScheduler)
                    .Do(onNext: subscriberSource =>
                    {
                        var msg = "ClientPostedTripleSavedToMemoryCloudAction-1";
                        Log.WriteLine("{0} Subscription happened on this Thread: {1}", msg,
                            Thread.CurrentThread.ManagedThreadId);
                    })
                    .SubscribeOn(GraphEngineTripleServerAPI.SubscribeOnEventLoopScheduler)
                    .Synchronize()
                    .Subscribe(onNext: async tripleStoreMemoryContext =>
                    {
                        using var reactToTriplePostedSavedToMemoryCloudTask = Task.Factory.StartNew(async () =>
                        {
                             //await Task.Yield();

                            await Task.Delay(0).ConfigureAwait(false);

                            Log.WriteLine("Success! Found the Triple in the TripleStore MemoryCloud");

                            var tripleStore = tripleStoreMemoryContext.NewTripleStore;
                            var subjectNode = tripleStore.TripleCell.Subject;
                            var predicateNode = tripleStore.TripleCell.Predicate;
                            var objectNode = tripleStore.TripleCell.Object;

                            Log.WriteLine(
                            $"Triple CellId in MemoryCloud: {tripleStoreMemoryContext.NewTripleStore.CellId}");
                            Log.WriteLine($"Subject Node: {subjectNode}");
                            Log.WriteLine($"Predicate Node: {predicateNode}");
                            Log.WriteLine($"Object Node: {objectNode}");
                        }, cancellationToken: CancellationToken.None,
                            creationOptions: TaskCreationOptions.HideScheduler,
                            scheduler: TaskScheduler.Current).Unwrap().ContinueWith(async _ =>
                            {
                                await Task.Delay(0);

                                Console.WriteLine("Task ClientPostedTripleSavedToMemoryCloudAction Complete...");
                            }, cancellationToken: CancellationToken.None);

                        var storeFromMemoryCloudTask = reactToTriplePostedSavedToMemoryCloudTask;

                        await storeFromMemoryCloudTask;
                    });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trinityLogCollection"></param>
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

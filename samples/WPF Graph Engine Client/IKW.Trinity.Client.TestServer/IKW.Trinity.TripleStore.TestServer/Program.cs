using System.Globalization;
using System.Reactive.Linq;
using Trinity.Client.TrinityClientModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Trinity;
using Trinity.Client.TestProtocols;
using Trinity.Client.TestProtocols.TripleServer;
using Trinity.Diagnostics;
using Trinity.Network;

namespace Trinity.TripleStore.TestServer
{
    internal static class Program
    {
        private static TripleModule GraphEngineTripleServerAPI { get; set; }
        //private static TripleStoreDemoServerModule GraphEngineTripleStoreDemoServerImpl { get; set; }
        private static TrinityClientModule GraphEngineTripleClientAPI { get; set; }
        private static TrinityServer TripleStoreServer { get; set; }

        //private static ServerInfo TripleStoreServerInfo { get; } =
        //    new ServerInfo("GenNexusPrime.inknowworks.dev.net", 6808, @"U:\Trinity TripleStore Server Deployment", LogLevel.Debug);

        private static string TripleStoreStorageRoot { get; } = @"D:\\GraphEngine-Storage";
        private static AvailabilityGroup graphEngineCluster;

        private static async Task Main(string[] args)
        {
            TrinityConfig.LoadConfig(@"D:\Trinity TripleStore Server Deployment\trinity.xml");

            //graphEngineCluster = new AvailabilityGroup()
            //{
            //    Id = "rub-truespark-ge-cluster",
            //    Instances = new List<ServerInfo>()
            //    {
            //        new ServerInfo("GenNexusPrime.inknowworks.dev.net", 9888, string.Empty, LogLevel.Verbose),
            //        new ServerInfo("metagengraph-a.inknowworks.dev.net", 9888, string.Empty, LogLevel.Verbose),
            //        new ServerInfo("metagengraph-b.inknowworks.dev.net", 9888, string.Empty, LogLevel.Verbose)
            //        //new ServerInfo("truesparksf03.inknowworks.dev.net", 7808, string.Empty, LogLevel.Verbose),
            //        //new ServerInfo("truesparksf04.inknowworks.dev.net", 7808, string.Empty, LogLevel.Verbose),
            //        //new ServerInfo("truesparksf05.inknowworks.dev.net", 7808, string.Empty, LogLevel.Verbose)
            //    }
            //};

            TrinityConfig.LoggingLevel = LogLevel.Debug;
            //TrinityConfig.Servers.Add(graphEngineCluster);
            //TrinityConfig.CurrentRunningMode = RunningMode.Server;
            //TrinityConfig.AddServer(TripleStoreServerInfo);
            //TrinityConfig.LogEchoOnConsole = true;
            //TrinityConfig.StorageRoot = TripleStoreStorageRoot;

            //Log.LogsWritten += Log_LogsWritten;

            TripleStoreServer = new TrinityServer();

            var registeredModuleTypes = TripleStoreServer.GetRegisteredCommunicationModuleTypes();

            var moduleTypes = registeredModuleTypes as Type[] ?? registeredModuleTypes.ToArray();

            if (!moduleTypes.Contains(typeof(TrinityClientModule)))
            {
                TripleStoreServer.RegisterCommunicationModule<TrinityClientModule>();
            }

            if (!moduleTypes.Contains(typeof(TripleModule)))
            {
                TripleStoreServer.RegisterCommunicationModule<TripleModule>();
            }

            //TripleStoreServer.Started += TripleStoreServer_Started; 
            //TripleStoreServer.RegisterCommunicationModule<TripleStoreDemoServerModule>();

            TripleStoreServer.Start();

            //Global.CloudStorage.ResetStorage();
            //Global.LocalStorage.ResetStorage();

            //Global.LocalStorage.LoadStorage();

            GraphEngineTripleClientAPI  = TripleStoreServer.GetCommunicationModule<TrinityClientModule>();

            // We inject an instance of the TripleModule class object so that we hook-up to our custom sever-side code

            GraphEngineTripleServerAPI = TripleStoreServer.GetCommunicationModule<TripleModule>();

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

                            Log.WriteLine($"Processing Timestamp: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
                            Log.WriteLine($"Triple Received from Client has been saved Received.");
                            Log.WriteLine($"Triple Subject Node  : {tripleFromClient.Subject}");
                            Log.WriteLine($"Triple Predicate Node: {tripleFromClient.Predicate}");
                            Log.WriteLine($"Triple Object Node   : {tripleFromClient.Object}");
                        }, cancellationToken: CancellationToken.None,
                           creationOptions: TaskCreationOptions.HideScheduler,
                           scheduler: TaskScheduler.Current).Unwrap().ContinueWith(async _ =>
                        {
                            await Task.Delay(0);

                            Log.WriteLine("Task TriplePostedToServerReceivedAction Complete...");
                        }, cancellationToken: CancellationToken.None);

                        var writeToConsoleTask = graphEngineResponseTask;

                        await writeToConsoleTask;
                    });

                // Reactive Event: ServerStreamedTripleSavedToMemoryCloudAction
                // Description: Setup Reactive Subscription on Cold Observable. Logic is trigger/executed whenever
                // a client call RPC Method "PostTripleToServer."

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

                            Log.WriteLine($"Processing Timestamp: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
                            Log.WriteLine($"Triple Streamed to Pushed Client has been saved to MemoryCloud.");
                            Log.WriteLine($"TripleStore CellID   : {savedTriple.NewTripleStore.CellId}");
                            Log.WriteLine($"Triple Subject Node  : {savedTriple.NewTripleStore.TripleCell.Subject}");
                            Log.WriteLine($"Triple Predicate Node: {savedTriple.NewTripleStore.TripleCell.Predicate}");
                        }, cancellationToken: CancellationToken.None,
                        creationOptions: TaskCreationOptions.HideScheduler,
                        scheduler: TaskScheduler.Current).Unwrap().ContinueWith(async _ =>
                        {
                            await Task.Delay(0).ConfigureAwait(false);

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
                            // await Task.Yield();
                            await Task.Delay(0).ConfigureAwait(false);

                            var isLocalCell = Global.CloudStorage.IsLocalCell(tripleObject.CellId);

                            if (isLocalCell)
                            {
                                Log.WriteLine($"Found TripleStore Object: {tripleObject.CellId} in Local MemoryCloud!");

                                using var getRequest = new TripleGetRequestWriter()
                                {
                                    TripleCellId = tripleObject.CellId,
                                    Subject = tripleObject.TripleCell.Subject,
                                    Predicate = tripleObject.TripleCell.Predicate,
                                    Namespace = tripleObject.TripleCell.Namespace,
                                    Object = tripleObject.TripleCell.Object
                                };

                                Log.WriteLine($"Make Client-Side call from Server-side: GetTripleByCellId.");

                                await GraphEngineTripleServerAPI.GetTripleByCellId(0, getRequest).ConfigureAwait(false);
                            }
                        }, cancellationToken: CancellationToken.None,
                           creationOptions: TaskCreationOptions.HideScheduler,
                           scheduler: TaskScheduler.Current).Unwrap().ContinueWith(async _ =>
                        {
                            await Task.Delay(0).ConfigureAwait(false);

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

                                //using var getRequest = new TripleGetRequestWriter()
                                //{
                                //    TripleCellId = tripleObject.CellId,
                                //    Subject = tripleObject.TripleCell.Subject,
                                //    Predicate = tripleObject.TripleCell.Predicate,
                                //    Namespace = tripleObject.TripleCell.Namespace,
                                //    Object = tripleObject.TripleCell.Object
                                //};

                                //await GraphEngineTripleServerAPI.GetTripleByCellId(0, getRequest).ConfigureAwait(false);

                                var isLocalCell = Global.CloudStorage.IsLocalCell(tripleObject.CellId);

                                if (isLocalCell)
                                {
                                    Log.WriteLine($"Found TripleStore Object: {tripleObject.CellId} in Local MemoryCloud!");

                                    using var getRequest = new TripleGetRequestWriter()
                                    {
                                        TripleCellId = tripleObject.CellId,
                                        Subject = tripleObject.TripleCell.Subject,
                                        Predicate = tripleObject.TripleCell.Predicate,
                                        Namespace = tripleObject.TripleCell.Namespace,
                                        Object = tripleObject.TripleCell.Object
                                    };

                                    Log.WriteLine($"Make Client-Side call from Server-side: GetTripleByCellId.");

                                    await GraphEngineTripleServerAPI
                                          .GetTripleByCellId(0, getRequest).ConfigureAwait(false);
                                }

                            }, cancellationToken: CancellationToken.None,
                               creationOptions: TaskCreationOptions.HideScheduler,
                               scheduler: TaskScheduler.Current).Unwrap().ContinueWith(async _ =>
                        {
                            await Task.Delay(0).ConfigureAwait(false);

                            Log.WriteLine("Task ClientPostedTripleStoreReadyInMemoryCloudHotAction Complete...");
                        }, cancellationToken: CancellationToken.None);

                        var writeToConsoleTask = getTripleByCellIdTask;

                        await writeToConsoleTask;
                    });

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

                            Log.WriteLine("Reactive Async - Server-Side Get Request on behalf of the Client.");
                            Log.WriteLine($"Processing Timestamp: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
                            Log.WriteLine($"Triple Object CellID : {tripleObjectFromGetRequest.CellId}.");
                            Log.WriteLine($"Triple Subject Node  : {tripleObjectFromGetRequest.TripleCell.Subject}");
                            Log.WriteLine($"Triple Predicate Node: {tripleObjectFromGetRequest.TripleCell.Predicate}");
                            Log.WriteLine($"Triple Object Node   : {tripleObjectFromGetRequest.TripleCell.Object}.");

                        }, cancellationToken: CancellationToken.None,
                           creationOptions: TaskCreationOptions.HideScheduler,
                           scheduler: TaskScheduler.Current).Unwrap().ContinueWith(async _ =>
                        {
                            await Task.Delay(0).ConfigureAwait(false);

                            Log.WriteLine("Task TripleByCellIdReceivedAction Complete...");
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

                            var tripleStore   = tripleStoreMemoryContext.NewTripleStore;
                            var subjectNode   = tripleStore.TripleCell.Subject;
                            var predicateNode = tripleStore.TripleCell.Predicate;
                            var objectNode    = tripleStore.TripleCell.Object;

                            Log.WriteLine($"Triple CellId in MemoryCloud: {tripleStoreMemoryContext.NewTripleStore.CellId}");
                            Log.WriteLine($"Subject Node: {subjectNode}");
                            Log.WriteLine($"Predicate Node: {predicateNode}");
                            Log.WriteLine($"Object Node: {objectNode}");

                        }, cancellationToken: CancellationToken.None,
                           creationOptions: TaskCreationOptions.HideScheduler,
                           scheduler: TaskScheduler.Current).Unwrap().ContinueWith(async _ =>
                        {
                            await Task.Delay(0).ConfigureAwait(false);

                            Log.WriteLine("Task ClientPostedTripleSavedToMemoryCloudAction Complete...");
                        }, cancellationToken: CancellationToken.None);

                        var storeFromMemoryCloudTask = reactToTriplePostedSavedToMemoryCloudTask;

                        await storeFromMemoryCloudTask;
                    });
            }

            //Console.ReadLine();

            Log.WriteLine("Graph Engine Server is up, running and waiting to client connections");

            //Thread.Sleep(Timeout.Infinite);

            //GraphEngineTripleStoreDemoServerImpl =
            //    TripleStoreServer.GetCommunicationModule<TripleStoreDemoServerModule>();

            while (true)
            {
                // Each time we pass through the look check to see how many active clients are connected

                using var processingLoopTask = Task.Factory.StartNew(async () =>
                {
                    if (GraphEngineTripleClientAPI.Clients.Any())
                    {
                        var tripleStoreClients = GraphEngineTripleClientAPI.Clients.ToList();

                        Log.WriteLine($"The number of real-time Connected TripleStore Client: {tripleStoreClients.Count}.");

                        foreach (var connectedTripleStoreClient in tripleStoreClients.Where(connectedTripleStoreClient => connectedTripleStoreClient != null))
                        {
                            try
                            {
                                List<Triple> triples = new List<Triple> { new Triple { Subject = "GraphEngineServer", Predicate = "is", Object = "Running" } };

                                // New-up the Request Message!

                                using var message = new TripleStreamWriter(triples);

                                // Push a triple to the Client

                                await connectedTripleStoreClient.StreamTriplesAsync(message).ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                Log.WriteLine(ex.ToString());
                            }
                        }
                    }

                    await Task.Delay(1000).ConfigureAwait(false);

                }, cancellationToken: CancellationToken.None,
                   creationOptions: TaskCreationOptions.HideScheduler,
                   scheduler: TaskScheduler.Current).Unwrap();

                var mainLoopTask = processingLoopTask;

                await mainLoopTask;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void TripleStoreServer_Started()
        {
            Log.WriteLine($"{nameof(TripleStoreServer)} is update and running.");
        }

        /// <summary>
        /// This is how you intercept LOG I/O from the Trinity Communications Runtime
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


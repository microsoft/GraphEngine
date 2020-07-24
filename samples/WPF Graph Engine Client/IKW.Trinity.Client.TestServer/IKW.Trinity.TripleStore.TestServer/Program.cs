using System.Globalization;
using System.Reactive.Linq;

namespace Trinity.TripleStore.TestServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Client.TestProtocols;
    using Client.TestProtocols.TripleServer;
    using Client.TrinityClientModule;
    using Diagnostics;
    using Network;
    using Trinity;

    class Program
    {
        private static TripleModule GraphEngineTripleModuleImpl { get; set; }
        //private static TripleStoreDemoServerModule GraphEngineTripleStoreDemoServerImpl { get; set; }
        private static TrinityClientModule TrinityClientModuleRuntime { get; set; }
        private static TrinityServer TripleStoreServer { get; set; }

        private static ServerInfo TripleStoreServerInfo { get; } =
            new ServerInfo("localhost", 9898, String.Empty, LogLevel.Debug);

        private static string TripleStoreStorageRoot { get; } = @"C:\GE-TripleStore-Storage";

        private static async Task Main(string[] args)
        {
            TrinityConfig.CurrentRunningMode   = RunningMode.Server;
            TrinityConfig.AddServer(TripleStoreServerInfo);
            TrinityConfig.LogEchoOnConsole     = false;
            TrinityConfig.StorageRoot          = TripleStoreStorageRoot;

            Log.LogsWritten += Log_LogsWritten;

            TripleStoreServer = new TrinityServer();

            TripleStoreServer.RegisterCommunicationModule<TrinityClientModule>();
            TripleStoreServer.RegisterCommunicationModule<TripleModule>();

            //TripleStoreServer.RegisterCommunicationModule<TripleStoreDemoServerModule>();

            TripleStoreServer.Start();

            Global.CloudStorage.ResetStorage();
            Global.LocalStorage.ResetStorage();

            //Global.LocalStorage.LoadStorage();

            TrinityClientModuleRuntime  = TripleStoreServer.GetCommunicationModule<TrinityClientModule>();

            // We inject an instance of the TripleModule class object so that we hook-up to our custom sever-side code

            GraphEngineTripleModuleImpl = TripleStoreServer.GetCommunicationModule<TripleModule>();

            if (GraphEngineTripleModuleImpl != null)
            {
                Log.WriteLine("Setup Reactive Event Stream Processing!");

                GraphEngineTripleModuleImpl.TriplePostedToServerReceivedAction
                    .ObserveOn(GraphEngineTripleModuleImpl.ObserverOnNewThreadScheduler)
                    .Do(onNext: subscriberSource =>
                    {
                        var msg = "TriplePostedToServerReceivedAction-1";
                        Console.WriteLine("{0} Subscription happened on this Thread: {1}", msg, Thread.CurrentThread.ManagedThreadId);

                    })
                    .SubscribeOn(GraphEngineTripleModuleImpl.SubscribeOnEventLoopScheduler)
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

                            Console.WriteLine("Task TriplePostedToServerReceivedAction Complete...");
                        }, cancellationToken: CancellationToken.None);

                        var writeToConsoleTask = graphEngineResponseTask;

                        await writeToConsoleTask;
                    });

                // Reactive Event: ServerStreamedTripleSavedToMemoryCloudAction
                // Description: Setup Reactive Subscription on Cold Observable. Logic is trigger/executed whenever
                // a client call RPC Method "PostTripleToServer."

                GraphEngineTripleModuleImpl.ServerStreamedTripleSavedToMemoryCloudAction
                    .ObserveOn(GraphEngineTripleModuleImpl.ObserverOnNewThreadScheduler)
                    .Do(onNext: subscriberSource =>
                    {
                        var msg = "ServerStreamedTripleSavedToMemoryCloudAction-1";
                        Console.WriteLine("{0} Subscription happened on this Thread: {1}", msg,
                            Thread.CurrentThread.ManagedThreadId);
                    })
                    .SubscribeOn(GraphEngineTripleModuleImpl.SubscribeOnEventLoopScheduler)
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
                            await Task.Delay(0);

                            Console.WriteLine("Task ServerStreamedTripleSavedToMemoryCloudAction Complete...");
                        }, cancellationToken: CancellationToken.None);

                        var writeToConsoleTask = graphEngineResponseTask;

                        await writeToConsoleTask;
                    });

                // ClientPostedTripleStoreReadyInMemoryCloudAction

                GraphEngineTripleModuleImpl.ClientPostedTripleStoreReadyInMemoryCloudAction
                    .ObserveOn(GraphEngineTripleModuleImpl.ObserverOnNewThreadScheduler)
                    .Do(onNext: subscriberSource =>
                    {
                        var msg = "ClientPostedTripleStoreReadyInMemoryCloudAction-1";
                        Console.WriteLine("{0} Subscription happened on this Thread: {1}", msg,
                            Thread.CurrentThread.ManagedThreadId);
                    })
                    .SubscribeOn(GraphEngineTripleModuleImpl.SubscribeOnEventLoopScheduler)
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

                                await GraphEngineTripleModuleImpl.GetTripleByCellId(0, getRequest).ConfigureAwait(false);
                            }
                        }, cancellationToken: CancellationToken.None,
                           creationOptions: TaskCreationOptions.HideScheduler,
                           scheduler: TaskScheduler.Current).Unwrap().ContinueWith(async _ =>
                        {
                            await Task.Delay(0);

                            Console.WriteLine("Task ClientPostedTripleStoreReadyInMemoryCloudAction Complete...");
                        }, cancellationToken: CancellationToken.None);

                        var writeToConsoleTask = getTripleByCellIdTask;

                        await writeToConsoleTask;
                    });

                // Reactive: ClientPostedTripleStoreReadyInMemoryCloudHotAction

                GraphEngineTripleModuleImpl.ClientPostedTripleStoreReadyInMemoryCloudHotAction
                    .ObserveOn(GraphEngineTripleModuleImpl.HotObservableSchedulerContext)
                    .Do(onNext: subscriberSource =>
                    {
                        var msg = "ClientPostedTripleStoreReadyInMemoryCloudHotAction-1";
                        Console.WriteLine("{0} Subscription happened on this Thread: {1}", msg,
                            Thread.CurrentThread.ManagedThreadId);
                    })
                    .SubscribeOn(GraphEngineTripleModuleImpl.HotObservableSchedulerContext)
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

                                //await GraphEngineTripleModuleImpl.GetTripleByCellId(0, getRequest).ConfigureAwait(false);

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

                                    await GraphEngineTripleModuleImpl
                                          .GetTripleByCellId(0, getRequest).ConfigureAwait(false);
                                }

                            }, cancellationToken: CancellationToken.None,
                               creationOptions: TaskCreationOptions.HideScheduler,
                               scheduler: TaskScheduler.Current).Unwrap().ContinueWith(async _ =>
                        {
                            await Task.Delay(0);

                            Console.WriteLine("Task ClientPostedTripleStoreReadyInMemoryCloudHotAction Complete...");
                        }, cancellationToken: CancellationToken.None);

                        var writeToConsoleTask = getTripleByCellIdTask;

                        await writeToConsoleTask;
                    });

                GraphEngineTripleModuleImpl.ClientPostedTripleStoreReadyInMemoryCloudHotAction.Connect();

                GraphEngineTripleModuleImpl.TripleByCellIdReceivedAction
                    .ObserveOn(GraphEngineTripleModuleImpl.ObserverOnNewThreadScheduler)
                    .Do(onNext: subscriberSource =>
                    {
                        var msg = "TripleByCellIdReceivedAction-1";
                        Console.WriteLine("{0} Subscription happened on this Thread: {1}", msg,
                            Thread.CurrentThread.ManagedThreadId);
                    })
                    .SubscribeOn(GraphEngineTripleModuleImpl.SubscribeOnEventLoopScheduler)
                    .Synchronize()
                    .Subscribe(onNext: async tripleObjectFromGetRequest =>
                    {
                        using var getTripleBySubjectTask = Task.Factory.StartNew(async () =>
                        {
                            //await Task.Yield();

                            await Task.Delay(0).ConfigureAwait(false);


                            Console.WriteLine("Reactive Async - Server-Side Get Request on behalf of the Client.");
                            Console.WriteLine($"Processing Timestamp: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
                            Console.WriteLine($"Triple Object CellID : {tripleObjectFromGetRequest.CellId}.");
                            Console.WriteLine($"Triple Subject Node  : {tripleObjectFromGetRequest.TripleCell.Subject}");
                            Console.WriteLine($"Triple Predicate Node: {tripleObjectFromGetRequest.TripleCell.Predicate}");
                            Console.WriteLine($"Triple Object Node   : {tripleObjectFromGetRequest.TripleCell.Object}.");

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

                GraphEngineTripleModuleImpl.ClientPostedTripleSavedToMemoryCloudAction
                    .ObserveOn(GraphEngineTripleModuleImpl.ObserverOnNewThreadScheduler)
                    .Do(onNext: subscriberSource =>
                    {
                        var msg = "ClientPostedTripleSavedToMemoryCloudAction-1";
                        Console.WriteLine("{0} Subscription happened on this Thread: {1}", msg,
                            Thread.CurrentThread.ManagedThreadId);
                    })
                    .SubscribeOn(GraphEngineTripleModuleImpl.SubscribeOnEventLoopScheduler)
                    .Synchronize()
                    .Subscribe(onNext: async tripleStoreMemoryContext =>
                    {
                        using var reactToTriplePostedSavedToMemoryCloudTask = Task.Factory.StartNew(async () =>
                        {
                            //await Task.Yield();

                            await Task.Delay(0).ConfigureAwait(false);

                            Console.WriteLine("Success! Found the Triple in the TripleStore MemoryCloud");

                            var tripleStore = tripleStoreMemoryContext.NewTripleStore;
                            var subjectNode = tripleStore.TripleCell.Subject;
                            var predicateNode = tripleStore.TripleCell.Predicate;
                            var objectNode = tripleStore.TripleCell.Object;

                            Console.WriteLine(
                                $"Triple CellId in MemoryCloud: {tripleStoreMemoryContext.NewTripleStore.CellId}");
                            Console.WriteLine($"Subject Node: {subjectNode}");
                            Console.WriteLine($"Predicate Node: {predicateNode}");
                            Console.WriteLine($"Object Node: {objectNode}");
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

            Console.ReadLine();

            Thread.Sleep(Timeout.Infinite);

            //GraphEngineTripleStoreDemoServerImpl =
            //    TripleStoreServer.GetCommunicationModule<TripleStoreDemoServerModule>();

            while (true)
            {
                // Each time we pass through the look check to see how many active clients are connected

                using var processingLoopTask = Task.Factory.StartNew(async () =>
                {
                    if (TrinityClientModuleRuntime.Clients != null)
                    {
                        var tripleStoreClients = TrinityClientModuleRuntime.Clients.ToList();

                        Console.WriteLine($"The number of real-time Connected TripleStore Client: {tripleStoreClients.Count}.");

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

                    await Task.Delay(2000).ConfigureAwait(false);

                }, cancellationToken: CancellationToken.None,
                    creationOptions: TaskCreationOptions.HideScheduler,
                    scheduler: TaskScheduler.Current).Unwrap();

                var mainLoopTask = processingLoopTask;

                await mainLoopTask;
            }
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

                        Console.WriteLine($"TrinityServer LOG: {logEntry.logTime}, {formatLogLevel}, {logEntry.logMessage}");
                    }
                }, cancellationToken: CancellationToken.None,
                creationOptions: TaskCreationOptions.HideScheduler,
                scheduler: TaskScheduler.Current).Unwrap();

            var ioTask = trinityLog.ConfigureAwait(false);

            await ioTask;
        }
    }


}


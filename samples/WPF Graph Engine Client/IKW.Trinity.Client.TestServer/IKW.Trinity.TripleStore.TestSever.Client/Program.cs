using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Trinity.TripleStore.TestSever.Client
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Diagnostics;
    using Trinity;
    using Trinity.Client;
    using Trinity.Client.TestProtocols;
    using Trinity.Client.TestProtocols.TripleServer;

    class Program
    {
        private static TrinityClient TrinityTripleModuleClient = null;
        private static TripleModule  TripleClientSideModule { get; set; } = null;

        static async Task Main(string[] args)
        {
            TrinityConfig.CurrentRunningMode = RunningMode.Client;
            TrinityConfig.LogEchoOnConsole = false;
            TrinityConfig.LoggingLevel = LogLevel.Info;

            Log.LogsWritten += Log_LogsWritten;

            
            //TrinityTripleModuleClient = new TrinityClient("testcluster100.southcentralus.cloudapp.azure.com:19081/TrinityServiceFabric.NativeClusterRemoting.Application/Trinity.ServiceFabric.GraphEngine.GraphDataService");

            //TrinityTripleModuleClient = new TrinityClient("GenNexusPrime.inknowworks.dev.net:8898");

            TrinityTripleModuleClient = new TrinityClient("GenNexusPrime.inknowworks.dev.net:9898");

            TrinityTripleModuleClient.RegisterCommunicationModule<TripleModule>();
            TrinityTripleModuleClient.Start();

            TripleClientSideModule = TrinityTripleModuleClient.GetCommunicationModule<TripleModule>();

            TripleClientSideModule.TripleBySubjectReceivedAction
                .ObserveOn(TripleClientSideModule.ObserverOnNewThreadScheduler)
                .Do(onNext: subscriberSource =>
                {
                    var msg = "TripleBySubjectReceivedAction-1";
                    Console.WriteLine("{0} Subscription happened on this Thread: {1}", msg, Thread.CurrentThread.ManagedThreadId);

                })
                .SubscribeOn(TripleClientSideModule.SubscribeOnEventLoopScheduler)
                .Subscribe(onNext: async tripleObjectFromServer =>
                {
                    using var reactiveGraphEngineResponseTask = Task.Factory.StartNew(async () =>
                        {
                            //await Task.Yield();

                            await Task.Delay(0).ConfigureAwait(false);

                            Console.WriteLine("Reactive Async - Parallel-Tasking return from Server-Side Get Request on behalf of the Client.");
                            Console.WriteLine($"Processing Timestamp: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
                            Console.WriteLine($"Triple Object CellID: {tripleObjectFromServer.CellId}.");
                            Console.WriteLine($"Triple Subject Node: {tripleObjectFromServer.TripleCell.Subject}");
                            Console.WriteLine($"Triple Predicate Node: {tripleObjectFromServer.TripleCell.Predicate}");
                            Console.WriteLine($"Triple Object Node: {tripleObjectFromServer.TripleCell.Object}.");

                        }, cancellationToken: CancellationToken.None,
                        creationOptions: TaskCreationOptions.HideScheduler,
                        scheduler: TaskScheduler.Current).Unwrap().ContinueWith(async _ =>
                    {
                        await Task.Delay(0);

                        Console.WriteLine("Task TripleObjectStreamedFromServerReceivedAction Complete...");
                    }, cancellationToken: CancellationToken.None);

                    var writeToConsoleTask = reactiveGraphEngineResponseTask;

                    await writeToConsoleTask;
                });

            TripleClientSideModule.TripleObjectStreamedFromServerReceivedAction
                .ObserveOn(TripleClientSideModule.ObserverOnNewThreadScheduler)
                .Do(onNext: subscriberSource =>
                {
                    var msg = "TripleObjectStreamedFromServerReceivedAction-1";
                    Console.WriteLine("{0} Subscription happened on this Thread: {1}", msg, Thread.CurrentThread.ManagedThreadId);

                })
                .SubscribeOn(TripleClientSideModule.SubscribeOnEventLoopScheduler)
                .Subscribe(onNext: async tripleObjectFromServer =>
            {
                using var reactiveGraphEngineResponseTask = Task.Factory.StartNew(async () =>
                    {
                        //await Task.Yield();

                        await Task.Delay(0).ConfigureAwait(false);

                        Console.WriteLine("Incoming Triple Object Received from GE Server.");
                        Console.WriteLine($"Processing Timestamp: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
                        Console.WriteLine($"Triple Subject Node: {tripleObjectFromServer.Subject}");
                        Console.WriteLine($"Triple Predicate Node: {tripleObjectFromServer.Predicate}");
                        Console.WriteLine($"Triple Object Node: {tripleObjectFromServer.Object}");

                    }, cancellationToken: CancellationToken.None,
                    creationOptions: TaskCreationOptions.HideScheduler,
                    scheduler: TaskScheduler.Current).Unwrap().ContinueWith(async _ =>
                {
                    await Task.Delay(0);

                    Console.WriteLine("Task TripleObjectStreamedFromServerReceivedAction Complete...");
                }, cancellationToken: CancellationToken.None);

                var writeToConsoleTask = reactiveGraphEngineResponseTask;

                await writeToConsoleTask;
            });

            // ClientPostedTripleStoreReadyInMemoryCloudAction

            TripleClientSideModule.ClientPostedTripleStoreReadyInMemoryCloudAction
                .ObserveOn(TripleClientSideModule.ObserverOnNewThreadScheduler)
                .Do(onNext: subscriberSource =>
                {
                    var msg = "ClientPostedTripleStoreReadyInMemoryCloudAction-1";
                    Console.WriteLine("{0} Subscription happened on this Thread: {1}", msg, Thread.CurrentThread.ManagedThreadId);

                })
                .SubscribeOn(TripleClientSideModule.SubscribeOnEventLoopScheduler)
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
                                Subject      = tripleObject.TripleCell.Subject,
                                Predicate    = tripleObject.TripleCell.Predicate,
                                Namespace    = tripleObject.TripleCell.Namespace,
                                Object       = tripleObject.TripleCell.Object
                            };

                            using var rpcApiResponseCode =
                                await TrinityTripleModuleClient.GetTripleByCellId(getRequest);

                            Console.WriteLine($"RPC API Response Code: {rpcApiResponseCode.errno}");
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

            TripleClientSideModule.ClientPostedTripleStoreReadyInMemoryCloudHotAction
                .ObserveOn(TripleClientSideModule.HotObservableSchedulerContext)
                .Do(onNext: subscriberSource =>
                {
                    var msg = "ClientPostedTripleStoreReadyInMemoryCloudHotAction-1";
                    Console.WriteLine("{0} Subscription happened on this Thread: {1}", msg, Thread.CurrentThread.ManagedThreadId);

                })
                .SubscribeOn(TripleClientSideModule.HotObservableSchedulerContext)
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

                        using var rpcApiResponseCode =
                            await TrinityTripleModuleClient.GetTripleByCellId(getRequest);

                        Console.WriteLine($"RPC API Response Code: {rpcApiResponseCode.errno}");
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

            TripleClientSideModule.ClientPostedTripleStoreReadyInMemoryCloudHotAction.Connect();

            TripleClientSideModule.TripleByCellIdReceivedAction
                .ObserveOn(TripleClientSideModule.ObserverOnNewThreadScheduler)
                .Do(onNext: subscriberSource =>
                {
                    var msg = "TripleByCellIdReceivedAction-1";
                    Console.WriteLine("{0} Subscription happened on this Thread: {1}", msg, Thread.CurrentThread.ManagedThreadId);

                })
                .SubscribeOn(TripleClientSideModule.SubscribeOnEventLoopScheduler)
                .Synchronize()
                .Subscribe(onNext: async tripleObjectFromGetRequest =>
            {
                using var getTripleBySubjectTask = Task.Factory.StartNew(async () =>
                    {
                        //await Task.Yield();

                        await Task.Delay(0).ConfigureAwait(false);


                        Console.WriteLine("Reactive Async - Parallel-Tasking return from Server-Side Get Request on behalf of the Client.");
                        Console.WriteLine($"Processing Timestamp: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
                        Console.WriteLine($"Triple Object CellID: {tripleObjectFromGetRequest.CellId}.");
                        Console.WriteLine($"Triple Subject Node: {tripleObjectFromGetRequest.TripleCell.Subject}");
                        Console.WriteLine($"Triple Predicate Node: {tripleObjectFromGetRequest.TripleCell.Predicate}");
                        Console.WriteLine($"Triple Object Node: {tripleObjectFromGetRequest.TripleCell.Object}.");

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

            TripleClientSideModule.ClientPostedTripleSavedToMemoryCloudAction
                .ObserveOn(TripleClientSideModule.ObserverOnNewThreadScheduler)
                .Do(onNext: subscriberSource =>
                {
                    var msg = "ClientPostedTripleSavedToMemoryCloudAction-1";
                    Console.WriteLine("{0} Subscription happened on this Thread: {1}", msg, Thread.CurrentThread.ManagedThreadId);

                })
                .SubscribeOn(TripleClientSideModule.SubscribeOnEventLoopScheduler)
                .Synchronize()
                .Subscribe(onNext: async tripleStoreMemoryContext =>
            {
                using var reactToTriplePostedSavedToMemoryCloudTask = Task.Factory.StartNew(async () =>
                    {
                        //await Task.Yield();

                        await Task.Delay(0).ConfigureAwait(false);

                        Console.WriteLine("Success! Found the Triple in the TripleStore MemoryCloud");

                        var tripleStore         = tripleStoreMemoryContext.NewTripleStore;
                        var subjectNode   = tripleStore.TripleCell.Subject;
                        var predicateNode = tripleStore.TripleCell.Predicate;
                        var objectNode    = tripleStore.TripleCell.Object;

                        Console.WriteLine($"Triple CellId in MemoryCloud: {tripleStoreMemoryContext.NewTripleStore.CellId}");
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

            // Main Processing Loop!

            while (true)
            {
                try
                {
                    using var trinityClientProcessingLoopTask = Task.Factory.StartNew(async () =>
                        {
                            //await Task.Yield();

                            var sampleTriple = new List<Triple>
                                {new Triple {Subject = "Test-GraphEngineClient", Predicate = "is", Object = "Running"}};

                            using var tripleStreamWriter = new TripleStreamWriter(sampleTriple);
                            using var responseReader = await TrinityTripleModuleClient.PostTriplesToServer(tripleStreamWriter);
                            Console.WriteLine($"Server responses with rsp code={responseReader.errno}");
                        }, cancellationToken: CancellationToken.None,
                        creationOptions: TaskCreationOptions.HideScheduler,
                        scheduler: TaskScheduler.Current).Unwrap();

                    var mainLoopTask = trinityClientProcessingLoopTask;

                    await mainLoopTask;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                await Task.Delay(1000).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// This is how you intercept LOG I/O from the Trinity Communications Runtime
        /// </summary>
        /// <param name="trinityLogCollection"></param>
        private static async void Log_LogsWritten(IList<LOG_ENTRY> trinityLogCollection)
        {
            const string logLevelFormat = "{0,-8}";

            using var trinityLogIOTask = Task.Factory.StartNew(async () =>
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

            var taskAwaitable = trinityLogIOTask;

            await taskAwaitable;
        }
    }
}

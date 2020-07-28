using System;
using System.Collections.Generic;
using System.Fabric;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Trinity;
using Trinity.Client;
using Trinity.Client.TestProtocols;
using Trinity.Client.TestProtocols.TripleServer;
using Trinity.Diagnostics;

namespace SFCluster.GraphEngineClient
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class GraphEngineClient : StatelessService
    {
        private static TrinityClient TrinityTripleModuleClient = null;
        private static TripleModule TripleClientSideModule { get; set; } = null;

        public GraphEngineClient(StatelessServiceContext context)
            : base(context)
        {
            TrinityConfig.LoadConfig();

            TrinityConfig.CurrentRunningMode = RunningMode.Client;
            TrinityConfig.LogEchoOnConsole = false;
            TrinityConfig.LoggingLevel = LogLevel.Debug;

            TrinityConfig.SaveConfig();

            Log.LogsWritten += Log_LogsWritten;


            TrinityTripleModuleClient = new TrinityClient("GenNexusPrime.inknowworks.dev.net:8800");
            //TrinityTripleModuleClient = new TrinityClient("fabric:/GraphEngine.ServiceFabric.TripleStoreApplication/Stateful.TripleStore.GraphDataService");

            TrinityTripleModuleClient.UnhandledException += TrinityTripleModuleClient_UnhandledException1;
            TrinityTripleModuleClient.Started += TrinityTripleModuleClientStarted;

            TrinityTripleModuleClient.Start();
        }

        private void TrinityTripleModuleClient_UnhandledException1(object sender, Trinity.Network.Messaging.MessagingUnhandledExceptionEventArgs e)
        {
            Log.WriteLine($"Yikes! An unexpected Trinity Networking Error has been detected: {e.ToString()}");
        }

        private void TrinityTripleModuleClientStarted()
        {
            Log.WriteLine("Setup Reactive Event Stream Processing!");

            TripleClientSideModule = TrinityTripleModuleClient.GetCommunicationModule<TripleModule>();

            TripleClientSideModule.TripleBySubjectReceivedAction
                .ObserveOn(TripleClientSideModule.ObserverOnNewThreadScheduler)
                .Do(onNext: subscriberSource =>
                {
                    var msg = "TripleBySubjectReceivedAction-1";
                    Console.WriteLine("{0} Subscription happened on this Thread: {1}", msg,
                        Thread.CurrentThread.ManagedThreadId);
                })
                .SubscribeOn(TripleClientSideModule.SubscribeOnEventLoopScheduler)
                .Subscribe(onNext: async tripleStore =>
                {
                    using var reactiveGetTripleBySubjectTask = Task.Factory.StartNew(async () =>
                    {
                        //await Task.Yield();

                        await Task.Delay(0).ConfigureAwait(false);

                        Console.WriteLine(
                                "Reactive Async - Parallel-Tasking return from Server-Side Get Request on behalf of the Client.");
                        Console.WriteLine(
                                $"Processing Timestamp: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
                        Console.WriteLine($"Triple Object CellID: {tripleStore.CellId}.");
                        Console.WriteLine($"Triple Subject Node: {tripleStore.TripleCell.Subject}");
                        Console.WriteLine(
                                $"Triple Predicate Node: {tripleStore.TripleCell.Predicate}");
                        Console.WriteLine($"Triple Object Node: {tripleStore.TripleCell.Object}.");

                        var getTripleByCellRequestWriter = new TripleGetRequestWriter()
                        {
                            TripleCellId = tripleStore.CellId,
                            Subject = tripleStore.TripleCell.Subject,
                            Predicate = tripleStore.TripleCell.Predicate,
                            Object = tripleStore.TripleCell.Object,
                            Namespace = tripleStore.TripleCell.Namespace
                        };

                        await TrinityTripleModuleClient.GetTripleSubject(getTripleByCellRequestWriter);

                    }, cancellationToken: CancellationToken.None,
                        creationOptions: TaskCreationOptions.HideScheduler,
                        scheduler: TaskScheduler.Current).Unwrap().ContinueWith(async _ =>
                        {
                            await Task.Delay(0);

                            Console.WriteLine("Task TripleObjectStreamedFromServerReceivedAction Complete...");
                        }, cancellationToken: CancellationToken.None);

                    var writeToConsoleTask = reactiveGetTripleBySubjectTask;

                    await writeToConsoleTask;
                });

            // TripleObjectStreamedFromServerReceivedAction

            TripleClientSideModule.TripleObjectStreamedFromServerReceivedAction
                .ObserveOn(TripleClientSideModule.ObserverOnNewThreadScheduler)
                .Do(onNext: subscriberSource =>
                {
                    var msg = "TripleObjectStreamedFromServerReceivedAction-1";
                    Console.WriteLine("{0} Subscription happened on this Thread: {1}", msg,
                        Thread.CurrentThread.ManagedThreadId);
                })
                .SubscribeOn(TripleClientSideModule.SubscribeOnEventLoopScheduler)
                .Subscribe(onNext: async tripleObjectFromServer =>
                {
                    using var reactiveGraphEngineResponseTask = Task.Factory.StartNew(async () =>
                    {
                        //await Task.Yield();

                        await Task.Delay(0).ConfigureAwait(false);

                        Console.WriteLine("Incoming Triple Object Received from GE Server.");
                        Console.WriteLine(
                                $"Processing Timestamp: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
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

            // ServerStreamedTripleSavedToMemoryCloudAction

            TripleClientSideModule.ServerStreamedTripleSavedToMemoryCloudAction
                .ObserveOn(TripleClientSideModule.ObserverOnNewThreadScheduler)
                .Do(onNext: subscriberSource =>
                {
                    var msg = "ServerStreamedTripleSavedToMemoryCloudAction-1";
                    Console.WriteLine("{0} Subscription happened on this Thread: {1}", msg,
                        Thread.CurrentThread.ManagedThreadId);
                })
                .SubscribeOn(TripleClientSideModule.SubscribeOnEventLoopScheduler)
                .Subscribe(onNext: async tripleObjectFromMC =>
                {
                    using var reactiveGraphEngineResponseTask = Task.Factory.StartNew(async () =>
                    {
                        //await Task.Yield();

                        await Task.Delay(0).ConfigureAwait(false);

                        var myTripleStore = tripleObjectFromMC.NewTripleStore;

                        Console.WriteLine("Incoming TripleStore Object retrieved from MemoryCloud.");
                        Console.WriteLine(
                                $"Processing Timestamp: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
                        Console.WriteLine($"Triple Object CellID: {myTripleStore.CellId}");
                        Console.WriteLine($"Triple Subject Node: {myTripleStore.TripleCell.Subject}");
                        Console.WriteLine($"Triple Predicate Node: {myTripleStore.TripleCell.Predicate}");
                        Console.WriteLine($"Triple Object Node: {myTripleStore.TripleCell.Object}");
                    }, cancellationToken: CancellationToken.None,
                        creationOptions: TaskCreationOptions.HideScheduler,
                        scheduler: TaskScheduler.Current).Unwrap().ContinueWith(async _ =>
                        {
                            await Task.Delay(0);

                            Console.WriteLine("Task ServerStreamedTripleSavedToMemoryCloudAction Complete...");
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
                    Console.WriteLine("{0} Subscription happened on this Thread: {1}", msg,
                        Thread.CurrentThread.ManagedThreadId);
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
                            Subject = tripleObject.TripleCell.Subject,
                            Predicate = tripleObject.TripleCell.Predicate,
                            Namespace = tripleObject.TripleCell.Namespace,
                            Object = tripleObject.TripleCell.Object
                        };

                        await TrinityTripleModuleClient.GetTripleByCellId(getRequest);
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
                    Console.WriteLine("{0} Subscription happened on this Thread: {1}", msg,
                        Thread.CurrentThread.ManagedThreadId);
                })
                .SubscribeOn(TripleClientSideModule.HotObservableSchedulerContext)
                .Synchronize()
                .Subscribe(onNext: async tripleObject =>
                {
                    using var getTripleByCellIdTask = Task.Factory.StartNew(async () =>
                    {
                        // await Task.Yield();
                        await Task.Delay(0).ConfigureAwait(false);

                        Console.WriteLine("Reactive Event Stream Processing from Hot Observable");

                        using var getRequest = new TripleGetRequestWriter()
                        {
                            TripleCellId = tripleObject.CellId,
                            Subject = tripleObject.TripleCell.Subject,
                            Predicate = tripleObject.TripleCell.Predicate,
                            Namespace = tripleObject.TripleCell.Namespace,
                            Object = tripleObject.TripleCell.Object
                        };

                        await TrinityTripleModuleClient.GetTripleByCellId(getRequest);

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

            TripleClientSideModule.ClientPostedTripleStoreReadyInMemoryCloudHotAction.Connect();

            TripleClientSideModule.TripleByCellIdReceivedAction
                .ObserveOn(TripleClientSideModule.ObserverOnNewThreadScheduler)
                .Do(onNext: subscriberSource =>
                {
                    var msg = "TripleByCellIdReceivedAction-1";
                    Console.WriteLine("{0} Subscription happened on this Thread: {1}", msg,
                        Thread.CurrentThread.ManagedThreadId);
                })
                .SubscribeOn(TripleClientSideModule.SubscribeOnEventLoopScheduler)
                .Synchronize()
                .Subscribe(onNext: async tripleObjectFromGetRequest =>
                {
                    using var getTripleBySubjectTask = Task.Factory.StartNew(async () =>
                    {
                        //await Task.Yield();

                        await Task.Delay(0).ConfigureAwait(false);

                        Console.WriteLine(
                                "Reactive Async - Server-Side Get Request on behalf of the Client.");
                        Console.WriteLine(
                                $"Processing Timestamp: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
                        Console.WriteLine($"Triple Object CellID: {tripleObjectFromGetRequest.CellId}.");
                        Console.WriteLine(
                                $"Triple Subject Node: {tripleObjectFromGetRequest.TripleCell.Subject}");
                        Console.WriteLine(
                                $"Triple Predicate Node: {tripleObjectFromGetRequest.TripleCell.Predicate}");
                        Console.WriteLine(
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

            TripleClientSideModule.ClientPostedTripleSavedToMemoryCloudAction
                .ObserveOn(TripleClientSideModule.ObserverOnNewThreadScheduler)
                .Do(onNext: subscriberSource =>
                {
                    var msg = "ClientPostedTripleSavedToMemoryCloudAction-1";
                    Console.WriteLine("{0} Subscription happened on this Thread: {1}", msg,
                        Thread.CurrentThread.ManagedThreadId);
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

                        var tripleStore = tripleStoreMemoryContext.NewTripleStore;
                        var subjectNode = tripleStore.TripleCell.Subject;
                        var predicateNode = tripleStore.TripleCell.Predicate;
                        var objectNode = tripleStore.TripleCell.Object;

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
                       var formatLogLevel =
                           string.Format(logLevelFormat,
                               logEntry.logLevel);

                       Console.WriteLine(
                           $"TrinityClient LOG: {logEntry.logTime}, {formatLogLevel}, {logEntry.logMessage}");
                   }
               }, cancellationToken: CancellationToken.None,
                creationOptions: TaskCreationOptions.HideScheduler,
                scheduler: TaskScheduler.Current).Unwrap();

            var taskAwaitable = trinityLogIOTask;

            await taskAwaitable;
        }


        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(1000, cancellationToken).ConfigureAwait(false);

            using var trinityClientProcessingLoopTask = Task.Factory.StartNew(async () =>
              {
                  while (true)
                  {
                      try
                      {
                          //await Task.Yield();

                          var sampleTriple = new List<Triple>
                              {new Triple {Subject = "Test-GraphEngineClient", Predicate = "is", Object = "Running"}};

                          using var tripleStreamWriter = new TripleStreamWriter(sampleTriple);

                          await TrinityTripleModuleClient.PostTriplesToServer(tripleStreamWriter).ConfigureAwait(false);

                      }
                      catch (Exception ex)
                      {
                          Console.WriteLine(ex.ToString());
                      }

                      await Task.Delay(3000, cancellationToken).ConfigureAwait(false);

                  }

                  return;

              }, cancellationToken: CancellationToken.None,
                creationOptions: TaskCreationOptions.HideScheduler,
                scheduler: TaskScheduler.Current).Unwrap();

            var mainLoopTask = trinityClientProcessingLoopTask;

        }
    }
}

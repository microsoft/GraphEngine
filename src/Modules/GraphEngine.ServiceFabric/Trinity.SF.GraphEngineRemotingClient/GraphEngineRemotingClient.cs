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
using Trinity.Client;
using Trinity.Client.TestProtocols;
using Trinity.Client.TestProtocols.TripleServer;
using Trinity.Diagnostics;
using Trinity.Network.Messaging;

namespace Trinity.SF.GraphEngineRemotingClient
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class GraphEngineRemotingClient : StatelessService
    {
        private readonly TrinityClient m_trinity;
        private static TrinityClient TrinityTripleModuleClient = null;
        private static TripleModule TripleClientSideModule { get; set; } = null;

        public GraphEngineRemotingClient(StatelessServiceContext context)
            : base(context)
        {
            TrinityConfig.LoadConfig();

            TrinityConfig.CurrentRunningMode = RunningMode.Client;
            TrinityConfig.LogEchoOnConsole = false;
            TrinityConfig.LoggingLevel = LogLevel.Verbose;

            TrinityConfig.SaveConfig();

            Log.LogsWritten += Log_LogsWritten;

            TrinityTripleModuleClient = new TrinityClient("fabric:/Trinity.ServiceFabric.TestHarnes.Application/Trinity.SF.GraphEngineDataService");

            TrinityTripleModuleClient.UnhandledException += TrinityTripleModuleClient_UnhandledException;
            TrinityTripleModuleClient.Started += TrinityTripleModuleClientStarted;

            TrinityTripleModuleClient.RegisterCommunicationModule<TripleModule>();

            TrinityTripleModuleClient.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        private void TrinityTripleModuleClientStarted()
        {
            Log.WriteLine("Setup Reactive Event Stream Processing!");

            TripleClientSideModule = TrinityTripleModuleClient.GetCommunicationModule<TripleModule>();

            TripleClientSideModule.TripleBySubjectReceivedAction
                .ObserveOn(TripleClientSideModule.ObserverOnNewThreadScheduler)
                .Do(onNext: subscriberSource =>
                {
                    var msg = "TripleBySubjectReceivedAction-1";
                    Log.WriteLine("{0} Subscription happened on this Thread: {1}", msg,
                        Thread.CurrentThread.ManagedThreadId);
                })
                .SubscribeOn(TripleClientSideModule.SubscribeOnEventLoopScheduler)
                .Subscribe(onNext: async tripleStore =>
                {
                    using var reactiveGetTripleBySubjectTask = Task.Factory.StartNew(async () =>
                    {
                            //await Task.Yield();

                            await Task.Delay(0).ConfigureAwait(false);

                        Log.WriteLine(
                                "Reactive Async - Parallel-Tasking return from Server-Side Get Request on behalf of the Client.");
                        Log.WriteLine(
                                $"Processing Timestamp: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
                        Log.WriteLine($"Triple Object CellID: {tripleStore.CellId}.");
                        Log.WriteLine($"Triple Subject Node: {tripleStore.TripleCell.Subject}");
                        Log.WriteLine(
                                $"Triple Predicate Node: {tripleStore.TripleCell.Predicate}");
                        Log.WriteLine($"Triple Object Node: {tripleStore.TripleCell.Object}.");

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

                            Log.WriteLine("Task TripleObjectStreamedFromServerReceivedAction Complete...");
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
                    Log.WriteLine("{0} Subscription happened on this Thread: {1}", msg,
                        Thread.CurrentThread.ManagedThreadId);
                })
                .SubscribeOn(TripleClientSideModule.SubscribeOnEventLoopScheduler)
                .Subscribe(onNext: async tripleObjectFromServer =>
                {
                    using var reactiveGraphEngineResponseTask = Task.Factory.StartNew(async () =>
                    {
                            //await Task.Yield();

                            await Task.Delay(0).ConfigureAwait(false);

                        Log.WriteLine("Incoming Triple Object Received from GE Server.");
                        Log.WriteLine(
                                $"Processing Timestamp: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
                        Log.WriteLine($"Triple Subject Node: {tripleObjectFromServer.Subject}");
                        Log.WriteLine($"Triple Predicate Node: {tripleObjectFromServer.Predicate}");
                        Log.WriteLine($"Triple Object Node: {tripleObjectFromServer.Object}");
                    }, cancellationToken: CancellationToken.None,
                        creationOptions: TaskCreationOptions.HideScheduler,
                        scheduler: TaskScheduler.Current).Unwrap().ContinueWith(async _ =>
                        {
                            await Task.Delay(0);

                            Log.WriteLine("Task TripleObjectStreamedFromServerReceivedAction Complete...");
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
                    Log.WriteLine("{0} Subscription happened on this Thread: {1}", msg,
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

                        Log.WriteLine("Incoming TripleStore Object retrieved from MemoryCloud.");
                        Log.WriteLine(
                                $"Processing Timestamp: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
                        Log.WriteLine($"Triple Object CellID: {myTripleStore.CellId}");
                        Log.WriteLine($"Triple Subject Node: {myTripleStore.TripleCell.Subject}");
                        Log.WriteLine($"Triple Predicate Node: {myTripleStore.TripleCell.Predicate}");
                        Log.WriteLine($"Triple Object Node: {myTripleStore.TripleCell.Object}");
                    }, cancellationToken: CancellationToken.None,
                        creationOptions: TaskCreationOptions.HideScheduler,
                        scheduler: TaskScheduler.Current).Unwrap().ContinueWith(async _ =>
                        {
                            await Task.Delay(0);

                            Log.WriteLine("Task ServerStreamedTripleSavedToMemoryCloudAction Complete...");
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
                    Log.WriteLine("{0} Subscription happened on this Thread: {1}", msg,
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

                            Log.WriteLine("Task ClientPostedTripleStoreReadyInMemoryCloudAction Complete...");
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
                    Log.WriteLine("{0} Subscription happened on this Thread: {1}", msg,
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

                        Log.WriteLine("Reactive Event Stream Processing from Hot Observable");

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

                            Log.WriteLine("Task ClientPostedTripleStoreReadyInMemoryCloudHotAction Complete...");
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
                    Log.WriteLine("{0} Subscription happened on this Thread: {1}", msg,
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

                        Log.WriteLine(
                                "Reactive Async - Server-Side Get Request on behalf of the Client.");
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

                            Log.WriteLine("Task TripleByCellIdReceivedAction Complete...");
                        }, cancellationToken: CancellationToken.None);

                    var writeToConsoleTask = getTripleBySubjectTask;

                    await writeToConsoleTask;
                });

            TripleClientSideModule.ClientPostedTripleSavedToMemoryCloudAction
                .ObserveOn(TripleClientSideModule.ObserverOnNewThreadScheduler)
                .Do(onNext: subscriberSource =>
                {
                    var msg = "ClientPostedTripleSavedToMemoryCloudAction-1";
                    Log.WriteLine("{0} Subscription happened on this Thread: {1}", msg,
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

                        Log.WriteLine("Success! Found the Triple in the TripleStore MemoryCloud");

                        var tripleStore = tripleStoreMemoryContext.NewTripleStore;
                        var subjectNode = tripleStore.TripleCell.Subject;
                        var predicateNode = tripleStore.TripleCell.Predicate;
                        var objectNode = tripleStore.TripleCell.Object;

                        Log.WriteLine($"Triple CellId in MemoryCloud: {tripleStoreMemoryContext.NewTripleStore.CellId}");
                        Log.WriteLine($"Subject Node: {subjectNode}");
                        Log.WriteLine($"Predicate Node: {predicateNode}");
                        Log.WriteLine($"Object Node: {objectNode}");

                    }, cancellationToken: CancellationToken.None,
                           creationOptions: TaskCreationOptions.HideScheduler,
                           scheduler: TaskScheduler.Current).Unwrap().ContinueWith(async _ =>
                           {
                               await Task.Delay(0);

                               Log.WriteLine("Task ClientPostedTripleSavedToMemoryCloudAction Complete...");
                           }, cancellationToken: CancellationToken.None);

                    var storeFromMemoryCloudTask = reactToTriplePostedSavedToMemoryCloudTask;

                    await storeFromMemoryCloudTask;
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void TrinityTripleModuleClient_UnhandledException(object sender, Network.Messaging.MessagingUnhandledExceptionEventArgs e)
        {
            Log.WriteLine($"Yikes! An unexpected Trinity Networking Error has been detected: {e.ToString()}");
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
                    await Task.Delay(0).ConfigureAwait(false);

                    foreach (var logEntry in trinityLogCollection)
                    {
                        var formatLogLevel = string.Format(logLevelFormat, logEntry.logLevel);

                        Log.WriteLine($"TrinityClient LOG: {logEntry.logTime}, {formatLogLevel}, {logEntry.logMessage}");
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
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var sampleTriple = new List<Triple>
                {new Triple {Subject = "Test-GraphEngineClient", Predicate = "is", Object = "Running"}};

            using var tripleStreamWriter = new TripleStreamWriter(sampleTriple);

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (TrinityTripleModuleClient.CloudStorage != null)
                {
                    ServiceEventSource.Current.ServiceMessage(this.Context, "RPC Call PostTriplesToServer.");

                    TrinityTripleModuleClient?.PostTriplesToServer(tripleStreamWriter);
                }
                else
                {
                    Log.WriteLine($"Waiting for TrinityClient to Connect to NativeTrinityListener");
                }

                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            }
        }
    }
}

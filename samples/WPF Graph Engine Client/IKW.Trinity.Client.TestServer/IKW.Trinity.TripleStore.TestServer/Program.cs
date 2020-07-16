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

        private static async Task Main(string[] args)
        {
            TrinityConfig.CurrentRunningMode = RunningMode.Server;
            TrinityConfig.AddServer(new ServerInfo("GenNexusPrime.inknowworks.dev.net", 9898, string.Empty, LogLevel.Info));
            TrinityConfig.LogEchoOnConsole   = false;
            TrinityConfig.LoggingLevel       = LogLevel.Info;
            TrinityConfig.StorageRoot        = @"C:\GE-TripleStore-Storage";

            Log.LogsWritten += Log_LogsWritten;

            TripleStoreServer = new TrinityServer();

            TripleStoreServer.RegisterCommunicationModule<TrinityClientModule>();
            TripleStoreServer.RegisterCommunicationModule<TripleModule>();
            //TripleStoreServer.RegisterCommunicationModule<TripleStoreDemoServerModule>();

            TripleStoreServer.Start();

            TrinityClientModuleRuntime  = TripleStoreServer.GetCommunicationModule<TrinityClientModule>();

            // We inject an instance of the TripleModule class object so that we hook-up to our custom sever-side code

            GraphEngineTripleModuleImpl = TripleStoreServer.GetCommunicationModule<TripleModule>();

            GraphEngineTripleModuleImpl.TriplePostedToServerReceivedAction
                //.ObserveOn(GraphEngineTripleModuleImpl.ObserverOnNewThreadScheduler)
                //.Do(onNext: subscriberSource =>
                //{
                //    var msg = "TriplePostedToServerReceivedAction-1";
                //    Console.WriteLine("{0} Subscription happened on this Thread: {1}", msg, Thread.CurrentThread.ManagedThreadId);

                //})
                //.SubscribeOn(GraphEngineTripleModuleImpl.SubscribeOnEventLoopScheduler)
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
                    Console.WriteLine("{0} Subscription happened on this Thread: {1}", msg, Thread.CurrentThread.ManagedThreadId);

                    Console.WriteLine("Task TriplePostedToServerReceivedAction Complete...");
                }, cancellationToken: CancellationToken.None);

                var writeToConsoleTask = graphEngineResponseTask;

                await writeToConsoleTask;

            });

            GraphEngineTripleModuleImpl.ServerStreamedTripleSavedToMemoryCloudAction
                .ObserveOn(GraphEngineTripleModuleImpl.ObserverOnNewThreadScheduler)
                .Do(onNext: subscriberSource =>
                {
                    var msg = "ServerStreamedTripleSavedToMemoryCloudAction-1";
                    Console.WriteLine("{0} Subscription happened on this Thread: {1}", msg, Thread.CurrentThread.ManagedThreadId);

                })
                .SubscribeOn(GraphEngineTripleModuleImpl.SubscribeOnEventLoopScheduler)
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

                                var responseReader = await connectedTripleStoreClient.StreamTriplesAsync(message).ConfigureAwait(false);

                                Console.WriteLine($"Server responses with rsp code={responseReader.errno}");
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


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

            Log.LogsWritten += Log_LogsWritten;

            TrinityTripleModuleClient = new TrinityClient("localhost:9898");

            TrinityTripleModuleClient.RegisterCommunicationModule<TripleModule>();
            TrinityTripleModuleClient.Start();

            TripleClientSideModule = TrinityTripleModuleClient.GetCommunicationModule<TripleModule>();

            TripleClientSideModule.ExternalTripleReceivedAction.Subscribe(onNext: async tripleObjectFromServer =>
            {
                using var reactiveGraphEngineResponseTask = Task.Factory.StartNew(async () =>
                    {
                        //await Task.Yield();

                        await Task.Delay(0).ConfigureAwait(false);

                        Console.WriteLine($"Processing Timestamp: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
                        Console.WriteLine($"Triple Subject Node: {tripleObjectFromServer.Subject}");
                        Console.WriteLine($"Triple Predicate Node: {tripleObjectFromServer.Predicate}");
                        Console.WriteLine($"Triple Object Node: {tripleObjectFromServer.Object}");

                    }, cancellationToken: CancellationToken.None,
                    creationOptions: TaskCreationOptions.HideScheduler,
                    scheduler: TaskScheduler.Current).Unwrap().ContinueWith(async _ =>
                {
                    await Task.Delay(0);

                    Console.WriteLine("Task ExternalTripleReceivedAction Complete...");
                }, cancellationToken: CancellationToken.None);

                var writeToConsoleTask = reactiveGraphEngineResponseTask;

                await writeToConsoleTask;
            });

            TripleClientSideModule.ExternalTripleSavedToMemoryAction.Subscribe(onNext: async tripleStoreMemoryContext =>
            {
                using var retrieveTripleStoreFromMemoryCloudTask = Task.Factory.StartNew(async () =>
                    {
                        //await Task.Yield();
                        await Task.Delay(0).ConfigureAwait(false);

                        Console.WriteLine("Try locate the Triple in the TripleStore MemoryCloud");

                        foreach (var tripleNode in Global.LocalStorage.TripleStore_Selector())
                        {
                            if (tripleStoreMemoryContext.CellId != tripleNode.CellId) continue;

                            Console.WriteLine("Success! Found the Triple in the TripleStore MemoryCloud");

                            var node          = tripleNode.TripleCell;
                            var subjectNode   = node.Subject;
                            var predicateNode = node.Predicate;
                            var objectNode    = node.Object;

                            Console.WriteLine($"Triple CellId in MemoryCloud: {tripleNode.CellId}");
                            Console.WriteLine($"Subject Node: {subjectNode}");
                            Console.WriteLine($"Predicate Node: {predicateNode}");
                            Console.WriteLine($"Object Node: {objectNode}");

                            break;
                        }

                    }, cancellationToken: CancellationToken.None,
                    creationOptions: TaskCreationOptions.HideScheduler,
                    scheduler: TaskScheduler.Current).Unwrap().ContinueWith(async _ =>
                {
                    await Task.Delay(0);

                    Console.WriteLine("Task ExternalTripleSavedToMemoryAction Complete...");
                }, cancellationToken: CancellationToken.None);

                var storeFromMemoryCloudTask = retrieveTripleStoreFromMemoryCloudTask;

                await storeFromMemoryCloudTask;
            });

            // -------------------------------------------------------

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

                            var responseReader =
                                await TrinityTripleModuleClient.PostTriplesToServer(tripleStreamWriter);

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

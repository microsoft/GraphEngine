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
            TrinityConfig.AddServer(new ServerInfo("localhost", 9898, string.Empty, LogLevel.Verbose));
            TrinityConfig.LogEchoOnConsole   = true;
            TrinityConfig.LoggingLevel       = LogLevel.Verbose;
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

            //GraphEngineTripleStoreDemoServerImpl =
            //    TripleStoreServer.GetCommunicationModule<TripleStoreDemoServerModule>();

            while (true)
            {
                // Each time we pass through the look check to see how many active clients are connected

                if (TrinityClientModuleRuntime.Clients != null)
                {
                    var tripleStoreClients = TrinityClientModuleRuntime.Clients.ToList();

                    Console.WriteLine($"The number of real-time Connected TripleStore Client: {tripleStoreClients.Count}.");

                    foreach (var connectedTripleStoreClient in tripleStoreClients.Where(connectedTripleStoreClient => connectedTripleStoreClient != null))
                    {
                        try
                        {
                            List<Triple> triples = new List<Triple>{ new Triple { Subject = "GraphEngineServer", Predicate = "is",  Object = "Running" } };

                            // New-up the Request Message!

                            using var message = new TripleStreamWriter(triples);

                            // Push a triple to the Client

                            using var rsp = await connectedTripleStoreClient.StreamTriplesAsync(message).ConfigureAwait(false);

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
                            Console.WriteLine(ex.ToString());
                        }
                    }
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
            //foreach (var logEntry in trinityLogCollection)
            //{        
            //    Console.WriteLine($"TrinityServer LOG: {logEntry.logTime}");
            //    Console.WriteLine($"TrinityServer LOG: {logEntry.logLevel}");
            //    Console.WriteLine($"TrinityServer LOG: {logEntry.logMessage}");
            //}

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


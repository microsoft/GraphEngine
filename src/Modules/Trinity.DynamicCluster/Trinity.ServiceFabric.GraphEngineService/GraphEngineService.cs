using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Trinity.DynamicCluster;
using Trinity.Utilities;
using Trinity.Network;
using Trinity.Diagnostics;
using Trinity.Configuration;

namespace Trinity.ServiceFabric
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    public sealed class GraphEngineService : StatefulService
    {
        internal TrinityServer TrinityServer = null;
        internal NodeContext NodeContext = null;
        internal FabricClient FabricClient = null;

        internal static GraphEngineService Instance = null;
        private static object s_lock = new object();

        public List<System.Fabric.Query.Partition> Partitions { get; private set; }
        public int PartitionCount { get; private set; }
        public int PartitionId { get; private set; }
        public int Port { get; private set; }
        public int HttpPort { get; private set; }
        public string Address { get; private set; }

        //  Passive Singleton
        public GraphEngineService(StatefulServiceContext context)
            : base(context)
        {
            //  Initialize Trinity server.
            TrinityServer = AssemblyUtility.GetAllClassInstances(
                t => t != typeof(TrinityServer) ?
                t.GetConstructor(new Type[] { }).Invoke(new object[] { }) as TrinityServer :
                null)
                .FirstOrDefault();
            if (TrinityServer == null)
            {
                TrinityServer = new TrinityServer();
                Log.WriteLine(LogLevel.Warning, "GraphEngineService: using the default communication instance.");
            }

            //  Initialize other fields and properties.
            NodeContext = context.NodeContext;
            FabricClient = new FabricClient();
            Address = NodeContext.IPAddressOrFQDN;
            Partitions = FabricClient.QueryManager
                        .GetPartitionListAsync(context.ServiceName)
                        .GetAwaiter().GetResult()
                        .OrderBy(p => p.PartitionInformation.Id)
                        .ToList();
            PartitionId = Partitions.FindIndex(p => p.PartitionInformation.Id == Context.PartitionId);
            PartitionCount = Partitions.Count;
            Port = Context.CodePackageActivationContext.GetEndpoint("TrinityProtocolEndpoint").Port;
            HttpPort = Context.CodePackageActivationContext.GetEndpoint("HttpEndpoint").Port;

            var ags = TrinityConfig.CurrentClusterConfig.Servers;
            ags.Clear();
            ags.Add(new AvailabilityGroup("LOCAL", new ServerInfo("localhost", Port, null, LogLevel.Info)));
            TrinityConfig.HttpPort = HttpPort;

            lock (s_lock)
            {
                if (Instance != null)
                {
                    throw new InvalidOperationException("Only one GraphEngineService allowed in one process.");
                }
                Instance = this;
            }
        }

        internal TrinityErrorCode Start()
        {
            lock (s_lock)
            {
                TrinityServer.Start();
                return TrinityErrorCode.E_SUCCESS;
            }
        }

        internal TrinityErrorCode Stop()
        {
            lock (s_lock)
            {
                TrinityServer.Stop();
                return TrinityErrorCode.E_SUCCESS;
            }
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] {
                new ServiceReplicaListener(ctx => new GraphEngineListener(ctx), listenOnSecondary: true),
                new ServiceReplicaListener(ctx => new GraphEngineHttpListener(ctx), listenOnSecondary: false),
            };
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    GraphEngineServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}

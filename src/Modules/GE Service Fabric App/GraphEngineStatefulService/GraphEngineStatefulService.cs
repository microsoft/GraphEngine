using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Trinity.GraphEngine.ServiceFabric.Core;
using Trinity.GraphEngine.ServiceFabric.Listeners;

namespace Trinity.ServiceFabric
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class GraphEngineStatefulService : StatefulService
    {
        readonly GraphEngineStatefulServiceCore graphEngineStatefulServiceCore;

        public GraphEngineStatefulService(StatefulServiceContext context)
            : base(context)
        {
            graphEngineStatefulServiceCore = new GraphEngineStatefulServiceCore(context);
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
            //  See https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-reliable-services-communication:
            //  When creating multiple listeners for a service, each listener must be given a unique name.
            return new[] {
                new ServiceReplicaListener(ctx => new GraphEngineListener(ctx, graphEngineStatefulServiceCore), "GraphEngineTrinityProtocolListener", listenOnSecondary: true),
                new ServiceReplicaListener(ctx => new GraphEngineHttpListener(ctx, graphEngineStatefulServiceCore), "GraphEngineHttpListener", listenOnSecondary: true),
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

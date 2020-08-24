using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using FanoutSearch;
using Microsoft.ServiceFabric.Services.Runtime;
using Trinity.Azure.Storage;
using Trinity.Client.TestProtocols;
using Trinity.ServiceFabric.NativeClient.Remoting.Interfaces;
using Trinity.ServiceFabric.Remoting;

namespace Trinity.SF.GraphEngineRemotingClient
{
    // Workaround: extension assembly will be removed by the
    // compiler if it is not explicitly used in the code.
    [UseExtension(typeof(ITrinityOverRemotingService))]
    [UseExtension(typeof(TripleModule))]
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.

                ServiceRuntime.RegisterServiceAsync("Trinity.SF.GraphEngineRemotingClientType",
                    context => new GraphEngineRemotingClient(context)).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, nameof(GraphEngineRemotingClient));

                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}

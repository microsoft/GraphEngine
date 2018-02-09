using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Trinity.ServiceFabric.Stateless;
using Trinity;
using DemoWebApiStateless.Controllers;

namespace DemoWebApiStateless
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class DemoWebApiStateless : StatelessService
    {
        private static readonly Uri TrinityServiceName = new Uri("fabric:/DemoTrinityApp/DemoTrinityStateless");

        private StatelessClusterConfig ClusterConfig { get; set; }

        public DemoWebApiStateless(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ClusterConfig = StatelessClusterConfig.Resolve(TrinityServiceName, RunningMode.Client);
                        TrinityConfig.CreateClusterConfig = () => ClusterConfig;

                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Resolved trinity cluster [ServerCount={Global.ServerCount}]. Starting Kestrel on {url}");

                        //for (var partition = 0; partition < 256; partition++)
                        //{
                        //    for (var i = 0; i < 5; i++)
                        //    {
                        //        CellsController.GetOrUpdateCell((partition << 8) + i, $"[{partition}][{i}]");
                        //    }
                        //}

                        return new WebHostBuilder()
                                    .UseKestrel()
                                    .ConfigureServices(
                                        services => services
                                            .AddSingleton<StatelessServiceContext>(serviceContext))
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseStartup<Startup>()
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url)
                                    .Build();
                    }))
            };
        }
    }
}

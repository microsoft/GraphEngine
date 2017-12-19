using Microsoft.ServiceFabric.Services.Communication.Runtime;
using System.Threading;
using System.Threading.Tasks;
using Trinity.ServiceFabric.Diagnostics;
using Trinity.ServiceFabric.Stateless;

namespace Trinity.ServiceFabric.Communication
{
    class TrinityServerCommunicationListener : ICommunicationListener
    {
        internal static string Name => "trinity-server";

        private TrinityStatelessService service;

        public TrinityServerCommunicationListener(TrinityStatelessService service)
        {
            this.service = service;
        }

        public void Abort()
        {
            Log.Info("TrinityServerCommunicationListener Abort: instanceId = {0}", service.Context.InstanceId);
            service.TrinityServer.Stop();
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            Log.Info("TrinityServerCommunicationListener CloseAsync: instanceId = {0}", service.Context.InstanceId);
            service.TrinityServer.Stop();
            return Task.CompletedTask;
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            Log.Info("TrinityServerCommunicationListener OpenAsync: instanceId = {0}", service.Context.InstanceId);
            var context = service.Context;
            var endpoint = context.CodePackageActivationContext.GetEndpoint(service.TrinityServerEndpointName);
            var publishUri = $"{endpoint.IpAddressOrFqdn}:{endpoint.Port}";

            service.TrinityServer.StartTrinityServer(endpoint.Port);
            Log.Info("Trinity server started. InstanceId={0}, Uri={1}", service.Context.InstanceId, publishUri);

            return Task.FromResult(publishUri);
        }
    }
}

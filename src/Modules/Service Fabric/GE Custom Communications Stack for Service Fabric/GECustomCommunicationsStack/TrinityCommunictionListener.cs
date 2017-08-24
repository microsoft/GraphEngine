using System.Fabric;
using System.Net;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Trinity;
using Trinity.Network;

namespace GE.ServiceFabric.Services.Communiction.Trinity.Runtime
{

    /// <summary>
    /// A Microsoft Trinity Communiactions based listener for Service Fabric based stateless and stateful
    /// service.
    /// </summary>
    public class TrinityCommunictionListener<TServiceContract> : TrinityServer, ICommunicationListener
    {
        public TrinityCommunictionListener(ServiceContext serviceContext, TServiceContract trinityServiceObject)
        {
            
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public void Abort()
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        protected override void RegisterMessageHandler()
        {
            base.RegisterMessageHandler();
        }

        protected override void RootHttpHandler(HttpListenerContext ctx)
        {
            base.RootHttpHandler(ctx);
        }

        protected override void DispatchHttpRequest(HttpListenerContext ctx, string handlerName, string url)
        {
            base.DispatchHttpRequest(ctx, handlerName, url);
        }
    }
}

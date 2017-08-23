using System.Fabric;
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
    public class TrinityCommunictionListener<TServiceContract> : CommunicationInstance, ICommunicationListener
    {
        public TrinityCommunictionListener(ServiceContext serviceContext, TServiceContract trinityServiceObject)
        {
            
        }

        public TrinityCommunictionListener(ServiceContext serviceContext, TServiceContract trinityServiceObject,
                                           Binding listenerBinding = null, string endpointResourceName = null)
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
    }
}

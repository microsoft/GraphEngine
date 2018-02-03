using Microsoft.ServiceFabric.Services.Communication.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Utilities;

namespace Trinity.ServiceFabric.Listeners
{
    public static class GraphEngineCommunicationListenerLoader
    {
        public static IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return AssemblyUtility.GetAllClassInstances<IGraphEngineCommunicationListener>()
                                  .Select(l => new ServiceReplicaListener(ctx => l, l.ListenerName, l.ListenOnSecondaries));
        }
    }
}

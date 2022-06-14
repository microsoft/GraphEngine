using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Utilities;

namespace Trinity.ServiceFabric.Listener
{
    internal static class GraphEngineCommunicationListenerLoader
    {
        public static IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners(StatefulServiceContext context) => 
            AssemblyUtility.GetAllClassInstances<GraphEngineListenerBase>()
            .Select(l => l._Setup(context))
            .Select(l=> new ServiceReplicaListener( ctx => l, l.ListenerName, l.ListenOnSecondaries));
    }
}

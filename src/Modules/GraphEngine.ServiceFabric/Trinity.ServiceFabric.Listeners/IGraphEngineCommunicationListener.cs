using Microsoft.ServiceFabric.Services.Communication.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.ServiceFabric.Infrastructure;

namespace Trinity.ServiceFabric.Listeners
{
    public interface IGraphEngineCommunicationListener : ICommunicationListener
    {
        string ListenerName { get; }
        /// <summary>
        /// Transports specifically designed for client-server communication
        /// will not listen on secondaries.
        /// </summary>
        bool ListenOnSecondaries { get; }
    }
}

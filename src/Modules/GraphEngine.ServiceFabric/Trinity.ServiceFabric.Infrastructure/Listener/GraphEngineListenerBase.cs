using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Trinity.Network;
using Trinity.ServiceFabric.Infrastructure;

namespace Trinity.ServiceFabric.Listener
{
    // Design notes: Tavi Truman
    // This class 
    public abstract class GraphEngineListenerBase : ICommunicationListener
    {
        public abstract string EndpointName { get; }

        public abstract string ListenerName { get; }

        public abstract bool ListenOnSecondaries { get; }

        private int m_port;

        /// <summary>
        /// Configures the GraphEngine side, given
        /// a port acquired from ServiceFabric.
        /// </summary>
        /// <remarks>
        /// Implementation must not throw.
        /// </remarks>
        public abstract void Configure(int port, StatefulServiceContext ctx);

        internal GraphEngineListenerBase _Setup(StatefulServiceContext ctx)
        {
            m_port = ctx.CodePackageActivationContext.GetEndpoint(EndpointName).Port;
            Configure(m_port, ctx);
            return this;
        }

        /// <summary>
        /// Default or Base behavior calls the GraphEngine Stateful Service runtime to call into the
        /// Trinity Server Network stack to Stop.
        /// </summary>
        public virtual void Abort()
        {
            GraphEngineStatefulServiceRuntime.Instance.TrinityServerRuntime.Stop();
        }

        /// <summary>
        /// Makes an asynchronous call into the Trinity Server Networking stack to stop the Trinity TCP server
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task CloseAsync(CancellationToken cancellationToken)
        {
            GraphEngineStatefulServiceRuntime.Instance.TrinityServerRuntime.Stop();
            return Task.FromResult(0);
        }

        /// <summary>
        /// Default behavior will call up to the Trinity Server Network stack to make an
        /// asynchronous call to start the TrinityServer native TCP networking.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            var statefulServiceRuntime = GraphEngineStatefulServiceRuntime.Instance;

            var trinityErrorCode = await Task.Factory
                .StartNew(() => statefulServiceRuntime.TrinityServerRuntime.Start(),
                    TaskCreationOptions.RunContinuationsAsynchronously).ConfigureAwait(false);

            return $"tcp://{statefulServiceRuntime.TrinityServerRuntime.Address}:{m_port}";
        }
    }
}
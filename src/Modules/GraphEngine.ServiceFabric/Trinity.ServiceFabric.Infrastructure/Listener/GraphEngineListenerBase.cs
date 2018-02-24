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

        /// <inheritdoc />
        public virtual void Abort()
        {
            GraphEngineStatefulServiceRuntime.Instance.TrinityServerRuntime.Stop();
        }

        public virtual Task CloseAsync(CancellationToken cancellationToken)
        {
            GraphEngineStatefulServiceRuntime.Instance.TrinityServerRuntime.Stop();
            return Task.FromResult(0);
        }

        public virtual async Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            var rt = GraphEngineStatefulServiceRuntime.Instance;
            await Task.Factory.StartNew(() => rt.TrinityServerRuntime.Start(), TaskCreationOptions.RunContinuationsAsynchronously).ConfigureAwait(false);
            return $"tcp://{rt.TrinityServerRuntime.Address}:{m_port}";
        }
    }
}
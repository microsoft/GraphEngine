using System;
using System.Threading;

namespace Trinity.DynamicCluster.Consensus
{
    /// <summary>
    /// When <see cref="IDisposable.Dispose"/> is called, a service should
    /// block until it cleans up the resources and stops.
    /// </summary>
    public interface IService : IDisposable
    {
        /// <summary>
        /// Starts the service. A service is responsible to keep track
        /// of the cancellation token, which will be fired on instance
        /// shutdown.
        /// </summary>
        TrinityErrorCode Start(CancellationToken cancellationToken);
    }
}
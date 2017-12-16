// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
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
        void Start(CancellationToken cancellationToken);
        /// <summary>
        /// Indicates whether the service is a master replica with
        /// read/write access to the persistent states.
        /// </summary>
        bool IsMaster { get; }
    }
}
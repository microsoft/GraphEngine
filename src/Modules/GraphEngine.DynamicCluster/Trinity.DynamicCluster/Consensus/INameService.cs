// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Storage;

namespace Trinity.DynamicCluster.Consensus
{
    public interface INameService: IService
    {
        /// <summary>
        /// Obtains a unique identifier for the local instance.
        /// Note, on the platforms where multiple instances run
        /// on the same server, generating Ids with MAC address
        /// or other machine fingerprints will fail.
        /// </summary>
        Guid InstanceId { get; }
        /// <summary>
        /// Obtains the FQDN or IP address of the local instance.
        /// </summary>
        string Address { get; }
        /// <summary>
        /// Obtains the numbers of partitions.
        /// </summary>
        int PartitionCount { get; }
        /// <summary>
        /// My partition Id.
        /// </summary>
        int PartitionId { get; }
        /// <summary>
        /// Obtains a list of all published replicas in a partition.
        /// </summary>
        Task<IEnumerable<ReplicaInformation>> ResolvePartition(int partitionId);
        /// <summary>
        /// Indicates whether the service is a master replica with
        /// read/write access to the persistent states.
        /// </summary>
        bool IsMaster { get; }
    }
}

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
        /// Obtains the Tcp port for Trinity protocols on the local instance.
        /// </summary>
        int Port { get; }
        /// <summary>
        /// Obtains the Http port on the local instance.
        /// </summary>
        int HttpPort { get; }
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
        Task<ReplicaInformation> ResolveMasterReplica(int partitionId);
    }
}

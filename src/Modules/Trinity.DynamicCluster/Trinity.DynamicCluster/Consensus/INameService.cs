using System;
using System.Net;
using System.Text;
using System.Threading;
using Trinity.Network;

namespace Trinity.DynamicCluster.Consensus
{
    public class ReplicaInformation
    {
        public ReplicaInformation(string addr, int port, Guid id, int partitionId)
        {
            Address = addr;
            Port = port;
            Id = id;
            PartitionId = partitionId;
        }
        public string Address { get; }
        public int Port { get; }
        public Guid Id { get; }
        public int PartitionId { get; }
    }
    public interface INameService: IService
    {
        /// <summary>
        /// Guarantees the delivery of informations regarding servers that
        /// the name service decides that this instance should connect to,
        /// regardless of whether these target instances have registered before
        /// or after <see cref="Start"/> is called. Also, a name service
        /// should not report itself (no loopback connections shall be made).
        /// </summary>
        event EventHandler<ReplicaInformation> NewReplicaInformationPublished;
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
    }
}

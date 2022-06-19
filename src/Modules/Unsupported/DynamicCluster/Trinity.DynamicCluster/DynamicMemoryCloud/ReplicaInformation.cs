using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Storage
{
    /// <summary>
    /// In a dynamic Trinity memory cloud, one replica corresponds to a specific communication instance
    /// (i.e. a TrinityServer, or its derivative). A partition contains one or more replicas to guarantee
    /// availability.
    /// </summary>
    [Serializable]
    public class ReplicaInformation : IEquatable<ReplicaInformation>
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

        public bool Equals(ReplicaInformation other) => 
            Id == other.Id && PartitionId == other.PartitionId && Address == other.Address && Port == other.Port;

        public override int GetHashCode() => Id.GetHashCode();
    }

}

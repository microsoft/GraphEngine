using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;

namespace Trinity.ServiceFabric.GarphEngine.Infrastructure.Interfaces
{
    public interface IGraphEngineStatefulServiceRuntime
    {
        List<Partition> Partitions { get; }
        int PartitionCount { get; }
        int PartitionId { get; }
        int Port { get; }
        int HttpPort { get; }
        string Address { get; }
        ReplicaRole Role { get; }
        StatefulServiceContext Context { get; }
    }
}
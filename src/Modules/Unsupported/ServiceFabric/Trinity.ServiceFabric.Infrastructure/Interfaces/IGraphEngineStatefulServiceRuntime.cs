// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;

namespace Trinity.ServiceFabric.Infrastructure.Interfaces
{
    public interface IGraphEngineStatefulServiceRuntime
    {
        List<Partition> Partitions { get; }
        int PartitionCount { get; }
        int PartitionId { get; }
        string Address { get; }
        //ReplicaRole Role { get; }
        StatefulServiceContext Context { get; }
    }
}

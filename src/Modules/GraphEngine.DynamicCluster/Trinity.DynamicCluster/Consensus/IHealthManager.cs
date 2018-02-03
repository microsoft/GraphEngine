// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Health;
using Trinity.Network;

namespace Trinity.DynamicCluster.Consensus
{
    public interface IHealthManager : IService
    {
        Task ReportModuleStatus(HealthStatus health, string moduleName, string message);
        Task ReportReplicaStatus(HealthStatus health, Guid id, string message);
        Task ReportPartitionStatus(HealthStatus health, int id, string message);
        Task ReportMemoryCloudStatus(HealthStatus health, string message);
    }
}

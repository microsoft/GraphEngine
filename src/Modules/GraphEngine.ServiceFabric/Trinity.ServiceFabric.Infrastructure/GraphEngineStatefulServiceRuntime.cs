// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Trinity.Utilities;
using Trinity.ServiceFabric.Infrastructure.Interfaces;
using Trinity.Diagnostics;

namespace Trinity.ServiceFabric.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public class GraphEngineStatefulServiceRuntime : IGraphEngineStatefulServiceRuntime
    {
        // We integrate ourselves with Azure Service Fabric here by gaining processing context for
        // NodeContext and we use the FabricClient to gain deeper data and information required
        // to setup and drive the Trinity and GraphEngine components.

        private volatile TrinityServerRuntimeManager m_trinityServerRuntime = null;

        public List<Partition> Partitions { get; set; }

        internal List<long> PartitionLowKeys { get; set; }

        public int PartitionCount { get; private set; }

        public int PartitionId { get; private set; }

        public int Port { get; private set; }

        public int HttpPort { get; private set; }

        public string Address { get; private set; }

        public ReplicaRole Role { get; set; }

        public StatefulServiceContext Context { get; private set; }

        public IReliableStateManager StateManager { get; }

        internal Func<BackupDescription, Task> Backup;
        internal event EventHandler<RestoreEventArgs> RequestRestore = delegate { };
        internal NodeContext NodeContext;
        internal FabricClient FabricClient;

        private static readonly object s_singletonLock = new object();
        private static GraphEngineStatefulServiceRuntime s_instance = null;

        public static GraphEngineStatefulServiceRuntime Instance => s_instance ?? throw new InvalidOperationException($"{nameof(GraphEngineStatefulServiceRuntime)} is not initialized.");
        public static void Initialize(IGraphEngineStatefulService svc)
        {
            lock (s_singletonLock)
            {
                if (s_instance != null)
                {
                    Log.WriteLine(LogLevel.Warning, "Only one TrinityServerRuntimeManager allowed in one process.");
                    return;
                }
                s_instance = new GraphEngineStatefulServiceRuntime(svc);
            }
        }

        public TrinityServerRuntimeManager TrinityServerRuntime
        {
            get
            {
                TrinityServerRuntimeManager runtime_mgr;
                while (null == (runtime_mgr = m_trinityServerRuntime)) Thread.Sleep(1000);
                return runtime_mgr;
            }
            private set => m_trinityServerRuntime = value;
        }


        private GraphEngineStatefulServiceRuntime(IGraphEngineStatefulService svc)
        {
            //  Initialize other fields and properties.
            Context = svc.Context;
            StateManager = svc.StateManager;
            Backup = svc.BackupAsync;
            svc.RequestRestore += RequestRestore;
            NodeContext = Context.NodeContext;
            FabricClient = new FabricClient();
            Address = NodeContext.IPAddressOrFQDN;
            Partitions = FabricClient.QueryManager
                           .GetPartitionListAsync(Context.ServiceName)
                           .GetAwaiter().GetResult()
                           .OrderBy(p => p.PartitionInformation.Id)
                           .ToList();
            PartitionLowKeys = Partitions
                           .Select(p => p.PartitionInformation)
                           .OfType<Int64RangePartitionInformation>()
                           .Select(pi => pi.LowKey)
                           .ToList();
            Role = ReplicaRole.Unknown;

            PartitionId = Partitions.FindIndex(p => p.PartitionInformation.Id == Context.PartitionId);
            PartitionCount = Partitions.Count;

            Port = Context.CodePackageActivationContext.GetEndpoint(GraphEngineConstants.TrinityProtocolEndpoint).Port;
            HttpPort = Context.CodePackageActivationContext.GetEndpoint(GraphEngineConstants.TrinityHttpProtocolEndpoint).Port;

            var contextDataPackage = (Partitions: this.Partitions,
                                      PartitionCount: this.PartitionCount,
                                      PartitionId: this.PartitionId,
                                      Port: this.Port,
                                      HttpPort: this.HttpPort,
                                      IPAddress: this.Address,
                                      StatefulServiceContext: Context);

            // Okay let's new-up the TrinityServer runtime environment ...

            TrinityServerRuntime = new TrinityServerRuntimeManager(ref contextDataPackage);

            // TBD .. YataoL & Tavi T.
            //WCFPort = context.CodePackageActivationContext.GetEndpoint(GraphEngineConstants.TrinityWCFProtocolEndpoint).Port;

            if (PartitionCount != PartitionLowKeys.Count)
            {
                Log.WriteLine(LogLevel.Fatal, "Graph Engine Service requires all partitions to be int64-ranged.");
                Thread.Sleep(5000);
                throw new InvalidOperationException("Graph Engine Service requires all partitions to be int64-ranged.");
            }

        }

        public Guid GetInstanceId()
        {
            var replicaId = Context.ReplicaId;
            var partitionId = PartitionId;
            return NameService.GetInstanceId(replicaId, partitionId);
        }

        internal async Task<ReplicaRole> GetRoleAsync()
        {
            while (true)
            {
                var role = this.Role;
                if (role != ReplicaRole.None && role != ReplicaRole.Unknown && role != ReplicaRole.IdleSecondary)
                    return role;
                await Task.Delay(1000);
            }
        }

    }
}
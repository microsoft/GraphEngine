using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using System.Linq;
using System.Threading;
using Trinity.Diagnostics;
using Trinity.ServiceFabric.GarphEngine.Infrastructure.Interfaces;
using Microsoft.ServiceFabric.Data;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Persistency;

namespace Trinity.ServiceFabric.GarphEngine.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public class GraphEngineStatefulServiceRuntime : IGraphEngineStatefulServiceRuntime
    {
        // We integrate ourselves with Azure Service Fabric here by gaining processing context for
        // NodeContext and we use the FabricClient to gain deeper data and information required
        // to setup and drive the Trinity and GraphEngine components.
        internal NodeContext  NodeContext;
        internal FabricClient FabricClient;

        public volatile TrinityServerRuntimeManager m_trinityServerRuntime = null;

        public List<Partition> Partitions { get; set; }

        public int PartitionCount { get; private set; }

        public int PartitionId { get; private set; }

        public int Port { get; private set; }

        public int HttpPort { get; private set; }

        public string Address { get; private set; }

        public ReplicaRole Role { get; set; }

        public StatefulServiceContext Context { get; private set; }
        public IReliableStateManager StateManager { get; }

        internal Func<BackupDescription, Task> Backup;
        internal event EventHandler<RestoreEventArgs> RequestRestore = delegate{ };
        private static object SingletonLockObject => m_singletonLockObject;

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

        public static GraphEngineStatefulServiceRuntime Instance = null;

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

        private static readonly object m_singletonLockObject = new object();


        public GraphEngineStatefulServiceRuntime(IGraphEngineStatefulService svc)
        {
            //  Initialize other fields and properties.
            Context      = svc.Context;
            StateManager = svc.StateManager;
            Backup       = svc.BackupAsync;
            svc.RequestRestore += RequestRestore;
            NodeContext  = Context.NodeContext;
            FabricClient = new FabricClient();
            Address      = NodeContext.IPAddressOrFQDN;
            Partitions   = FabricClient.QueryManager
                           .GetPartitionListAsync(Context.ServiceName)
                           .GetAwaiter().GetResult()
                           .OrderBy(p => p.PartitionInformation.Id)
                           .ToList();
            Role         = ReplicaRole.Unknown;

            PartitionId    = Partitions.FindIndex(p => p.PartitionInformation.Id == Context.PartitionId);
            PartitionCount = Partitions.Count;

            Port     = Context.CodePackageActivationContext.GetEndpoint(GraphEngineConstants.TrinityProtocolEndpoint).Port;
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

            lock (SingletonLockObject)
            {
                if (Instance != null)
                {
                    Log.WriteLine(LogLevel.Fatal, "Only one TrinityServerRuntimeManager allowed in one process.");
                    Thread.Sleep(5000);
                    Environment.Exit(-1);
                    //throw new InvalidOperationException("Only one GraphEngineService allowed in one process.");
                }
                Instance = this;
            }
        }

        public Guid GetInstanceId()
        {
            var replicaId   = Context.ReplicaId;
            var partitionId = PartitionId;
            return NameService.GetInstanceId(replicaId, partitionId);
        }
    }
}
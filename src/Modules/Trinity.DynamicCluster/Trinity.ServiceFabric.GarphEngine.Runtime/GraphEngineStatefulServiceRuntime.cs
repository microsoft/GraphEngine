using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using System.IO;
using System.Linq;
using System.Threading;
using Trinity.Diagnostics;
using Trinity.Network;
using Trinity.ServiceFabric.GarphEngine.Infrastructure.Interfaces;
using Trinity.ServiceFabric.GarphEngine.Infrastructure;

namespace Trinity.ServiceFabric.GarphEngine.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public class GraphEngineStatefulServiceRuntime: IGraphEngineStatefulServiceRuntime
    {
        // We integrate ourselves with Azure Service Fabric here by gaining processing context for
        // NodeContext and we use the FabricClient to gain deeper data and information required
        // to setup and drive the Trinity and GraphEngine components.
        internal NodeContext  NodeContext;
        internal FabricClient FabricClient;

        public TrinityServerRuntimeManager m_trinityServerRuntime = null;

        public List<System.Fabric.Query.Partition> Partitions { get; set; }

        public int PartitionCount { get; private set; }

        public int PartitionId { get; private set; }

        public int Port { get; private set; }

        public int HttpPort { get; private set; }

        public string Address { get; private set; }

        public ReplicaRole Role { get; set; }

        public StatefulServiceContext Context { get; private set; }

        private static object SingletonLockObject => m_singletonLockObject;

        public TrinityServerRuntimeManager TrinityServerRuntime
        {
            get => m_trinityServerRuntime;
            private set => m_trinityServerRuntime = value;
        }

        public static GraphEngineStatefulServiceRuntime Instance = null;
        private static readonly object m_singletonLockObject = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public GraphEngineStatefulServiceRuntime(StatefulServiceContext context)
        {
            //  Initialize other fields and properties.
            this.Context = context;
            NodeContext  = context.NodeContext;
            FabricClient = new FabricClient();
            Address      = NodeContext.IPAddressOrFQDN;
            Partitions   = FabricClient.QueryManager
                           .GetPartitionListAsync(context.ServiceName)
                           .GetAwaiter().GetResult()
                           .OrderBy(p => p.PartitionInformation.Id)
                           .ToList();

            PartitionId    = Partitions.FindIndex(p => p.PartitionInformation.Id == context.PartitionId);
            PartitionCount = Partitions.Count;

            Port     = context.CodePackageActivationContext.GetEndpoint(GraphEngineConstants.TrinityProtocolEndpoint).Port;
            HttpPort = context.CodePackageActivationContext.GetEndpoint(GraphEngineConstants.TrinityHttpProtocolEndpoint).Port;

            var contextDataPackage = (Partitions: this.Partitions,
                                      PartitionCount: this.PartitionCount,
                                      PartitionId: this.PartitionId,
                                      Port: this.Port,
                                      HttpPort: this.HttpPort,
                                      IPAddress: this.Address,
                                      Role: this.Role,
                                      StatefulServiceContext: context);

            // Okay let's new-up the TrinityServer runtime environment ...

            TrinityServerRuntime = new TrinityServerRuntimeManager(ref contextDataPackage);

            // TBD .. YataoL & Tavi T.
            //WCFPort = context.CodePackageActivationContext.GetEndpoint(GraphEngineConstants.TrinityWCFProtocolEndpoint).Port;

            lock (SingletonLockObject)
            {
                if (Instance != null)
                {
                    Log.WriteLine(LogLevel.Fatal, "Only one GraphEngineService allowed in one process.");
                    Thread.Sleep(5000);
                    Environment.Exit(-1);
                    //throw new InvalidOperationException("Only one GraphEngineService allowed in one process.");
                }
                Instance = this;
            }
        }
    }
}
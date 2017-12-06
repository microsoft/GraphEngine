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

namespace Trinity.ServiceFabric.GarphEngine.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public class GraphEngineStatefulServiceRuntime: IGraphEngineStatefulServiceRuntime
    {
        internal NodeContext  NodeContext = null;
        internal FabricClient FabricClient = null;

        public List<Partition> Partitions { get; set; }

        public int PartitionCount { get; set; }

        public int PartitionId { get; set; }

        public int Port { get; set; }

        public int HttpPort { get; set; }

        public string Address { get; set; }

        public ReplicaRole Role { get; set; }

        public StatefulServiceContext Context { get; private set; }

        public static GraphEngineStatefulServiceRuntime Instance = null;
        private static object s_lock = new object();

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

            PartitionId = Partitions.FindIndex(p => p.PartitionInformation.Id == context.PartitionId);
            PartitionCount = Partitions.Count;

            Port = context.CodePackageActivationContext.GetEndpoint("TrinityProtocolEndpoint").Port;
            HttpPort = context.CodePackageActivationContext.GetEndpoint("HttpEndpoint").Port;

            

            lock (s_lock)
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
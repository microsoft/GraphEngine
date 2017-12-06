using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.Network;
using Trinity.Utilities;

namespace Trinity.GraphEngine.ServiceFabric.Core
{
    public sealed class GraphEngineStatefulServiceCore
    {
        internal TrinityServer TrinityServer = null;
        internal NodeContext NodeContext     = null;
        internal FabricClient FabricClient   = null;

        public static GraphEngineStatefulServiceCore Instance = null;
        private static object s_lock = new object();

        public List<System.Fabric.Query.Partition> Partitions { get; }
        public int PartitionCount { get; }
        public int PartitionId { get; }
        public int Port { get; }
        public int HttpPort { get; }
        public string Address { get; }
        public ReplicaRole Role { get; }
        public StatefulServiceContext Context { get; private set;}

        //  Passive Singleton
        public GraphEngineStatefulServiceCore(StatefulServiceContext context)
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

            Port     = context.CodePackageActivationContext.GetEndpoint("TrinityProtocolEndpoint").Port;
            HttpPort = context.CodePackageActivationContext.GetEndpoint("HttpEndpoint").Port;

            var ags = TrinityConfig.CurrentClusterConfig.Servers;
            ags.Clear();
            ags.Add(new AvailabilityGroup("LOCAL", new ServerInfo("localhost", Port, null, LogLevel.Info)));

            TrinityConfig.HttpPort = HttpPort;

            Log.WriteLine("{0}", $"WorkingDirectory={context.CodePackageActivationContext.WorkDirectory}");
            TrinityConfig.StorageRoot = Path.Combine(context.CodePackageActivationContext.WorkDirectory, $"P{PartitionId}{Path.GetRandomFileName()}");
            Log.WriteLine("{0}", $"StorageRoot={TrinityConfig.StorageRoot}");

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

        public TrinityErrorCode Start()
        {
            lock (s_lock)
            {
                //  Initialize Trinity server.
                if (TrinityServer == null)
                {
                    TrinityServer = AssemblyUtility.GetAllClassInstances(
                        t => t != typeof(TrinityServer) ?
                        t.GetConstructor(new Type[] { })?.Invoke(new object[] { }) as TrinityServer :
                        null)
                        .FirstOrDefault();
                }
                if (TrinityServer == null)
                {
                    TrinityServer = new TrinityServer();
                    Log.WriteLine(LogLevel.Warning, "GraphEngineService: using the default communication instance.");
                }
                else
                {
                    Log.WriteLine(LogLevel.Info, "{0}", $"GraphEngineService: using [{TrinityServer.GetType().Name}] communication instance.");
                }

                TrinityServer.Start();
                return TrinityErrorCode.E_SUCCESS;
            }
        }

        public TrinityErrorCode Stop()
        {
            lock (s_lock)
            {
                TrinityServer?.Stop();
                TrinityServer = null;
                return TrinityErrorCode.E_SUCCESS;
            }
        }
    }
}

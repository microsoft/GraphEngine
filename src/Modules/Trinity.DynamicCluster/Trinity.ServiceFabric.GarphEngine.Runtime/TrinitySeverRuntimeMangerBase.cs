using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using System.IO;
using System.Threading;
using Trinity.Diagnostics;
using Trinity.Network;
using Trinity.ServiceFabric.GarphEngine.Infrastructure.Interfaces;

namespace Trinity.ServiceFabric.GarphEngine.Infrastructure
{
    public abstract class TrinitySeverRuntimeMangerBase : ITrinityServerRuntimeManager
    {
        private TrinityServer m_trinityServer;

        // We manage and treat this groping of data as immutable
        private readonly (List<System.Fabric.Query.Partition> Partitions,
                          int PartitionCount,
                          int PartitionId,
                          int Port,
                          int HttpPort,
                          string IPAddress,
                          ReplicaRole Role,
                          StatefulServiceContext Context) m_serviceFabricRuntimeContext;

        public static TrinitySeverRuntimeMangerBase Instance = null;
        private static readonly object m_singletonLockObject = new object();


        internal static object SingletonLockObject => m_singletonLockObject;

        public virtual TrinityErrorCode Start()
        {
            throw new System.NotImplementedException();
        }

        public virtual TrinityErrorCode Stop()
        {
            throw new System.NotImplementedException();
        }

        public List<Partition> Partitions => ServiceFabricRuntimeContext.Partitions;

        public int PartitionCount => ServiceFabricRuntimeContext.PartitionCount;

        public int PartitionId => ServiceFabricRuntimeContext.PartitionId;

        public int Port => ServiceFabricRuntimeContext.Port;

        public int HttpPort => ServiceFabricRuntimeContext.HttpPort;

        public string Address => ServiceFabricRuntimeContext.IPAddress;

        public ReplicaRole Role => ServiceFabricRuntimeContext.Role;

        public StatefulServiceContext Context => ServiceFabricRuntimeContext.Context;

        protected internal (List<Partition> Partitions, 
                            int PartitionCount, 
                            int PartitionId, 
                            int Port, 
                            int HttpPort,
                            string IPAddress, 
                            ReplicaRole Role, 
                            StatefulServiceContext Context) ServiceFabricRuntimeContext => m_serviceFabricRuntimeContext;

        internal TrinityServer ServiceFabricTrinityServerInstance
        {
            get => m_trinityServer;
            set => m_trinityServer = value;
        }

        protected TrinitySeverRuntimeMangerBase(ref (List<System.Fabric.Query.Partition> Partitions,
                                                int PartitionCount,
                                                int PartitionId,
                                                int Port,
                                                int HttpPort,
                                                string IPAddress,
                                                ReplicaRole Role,
                                                StatefulServiceContext Context) runtimeContext)
        {
            // load a reference pointer so that we can get to this data from a different place in STAP
            m_serviceFabricRuntimeContext = runtimeContext;

            // Let's configure the Trinity Server Configuration gotten from the Service Fabric Runtime Stateful-Service contexte

            var groupOfAvailabilityServers = TrinityConfig.CurrentClusterConfig.Servers;

            // Clear out any default configure in place!
            groupOfAvailabilityServers.Clear();
            // Now load and configure TrinityServer via dynamically acquired SF Cluster information
            groupOfAvailabilityServers.Add(new AvailabilityGroup(GraphEngineConstants.LocalAvailabilityGroup, 
                                           new ServerInfo(GraphEngineConstants.AvailabilityGroupLocalHost,
                                           runtimeContext.Port, null, 
                                           LogLevel.Info)));

            // Get the Http port

            TrinityConfig.HttpPort = runtimeContext.HttpPort;

            Log.WriteLine("{0}", $"WorkingDirectory={runtimeContext.Context.CodePackageActivationContext.WorkDirectory}");

            TrinityConfig.StorageRoot = Path.Combine(runtimeContext.Context.CodePackageActivationContext.WorkDirectory, 
                                                     $"P{runtimeContext.Context.PartitionId}{Path.GetRandomFileName()}");

            Log.WriteLine("{0}", $"StorageRoot={TrinityConfig.StorageRoot}");

            // Just setting up the object instance as we should only run a single instance of this class per SF Node instance

            lock (SingletonLockObject)
            {
                if (Instance != null)
                {
                    Log.WriteLine(LogLevel.Fatal, "Only one TrinitySeverRuntimeMangerBase allowed in one process.");
                    Thread.Sleep(5000);
                    Environment.Exit(-2);
                    //throw new InvalidOperationException("Only one GraphEngineService allowed in one process.");
                }
                Instance = this;
            }
        }

    }
}
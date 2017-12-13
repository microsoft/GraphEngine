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
                          StatefulServiceContext Context) m_serviceFabricRuntimeContext;

        public static TrinitySeverRuntimeMangerBase Instance = null;
        private static readonly object m_singletonLockObject = new object();


        internal static object SingletonLockObject => m_singletonLockObject;

        public abstract TrinityErrorCode Start();

        public abstract TrinityErrorCode Stop();

        public List<Partition> Partitions => ServiceFabricRuntimeContext.Partitions;

        public int PartitionCount => ServiceFabricRuntimeContext.PartitionCount;

        public int PartitionId => ServiceFabricRuntimeContext.PartitionId;

        public int Port => ServiceFabricRuntimeContext.Port;

        public int HttpPort => ServiceFabricRuntimeContext.HttpPort;

        public string Address => ServiceFabricRuntimeContext.IPAddress;

        public StatefulServiceContext Context => ServiceFabricRuntimeContext.Context;

        protected internal (List<Partition> Partitions, 
                            int PartitionCount, 
                            int PartitionId, 
                            int Port, 
                            int HttpPort,
                            string IPAddress, 
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
                                                StatefulServiceContext Context) runtimeContext)
        {
            // load trinity configuration from service fabric settings
            LoadTrinityConfiguration(runtimeContext.Context);

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

            Log.WriteLine("{0}: {1}", nameof(Trinity.ServiceFabric), $"WorkingDirectory={runtimeContext.Context.CodePackageActivationContext.WorkDirectory}");

            TrinityConfig.StorageRoot = Path.Combine(runtimeContext.Context.CodePackageActivationContext.WorkDirectory, 
                                                     $"P{runtimeContext.Context.PartitionId}{Path.GetRandomFileName()}");

            Log.WriteLine("{0}: {1}", nameof(Trinity.ServiceFabric), $"StorageRoot={TrinityConfig.StorageRoot}");

            // Just setting up the object instance as we should only run a single instance of this class per SF Node instance

            lock (SingletonLockObject)
            {
                if (Instance != null)
                {
                    Log.WriteLine(LogLevel.Fatal, $"{nameof(Trinity.ServiceFabric)}: Only one {nameof(TrinitySeverRuntimeMangerBase)} allowed in one process.");
                    Thread.Sleep(5000);
                    Environment.Exit(-2);
                    //throw new InvalidOperationException("Only one GraphEngineService allowed in one process.");
                }
                Instance = this;
            }
        }

        private void LoadTrinityConfiguration(StatefulServiceContext context)
        {
            try
            {
                var dataPackage = context.CodePackageActivationContext.GetDataPackageObject("Data");
                var configPackage = context.CodePackageActivationContext.GetConfigurationPackageObject("Config");
                Log.WriteLine("{0}: {1}", nameof(Trinity.ServiceFabric), $"Data package version {dataPackage.Description.Version}, Config package version {configPackage.Description.Version}");
                var filename = configPackage.Settings
                    .Sections[GraphEngineConstants.ServiceFabricConfigSection]
                    .Parameters[GraphEngineConstants.ServiceFabricConfigSection]
                    .Value;
                var config_file = Path.Combine(dataPackage.Path, filename);
                TrinityConfig.LoadConfig(config_file);
            }
            catch
            {
                throw new ConfigurationNotLoadedException("Trinity Configuration settings failed to load.");
            }
        }
    }
}
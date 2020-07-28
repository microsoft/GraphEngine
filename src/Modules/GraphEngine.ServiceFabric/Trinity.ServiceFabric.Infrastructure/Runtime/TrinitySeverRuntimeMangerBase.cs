// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using System.IO;
using System.Threading;
using Trinity.Diagnostics;
using Trinity.Network;
using Trinity.ServiceFabric.Infrastructure.Interfaces;

namespace Trinity.ServiceFabric.Infrastructure
{
    public abstract class TrinitySeverRuntimeMangerBase : ITrinityServerRuntimeManager
    {
        // We manage and treat this groping of data as immutable

        public static TrinitySeverRuntimeMangerBase Instance = null;
        private static readonly object m_singletonLockObject = new object();


        internal static object SingletonLockObject => m_singletonLockObject;

        public abstract TrinityErrorCode Start();

        public abstract TrinityErrorCode Stop();

        public List<Partition> Partitions => ServiceFabricRuntimeContext.Partitions;

        public int PartitionCount => ServiceFabricRuntimeContext.PartitionCount;

        public int PartitionId => ServiceFabricRuntimeContext.PartitionId;

        public string Address => ServiceFabricRuntimeContext.IPAddress;

        public StatefulServiceContext Context => ServiceFabricRuntimeContext.Context;

        private (List<Partition> Partitions, 
                 int PartitionCount, 
                 int PartitionId, 
                 string IPAddress, 
                 StatefulServiceContext Context) ServiceFabricRuntimeContext { get; }

        internal TrinityServer ServiceFabricTrinityServerInstance { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="runtimeContext"></param>
        protected TrinitySeverRuntimeMangerBase(ref (List<System.Fabric.Query.Partition> Partitions,
                                                int PartitionCount,
                                                int PartitionId,
                                                string IPAddress,
                                                StatefulServiceContext Context) runtimeContext)
        {
            // initialize event source
            GraphEngineStatefulServiceEventSource.Current.GraphEngineLogInfo($"{nameof(TrinityServerRuntimeManager)}: Initializing Trinity runtime environment.");

            // load trinity configuration from service fabric settings
            LoadTrinityConfiguration(runtimeContext.Context);

            // load a reference pointer so that we can get to this data from a different place in STAP
            ServiceFabricRuntimeContext = runtimeContext;

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
                    .Parameters[GraphEngineConstants.ServiceFabricConfigParameter]
                    .Value;
                var config_file = Path.Combine(dataPackage.Path, filename);
                TrinityConfig.LoadConfig(config_file);
                Log.WriteLine("{0}: {1}", nameof(Trinity.ServiceFabric), $"TrinityConfig loaded from {config_file}");
            }
            catch(Exception ex)
            {
                Log.WriteLine("{0}: {1}", nameof(Trinity.ServiceFabric), $"Trinity Configuration settings failed to load: {ex.ToString()}");
                throw new ConfigurationNotLoadedException("Trinity Configuration settings failed to load.", ex);
            }
        }
    }
}
// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using System.Linq;
using Trinity.Configuration;
using Trinity.Diagnostics;
using Trinity.DynamicCluster.Storage;
using Trinity.Network;
using Trinity.Utilities;

namespace Trinity.ServiceFabric.Infrastructure
{
    public class TrinityServerRuntimeManager : TrinitySeverRuntimeMangerBase
    {
        public TrinityServerRuntimeManager(ref (List<System.Fabric.Query.Partition> Partitions,
                                               int PartitionCount,
                                               int PartitionId,
                                               string IPAddress,
                                               StatefulServiceContext Context) runtimeContext) : base(ref runtimeContext)
        {
        }

        /// <summary>
        /// The Start method will start the Trinity protocol stack listening ..
        /// </summary>
        /// <returns></returns>
        public override TrinityErrorCode Start()
        {
            lock (SingletonLockObject)
            {
                //  Already started?
                if (ServiceFabricTrinityServerInstance != null) { return TrinityErrorCode.E_SUCCESS; }

                //  Initialize Trinity server.
                var priorities = ExtensionConfig.Instance.ResolveTypePriorities();
                ServiceFabricTrinityServerInstance = AssemblyUtility
                    .GetBestClassInstance<TrinityServer, TrinityServer>(ranking: 
                    t => priorities.TryGetValue(t, out int p) ? p : 0);
                Log.WriteLine(LogLevel.Info, $"{nameof(TrinityServerRuntimeManager)}: using the communication instance type '{ServiceFabricTrinityServerInstance.GetType().FullName}'.");

                ServiceFabricTrinityServerInstance.Start();
                return TrinityErrorCode.E_SUCCESS;
            }
        }

        /// <summary>
        /// Execute orderly shutdown of the TrinityServer
        /// </summary>
        /// <returns></returns>
        public override TrinityErrorCode Stop()
        {
            lock (SingletonLockObject)
            {
                (Global.CloudStorage as DynamicMemoryCloud)?.Close();
                ServiceFabricTrinityServerInstance?.Stop();
                ServiceFabricTrinityServerInstance = null;
                return TrinityErrorCode.E_SUCCESS;
            }
        }
    }
}

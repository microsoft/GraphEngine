// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

using Trinity;
using Trinity.Daemon;
using Trinity.Network;
using Trinity.Storage;
using Trinity.Utilities;
using Trinity.Diagnostics;
using System.Runtime.CompilerServices;
using Trinity.Extension;
using Trinity.Configuration;

namespace Trinity
{
    /// <summary>
    /// Provides global runtime information about the system. It also provides methods for safely exiting the system. This class cannot be inherited.
    /// </summary>
    public static partial class Global
    {
        internal static bool isCloudStorageInited = false;
        internal static bool isLocalStorageInited = false;

        //Global Objects
        internal static Storage.LocalMemoryStorage local_storage = null;
        internal static Storage.MemoryCloud cloud_storage = null;
        internal static IGenericCellOperations generic_cell_ops = null;
        internal static Func<MemoryCloud> new_memorycloud_func = null;
        internal static IStorageSchema storage_schema = null;
        internal static CommunicationInstance communication_instance = null;
        internal static Dictionary<IPEndPoint, Storage.RemoteStorage> ProxyTable = new Dictionary<IPEndPoint, Storage.RemoteStorage>();
        private static object s_storage_init_lock = new object();
        private static object s_comm_instance_lock = new object();
        private static bool s_master_init_flag = false;
        private static List<Storage.MemoryCloud> s_registered_memoryclouds = new List<MemoryCloud>();

        /// <summary>
        /// Raised when initialization is complete.
        /// </summary>
        public static event Action Initialized = delegate { };
        /// <summary>
        /// Raised when uninitialization is complete.
        /// </summary>
        public static event Action Uninitialized = delegate { };

        /// <summary>
        /// Raised when the communication instance (server or proxy) is started.
        /// </summary>
        public static event Action CommunicationInstanceStarted = delegate { };

        static Global()
        {
            Initialize();
        }

        internal static IEnumerable<ICommunicationSchema> ScanForTSLCommunicationSchema(RunningMode schemaRunningMode)
        {
            Debug.Assert(schemaRunningMode == RunningMode.Server || schemaRunningMode == RunningMode.Proxy);

            var schema_interface_type = typeof(ICommunicationSchema);
            var comm_instance_base_type = schemaRunningMode == RunningMode.Server ? typeof(TrinityServer) : typeof(TrinityProxy);
            var default_comm_schema = typeof(DefaultCommunicationSchema);

            return AssemblyUtility.GetAllClassInstances<ICommunicationSchema, CommunicationSchemaAttribute>(
                _ => comm_instance_base_type.IsAssignableFrom(_) && _ != default_comm_schema,
                _ => _.CommunicationSchemaType
                      .GetConstructor(new Type[] { })
                      .Invoke(new object[] { })
                      as ICommunicationSchema);
        }

        private static void _RegisterTSLStorageExtension(IGenericCellOperations genericCellOps, IStorageSchema storageSchema)
        {
            if (genericCellOps == null) { throw new ArgumentNullException("genericCellOps"); }
            if (storageSchema == null) { throw new ArgumentNullException("storageSchema"); }
            if (genericCellOps.GetType().Assembly != storageSchema.GetType().Assembly) { throw new ArgumentException("Components being registered are from different storage extensions."); }

            generic_cell_ops = genericCellOps;
            storage_schema   = storageSchema;
        }

        private static Tuple<IGenericCellOperations, IStorageSchema, int> _LoadTSLStorageExtension(Assembly extension_assembly)
        {
            Type default_provider_type = typeof(DefaultGenericCellOperations);
            Type schema_interface_type = typeof(IStorageSchema);
            Type default_storage_schema_type = typeof(DefaultStorageSchema);
            IGenericCellOperations _generic_cell_ops = null;
            IStorageSchema _storage_schema = null;
            int priority = 0;

            var provider_type = AssemblyUtility.GetAllClassTypes<IGenericCellOperations>(_ => _ != default_provider_type, extension_assembly).FirstOrDefault();
            if (provider_type == null) goto _return;

            var schema_type = AssemblyUtility.GetAllClassTypes<IStorageSchema>(_ => _ != default_storage_schema_type, extension_assembly).FirstOrDefault();
            if (schema_type == null) goto _return;

            var pdict = ExtensionConfig.Instance.ResolveTypePriorities();

            try
            {
                _generic_cell_ops = provider_type.GetConstructor(new Type[] { }).Invoke(new object[] { }) as IGenericCellOperations;
                _storage_schema = schema_type.GetConstructor(new Type[] { }).Invoke(new object[] { }) as IStorageSchema;

                if (pdict.TryGetValue(provider_type, out var provider_priority)) priority = provider_priority;
                if (pdict.TryGetValue(schema_type, out var schema_priority)) priority = schema_priority;
            }
            catch
            {
                _generic_cell_ops = null;
                _storage_schema = null;
            }

_return:
            return Tuple.Create(_generic_cell_ops, _storage_schema, priority);
        }

        /// <returns>E_SUCCESS if all the tasks are successfully executed. E_FAILURE otherwise.</returns>
        private static TrinityErrorCode _ScanForStartupTasks()
        {
            Log.WriteLine(LogLevel.Info, "Scanning for startup tasks.");
            bool all_good = true;
            var tasks = AssemblyUtility.GetAllClassInstances<IStartupTask>();
            foreach (var task in tasks)
            {
                try
                {
                    task.Run();
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "An error occured while executing a startup task: {0}", ex.ToString());
                    all_good = false;
                }
            }

            return all_good ? TrinityErrorCode.E_SUCCESS : TrinityErrorCode.E_FAILURE;
        }


        /// <returns>E_SUCCESS or E_FAILURE</returns>
        private static TrinityErrorCode _ScanForTSLStorageExtension()
        {
            Log.WriteLine(LogLevel.Info, "Scanning for TSL storage extension.");
            var priorities = ExtensionConfig.Instance.ResolveTypePriorities();

            var loaded_tuple = AssemblyUtility
                  .ForAllAssemblies(_LoadTSLStorageExtension)
                  .Where(_ => _.Item1 != null && _.Item2 != null)
                  .OrderByDescending(_ => _.Item3)
                  .FirstOrDefault();

            if (loaded_tuple == default(Tuple<IGenericCellOperations, IStorageSchema, int>))
            {
                Log.WriteLine(LogLevel.Info, "No TSL storage extension loaded.");
                _RegisterTSLStorageExtension(new DefaultGenericCellOperations(), new DefaultStorageSchema());
                return TrinityErrorCode.E_FAILURE;
            }
            else
            {
                Log.WriteLine(LogLevel.Info, "TSL storage extension loaded.");
                _RegisterTSLStorageExtension(loaded_tuple.Item1, loaded_tuple.Item2);
                return TrinityErrorCode.E_SUCCESS;
            }
        }

        /// <returns>E_SUCCESS or E_FAILURE</returns>
        private static TrinityErrorCode _ScanForMemoryCloudExtension()
        {
            Log.WriteLine(LogLevel.Info, "Scanning for MemoryCloud extensions.");
            new_memorycloud_func = () => new FixedMemoryCloud();

            var mc_ext_rank = ExtensionConfig.Instance.ResolveTypePriorities();
            Func<Type, int> mc_rank_func = t =>
            {
                if(mc_ext_rank.TryGetValue(t, out var r)) return r;
                else return 0;
            };

            var memcloud_types = AssemblyUtility.GetAllClassTypes<MemoryCloud>().OrderByDescending(mc_rank_func);
            if (!memcloud_types.Any(t => t != typeof(FixedMemoryCloud)))
            {
                Log.WriteLine(LogLevel.Info, "No MemoryCloud extension found.");
                return TrinityErrorCode.E_FAILURE;
            }

            new_memorycloud_func = () =>
            {
                //TODO read memcloud type from config
                //currently we just pick the first one
                foreach (var mc_type in memcloud_types)
                {
                    if (mc_type == null) continue;
                    try
                    {
                        var ctor = mc_type.GetConstructor(new Type[] { });
                        if (ctor == null)
                        {
                            Log.WriteLine(LogLevel.Warning, "MemoryCloud extension '{0}': no default constructor, skipping.", mc_type.Name);
                            continue;
                        }
                        var mc = ctor.Invoke(new object[] { }) as MemoryCloud;
                        Log.WriteLine(LogLevel.Info, "MemoryCloud extension '{0}' loaded.", mc_type.Name);
                        return mc;
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLine(LogLevel.Error, "Exception thrown while loading MemoryCloud extension '{0}': {1}.", mc_type.Name, ex.ToString());
                        continue;
                    }
                }
                // should never reach here
                throw new InvalidOperationException();
            };
            return TrinityErrorCode.E_SUCCESS;
        }

        private static void _LoadGraphEngineExtensions()
        {
            // Sometimes even if the assembly is tagged with the import extension attribute,
            // the extension assembly still won't get loaded. To alleviate this, we acquire
            // all currently loaded assemblies (including the one that calls into Trinity.Core and references the extension),
            // and forcibly enumerate its custom attributes. When the import extension
            // attribute is enumerated, it will trigger CLR to load its assembly, and thus
            // AppDomain.CurrentDomain.GetAssemblies will contain the extension assembly.
            AssemblyUtility.ForAllAssemblies(_ => _.GetCustomAttributes().ToList());
        }

        internal static void _RaiseCommunicationInstanceStarted()
        {
            CommunicationInstanceStarted();
        }
    }
}

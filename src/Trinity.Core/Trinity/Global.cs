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
        internal static IStorageSchema storage_schema = null;
        internal static CommunicationInstance communication_instance = null;
        internal static Dictionary<IPEndPoint, Storage.RemoteStorage> ProxyTable = new Dictionary<IPEndPoint, Storage.RemoteStorage>();
        private static object s_storage_init_lock = new object();
        private static object s_comm_instance_lock = new object();
        private static bool   s_master_init_flag = false;
        private static List<Storage.MemoryCloud> s_registered_memoryclouds = new List<MemoryCloud>();

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

            //  When no other communication schemas are loaded, 
            //  it is guaranteed that DefaultCommunicationSchema
            //  will be detected.

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

        private static Tuple<IGenericCellOperations, IStorageSchema> _LoadTSLStorageExtension(Assembly extension_assembly)
        {
            Type                   default_provider_type       = typeof(DefaultGenericCellOperations);
            Type                   schema_interface_type       = typeof(IStorageSchema);
            Type                   default_storage_schema_type = typeof(DefaultStorageSchema);
            IGenericCellOperations _generic_cell_ops           = null;
            IStorageSchema         _storage_schema             = null;

            var provider_type = AssemblyUtility.GetAllClassTypes<IGenericCellOperations>(_ => _ != default_provider_type, extension_assembly).FirstOrDefault();
            if (provider_type == null) goto _return;

            var schema_type = AssemblyUtility.GetAllClassTypes<IStorageSchema>( _ => _ != default_storage_schema_type, extension_assembly).FirstOrDefault();
            if (schema_type == null) goto _return;

            try
            {
                _generic_cell_ops = provider_type.GetConstructor(new Type[] { }).Invoke(new object[] { }) as IGenericCellOperations;
                _storage_schema = schema_type.GetConstructor(new Type[] { }).Invoke(new object[] { }) as IStorageSchema;
            }
            catch
            {
                _generic_cell_ops = null;
                _storage_schema = null;
            }

            _return:
            return Tuple.Create(_generic_cell_ops, _storage_schema);
        }

        /// <returns>E_SUCCESS or E_FAILURE</returns>
        private static TrinityErrorCode _ScanForTSLStorageExtension()
        {
            Log.WriteLine(LogLevel.Info, "Scanning for TSL storage extension.");

            Tuple<IGenericCellOperations, IStorageSchema> loaded_tuple 
                = AssemblyUtility
                  .ForAllAssemblies(_LoadTSLStorageExtension)
                  .Where(_ => _.Item1 != null && _.Item2 != null)
                  .FirstOrDefault();

            if (loaded_tuple == default(Tuple<IGenericCellOperations, IStorageSchema>))
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

        private static void _LoadTSLExtensions()
        {
            // Sometimes even if the assembly is tagged with the import extension attribute,
            // the extension assembly still won't get loaded. To alleviate this, we acquire
            // all currently loaded assemblies (including the one that calls into Trinity.Core and references the extension),
            // and forcibly enumerate its custom attributes. When the import extension
            // attribute is enumerated, it will trigger CLR to load its assembly, and thus
            // AppDomain.CurrentDomain.GetAssemblies will contain the extension assembly.
            AssemblyUtility.ForAllAssemblies(_ => _.GetCustomAttributes().ToList());
        }
    }
}

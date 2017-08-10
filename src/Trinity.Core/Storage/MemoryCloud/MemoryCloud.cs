// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Trinity;
using Trinity.Network.Messaging;
using Trinity.Diagnostics;
using System.Runtime.CompilerServices;
using Trinity.Network;
using Trinity.Utilities;

namespace Trinity.Storage
{
    /// <summary>
    /// Provides methods for interacting with the distributed memory store.
    /// </summary>
    public unsafe partial class MemoryCloud
    {
        internal Storage[] StorageTable;

        private ClusterConfig cluster_config;
        private int server_count = -1;
        private int my_server_id = -1;
        private int my_proxy_id = -1;

        internal MemoryCloud(ClusterConfig config)
        {
            this.cluster_config = config;
            server_count = cluster_config.Servers.Count;
            my_server_id = cluster_config.MyServerId;
            my_proxy_id = cluster_config.MyProxyId;
        }

        public bool Open(bool nonblocking)
        {
            bool server_found = false;
            StorageTable = new Storage[server_count];

            if (server_count == 0)
                goto server_not_found;

            for (int i = 0; i < server_count; i++)
            {
                if (cluster_config.RunningMode == RunningMode.Server &&
                    (cluster_config.Servers[i].Has(Global.MyIPAddresses, Global.MyIPEndPoint.Port) || cluster_config.Servers[i].HasLoopBackEndpoint(Global.MyIPEndPoint.Port))
                    )
                {
                    StorageTable[i] = Global.LocalStorage;
                    server_found = true;
                }
                else
                {
                    StorageTable[i] = new RemoteStorage(cluster_config.Servers[i], TrinityConfig.ClientMaxConn, this, i, nonblocking);
                }
            }

            if (cluster_config.RunningMode == RunningMode.Server && !server_found)
            {
                goto server_not_found;
            }

            StaticGetServerIdByCellId = this.GetServerIdByCellIdDefault;

            if (!nonblocking) { CheckServerProtocolSignatures(); } // TODO should also check in nonblocking setup when any remote storage is connected
            // else this.ServerConnected += (_, __) => {};

            return true;

        server_not_found:
            if (cluster_config.RunningMode == RunningMode.Server || cluster_config.RunningMode == RunningMode.Client)
                Log.WriteLine(LogLevel.Warning, "Incorrect server configuration. Message passing via CloudStorage not possible.");
            else if (cluster_config.RunningMode == RunningMode.Proxy)
                Log.WriteLine(LogLevel.Warning, "No servers are found. Message passing to servers via CloudStorage not possible.");
        return false;
        }

        /// <summary>
        /// Gets the ID of current server instance in the cluster.
        /// </summary>
        public int MyServerId
        {
            get
            {
                return my_server_id;
            }
        }

        /// <summary>
        /// Gets the ID of current proxy instance in the cluster.
        /// </summary>
        public int MyProxyId
        {
            get
            {
                return my_proxy_id;
            }
        }

        private void CheckServerProtocolSignatures()
        {
            Log.WriteLine("Checking {0}-Server protocol signatures...", cluster_config.RunningMode);
            int my_server_id = (cluster_config.RunningMode == RunningMode.Server) ? MyServerId : -1;
            var storage = StorageTable.Where((_, idx) => idx != my_server_id).FirstOrDefault() as RemoteStorage;

            CheckProtocolSignatures_impl(storage, cluster_config.RunningMode, RunningMode.Server);
        }

        private void CheckProxySignatures(IEnumerable<RemoteStorage> proxy_list)
        {
            Log.WriteLine("Checking {0}-Proxy protocol signatures...", cluster_config.RunningMode);
            int my_proxy_id = (cluster_config.RunningMode == RunningMode.Proxy) ? MyProxyId : -1;
            var storage = proxy_list.Where((_, idx) => idx != my_proxy_id).FirstOrDefault();

            CheckProtocolSignatures_impl(storage, cluster_config.RunningMode, RunningMode.Proxy);
        }

        private unsafe void CheckProtocolSignatures_impl(RemoteStorage storage, RunningMode from, RunningMode to)
        {
            if (storage == null)
                return;

            string my_schema_name;
            string my_schema_signature;
            string remote_schema_name;
            string remote_schema_signature;
            ICommunicationSchema my_schema;

            storage.GetCommunicationSchema(out remote_schema_name, out remote_schema_signature);

            if (from != to)// Asymmetrical checking, need to scan for matching local comm schema first.
            {
                var local_candidate_schemas = Global.ScanForTSLCommunicationSchema(to);

                /* If local or remote is default, we skip the verification. */

                if (local_candidate_schemas.Count() == 0)
                {
                    Log.WriteLine(LogLevel.Info, "{0}-{1}: Local instance has default communication capabilities.", from, to);
                    return;
                }

                if (remote_schema_name == DefaultCommunicationSchema.GetName() || remote_schema_signature == "{[][][]}")
                {
                    Log.WriteLine(LogLevel.Info, "{0}-{1}: Remote cluster has default communication capabilities.", from, to);
                    return;
                }

                /* Both local and remote are not default instances. */

                my_schema = local_candidate_schemas.FirstOrDefault(_ => _.Name == remote_schema_name);

                if (my_schema == null)
                {
                    Log.WriteLine(LogLevel.Fatal, "No candidate local communication schema signature matches the remote one.\r\n\tName: {0}\r\n\tSignature: {1}", remote_schema_name, remote_schema_signature);
                    Global.Exit(-1);
                }
            }
            else
            {
                my_schema = Global.CommunicationSchema;
            }

            my_schema_name = my_schema.Name;
            my_schema_signature = CommunicationSchemaSerializer.SerializeProtocols(my_schema);

            if (my_schema_name != remote_schema_name)
            {
                Log.WriteLine(LogLevel.Error, "Local communication schema name not matching the remote one.\r\n\tLocal: {0}\r\n\tRemote: {1}", my_schema_name, remote_schema_name);
            }

            if (my_schema_signature != remote_schema_signature)
            {
                Log.WriteLine(LogLevel.Fatal, "Local communication schema signature not matching the remote one.\r\n\tLocal: {0}\r\n\tRemote: {1}", my_schema_signature, remote_schema_signature);
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Gets an instance of a registered communication module on the started communication instance.
        /// </summary>
        /// <typeparam name="T">The type of the communication module.</typeparam>
        /// <returns>A communication module object if a communication instance is started, and the module type is registered. Otherwise returns null.</returns>
        public T GetCommunicationModule<T>() where T : CommunicationModule
        {
            var comm_instance = Global.CommunicationInstance;

            if (comm_instance == null)
            {
                return default(T);
            }
            else
            {
                return comm_instance.GetCommunicationModule<T>();
            }
        }

        /// <summary>
        /// The number of servers in the cluster.
        /// </summary>
        public int ServerCount
        {
            get
            {
                return cluster_config.Servers.Count;
            }
        }

        /// <summary>
        /// Gets the number of proxies in the cluster.
        /// </summary>
        public int ProxyCount
        {
            get
            {
                return cluster_config.Proxies.Count;
            }
        }

        internal int GetServerIdByIPE(IPEndPoint ipe)
        {
            for (int i = 0; i < cluster_config.Servers.Count; i++)
            {
                if (cluster_config.Servers[i].Has(ipe))
                    return i;
            }
            return -1;
        }

        private volatile bool disposed = false;

        /// <summary>
        /// Disposes current MemoryCloud instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes current MemoryCloud instance.
        /// </summary>
        /// <param name="disposing">This parameter is not used.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                foreach (var storage in StorageTable)
                {
                    if (storage != null && storage != Global.local_storage)
                        storage.Dispose();
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// The deconstruction method of MemoryCloud class.
        /// </summary>
        ~MemoryCloud()
        {
            Dispose(false);
        }

        private Storage GetStorageByCellId(long cellId)
        {
            return StorageTable[GetServerIdByCellId(cellId)];
        }

        /// <summary>
        /// Indicates whether the cell with the specified Id is a local cell.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns>true if the cell is in local storage; otherwise, false.</returns>
        public bool IsLocalCell(long cellId)
        {
            return (StaticGetServerIdByCellId(cellId) == my_server_id);
        }

        /// <summary>
        /// Gets the Id of the server on which the cell with the specified cell Id is located.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns>The Id of the server containing the specified cell.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetServerIdByCellIdDefault(long cellId)
        {
            return (*(((byte*)&cellId) + 1)) % server_count;
        }

        /// <summary>
        /// Gets the Id of the server on which the cell with the specified cell Id is located.
        /// </summary>
        public GetServerIdByCellIdDelegate StaticGetServerIdByCellId;

        /// <summary>
        /// Gets the Id of the server on which the cell with the specified cell Id is located.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns>A Trinity server Id.</returns>
        public int GetServerIdByCellId(long cellId)
        {
            return StaticGetServerIdByCellId(cellId);
        }

        /// <summary>
        /// Sets a user-defined data partitioning method.
        /// </summary>
        /// <param name="getServerIdByCellIdMethod">A method that transforms a 64-bit cell Id to a Trinity server Id.</param>
        public void SetPartitionMethod(GetServerIdByCellIdDelegate getServerIdByCellIdMethod)
        {
            StaticGetServerIdByCellId = getServerIdByCellIdMethod;
        }
    }
}

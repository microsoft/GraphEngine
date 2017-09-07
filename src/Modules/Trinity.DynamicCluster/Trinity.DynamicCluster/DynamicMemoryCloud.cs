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
using Trinity.Storage;
using Trinity.DynamicCluster;

namespace Trinity.Storage
{
    public unsafe partial class DynamicMemoryCloud : MemoryCloud
    {
        //private int server_count = -1;
        private int partition_count = -1;    
        private int my_partition_id = -1;
        private int my_proxy_id = -1;
        private ChunkCollection m_chunks = new ChunkCollection();
        private ChunkedStorage[] ChunkedStorageTable;//better to integrate in memorycloud.cs? 

        private static List<long> chunk_range = new List<long>();
        public static List<long> ChunkRange
        {
            get { return chunk_range; }

            //set { chunk_range = value; }
        }

        internal ClusterConfig cluster_config;//TODO will be substituted
        internal DynamicClusterConfig dynamic_cluster_config;

        

        public TrinityErrorCode OnStorageJoin(RemoteStorage remoteStorage)
        {
            var remotestorage_info = new _QueryChunkedRemoteStorageInformationReuslt();
            TrinityErrorCode eResult = _QueryChunkedRemoteStorageInformation(remoteStorage, out remotestorage_info);
            if (eResult!=TrinityErrorCode.E_SUCCESS) return eResult;
            var chunks = remotestorage_info.chunks;
            var p = remotestorage_info.partitionid;
            ChunkedStorageTable[i].Mount(remoteStorage);
            return TrinityErrorCode.E_SUCCESS;
        }

        private struct _QueryChunkedRemoteStorageInformationReuslt
        {
            internal int partitionid;
            internal ChunkCollection chunks;
        }

        private TrinityErrorCode _QueryChunkedRemoteStorageInformation(RemoteStorage remoteStorage, out _QueryChunkedRemoteStorageInformationReuslt result)
        {
            int i = 0;
            for (i=0; !ChunkedStorageTable[i].ContainsStorage(remoteStorage); i++)
            {
                if (i == PartitionCount)
                {
                    result.partitionid = -1;
                    result.chunks = [];
                    return TrinityErrorCode.E_FAILURE; //TODO need new err code.
                }
            }
            result.partitionid = i;
            result.chunks = ChunkedStorageTable[i].QueryChunkCollection(remoteStorage);
            return TrinityErrorCode.E_SUCCESS;
        }

        /// <summary>
        /// It is guaranteed that OnStorageLeave will only be called
        /// on those storages that has been previously sent to
        /// [OnStorageJoin]
        /// </summary>
        /// <param name="remoteStorage"></param>
        public TrinityErrorCode OnStorageLeave(RemoteStorage remoteStorage)
        {
            var remotestorage_info = new _QueryChunkedRemoteStorageInformationReuslt();
            TrinityErrorCode eResult = _QueryChunkedRemoteStorageInformation(remoteStorage, out remotestorage_info);
            if (eResult != TrinityErrorCode.E_SUCCESS) return eResult;
            var chunks = remotestorage_info.chunks;
            var p = remotestorage_info.partitionid;
            ChunkedStorageTable[i].Unmount(remoteStorage);
            return TrinityErrorCode.E_SUCCESS;
        }

        public override IEnumerable<int> MyChunkIds//better if named MyChunkCollection, return type is list?
        {
            get
            {
                return (m_chunks as IEnumerable<int>);
            }
        }

        public override int MyPartitionId
        {
            get
            {
                return dynamic_cluster_config.LocalPartitionId;
            }
        }

        public override int MyProxyId
        {
            get
            {
                return my_proxy_id;
            }
        }

        public override int ProxyCount
        {
            get
            {
                return cluster_config.Proxies.Count;
            }
        }

        public override int PartitionCount
        {
            get
            {
                return DynamicClusterConfig.Instance.PartitionCount;
            }
        }

        public int ChunkCount
        {
            get
            {
                return m_chunks.Count;
            }
        }

        public override bool IsLocalCell(long cellId)
        {
            return (GetStorageByCellId(cellId) as ChunkedStorage).IsLocal(cellId);
        }

        public override bool Open(ClusterConfig config, bool nonblocking)
        {
            this.cluster_config = config;
            my_partition_id = DynamicClusterConfig.Instance.LocalPartitionId;
            my_proxy_id = -1;
            partition_count = DynamicClusterConfig.Instance.PartitionCount;
            StorageTable = new ChunkedStorage[partition_count];

            if (partition_count == 0)
                goto server_not_found;

            for (int i = 0; i < partition_count; i++)
            {
                StorageTable[i] = new ChunkedStorage();
            }

            if (cluster_config.RunningMode == RunningMode.Server && !server_found)
            {
                goto server_not_found;
            }

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

        private void CheckServerProtocolSignatures()
        {
            Log.WriteLine("Checking {0}-Server protocol signatures...", cluster_config.RunningMode);
            int my_server_id = (cluster_config.RunningMode == RunningMode.Server) ? MyPartitionId : -1;
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

        internal int GetServerIdByIPE(IPEndPoint ipe)
        {
            for (int i = 0; i < cluster_config.Servers.Count; i++)
            {
                if (cluster_config.Servers[i].Has(ipe))
                    return i;
            }
            return -1;
        }
    }
}
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
        private int m_partitionId = -1;
        private int my_proxy_id = -1;
        //private int my_partition_id = -1;
        Dictionary<int, int> server_in_partition = new Dictionary<int, int>();
        private List<List<int>> server_host_chunk = new List<List<int>>();
        //private List<int> my_hosting_chunks = new List<int>();
        
        private List<long> chunk_range = new List<long>();

        internal ClusterConfig cluster_config;

        public void OnStorageJoin(RemoteStorage remoteStorage)
        {
            var remotestorage_info = _QueryChunkedRemoteStorageInformation(remoteStorage);
            var chunks = remotestorage_info.chunks;
            var p = remotestorage_info.partitionid;
        }

        private object _QueryChunkedRemoteStorageInformation(RemoteStorage remoteStorage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// It is guaranteed that OnStorageLeave will only be called
        /// on those storages that has been previously sent to
        /// [OnStorageJoin]
        /// </summary>
        /// <param name="remoteStorage"></param>
        public void OnStorageLeave(RemoteStorage remoteStorage)
        {

        }

        public override IEnumerable<int> MyChunkIds
        {
            get
            {
                List<int> my_hosting_chunks = new List<int>();
                my_hosting_chunks = server_host_chunk[m_partitionId];

                return my_hosting_chunks;
            }
        }

        public override int MyPartitionId
        {
            get
            {
                int my_partition_id = -1;
                my_partition_id = server_in_partition[m_partitionId];

                return my_partition_id;
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
                int chunk_count = -1;
                chunk_count = chunk_range.Count;

                return chunk_count;
            }
        }

        public override bool IsLocalCell(long cellId)
        {
            return (GetStorageByCellId(cellId) as ChunkedStorage).IsLocal(cellId);
        }

        public override bool Open(ClusterConfig config, bool nonblocking)
        {
            this.cluster_config = config;
            m_partitionId = DynamicClusterConfig.Instance.LocalPartitionId;
            my_proxy_id = -1;
            //
            chunk_range.Add(256); //C0
            chunk_range.Add(512); //C1
            chunk_range.Add(1024); //C2
            chunk_range.Add(long.MaxValue);//C3
            //
            server_host_chunk.Add(new List<int>() { 0, 2 });
            server_host_chunk.Add(new List<int>() { 0, 1, 3 });
            server_host_chunk.Add(new List<int>() { 0, 1, 2, 3 });
            //
            this.partition_count = 2;
            server_in_partition.Add(0, 0);
            server_in_partition.Add(1, 0);
            server_in_partition.Add(2, 1);

            bool server_found = false;
            int partition_count = this.PartitionCount;
            StorageTable = new Storage[partition_count];

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

        private int GetChunkIdByCellId(long cellId)
        {
            int chunk_id = 0;
            for (chunk_id = 0; chunk_id < ChunkCount; chunk_id++)
                if (cellId < chunk_range[chunk_id]) break;

            return chunk_id;
        }
    }
}
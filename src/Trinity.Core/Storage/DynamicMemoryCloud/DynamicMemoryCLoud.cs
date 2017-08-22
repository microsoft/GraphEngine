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

namespace Trinity.Storage
{
    public unsafe partial class DynamicMemoryCloud : MemoryCloud
    {
        //private int server_count = -1;
        private int partition_count = -1;    
        private int my_server_id = -1;
        private int my_proxy_id = -1;
        //private int my_partition_id = -1;
        Dictionary<int, int> server_in_partition = new Dictionary<int, int>();
        private List<List<int>> server_host_chunk = new List<List<int>>();
        //private List<int> my_hosting_chunks = new List<int>();
        
        private List<long> chunk_range = new List<long>();

        internal ClusterConfig cluster_config;

        public override IEnumerable<int> MyChunkIds
        {
            get
            {
                List<int> my_hosting_chunks = new List<int>();
                my_hosting_chunks = server_host_chunk[my_server_id];

                return my_hosting_chunks;
            }
        }

        public override int MyPartitionId
        {
            get
            {
                int my_partition_id = -1;
                my_partition_id = server_in_partition[my_server_id];

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

        

        public override int ServerCount
        {
            get
            {
                int server_count = -1;
                server_count = server_in_partition.Count;

                return server_count;
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
            return (StaticGetServerIdByCellId(cellId) == my_server_id);
        }

        public override bool Open(ClusterConfig config, bool nonblocking)
        {
            
            this.cluster_config = config;    
            my_server_id = 0;
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
            partition_count = 2;
            server_in_partition.Add(0, 0);
            server_in_partition.Add(1, 0);
            server_in_partition.Add(2, 1);





            bool server_found = false;
            int server_count = this.ServerCount;
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


        /// <summary>
        /// Gets the Id of the server on which the cell with the specified cell Id is located.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns>The Id of the server containing the specified cell.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetServerIdByCellIdDefault(long cellId)
        {
            int chunk_id = GetChunkIdByCellId(cellId);
            int partition_id = GetPartitionIdByCellId(cellId);
            int server_id = PickServerDefault(GetServerListByChunkAndPartition(partition_id, chunk_id));

            return server_id;
        }

        private int GetChunkIdByCellId(long cellId)
        {
            int chunk_id = 0;
            for (chunk_id = 0; chunk_id < ChunkCount; chunk_id++)
                if (cellId < chunk_range[chunk_id]) break;

            return chunk_id;
        }

        private IList<int> GetServerListByChunkAndPartition(int partitionId, int chunkId)
        {
            List<int> server_list = new List<int>();      
            int server_id = -1;
            for (server_id = 0; server_id < ServerCount; server_id++)
                if (server_in_partition[server_id] == partitionId)
                    if (server_host_chunk[server_id].Contains(chunkId))
                        server_list.Add(server_id);

            return server_list;
        }

        private int PickServerDefault (IList<int> serverList)
        {
            try
            {
                if (serverList.Contains(my_server_id)) return my_server_id;
                else return serverList[0];
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogLevel.Error, "An error occured when picking servers from the list.");
                Log.WriteLine(LogLevel.Error, ex.ToString());

                return -1;
            }
        }

        private int GetPartitionIdByCellId(long cellId)
        {
            return (*(((byte*)&cellId) + 1)) % partition_count;
        }

        static void Main(string[] args)
        {
            
        }
    }
}
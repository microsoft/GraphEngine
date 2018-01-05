using System;
using Trinity.Configuration;
using Trinity.DynamicCluster.Replication;

namespace Trinity.DynamicCluster.Config
{
    public class DynamicClusterConfig
    {
        private static DynamicClusterConfig s_instance = new DynamicClusterConfig();

        [ConfigInstance]
        public static DynamicClusterConfig Instance => s_instance;

        private DynamicClusterConfig()
        {
            ReplicationMode = ReplicationMode.Mirroring;
            MinimumReplica  = 2;
            BatchSaveSizeThreshold = 4.MiB();
            PersistedChunkSizeThreshold = 4.MiB();
            ConcurrentDownloads = 4 * Environment.ProcessorCount;
            ConcurrentUploads   = 4 * Environment.ProcessorCount;
            ConcurrentReplicationInPartition = 2;
        }

        [ConfigEntryName]
        internal static string ConfigEntry => nameof(Trinity.DynamicCluster);

        [ConfigSetting(Optional:true)]
        public int ConcurrentDownloads { get; set; }
        [ConfigSetting(Optional:true)]
        public int ConcurrentUploads { get; set; }
        [ConfigSetting(true)]
        public ReplicationMode ReplicationMode { get; set; }
        [ConfigSetting(true)]
        public int MinimumReplica { get; set; }
        [ConfigSetting(true)]
        public int ConcurrentReplicationInPartition { get; set; }
        [ConfigSetting(true)]
        public long BatchSaveSizeThreshold { get; set; }
        [ConfigSetting(true)]
        public long PersistedChunkSizeThreshold { get; set; }
    }
}

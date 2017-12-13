using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Configuration;
using Trinity.DynamicCluster.Storage;

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
        }

        [ConfigEntryName]
        internal static string ConfigEntry => nameof(Trinity.DynamicCluster);

        [ConfigSetting(true)]
        public ReplicationMode ReplicationMode { get; set; }
        [ConfigSetting(true)]
        public int MinimumReplica { get; set; }
    }
}

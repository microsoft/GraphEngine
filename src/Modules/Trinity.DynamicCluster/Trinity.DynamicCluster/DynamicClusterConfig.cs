using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Configuration;

namespace Trinity
{
    public class DynamicClusterConfig
    {
        private static DynamicClusterConfig _config = new DynamicClusterConfig();

        private DynamicClusterConfig()
        {
            // -1 means I have to discover my partition id.
            // TODO
            LocalPartitionId = -1;
        }

        [ConfigInstance]
        public static DynamicClusterConfig Instance => _config;

        [ConfigEntryName]
        public static string Name => "DynamicCluster";

        [ConfigSetting(Optional: true)]
        public int LocalPartitionId { get; internal set; }
        [ConfigSetting(Optional: false)]
        public int PartitionCount { get; set; }
    }
}

using System;
using Trinity.Configuration;

namespace Trinity.Azure.Storage
{
    public class BlobStorageConfig
    {
        static BlobStorageConfig s_instance = new BlobStorageConfig();
        private BlobStorageConfig() { }
        [ConfigInstance]
        public static BlobStorageConfig Instance => s_instance;
        [ConfigEntryName]
        internal static string ConfigEntry => "Trinity.Azure.Storage";
        [ConfigSetting(Optional:false)]
        public string ConnectionString { get; set; }
        [ConfigSetting(Optional:false)]
        public string ContainerName { get; set; }
    }
}

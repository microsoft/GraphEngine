using System;
using Trinity.Configuration;

namespace Trinity.Azure.Storage
{
    public class BlobStorageConfig
    {
        static BlobStorageConfig s_instance = new BlobStorageConfig();
        private BlobStorageConfig()
        {
            ConcurrentDownloads = 4 * Environment.ProcessorCount;
            ConcurrentUploads   = 4 * Environment.ProcessorCount;
        }
        [ConfigInstance]
        public static BlobStorageConfig Instance => s_instance;
        [ConfigEntryName]
        internal static string ConfigEntry => nameof(Trinity.Azure.Storage);
        [ConfigSetting(Optional:false)]
        public string ConnectionString { get; set; }
        [ConfigSetting(Optional:false)]
        public string ContainerName { get; set; }
        [ConfigSetting(Optional:true)]
        public int ConcurrentDownloads { get; set; }
        [ConfigSetting(Optional:true)]
        public int ConcurrentUploads { get; set; }
    }
}

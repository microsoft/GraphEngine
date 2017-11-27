using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Trinity.Configuration;
using Trinity.Utilities;

namespace GraphEngine.Configuration
{
    public sealed class PluginConfig
    {
        private static readonly PluginConfig s_config = new PluginConfig();
        private PluginConfig()
        {
            Plugin = new List<Plugin>();
            PackageRepository = "https://packages.nuget.org/api/v2";
            PluginInstallDirectory = Path.Combine(StorageConfig.Instance.StorageRoot, "plugins");
        }
        [ConfigEntryName]
        public static string Name => "Plugins";

        [ConfigInstance]
        public static PluginConfig Instance => s_config;
        [ConfigSetting(Optional: true)]
        public string PluginInstallDirectory { get; set; }
        [ConfigSetting(Optional: true)]
        public string PackageRepository { get; set; }
        [ConfigSetting(Optional: true)]
        public List<Plugin> Plugin { get; set; }
    }

    public sealed class Plugin
    {
        public Plugin()
        {
            Prerelease = false;
        }

        [ConfigSetting(Optional: true)]
        public string Package { get; set; }
        [ConfigSetting(Optional: true)]
        public bool Prerelease { get; set; }
        [ConfigSetting(Optional: true)]
        public string Local { get; set; }
    }
}

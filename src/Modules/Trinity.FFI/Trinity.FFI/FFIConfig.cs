using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Trinity.Configuration;

namespace Trinity.FFI
{
    public sealed class FFIConfig
    {
        #region Singleton
        private static FFIConfig s_instance = new FFIConfig();
        private FFIConfig() { ProgramDirectory = Path.Combine(StorageConfig.Instance.StorageRoot, "FFI"); }
        public static FFIConfig Instance { get { return s_instance; } }
        [ConfigEntryName]
        public static string ConfigEntry { get { return "FFI"; } }
        #endregion

        [ConfigSetting(Optional: true)]
        public string ProgramDirectory { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Trinity.FFI
{
    public static class Initializer
    {
        public static TrinityErrorCode Initialize(string config_path, string storage_root)
        {
            try
            {
                TrinityConfig.LoadConfig(config_path);
                TrinityConfig.StorageRoot = storage_root;
                Global.Initialize();
                return TrinityErrorCode.E_SUCCESS;
            }
            catch
            {
                return TrinityErrorCode.E_FAILURE;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Trinity;
using Trinity.Storage;

namespace config1
{
    public class config1
    {
        [Fact]
        public void save_config_no_exception()
        {
            TrinityConfig.SaveConfig();
        }
        [Fact]
        public void load_config_no_exception()
        {
            TrinityConfig.LoadConfig();
        }
        [Fact]
        // https://github.com/Microsoft/GraphEngine/issues/22
        public void can_set_non_accessible_storage_root()
        {
            TrinityConfig.StorageRoot = "Y:";
        }
    }
}

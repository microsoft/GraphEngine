using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Xunit;
using Trinity;
using Trinity.Storage;
using Trinity.Utilities;

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
        public void save_config_no_exception_2()
        {
            TrinityConfig.SaveConfig("test2.xml");
        }
        [Fact]
        public void load_config_no_exception()
        {
            TrinityConfig.LoadConfig();
        }
        [Fact]
        public void load_config_no_exception_2()
        {
            TrinityConfig.LoadConfig("test2.xml");
        }
        [Fact]
        // https://github.com/Microsoft/GraphEngine/issues/22
        public void can_set_non_accessible_storage_root()
        {
            TrinityConfig.StorageRoot = "Y:";
        }
        [Fact]
        public void saves_2_0_config_format()
        {
            TrinityConfig.SaveConfig("test.xml");
            Assert.True(File.ReadAllText(Path.Combine(AssemblyUtility.MyAssemblyPath, "test.xml")).Contains("ConfigVersion=\"2.0\""));
        }
    }
}

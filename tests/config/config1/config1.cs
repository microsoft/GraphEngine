using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using NUnit.Framework;
using Trinity;
using Trinity.Storage;

namespace config1
{
    public class config1
    {
        [Test]
        public void save_config_no_exception()
        {
            TrinityConfig.SaveConfig();
        }
        [Test]
        public void save_config_no_exception_2()
        {
            TrinityConfig.SaveConfig("test2.xml");
        }
        [Test]
        public void load_config_no_exception()
        {
            TrinityConfig.LoadConfig();
        }
        [Test]
        public void load_config_no_exception_2()
        {
            TrinityConfig.LoadConfig("test2.xml");
        }
        [Test]
        // https://github.com/Microsoft/GraphEngine/issues/22
        public void can_set_non_accessible_storage_root()
        {
            TrinityConfig.StorageRoot = "Y:";
        }
        [Test]
        public void saves_2_0_config_format()
        {
            TrinityConfig.SaveConfig("test.xml");
            Assert.True(File.ReadAllText("test.xml").Contains("ConfigVersion=\"2.0\""));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Trinity;
using Trinity.Storage;

namespace icall9
{
    public class icall9
    {
        [Test]
        public void icall_config_storageroot_set()
        {
            TrinityConfig.StorageRoot = ".";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Trinity;
using Trinity.Storage;

namespace icall8
{
    public class icall8
    {
        [Fact]
        public void icall_config_storageroot_get()
        {
            var val = TrinityConfig.StorageRoot;
        }
    }
}

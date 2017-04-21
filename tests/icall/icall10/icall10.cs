using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Trinity;
using Trinity.Storage;

namespace icall10
{
    public class icall10
    {
        [Fact]
        public void icall_config_storagecapacity_get()
        {
            var val = TrinityConfig.StorageCapacity;
        }
    }
}

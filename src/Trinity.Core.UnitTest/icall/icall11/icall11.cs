using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Trinity;
using Trinity.Storage;

namespace icall11
{
    public class icall11
    {
        [Fact]
        public void icall_config_readonly_get()
        {
            var val = TrinityConfig.ReadOnly;
        }
    }
}

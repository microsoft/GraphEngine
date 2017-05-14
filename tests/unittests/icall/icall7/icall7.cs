using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Trinity;
using Trinity.Storage;

namespace icall7
{
    public class icall7
    {
        [Test]
        public void icall_instantiate_local_storage()
        {
            var storage = Global.LocalStorage;
        }
    }
}

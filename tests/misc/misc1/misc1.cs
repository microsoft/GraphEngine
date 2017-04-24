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

namespace misc1
{
    public class misc1
    {
        [Fact]
        public void global_myassemblypath_is_functional()
        {
            var path = Global.MyAssemblyPath;
            Assert.True(Directory.GetFileSystemEntries(path).Any(_ => _.Contains("misc1")));
        }
    }
}

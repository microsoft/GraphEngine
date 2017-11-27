using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Trinity;
using Trinity.Storage;

namespace config2
{
    public class config2
    {
        [Test]
        public void LocalStorage_not_initialized_to_drive_root()
        {
            var drive_root = Path.GetPathRoot(Environment.CurrentDirectory);
            var A = Path.Combine(drive_root, "A");
            var B = Path.Combine(drive_root, "B");

            if(Directory.Exists(A)){
                Directory.Delete(A, recursive:true);
            }
            if(Directory.Exists(B)){
                Directory.Delete(B, recursive:true);
            }

            Global.LocalStorage.LoadStorage();

            Assert.False(Directory.Exists(A));
            Assert.False(Directory.Exists(B));
        }
    }
}

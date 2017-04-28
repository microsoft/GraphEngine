using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Trinity;
using Trinity.Storage;

namespace cleanup5
{
    public class cleanup5
    {
        [TestCase(10)]
        [TestCase(50)]
        [TestCase(100)]
        public void InitUninitStressTest(int count)
        {
            for (int i=0; i<count; ++i)
            {
                Global.Initialize();
                for (int j=0; j<10000; ++j)
                {
                    Global.LocalStorage.SaveCell(j, new byte[128]);
                }
                Global.Uninitialize();
            }
        }
    }
}

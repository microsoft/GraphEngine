using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster
{
    internal class Utils
    {
        public static IEnumerable<T> Infinity<T>(Func<T> gen)
        {
            while (true)
            {
                yield return gen();
            }
        }
    }
}

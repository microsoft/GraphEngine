using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster
{
    public static class Utils
    {
        public static IEnumerable<T> Infinity<T>(Func<T> IO)
        {
            while (true)
            {
                yield return IO();
            }
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach(var obj in list)
            {
                action(obj);
            }
            return list;
        }

        public static IEnumerable<int> Integers()
        {
            int i = 0;
            while (true)
            {
                yield return i++;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.DynamicCluster.Consensus;

namespace Trinity.DynamicCluster
{
    public static class Utils
    {
        public static void Deconstruct<T>(this T[] array, out T t1, out T t2)
        {
            t1 = array[0];
            t2 = array[1];
        }

        public static void Deconstruct<T>(this T[] array, out T t1, out T t2, out T t3)
        {
            t1 = array[0];
            t2 = array[1];
            t3 = array[2];
        }

        public static IEnumerable<T> Infinity<T>(Func<T> IO)
        {
            while (true)
            {
                yield return IO();
            }
        }

        public static IEnumerable<T> Infinity<T>() where T: new()
        {
            return Infinity(New<T>);
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach(var obj in list)
            {
                action(obj);
            }
            return list;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> list, Action<int, T> action)
        {
            int idx = 0;
            foreach(var obj in list)
            {
                action(idx++, obj);
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

        public static IEnumerable<int> Integers(int count)
        {
            return Integers().Take(count);
        }

        public static T New<T>() where T: new()
        {
            return new T();
        }

        public static T New<Tin, T>(Tin arg) where T: new()
        {
            return new T();
        }

        public static T Identity<T>(T _) => _;

        public static Guid GetMachineGuid()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            int hash = 0;
            foreach (var net in interfaces)
            {
                var mac = net.GetPhysicalAddress();
                hash = (hash << 5) ^ mac.GetHashCode();
            }
            Random r = new Random(hash);
            byte[] b = new byte[16];
            r.NextBytes(b);
            return new Guid(b);
        }

        public static string GenerateNickName(Guid instanceId)
        {
            string[] names;
            using (var sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Trinity.DynamicCluster.Resources.names.txt")))
            {
                names = sr.ReadToEnd().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            }
            int seed = (int)HashHelper.HashString2Int64(instanceId.ToString());
            Random r = new Random(seed);
            var n1 = names[r.Next(names.Length)];
            var n2 = names[r.Next(names.Length)];

            return n1 + " " + n2;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.Diagnostics;
using Trinity.DynamicCluster.Consensus;

namespace Trinity.DynamicCluster
{
    public static class Utils
    {
        /// <summary>
        /// Spawns a daemon task running intervally, until the cancellation token is fired.
        /// </summary>
        public static async Task Daemon(CancellationToken cancel, string name, int delay, Func<Task> daemonProc)
        {
            while (!cancel.IsCancellationRequested)
            {
                try
                {
                    await daemonProc();
                    await Task.Delay(delay, cancel);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, $"{name}: {ex.ToString()}");
                    await Task.Delay(1000, cancel);
                }
            }
        }

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

        public static IEnumerable<(T1, T2)> ZipWith<T1, T2>(this IEnumerable<T1> i1, IEnumerable<T2> i2)
        {
            return i1.Zip(i2, (e1, e2) => (e1, e2));
        }

        public static IEnumerable<T> Infinity<T>(Func<T> IO)
        {
            while (true)
            {
                yield return IO();
            }
        }

        public static IEnumerable<T> Infinity<T>() where T : new()
        {
            return Infinity(New<T>);
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var obj in list)
            {
                action(obj);
            }
            return list;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> list, Action<int, T> action)
        {
            int idx = 0;
            foreach (var obj in list)
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

        public static T New<T>() where T : new()
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

        public static T Deserialize<T>(byte[] payload)
        {
            IFormatter fmt = new BinaryFormatter();
            using (var ms = new MemoryStream(payload))
            {
                return (T)fmt.Deserialize(ms);
            }
        }

        public static byte[] Serialize<T>(T payload)
        {
            IFormatter fmt = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                fmt.Serialize(ms, payload);
                return ms.ToArray();
            }
        }

        public static async Task WhenAll(this IEnumerable<Task> tasks)
        {
            await Task.WhenAll(tasks.ToArray());
        }

        public static async Task<T[]> Unwrap<T>(this IEnumerable<Task<T>> tasks)
        {
            var ta = tasks.ToArray();
            await Task.WhenAll(ta);
            return tasks.Select(_ => _.Result).ToArray();
        }

        public static IEnumerable<Task> Then(this IEnumerable<Task> tasks, Func<Task> func)
        {
            return tasks.Select(t => t.ContinueWith(t_ => func()).Unwrap());
        }

        public static IEnumerable<Task<T>> Then<T>(this IEnumerable<Task> tasks, Func<Task<T>> func)
        {
            return tasks.Select(t => t.ContinueWith(t_ => func()).Unwrap());
        }

        public static IEnumerable<Task<Tout>> Then<Tin, Tout>(this IEnumerable<Task<Tin>> tasks, Func<Tin, Task<Tout>> func)
        {
            return tasks.Select(t => t.ContinueWith(t_ => func(t_.Result)).Unwrap());
        }

        public static IEnumerable<Task> Then<Tin>(this IEnumerable<Task<Tin>> tasks, Action<Tin> func)
        {
            return tasks.Select(t => t.ContinueWith(t_ => func(t_.Result)));
        }

        public static IEnumerable<Task> Then(this IEnumerable<Task> tasks, Action func)
        {
            return tasks.Select(t => t.ContinueWith(t_ => func()));
        }
    }
}

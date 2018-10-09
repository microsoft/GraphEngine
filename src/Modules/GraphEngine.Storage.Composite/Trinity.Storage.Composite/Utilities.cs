using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Trinity.Diagnostics;

namespace Trinity.Storage.Composite
{
    public static class Infix
    {
        public static G By<T, G>(this T obj, Func<T, G> fn) => fn(obj);
        public static void By<T>(this T obj, Action<T> fn) => fn(obj);

        public static G When<T, G>
            (this T obj, Func<T, bool> cond, Func<T, G> then, Func<T, G> otherwise = null)
             => cond(obj) ? then(obj) : otherwise != null ? otherwise(obj) : default(G);
        public static void When<T>
            (this T obj, Func<T, bool> cond, Action<T> then, Action<T> otherwise = null)
        {
            if (cond(obj))
                then(obj);
            else
                otherwise?.Invoke(obj);
        }

        #region sequential
        public static void Each<T>(this IEnumerable<T> collection, Action<T> fn, Action end = null)
        {
            foreach (var e in collection) { fn(e); }
            end?.Invoke();
        }
        public static void Each<ElemType, State>(this IEnumerable<ElemType> collection,
                                                 Action<ElemType, State> fn,
                                                 Action<State> end = null) where State : new()
        {
            State s = new State();
            foreach (var e in collection) { fn(e, s); }
            end?.Invoke(s);
        }
        #endregion

        #region composing functions
        public static Func<T, P> AndThen<T, G, P>(this Func<T, G> f1, Func<G, P> f2) => (_ => f2(f1(_)));
        public static Action<T> AndThen<T, G>(this Func<T, G> f1, Action<G> f2) => (_ => f2(f1(_)));
        #endregion

        #region curring
        public static Func<G, P> Curry<T, G, P>
        (this Func<T, G, P> fn, T arg) => _ => fn(arg, _);
        public static Func<T2, T3, R> Curry<T1, T2, T3, R>
        (this Func<T1, T2, T3, R> fn, T1 a1) => (a2, a3) => fn(a1, a2, a3);

        public static Func<T2, T3, T4, R> Curry<T1, T2, T3, T4, R>
        (this Func<T1, T2, T3, T4, R> fn, T1 a1) => (a2, a3, a4) => fn(a1, a2, a3, a4);
        #endregion

    }

    public static class Utils
    {
        private const string s_session_flag_file = ".session.inprogress";

        public static string MyAssemblyVersion()
        {
            var version = typeof(Utils).Assembly.GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }

        public static void Session(string path, Action start, Action<Exception> err, Action end, Action behavior, Action final = null)
        {
            Log.WriteLine(LogLevel.Verbose, $"{nameof(CompositeStorage)}: Begin I/O session.");
            string flag = Path.Combine(path, s_session_flag_file);
            string backup_ext = ".bak";
            var backups = Directory.GetFiles(path, $"*{backup_ext}");

            if (File.Exists(flag))
            {
                Log.WriteLine(LogLevel.Warning, $"{nameof(CompositeStorage)}: Interrupted I/O session detected, recovering from backup.");
                backups.Each(f =>
                {
                    var newfile = f.Substring(0, f.Length - backup_ext.Length);
                    File.Copy(f, newfile, overwrite: true);
                });
                Log.WriteLine(LogLevel.Debug, $"{nameof(CompositeStorage)}: Recovery complete.");
            }

            // remove old backup, and backup current folder content
            backups.Each(File.Delete);
            var old = Directory.GetFiles(path);
            old.Each(f => File.Copy(f, f + backup_ext));
            File.WriteAllBytes(flag, new byte[] { 0x01 });

            bool session_complete = false;

            try
            {
                start();
                behavior();
                end();
                session_complete = true;
            }
            catch (Exception e)
            {
                err(e);
            }

            final?.Invoke();
            if (session_complete)
            {
                File.Delete(flag);
                Log.WriteLine(LogLevel.Verbose, $"{nameof(CompositeStorage)}: Finish I/O session.");
            }
        }
    }

    public static class Serialization
    {
        static IFormatter formatter = new BinaryFormatter();
        public static void Serialize<T>(T obj, string fileName)
        {
            using (Stream stream = new FileStream(fileName,
                                           FileMode.Create,
                                           FileAccess.Write,
                                           FileShare.None))
            {
                formatter.Serialize(stream, obj);
            }
        }

        public static T Deserialize<T>(string fileName)
        {
            using (Stream stream = new FileStream(fileName,
                                           FileMode.Open,
                                           FileAccess.Read,
                                           FileShare.Read))
            {
                return (T)formatter.Deserialize(stream);
            }
        }
    }


}

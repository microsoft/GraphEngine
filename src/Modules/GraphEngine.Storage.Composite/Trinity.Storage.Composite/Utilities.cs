using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Trinity.Diagnostics;

namespace Trinity.Storage.CompositeExtension
{

    public static class LinqExtension
    {
        public static G Apply<T, G>(this T obj, Func<T, G> fn) => fn(obj);
        public static void Apply<T>(this T obj, Action<T> fn) => fn(obj);


        public static G WhenNotDefault<T, G>(this T obj, Func<T, G> fn) where T: class
        {
            if (obj != default(T))
                return fn(obj);
            return default(G);
        }
        public static void WhenNotDefault<T>(this T obj, Action<T> fn) where T: class{
            if (obj != default(T))
                fn(obj);
        }
    }
    public static class Utilities
    {
        static IFormatter formatter = new BinaryFormatter();
        public static void Serialize<T>(T obj, string fileName) {
            try
            {
                using (Stream stream = new FileStream(fileName,
                                               FileMode.Create,
                                               FileAccess.Write,
                                               FileShare.None))
                {
                    formatter.Serialize(stream, obj);
                }
            }
            catch(Exception e)
            {
                Log.WriteLine(e.Message);
            }
                
        }

        public static T Deserialize<T>(string fileName)
        {
            try
            {
                using (Stream stream = new FileStream(fileName,
                                               FileMode.Open,
                                               FileAccess.Read,
                                               FileShare.Read))
                {
                    return (T)formatter.Deserialize(stream);
                }
            }
            catch(Exception e)
            {
                Log.WriteLine(e.Message);
                return default(T);
            }
                
        }

    }
}

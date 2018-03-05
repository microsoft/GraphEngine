using System;
using System.Collections.Generic;
using System.IO;
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
        public static void Each<ElemType>
        (this IEnumerable<ElemType> collection, Action<ElemType> fn, Action end = null)
        {
            foreach (var e in collection)
                fn(e);
            end?.Invoke();
        }
        public static void Each<ElemType, State>
        (this IEnumerable<ElemType> colllection,
         Action<ElemType, State> fn,
         Action<State> end = null) where State : new()
        {
            State s = new State();
            foreach (var e in colllection)
                fn(e, s);
            end?.Invoke(s);
        }
        #endregion

        #region composing functions
        public static Func<T, P> AndThen<T, G, P>(this Func<T, G> f1, Func<G, P> f2) => (_ => f2(f1(_)));
        public static Action<T> AndThen<T, G>(this Func<T, G> f1, Action<G> f2) => (_ => f2(f1(_)));
        #endregion



        #region curring
        public static Func<G, P> ToCurrying<T, G, P>
        (this Func<T, G, P> fn, T arg) => _ => fn(arg, _);
        public static Func<T2, T3, R> ToCurrying<T1, T2, T3, R>
        (this Func<T1, T2, T3, R> fn, T1 a1) => (a2, a3) => fn(a1, a2, a3);

        public static Func<T2, T3, T4, R> ToCurrying<T1, T2, T3, T4, R>
        (this Func<T1, T2, T3, T4, R> fn, T1 a1) => (a2, a3, a4) => fn(a1, a2, a3, a4);
        #endregion

    }

    public static class LogisticHandler
    {

        public static G WhenNotDefault<T, G>(this T obj, Func<T, G> fn) where T : class
        {
            if (obj != default(T))
                return fn(obj);
            return default(G);
        }
        public static void WhenNotDefault<T>(this T obj, Action<T> fn) where T : class
        {
            if (obj != default(T))
                fn(obj);
        }

        public static void Session(Action start, Action<Exception> err, Action end, Action behavior, Action final = null)
        {
            start();
            try
            {
                behavior();
                end();
            }
            catch (Exception e)
            {
                err(e);
            }
            final?.Invoke();
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

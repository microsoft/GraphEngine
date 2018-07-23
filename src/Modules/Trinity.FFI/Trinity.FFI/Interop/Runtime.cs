using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Trinity.FFI.Interop
{
    [FFIPrefix("rt_")]
    internal static class Runtime
    {
        private static HashSet<Delegate> s_delegates = new HashSet<Delegate>();

        public static TrinityErrorCode getfunction(string assembly, string klass, string method)
        {
            //AppDomain.CurrentDomain.GetAssemblies()
            //    .Where(_ => _.GetName().Name == assembly)
            //    .SelectMany(_ => _.GetTypes())
            //    .Where(_ => _.FullName == klass)
            //    .SelectMany(_ => _.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
            //    .Where(_ => _.Name == method && !_.IsGenericMethod)
            //    .Select(_ => _.CreateDelegate(Expression.GetDelegateType(
            //        _.GetParameters()
            //        .Select(p => p.ParameterType)
            //        .Concat(new Type[] { _.ReturnType })
            //        .ToArray()))
            return TrinityErrorCode.E_SUCCESS;
        }
    }
}

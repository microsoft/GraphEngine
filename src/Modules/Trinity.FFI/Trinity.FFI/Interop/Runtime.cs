using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;

namespace Trinity.FFI.Interop
{
    [FFIPrefix("rt_")]
    internal static class Runtime
    {
        private static HashSet<Delegate> s_delegates = new HashSet<Delegate>();

        [FFIExport]
        public static TrinityErrorCode getfunction(string assembly, string klass, string method, out IntPtr lpfunc)
        {
            try
            {
                var d = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(AssemblyNameMatch(assembly))
                .SelectMany(_ => _.GetTypes())
                .Where(_ => _.FullName == klass)
                .SelectMany(_ => _.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
                .Where(_ => _.Name == method && !_.IsGenericMethod)
                .Select(_ => _.CreateDelegate(Expression.GetDelegateType(
                    _.GetParameters()
                    .Select(p => p.ParameterType)
                    .Concat(new Type[] { _.ReturnType })
                    .ToArray()), null))
                .FirstOrDefault();

                lpfunc = Marshal.GetFunctionPointerForDelegate(d);
                return TrinityErrorCode.E_SUCCESS;
            }
            catch
            {
                lpfunc = IntPtr.Zero;
                return TrinityErrorCode.E_FAILURE;
            }
        }

        private static Func<System.Reflection.Assembly, bool> AssemblyNameMatch(string assembly)
        {
            return _ => _.GetName().Name == assembly || _.GetName().FullName == assembly;
        }
    }
}

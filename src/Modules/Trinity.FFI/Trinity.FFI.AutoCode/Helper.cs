using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL.Lib;

namespace Trinity.FFI.AutoCode
{
    static class Helper
    {
        internal static string GenerateName(MethodInfo m)
        {
            var name = m.Name;
            var prefix = m.DeclaringType.GetCustomAttribute<FFIPrefixAttribute>();
            if (prefix == null) throw new ArgumentException($"No prefix found for method {name}");
            return prefix.Prefix + name;
        }

        internal static string GenerateReturnType(MethodInfo m)
            => (m.ReturnType == typeof(void)) ? "void" : m.ReturnType.Name;

        internal static string GenerateParameterList(MethodInfo m)
            => string.Join(", ", m.GetParameters().Select(GenerateParameter));

        internal static string GenerateParameter(ParameterInfo p)
        {
            string ret = "";
            var t = p.ParameterType;

            if (t == typeof(CellAccessOptions)) ret += "[MarshalAs(UnmanagedType.I4)]";
            ret += _signature(t, p.IsOut);

            ret += " p" + p.Position;
            return ret;
        }

        internal static string GenerateReturnTypeCpp(MethodInfo m)
            => _signature_cpp(m.ReturnType);

        internal static string GenerateParameterListCpp(MethodInfo m)
            => string.Join(", ", m.GetParameters().Select(GenerateParameterCpp));

        internal static string GenerateParameterCpp(ParameterInfo p)
        {
            return _signature_cpp(p.ParameterType);
        }

        private static string _signature(Type t, bool isout = false)
        {
            var e = t.GetElementType();

            if (isout) return "out " + _signature(e);
            if (t.IsByRef) return "ref " + _signature(e);
            if (t.IsArray) return _signature(e) + "[]";
            if (t.IsPointer) throw new NotSupportedException();
            if (t.IsGenericType) throw new NotSupportedException();
            if (t.HasElementType) throw new NotSupportedException();

            return t.Name;
        }

        private static string _signature_cpp(Type t)
        {
            var e = t.GetElementType();
            if (t.IsByRef) { return _signature_cpp(e) + "*"; }
            else if (t.IsPointer) throw new NotSupportedException();
            else if (t.IsArray) { return "void*"; }
            else if (t.IsGenericType) throw new NotSupportedException();
            else if (t.HasElementType) throw new NotSupportedException();

            // go through conversion map
            switch (t.Name)
            {
                case "IntPtr": return "void*";
                case "String": return "char*";
                case "Int64": return "long long";
                case "Int32": return "long";
                case "Void": return "void";
                default: return t.Name;
            }
        }
    }
}

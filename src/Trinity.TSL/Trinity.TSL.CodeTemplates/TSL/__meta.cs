using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.TSL
{
    public class __meta
    {
        internal static readonly int LT_DYNAMIC = 0;

        internal static void LITERAL_OUTPUT(string literal) { }

        internal static void MUTE() { }

        internal static void MUTE_END() { }

        internal static void FOREACH(string separator = "") { }

        internal static void END() { }

        internal static void IF(bool val) { }

        internal static void IF(string val) { }

        internal static void ELSE() { }

        internal static void ELIF(string val) { }

        internal static void ELIF(bool val) { }

        internal static void MODULE_CALL(string module, string target, params object[] parameters) { }

        internal static void MODULE_BEGIN() { }

        internal static void MODULE_END() { }

        internal static void META(string cmd) { }

        internal static void META_VAR(string type, string name, string value = "") { }

        internal static t_field_type META_OUTPUT(string cmd) { throw new NotImplementedException(); }

        internal static void TARGET(string type) { }

        internal static void TARGET_PUSH(string new_type, string new_target) { }

        internal static void TARGET_POP() { }

        internal static void USE_LIST(string listvar = "") { }

        internal static uint GET_ITERATOR_VALUE() { return 0; }

        internal static uint GET_ITERATOR_LENGTH() { return 0; }

        internal static void MAP_LIST(string from, string to, string MemberOf = "") { }

        internal static void MAP_VAR(string from, string to, string MemberOf = "") { }

        internal const uint t_uint = 0;

        internal const int t_int = 0;
        internal const int t_int_2 = 0;
        internal const int t_int_3 = 0;
        internal const int t_int_4 = 0;
        internal const bool t_bool = false;

        internal static string MemberOf = "";

        internal static int t_array_dimension_list = 0;
        internal const int t_ushort_2 = 0;
        internal const int t_ushort = 1;

        internal static void ___END()
        {
            throw new NotImplementedException();
        }

        internal static void ___ELSE()
        {
            throw new NotImplementedException();
        }

        internal static void ___IF(string p)
        {
            throw new NotImplementedException();
        }

        internal static void __IF(string p)
        {
            throw new NotImplementedException();
        }

        internal static void _IF(string p)
        {
            throw new NotImplementedException();
        }

        internal static void _ELSE()
        {
            throw new NotImplementedException();
        }

        internal static void _ELIF(string p)
        {
            throw new NotImplementedException();
        }

        internal static void __END()
        {
            throw new NotImplementedException();
        }
        internal static void _END()
        {
            throw new NotImplementedException();
        }

        internal static void __ELSE()
        {
            throw new NotImplementedException();
        }


        internal static void ____END()
        {
            throw new NotImplementedException();
        }

        internal static void ____ELSE()
        {
            throw new NotImplementedException();
        }

        internal static void ____ELIF(string p)
        {
            throw new NotImplementedException();
        }

        internal static void ________END()
        {
            throw new NotImplementedException();
        }

        internal static void ________ELSE()
        {
            throw new NotImplementedException();
        }

        internal static void ____________END()
        {
            throw new NotImplementedException();
        }

        internal static void ____________ELSE()
        {
            throw new NotImplementedException();
        }

        internal static void ____________IF(string p)
        {
            throw new NotImplementedException();
        }

        internal static void ________IF(string p)
        {
            throw new NotImplementedException();
        }

        internal static void ____IF(string p)
        {
            throw new NotImplementedException();
        }
    }
}

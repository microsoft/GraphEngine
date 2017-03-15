using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.TSL
{
    public static class __string_extension
    {
        public static int GetLength(this string x, uint dim)
        {
            throw new NotImplementedException();
        }
    }

    public struct t_data_type
    {
        public static bool operator ==(t_data_type a, int b)
        {
            throw new Exception();
        }

        public static bool operator !=(t_data_type a, int b)
        {
            throw new Exception();
        }

        public static implicit operator string(t_data_type x)
        {
            return "";
        }

        public static implicit operator t_accessor_type(t_data_type x)
        {
            throw new NotFiniteNumberException();
        }

        public static implicit operator t_data_type(BitArray x)
        {
            throw new NotImplementedException();
        }

        public static implicit operator t_data_type(DateTime x)
        {
            throw new NotImplementedException();
        }
        
        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public string this[int index]
        {
            get
            {
                return "";
            }
            set
            {

            }
        }

        internal void Add(string p)
        {
            throw new NotImplementedException();
        }

        internal static string Parse(string p)
        {
            throw new NotImplementedException();
        }

        internal void Add(t_Namespace.t_data_type_list_element_type element)
        {
            throw new NotImplementedException();
        }

        internal int GetLength(uint p)
        {
            throw new NotImplementedException();
        }

        public t_field_type t_field_name { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }


        public unsafe t_field_type Value { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public bool HasValue { get; set; }
        public unsafe void* CellPtr { get; internal set; }

        internal void Set(int idx, bool p)
        {
            throw new NotImplementedException();
        }

        internal string ToString(string p)
        {
            throw new NotImplementedException();
        }

        internal unsafe t_field_type_2 ToList()
        {
            throw new NotImplementedException();
        }

        internal string ToString(string p, CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
        }

        internal string ToString(CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
        }
    }

    /**
     * If a t_data_type is nullable, t_data_type_remove_nullable returns
     * the typestring of the corresponding non-nullable value type.
     */

    public class t_data_type_remove_nullable
    {
        // Specific for DateTime.TryParse
        internal static bool TryParse(string s, IFormatProvider provider, DateTimeStyles styles, out t_data_type_remove_nullable result)
        {
            throw new NotImplementedException();
        }
        internal static bool TryParse(string s, out t_data_type_remove_nullable value_type_value)
        {
            throw new NotImplementedException();
        }
        public static implicit operator t_data_type(t_data_type_remove_nullable x)
        {
            throw new NotImplementedException();
        }

        internal static bool TryParse(string s, out t_data_type_display value_type_value)
        {
            throw new NotImplementedException();
        }
    }

    public class t_data_type_display
    {
        internal static bool TryParse(string s, out t_data_type_display value_type_value)
        {
            throw new NotImplementedException();
        }
        public static implicit operator t_data_type(t_data_type_display x)
        {
            throw new NotImplementedException();
        }
    }
}

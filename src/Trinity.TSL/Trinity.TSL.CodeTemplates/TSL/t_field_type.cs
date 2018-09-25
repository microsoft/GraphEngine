using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network.Messaging;
using Trinity.TSL.Lib;

#pragma warning disable

namespace Trinity.TSL
{
    public class t_field_type_display
    {
        internal static unsafe void TryParse(string p, out t_field_type t_field_type)
        {
            throw new NotImplementedException();
        }

        public static implicit operator string(t_field_type_display x)
        {
            throw new NotImplementedException();
        }

    }

    public unsafe class t_field_type_2 : t_field_type
    {
        internal t_field_type this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        internal static unsafe bool TryParse(t_field_type value, out t_field_type_2 intermediate_result)
        {
            throw new NotImplementedException();
        }

        internal void Add(t_Namespace.t_field_type_2_element_type intermediate_result)
        {
            throw new NotImplementedException();
        }

        internal void Add(object p)
        {
            throw new NotImplementedException();
        }

        public static implicit operator t_field_type_2(string x)
        {
            throw new NotImplementedException();
        }

        public static implicit operator t_field_type_2(bool x)
        {
            throw new NotImplementedException();
        }
    }

    public unsafe class t_field_type_remove_nullable : t_field_type
    {

    }

    public unsafe class t_field_type
    {
        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        unsafe public delegate byte* ResizeDelegate(byte* a, int b, int c);
        unsafe public t_field_type(object foo, ResizeDelegate bar) { }
        unsafe public t_field_type() { }

        internal byte* m_ptr;
        public long CellId;

        internal unsafe ResizeFunctionDelegate ResizeFunction = null;
        public unsafe t_field_type t_member_name;
        public t_data_type t_field_name;

        public static implicit operator TrinityMessageType(t_field_type x)
        {
            return TrinityMessageType.ASYNC;
        }

        public static implicit operator string(t_field_type x)
        {
            return "";
        }

        public static implicit operator bool(t_field_type x)
        {
            return true;
        }

        public static implicit operator long(t_field_type x)
        {
            return 0;
        }

        public static implicit operator int(t_field_type x)
        {
            return 0;
        }

        public static implicit operator t_field_type(bool x)
        {
            throw new NotImplementedException();
        }


        public static bool operator ==(t_field_type x, string y)
        {
            throw new NotImplementedException();
        }

        public static bool operator !=(t_field_type x, string y)
        {
            throw new NotImplementedException();
        }

        public static implicit operator t_data_type(t_field_type x)
        {
            return new t_data_type();
        }

        public static implicit operator t_field_type(t_data_type x)
        {
            return new t_field_type();
        }

        public static implicit operator t_field_type(string x)
        {
            return new t_field_type();
        }

        public static bool TryParse(string s, out t_field_type x)
        {
            x = new t_field_type();
            return true;
        }

        internal static unsafe void TryParse_t_field_type_display(string p, out t_field_type t_field_type)
        {
            throw new NotImplementedException();
        }

        internal int GetLength(uint p)
        {
            throw new NotImplementedException();
        }

        internal void Add(object value)
        {
            throw new NotImplementedException();
        }

        internal void AddRange(object value)
        {
            throw new NotImplementedException();
        }

        internal void Add(string p)
        {
            throw new NotImplementedException();
        }

        internal void Add(t_Namespace.t_field_type_list_element_type parseResult)
        {
            throw new NotImplementedException();
        }

        internal void Add(t_Namespace.t_data_type_list_element_type element)
        {
            throw new NotImplementedException();
        }

        public t_field_type Value { get; set; }

        public bool HasValue { get; set; }

        internal void Remove_t_member_name()
        {
            throw new NotImplementedException();
        }

        internal char ToString(string p)
        {
            throw new NotImplementedException();
        }

        internal char[] ToString(string p, System.Globalization.CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
        }
    }
}

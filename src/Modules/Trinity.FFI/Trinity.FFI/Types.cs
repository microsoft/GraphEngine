using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trinity.Core.Lib;

namespace Trinity.FFI
{
    public enum TYPECODE : byte
    {
        /* NULL */
        NULL,
        /* Numbers */
        U8,
        I8,
        U16,
        I16,
        U32,
        I32,
        U64,
        I64,
        F32,
        F64,

        CHAR,
        STRING,
        BOOL,
        TUPLE,
        LIST,
    }

    public unsafe static class TypeCodec
    {
        public static void FreeTypeCode(ulong* up)
        {
            Memory.free(up);
        }

        private static List<TYPECODE> EncodeType_impl(Type T)
        {
            List<TYPECODE> code = new List<TYPECODE>();

            if(T == typeof(byte))
                code.Add(TYPECODE.U8);

            if(T == typeof(sbyte))
                code.Add(TYPECODE.I8);

            if(T == typeof(ushort))
                code.Add(TYPECODE.U16);

            if(T == typeof(short))
                code.Add(TYPECODE.I16);

            if(T == typeof(uint))
                code.Add(TYPECODE.U32);

            if(T == typeof(int))
                code.Add(TYPECODE.I32);

            if(T == typeof(ulong))
                code.Add(TYPECODE.U64);

            if(T == typeof(long))
                code.Add(TYPECODE.I64);

            if(T == typeof(float))
                code.Add(TYPECODE.F32);

            if(T == typeof(double))
                code.Add(TYPECODE.F64);

            if(T == typeof(char))
                code.Add(TYPECODE.CHAR);

            if(T == typeof(string))
                code.Add(TYPECODE.STRING);

            if(T == typeof(bool))
                code.Add(TYPECODE.BOOL);

            if(T.IsConstructedGenericType && T.GetGenericTypeDefinition() == typeof(List<>))
            {
                code.Add(TYPECODE.LIST);
                code.AddRange(EncodeType_impl(T.GetGenericArguments().First()));
            }

            if(!T.IsAbstract && (T.IsClass || (T.IsValueType && !T.IsPrimitive && !T.IsEnum)))
            {
                code.Add(TYPECODE.TUPLE);
                List<List<TYPECODE>> fields = new List<List<TYPECODE>>();
                foreach(var field in T.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    fields.Add(EncodeType_impl(field.FieldType));
                }
                if(fields.Count > 255)
                    throw new ArgumentException($"Too many fields in Tuple type {T.ToString()}");
                code.Add((TYPECODE)fields.Count);
                foreach(var field in fields)
                {
                    code.AddRange(field);
                }
            }

            // Throw if we don't pick up anything
            if(code.Count == 0)
                throw new ArgumentException($"Cannot encode the specified type {T.ToString()}");

            // Pad it to ulong
            while(code.Count % sizeof(ulong) != 0)
            {
                code.Add(TYPECODE.NULL);
            }

            return code;
        }

        public static ulong* EncodeType<T>()
        {
            return EncodeType(typeof(T));
        }

        public static ulong* EncodeType(Type T)
        {
            List<TYPECODE> code = EncodeType_impl(T);
            fixed(TYPECODE* bp = code.ToArray())
            {
                ulong* up = (ulong*)Memory.malloc((ulong)code.Count);
                Memory.memcpy(up, bp, (ulong)code.Count);
                return up;
            }
        }

        private static Type DecodeType_impl(ref TYPECODE* p)
        {
            switch (*p++)
            {
                case TYPECODE.BOOL:
                    return typeof(bool);
                case TYPECODE.CHAR:
                    return typeof(char);
                case TYPECODE.STRING:
                    return typeof(string);
                case TYPECODE.F32:
                    return typeof(float);
                case TYPECODE.F64:
                    return typeof(double);
                case TYPECODE.I8:
                    return typeof(sbyte);
                case TYPECODE.U8:
                    return typeof(byte);
                case TYPECODE.I16:
                    return typeof(short);
                case TYPECODE.U16:
                    return typeof(ushort);
                case TYPECODE.I32:
                    return typeof(int);
                case TYPECODE.U32:
                    return typeof(uint);
                case TYPECODE.I64:
                    return typeof(long);
                case TYPECODE.U64:
                    return typeof(ulong);
                case TYPECODE.LIST:
                    return typeof(List<>).MakeGenericType(DecodeType_impl(ref p));
                case TYPECODE.TUPLE:
                    {
                        byte cnt = (byte)*(p++);
                        Type[] elements = new Type[cnt];
                        for(int i=0;i<cnt;++i)
                        {
                            elements[i] = DecodeType_impl(ref p);
                        }
                        return typeof(ValueTuple<>).MakeGenericType(elements);
                    }
                default:
                    throw new ArgumentException("Cannot decode type");
            }
        }

        public static Type DecodeType(void* T)
        {
            TYPECODE* p = (TYPECODE*)T;
            return DecodeType_impl(ref p);
        }
    }
}

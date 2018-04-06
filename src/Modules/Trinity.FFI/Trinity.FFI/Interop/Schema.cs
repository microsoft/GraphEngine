using GraphEngine.Jit;
using GraphEngine.Jit.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Trinity.Core.Lib;
using Trinity.Storage;
using static GraphEngine.Jit.TypeSystem;

namespace Trinity.FFI.Interop
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct NativeAttributeDescriptor
    {
        internal byte* Name;
        internal byte* Value;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct NativeMemberDescriptor
    {
        internal byte* Name;
        internal NativeTypeDescriptor Type;
        internal byte Optional;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct NativeTypeDescriptor
    {
        internal byte* TypeName;
        internal byte* QualifiedName; // Full clr type name
        internal NativeTypeDescriptor* ElementType;
        internal NativeMemberDescriptor* Members;
        internal NativeAttributeDescriptor* TSLAttributes;
        internal int NrMember;
        internal int NrTSLAttribute;
        internal int ElementArity;
        internal int TypeCode;

        internal static NativeTypeDescriptor Make(ICellDescriptor desc)
            => GraphEngine.Jit.TypeSystem.Make(desc).ToNative();
    }

    internal static unsafe class InterpExtension
    {
        internal static NativeTypeDescriptor ToNative(this TypeDescriptor desc)
        {
            var ret = new NativeTypeDescriptor
            {
                TypeName = desc.TypeName.ToUtf8(),
                QualifiedName = desc.QualifiedName.ToUtf8(),
                TypeCode = desc.TypeCode.Tag,
                NrMember = desc.Members.Count(),
                NrTSLAttribute = desc.TSLAttributes.Count(),
                ElementArity = desc.ElementType.Count(),
            };

            if (ret.NrMember == 0) ret.Members = null;
            else ret.Members = (NativeMemberDescriptor*)Memory.malloc((ulong)ret.NrMember * (ulong)sizeof(NativeMemberDescriptor));
            for (int i = 0; i<ret.NrMember; ++i) { ret.Members[i] = desc.Members.ElementAt(i).ToNative(); }

            if (ret.ElementArity == 0) ret.ElementType = null;
            else ret.ElementType = (NativeTypeDescriptor*)Memory.malloc((ulong)ret.ElementArity * (ulong)sizeof(NativeTypeDescriptor));
            for (int i = 0; i<ret.ElementArity; ++i) { ret.ElementType[i] = desc.ElementType.ElementAt(i).ToNative(); }

            if (ret.NrTSLAttribute == 0) ret.TSLAttributes = null;
            else ret.TSLAttributes = (NativeAttributeDescriptor*)Memory.malloc((ulong)ret.NrTSLAttribute * (ulong)sizeof(NativeAttributeDescriptor));
            for (int i = 0; i<ret.NrTSLAttribute; ++i) { ret.TSLAttributes[i] = desc.TSLAttributes.ElementAt(i).ToNative(); }

            return ret;
        }

        private static int Strlen(byte* name)
        {
            for(int len = 0; ; ++len)
            {
                if (*name++ == 0) return len;
            }
        }

        internal static NativeMemberDescriptor ToNative(this MemberDescriptor desc)
            => new NativeMemberDescriptor
            {
                Name = desc.Name.ToUtf8(),
                Optional = (byte)(desc.Optional ? 1 : 0),
                Type = desc.Type.ToNative()
            };

        internal static NativeAttributeDescriptor ToNative(this AttributeDescriptor desc)
            => new NativeAttributeDescriptor
            {
                Name = desc.Name.ToUtf8(),
                Value = desc.Value.ToUtf8()
            };
    }

    [FFIPrefix("schema_")]
    internal static unsafe class Schema
    {
        [FFIExport]
        public static TrinityErrorCode get(out NativeTypeDescriptor[] schema, out int size)
        {
            try
            {
                schema = Global.StorageSchema.CellDescriptors.Select(NativeTypeDescriptor.Make).ToArray();
                size = schema.Length;
                return TrinityErrorCode.E_SUCCESS;
            }
            catch (Exception e)
            {
                PrintException(e);
                schema = null;
                size = 0;
                return TrinityErrorCode.E_FAILURE;
            }
        }

        private static void PrintException(Exception e)
        {
            Console.WriteLine(e.ToString());
            Console.WriteLine();
            if (e.InnerException != null)
            {
                Console.WriteLine("Inner exception: ");
                PrintException(e.InnerException);
            }
        }
    }
}

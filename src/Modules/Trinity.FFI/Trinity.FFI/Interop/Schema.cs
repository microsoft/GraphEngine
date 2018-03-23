using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Trinity.Core.Lib;
using Trinity.Storage;

namespace Trinity.FFI.Interop
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    internal unsafe struct FieldDescriptor
    {
        byte* Name;
        byte* TypeName;
        //TODO encoded typestring
        IntPtr Handle;

        internal static FieldDescriptor Make(IFieldDescriptor desc) =>
            new FieldDescriptor
            {
                Name = AllocString(desc.Name),
                TypeName = AllocString(desc.TypeName),
                Handle = GCHandle.ToIntPtr(GCHandle.Alloc(desc))
            };

        private static byte* AllocString(string str)
        {
            var buf = Encoding.UTF8.GetBytes(str);
            byte* ret = (byte*)Memory.malloc((ulong)buf.Length + 1);
            Memory.Copy(buf, ret, buf.Length);
            ret[buf.Length] = 0;
            return ret;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    internal unsafe struct CellDescriptor
    {
        byte* Name;
        IntPtr Handle;

        internal static CellDescriptor Make(ICellDescriptor desc) =>
            new CellDescriptor
            {
                Name = AllocString(desc.TypeName),
                Handle = GCHandle.ToIntPtr(GCHandle.Alloc(desc))
            };

        private static byte* AllocString(string str)
        {
            var buf = Encoding.UTF8.GetBytes(str);
            byte* ret = (byte*)Memory.malloc((ulong)buf.Length + 1);
            Memory.Copy(buf, ret, buf.Length);
            ret[buf.Length] = 0;
            return ret;
        }
    }

    [FFIPrefix("schema_")]
    internal static unsafe class Schema
    {
        [FFIExport]
        public static TrinityErrorCode get(out CellDescriptor[] schema, out int size)
        {
            try
            {
                schema = Global.StorageSchema.CellDescriptors.Select(CellDescriptor.Make).ToArray();
                size = schema.Length;
                return TrinityErrorCode.E_SUCCESS;
            }
            catch
            {
                schema = null;
                size = 0;
                return TrinityErrorCode.E_FAILURE;
            }
        }

        [FFIExport]
        public static TrinityErrorCode fields(IntPtr cellDesc, out FieldDescriptor[] fields, out int size)
        {
            try
            {
                var cellDescriptor = (ICellDescriptor)GCHandle.FromIntPtr(cellDesc).Target;
                fields = cellDescriptor.GetFieldDescriptors().Select(FieldDescriptor.Make).ToArray();
                size = fields.Length;
                return TrinityErrorCode.E_SUCCESS;
            }
            catch
            {
                fields = null;
                size = 0;
                return TrinityErrorCode.E_FAILURE;
            }
        }
    }
}

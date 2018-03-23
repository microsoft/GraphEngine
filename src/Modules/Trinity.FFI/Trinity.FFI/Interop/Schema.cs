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
    internal unsafe struct CellDescriptor
    {
        byte* Name;

        internal static CellDescriptor Make(ICellDescriptor desc) =>
            new CellDescriptor
            {
                Name = AllocString(desc.TypeName)
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
    }
}

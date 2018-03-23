using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Trinity.Storage;

namespace Trinity.FFI.Interop
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    internal struct CellDescriptor
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        string Name;

        internal static CellDescriptor Make(ICellDescriptor desc) => 
            new CellDescriptor
            {
                Name = desc.TypeName
            };
    }

    [FFIPrefix("schema_")]
    internal static unsafe class Schema
    {
        [FFIExport]
        public static TrinityErrorCode get(ref CellDescriptor[] schema)
        {
            try
            {
                schema = Global.StorageSchema.CellDescriptors.Select(CellDescriptor.Make).ToArray();
                return TrinityErrorCode.E_SUCCESS;
            }
            catch
            {
                return TrinityErrorCode.E_FAILURE;
            }
        }
    }
}

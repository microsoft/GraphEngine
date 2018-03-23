using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Trinity.Core.Lib;
using Trinity.Diagnostics;
using Trinity.Storage;
using Trinity.TSL.Lib;

namespace Trinity.FFI
{
    [FFIPrefix("fdenum_")]
    internal static unsafe class FieldDescriptorEnumerator
    {
        [FFIExport]
        internal static TrinityErrorCode movenext(IntPtr enumerator)
        {
            try
            {
                var h = GCHandle.FromIntPtr(enumerator);
                var desc = (IEnumerator<IFieldDescriptor>)h.Target;
                return desc.MoveNext() ? TrinityErrorCode.E_SUCCESS : TrinityErrorCode.E_ENUMERATION_END;
            }
            catch
            {
                return TrinityErrorCode.E_FAILURE;
            }
        }

        [FFIExport]
        internal static unsafe TrinityErrorCode fieldname(IntPtr enumerator, ref string value)
        {
            try
            {
                value = ((IEnumerator<IFieldDescriptor>)GCHandle.FromIntPtr(enumerator).Target).Current.Name;
                return TrinityErrorCode.E_SUCCESS;
            }
            catch { value = string.Empty; return TrinityErrorCode.E_FAILURE; }
        }

        [FFIExport]
        internal static unsafe TrinityErrorCode fieldoptional(IntPtr enumerator, ref int value)
        {
            try
            {
                value = ((IEnumerator<IFieldDescriptor>)GCHandle.FromIntPtr(enumerator).Target).Current.Optional ? 1 : 0;
                return TrinityErrorCode.E_SUCCESS;
            }
            catch { return TrinityErrorCode.E_FAILURE; }
        }

        [FFIExport]
        internal static TrinityErrorCode from_celldesc(IntPtr cell, ref IntPtr enumerator)
        {
            try
            {
                ICellDescriptor c = (ICellDescriptor)GCHandle.FromIntPtr(cell).Target;
                IEnumerator<IFieldDescriptor> etor = c.GetFieldDescriptors().GetEnumerator();
                GCHandle h = GCHandle.Alloc(etor);
                enumerator = h.AddrOfPinnedObject();
                return TrinityErrorCode.E_SUCCESS;
            }
            catch
            {
                return TrinityErrorCode.E_FAILURE;
            }
        }
    }
}

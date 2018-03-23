using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

namespace Trinity.FFI.Interop
{
    [FFIPrefix("enum_")]
    internal static unsafe class Enumeration
    {
        [FFIExport]
        internal static TrinityErrorCode next(IntPtr enumerator)
        {
            try
            {
                var h = GCHandle.FromIntPtr(enumerator);
                var desc = (IEnumerator)h.Target;
                return desc.MoveNext() ? TrinityErrorCode.E_SUCCESS : TrinityErrorCode.E_ENUMERATION_END;
            }
            catch
            {
                return TrinityErrorCode.E_FAILURE;
            }
        }
    }
}

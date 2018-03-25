using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Trinity.FFI.Interop
{
    [FFIPrefix("gc_")]
    internal static unsafe class GC
    {
        [FFIExport]
        public static TrinityErrorCode free(IntPtr value)
        {
            try
            {
                GCHandle.FromIntPtr(value).Free();
                return TrinityErrorCode.E_SUCCESS;
            }
            catch
            {
                return TrinityErrorCode.E_FAILURE;
            }
        }

        [FFIExport]
        public static TrinityErrorCode dispose(IntPtr accessor)
        {
            try
            {
                ((IDisposable)GCHandle.FromIntPtr(accessor).Target).Dispose();
                return TrinityErrorCode.E_SUCCESS;
            }
            catch
            {
                return TrinityErrorCode.E_FAILURE;
            }
        }
    }
}

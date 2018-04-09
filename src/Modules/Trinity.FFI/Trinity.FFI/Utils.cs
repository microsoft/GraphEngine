using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Trinity.Core.Lib;

namespace Trinity.FFI
{
    internal static unsafe class Utils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static IntPtr GCHandle(this object o)
        {
            return System.Runtime.InteropServices.GCHandle.ToIntPtr(System.Runtime.InteropServices.GCHandle.Alloc(o));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T Target<T>(this IntPtr handle)
        {
            return (T)System.Runtime.InteropServices.GCHandle.FromIntPtr(handle).Target;
        }
    }
}

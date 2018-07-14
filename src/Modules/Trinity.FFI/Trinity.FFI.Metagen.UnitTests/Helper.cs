using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Trinity.Core.Lib;
using static GraphEngine.Jit.JitNativeInterop;
using static GraphEngine.Jit.Utils;

namespace Trinity.FFI.Metagen.UnitTests
{
    internal static unsafe class Helper
    {
        internal static IntPtr LockCell(long cellId)
        {
            Global.LocalStorage.GetLockedCellInfo(cellId, out var Size, out var Type, out var CellPtr, out var EntryIndex);
            return Alloc(new NativeCellAccessor((IntPtr)CellPtr, cellId, Size, EntryIndex, Type, 0, 0));
        }
        
        internal static T ListToNativeAndThen<T>(List<int> lst, Func<IntPtr, T> andthen)
        {
            var arr = lst.ToArray();

            var buflen = arr.Length * sizeof(int);

            IntPtr i = IntPtr.Zero;
            if (buflen != 0L)
            {
                var p = (byte*)Memory.malloc((ulong)buflen + 4);
                *(int*)p = buflen;
                fixed(void* buf = arr)
                {
                    Memory.memcpy(p + sizeof(int), buf, (ulong)buflen);
                    i = Alloc(new NativeCellAccessor((IntPtr) p, 0, 0, buflen, 0, 0, 0));
                }
            }

            return andthen(i);
        }
    }

   
}

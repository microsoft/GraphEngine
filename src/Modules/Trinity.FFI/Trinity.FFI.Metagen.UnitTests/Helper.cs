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
            return Alloc(new NativeCellAccessor((IntPtr)CellPtr, cellId, Size, EntryIndex, Type));
        }
        
        internal static T ListToNativeAndThen<T>(List<int> lst, Func<IntPtr, T> andthen)
        {
            var arr = lst.ToArray();

            var buflen = (UInt64) (arr.Length * sizeof(int));

            IntPtr i = IntPtr.Zero;
            if (buflen != 0L)
            {
                var p = Memory.malloc(buflen + 4);
                fixed(void* buf = arr)
                {
                    i = Alloc(new NativeCellAccessor((IntPtr) Memory.memcpy(p,
                        buf, buflen + 4), 0, 0, (int)buflen, 0));
                }
            }

            return andthen(i);
        }
    }

   
}

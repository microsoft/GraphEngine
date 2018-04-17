using System;
using System.Collections.Generic;
using System.Text;
using Trinity.Core.Lib;
using static GraphEngine.Jit.JitNativeInterop;
using static GraphEngine.Jit.Utils;

namespace Trinity.FFI.OperationFactory.UnitTests
{
    internal static unsafe class Helper
    {
        internal static IntPtr LockCell(long cellId)
        {
            Global.LocalStorage.GetLockedCellInfo(cellId, out var Size, out var Type, out var CellPtr, out var EntryIndex);
            return Alloc(new NativeCellAccessor((IntPtr)CellPtr, cellId, Size, EntryIndex, Type));
        }
    }
}

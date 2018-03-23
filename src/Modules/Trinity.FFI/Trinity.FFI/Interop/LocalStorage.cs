using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Trinity.Diagnostics;
using Trinity.Storage;
using Trinity.TSL.Lib;

namespace Trinity.FFI.Interop
{
    [FFIPrefix("local_")]
    internal static unsafe class LocalStorage
    {
        [FFIExport]
        public static TrinityErrorCode loadcell(long cellId, ref IntPtr cell)
        {
            try
            {
                ICell c = Global.LocalStorage.LoadGenericCell(cellId);
                GCHandle handle = GCHandle.Alloc(c);
                cell = GCHandle.ToIntPtr(handle);
                return TrinityErrorCode.E_SUCCESS;
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogLevel.Error, ex.ToString());
                cell = IntPtr.Zero;
                return TrinityErrorCode.E_FAILURE;
            }
        }

        [FFIExport]
        public static TrinityErrorCode savecell_1(long cellId, IntPtr cell)
        {
            try
            {
                ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
                Global.LocalStorage.SaveGenericCell(cellId, c);
                return TrinityErrorCode.E_SUCCESS;
            }
            catch
            {
                return TrinityErrorCode.E_FAILURE;
            }
        }

        [FFIExport]
        public static TrinityErrorCode savecell_2(long cellId, CellAccessOptions options, IntPtr cell)
        {
            try
            {
                ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
                c.CellId = cellId;
                Global.LocalStorage.SaveGenericCell(options, c);
                return TrinityErrorCode.E_SUCCESS;
            }
            catch
            {
                return TrinityErrorCode.E_FAILURE;
            }
        }

        [FFIExport]
        public static TrinityErrorCode savecell_3(IntPtr cell)
        {
            try
            {
                ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
                Global.LocalStorage.SaveGenericCell(c);
                return TrinityErrorCode.E_SUCCESS;
            }
            catch
            {
                return TrinityErrorCode.E_FAILURE;
            }
        }

        [FFIExport]
        public static TrinityErrorCode savecell_4(CellAccessOptions options, IntPtr cell)
        {
            try
            {
                ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
                Global.LocalStorage.SaveGenericCell(options, c);
                return TrinityErrorCode.E_SUCCESS;
            }
            catch
            {
                return TrinityErrorCode.E_FAILURE;
            }
        }

        [FFIExport]
        public static TrinityErrorCode removecell(long cellId)
        {
            return Global.LocalStorage.RemoveCell(cellId);
        }

        //IEnumerable<ICellAccessor> EnumerateGenericCellAccessors(LocalMemoryStorage storage){ }
        //IEnumerable<ICell> EnumerateGenericCells(LocalMemoryStorage storage){ }
    }
}

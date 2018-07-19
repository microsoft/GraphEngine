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
                cell = Global.LocalStorage.LoadGenericCell(cellId).GCHandle();
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
                Global.LocalStorage.SaveGenericCell(cellId, cell.Target<ICell>());
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
                Global.LocalStorage.SaveGenericCell(options, cellId, cell.Target<ICell>());
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
                Global.LocalStorage.SaveGenericCell(cell.Target<ICell>());
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
                Global.LocalStorage.SaveGenericCell(options, cell.Target<ICell>());
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

        [FFIExport]
        public static TrinityErrorCode savestorage()
        {
            return Global.LocalStorage.SaveStorage();
        }

        [FFIExport]
        public static TrinityErrorCode loadstorage()
        {
            return Global.LocalStorage.LoadStorage();
        }

        [FFIExport]
        public static TrinityErrorCode resetstorage()
        {
            return Global.LocalStorage.ResetStorage();
        }

        //IEnumerable<ICellAccessor> EnumerateGenericCellAccessors(LocalMemoryStorage storage){ }
        //IEnumerable<ICell> EnumerateGenericCells(LocalMemoryStorage storage){ }
    }
}

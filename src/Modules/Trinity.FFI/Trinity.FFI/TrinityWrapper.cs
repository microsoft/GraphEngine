using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Trinity.Core.Lib;
using Trinity.Storage;
using Trinity.TSL.Lib;

namespace Trinity.FFI
{
    internal static unsafe class TrinityWrapper
    {
        public static void trinity_ffi_sync_registry(int methodId, TRINITY_FFI_SYNC_HANDLER handler)
        {

        }
        public static void trinity_ffi_async_registry(int methodId, TRINITY_FFI_ASYNC_HANDLER handler)
        {
        }

        public static string trinity_ffi_sync_send(int partitionId, int methodId, string content)
        {
            return "";
        }
        public static void trinity_ffi_async_send(int partitionId, int methodId, string content)
        {
        }

        //IEnumerable<ICellAccessor> EnumerateGenericCellAccessors(LocalMemoryStorage storage){ }
        //IEnumerable<ICell> EnumerateGenericCells(LocalMemoryStorage storage){ }
        public static TrinityErrorCode trinity_ffi_local_loadcell(long cellId, ref IntPtr cell)
        {
            try
            {
                ICell c = Global.LocalStorage.LoadGenericCell(cellId);
                GCHandle handle = GCHandle.Alloc(c);
                cell = GCHandle.ToIntPtr(handle);
                return TrinityErrorCode.E_SUCCESS;
            }
            catch
            {
                cell = IntPtr.Zero;
                return TrinityErrorCode.E_FAILURE;
            }
        }
        public static TrinityErrorCode trinity_ffi_cloud_loadcell(long cellId, ref IntPtr cell)
        {
            try
            {
                ICell c = Global.CloudStorage.LoadGenericCell(cellId);
                GCHandle handle = GCHandle.Alloc(c);
                cell = GCHandle.ToIntPtr(handle);
                return TrinityErrorCode.E_SUCCESS;
            }
            catch
            {
                cell = IntPtr.Zero;
                return TrinityErrorCode.E_FAILURE;
            }
        }

        public static TrinityErrorCode trinity_ffi_local_savecell_1(long cellId, IntPtr cell)
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
        public static TrinityErrorCode trinity_ffi_local_savecell_2(long cellId, CellAccessOptions options, IntPtr cell)
        {
            try
            {
                ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
                c.CellID = cellId;
                Global.LocalStorage.SaveGenericCell(options, c);
                return TrinityErrorCode.E_SUCCESS;
            }
            catch
            {
                return TrinityErrorCode.E_FAILURE;
            }
        }
        public static TrinityErrorCode trinity_ffi_cloud_savecell(long cellId, IntPtr cell)
        {
            try
            {
                ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
                c.CellID = cellId;
                Global.CloudStorage.SaveGenericCell(c);
                return TrinityErrorCode.E_SUCCESS;
            }
            catch
            {
                return TrinityErrorCode.E_FAILURE;
            }
        }

        public static TrinityErrorCode trinity_ffi_local_removecell(long cellId)
        {
            return Global.LocalStorage.RemoveCell(cellId);
        }
        public static TrinityErrorCode trinity_ffi_cloud_removecell(long cellId)
        {
            return Global.CloudStorage.RemoveCell(cellId);
        }

        public static TrinityErrorCode trinity_ffi_newcell_1(string cellType, ref IntPtr cell)
        {
            try
            {
                ICell c = Global.LocalStorage.NewGenericCell(cellType);
                GCHandle handle = GCHandle.Alloc(c);
                cell = GCHandle.ToIntPtr(handle);
                return TrinityErrorCode.E_SUCCESS;
            }
            catch
            {
                cell = IntPtr.Zero;
                return TrinityErrorCode.E_FAILURE;
            }
        }

        public static TrinityErrorCode trinity_ffi_newcell_2(long cellId, string cellType, ref IntPtr cell)
        {
            try
            {
                ICell c = Global.LocalStorage.NewGenericCell(cellId, cellType);
                GCHandle handle = GCHandle.Alloc(c);
                cell = GCHandle.ToIntPtr(handle);
                return TrinityErrorCode.E_SUCCESS;
            }
            catch
            {
                cell = IntPtr.Zero;
                return TrinityErrorCode.E_FAILURE;
            }
        }

        public static TrinityErrorCode trinity_ffi_newcell_3(string cellType, string cellContent, ref IntPtr cell)
        {
            try
            {
                ICell c = Global.LocalStorage.NewGenericCell(cellType, cellContent);
                GCHandle handle = GCHandle.Alloc(c);
                cell = GCHandle.ToIntPtr(handle);
                return TrinityErrorCode.E_SUCCESS;
            }
            catch
            {
                cell = IntPtr.Zero;
                return TrinityErrorCode.E_FAILURE;
            }
        }

        public static void trinity_ffi_cell_dispose(IntPtr cell)
        {
            try
            {
                GCHandle.FromIntPtr(cell).Free();
            }
            catch
            {
            }
        }

        public static string trinity_ffi_cell_tostring(IntPtr cell)
        {
            try
            {
                // XXX not called
                ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
                return c.ToString();
            }
            catch
            {
                return null;
            }
        }
        public static long trinity_ffi_cell_getid(IntPtr cell)
        {
            try
            {
                ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
                return c.CellID;
            }
            catch
            {
                return 0;
            }
        }

        public static void trinity_ffi_cell_setid(IntPtr cell, long cellId)
        {
            try
            {
                ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
                c.CellID = cellId;
            }
            catch
            {
            }
        }

        public static string trinity_ffi_cell_get(IntPtr cell, string field)
        {
            return "";
        }
        public static int trinity_ffi_cell_has(IntPtr cell, string field)
        {
            return 0;
        }
        public static void trinity_ffi_cell_set(IntPtr cell, string field, string content)
        {
        }
        public static void trinity_ffi_cell_append(IntPtr cell, string field, string content)
        {
        }
        public static void trinity_ffi_cell_delete(IntPtr cell, string field)
        {
        }
    }
}

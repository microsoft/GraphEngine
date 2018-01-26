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

        internal static TrinityErrorCode trinity_ffi_cellenum_movenext(IntPtr enumerator, IntPtr field_info)
        {
            try
            {
                var h = GCHandle.FromIntPtr(enumerator);
                var desc = (IEnumerator<IFieldDescriptor>)h.Target;
                if (field_info != IntPtr.Zero) { GCHandle.FromIntPtr(field_info).Free(); }
                return desc.MoveNext() ? TrinityErrorCode.E_SUCCESS : TrinityErrorCode.E_FAILURE;
            }
            catch
            {
                return TrinityErrorCode.E_FAILURE;
            }
        }

        internal static unsafe TrinityErrorCode trinity_ffi_fieldinfo_name(IntPtr field_info, out string value)
        {
            value = string.Empty;
            try
            {
                var h = GCHandle.FromIntPtr(field_info);
                var desc = (IFieldDescriptor)h.Target;
                value = desc.Name;
                return TrinityErrorCode.E_SUCCESS;
            }
            catch { return TrinityErrorCode.E_FAILURE; }
        }

        internal static TrinityErrorCode trinity_ffi_cellenum_current(IntPtr enumerator, ref IntPtr field_info)
        {
            try
            {
                var h = GCHandle.FromIntPtr(enumerator);
                var desc = (IEnumerator<IFieldDescriptor>)h.Target;
                h = GCHandle.Alloc(desc.Current, GCHandleType.Pinned);
                field_info = h.AddrOfPinnedObject();
                return TrinityErrorCode.E_SUCCESS;
            }
            catch { return TrinityErrorCode.E_FAILURE; }
        }

        internal static TrinityErrorCode trinity_ffi_cellenum_dispose(IntPtr enumerator)
        {
            try
            {
                var h = GCHandle.FromIntPtr(enumerator);
                var desc = (IEnumerator<IFieldDescriptor>)h.Target;
                desc.Dispose();
                h.Free();
                return TrinityErrorCode.E_SUCCESS;
            }
            catch
            {
                return TrinityErrorCode.E_FAILURE;
            }
        }

        internal static TrinityErrorCode trinity_ffi_cellenum_get(IntPtr cell, ref IntPtr enumerator)
        {
            try
            {
                ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
                IEnumerator<IFieldDescriptor> etor = c.GetFieldDescriptors().GetEnumerator();
                GCHandle h = GCHandle.Alloc(etor, GCHandleType.Pinned);
                enumerator = h.AddrOfPinnedObject();
                return TrinityErrorCode.E_SUCCESS;
            }
            catch
            {
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
            GCHandle.FromIntPtr(cell).Free();
        }

        public static string trinity_ffi_cell_tostring(IntPtr cell)
        {
            // XXX not called
            ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
            return c.ToString();
        }

        public static long trinity_ffi_cell_getid(IntPtr cell)
        {
            ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
            return c.CellID;
        }

        public static void trinity_ffi_cell_setid(IntPtr cell, long cellId)
        {
            ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
            c.CellID = cellId;
        }

        public static string trinity_ffi_cell_get(IntPtr cell, string field)
        {
            ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
            return c.GetField<string>(field);
        }
        public static int trinity_ffi_cell_has(IntPtr cell, string field)
        {
            ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
            return c.ContainsField(field) ? 1 : 0;
        }
        public static void trinity_ffi_cell_set(IntPtr cell, string field, string content)
        {
            ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
            c.SetField(field, content);
        }
        public static void trinity_ffi_cell_append(IntPtr cell, string field, string content)
        {
            ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
            c.AppendToField<string>(field, content);
        }
        public static void trinity_ffi_cell_delete(IntPtr cell, string field)
        {
            ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
            c.SetField<string>(field, null);
        }
    }
}

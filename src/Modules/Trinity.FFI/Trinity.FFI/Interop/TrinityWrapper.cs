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
        [FFIExport]
        public static void sync_registry(int methodId, TRINITY_FFI_SYNC_HANDLER handler)
        {
        }

        [FFIExport]
        public static void async_registry(int methodId, TRINITY_FFI_ASYNC_HANDLER handler)
        {
        }

        [FFIExport]
        public static string sync_send(int partitionId, int methodId, string content)
        {
            return "";
        }

        [FFIExport]
        public static void async_send(int partitionId, int methodId, string content)
        {
        }

        //IEnumerable<ICellAccessor> EnumerateGenericCellAccessors(LocalMemoryStorage storage){ }
        //IEnumerable<ICell> EnumerateGenericCells(LocalMemoryStorage storage){ }
        [FFIExport]
        public static TrinityErrorCode local_loadcell(long cellId, ref IntPtr cell)
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

        [FFIExport]
        public static TrinityErrorCode cloud_loadcell(long cellId, ref IntPtr cell)
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

        [FFIExport]
        internal static TrinityErrorCode cell_fieldenum_movenext(IntPtr enumerator, IntPtr field_info)
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

        [FFIExport]
        internal static unsafe TrinityErrorCode cell_fieldinfo_name(IntPtr field_info, out string value)
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

        [FFIExport]
        internal static TrinityErrorCode cell_fieldenum_current(IntPtr enumerator, ref IntPtr field_info)
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

        [FFIExport]
        internal static TrinityErrorCode cell_fieldenum_dispose(IntPtr enumerator)
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

        [FFIExport]
        internal static TrinityErrorCode cell_fieldenum_get(IntPtr cell, ref IntPtr enumerator)
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

        [FFIExport]
        #region local save cell
        public static TrinityErrorCode local_savecell_1(long cellId, IntPtr cell)
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
        public static TrinityErrorCode local_savecell_2(long cellId, CellAccessOptions options, IntPtr cell)
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

        [FFIExport]
        public static TrinityErrorCode local_savecell_3(IntPtr cell)
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
        public static TrinityErrorCode local_savecell_4(CellAccessOptions options, IntPtr cell)
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
        #endregion

        [FFIExport]
        public static TrinityErrorCode cloud_savecell(long cellId, IntPtr cell)
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

        [FFIExport]
        public static TrinityErrorCode local_removecell(long cellId)
        {
            return Global.LocalStorage.RemoveCell(cellId);
        }
        public static TrinityErrorCode cloud_removecell(long cellId)
        {
            return Global.CloudStorage.RemoveCell(cellId);
        }

        [FFIExport]
        #region new cell
        public static TrinityErrorCode newcell_1(string cellType, ref IntPtr cell)
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

        [FFIExport]
        public static TrinityErrorCode newcell_2(long cellId, string cellType, ref IntPtr cell)
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

        [FFIExport]
        public static TrinityErrorCode newcell_3(string cellType, string cellContent, ref IntPtr cell)
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
        #endregion

        #region local use cell

        [FFIExport]
        public static TrinityErrorCode local_usecell_1(long cellId, ref IntPtr cell)
        {
            try
            {
                ICell c = Global.LocalStorage.UseGenericCell(cellId);
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

        [FFIExport]
        public static TrinityErrorCode local_usecell_2(long cellId, CellAccessOptions options, ref IntPtr cell)
        {
            try
            {
                ICell c = Global.LocalStorage.UseGenericCell(cellId, options);
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

        [FFIExport]
        public static TrinityErrorCode local_usecell_3(long cellId, CellAccessOptions options, ref IntPtr cell, string cellType)
        {
            try
            {
                ICell c = Global.LocalStorage.UseGenericCell(cellId, options, cellType);
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
        #endregion

        [FFIExport]
        public static void cell_dispose(IntPtr cell)
        {
            GCHandle.FromIntPtr(cell).Free();
        }

        [FFIExport]
        public static string cell_tostring(IntPtr cell)
        {
            // XXX not called
            ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
            return c.ToString();
        }

        [FFIExport]
        public static long cell_getid(IntPtr cell)
        {
            ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
            return c.CellID;
        }

        [FFIExport]
        public static void cell_setid(IntPtr cell, long cellId)
        {
            ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
            c.CellID = cellId;
        }

        [FFIExport]
        public static string cell_get(IntPtr cell, string field)
        {
            ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
            return c.GetField<string>(field);
        }

        [FFIExport]
        public static int cell_has(IntPtr cell, string field)
        {
            ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
            return c.ContainsField(field) ? 1 : 0;
        }
        [FFIExport]
        public static void cell_set(IntPtr cell, string field, string content)
        {
            ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
            c.SetField(field, content);
        }

        [FFIExport]
        public static void cell_append(IntPtr cell, string field, string content)
        {
            ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
            c.AppendToField<string>(field, content);
        }

        [FFIExport]
        public static void cell_delete(IntPtr cell, string field)
        {
            ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
            c.SetField<string>(field, null);
        }
    }
}

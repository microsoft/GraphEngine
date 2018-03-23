using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Trinity.Storage;

namespace Trinity.FFI.Interop
{
    [FFIPrefix("cloud_")]
    internal static class CloudStorage
    {
        #region TODO networking
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
        #endregion

        [FFIExport]
        public static TrinityErrorCode loadcell(long cellId, ref IntPtr cell)
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
        public static TrinityErrorCode savecell(long cellId, IntPtr cell)
        {
            try
            {
                ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
                c.CellId = cellId;
                Global.CloudStorage.SaveGenericCell(c);
                return TrinityErrorCode.E_SUCCESS;
            }
            catch
            {
                return TrinityErrorCode.E_FAILURE;
            }
        }

        public static TrinityErrorCode removecell(long cellId)
        {
            return Global.CloudStorage.RemoveCell(cellId);
        }


    }
}

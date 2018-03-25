using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Trinity.Storage;
using Trinity.TSL.Lib;

namespace Trinity.FFI.Interop
{
    [FFIPrefix("accessor_")]
    internal static unsafe class Accessor
    {
        [FFIExport]
        public static TrinityErrorCode use_1(long cellId, ref IntPtr cell)
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
        public static TrinityErrorCode use_2(long cellId, CellAccessOptions options, ref IntPtr cell)
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
        public static TrinityErrorCode use_3(long cellId, CellAccessOptions options, ref IntPtr cell, string cellType)
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
    }
}

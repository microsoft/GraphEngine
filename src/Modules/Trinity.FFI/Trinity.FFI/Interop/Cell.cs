using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Trinity.Core.Lib;
using Trinity.Storage;

namespace Trinity.FFI.Interop
{
    [FFIPrefix("cell_")]
    internal static unsafe class Cell
    {
        [FFIExport]
        #region new cell
        public static TrinityErrorCode new_1(string cellType, ref IntPtr cell)
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
        public static TrinityErrorCode new_2(long cellId, string cellType, ref IntPtr cell)
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
        public static TrinityErrorCode new_3(string cellType, string cellContent, ref IntPtr cell)
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


        [FFIExport]
        public static string tostring(IntPtr cell)
        {
            ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
            return c.ToString();
        }

        [FFIExport]
        public unsafe static TrinityErrorCode tobinary(string cellType, string cellContent, out long cellid, out long buffer)
        {
            //XXX performance
            try
            {
                Console.WriteLine($"calling to binary {cellType} {cellContent}");
                var cell = Global.LocalStorage.NewGenericCell(cellType, cellContent);
                Console.WriteLine("New cell succeeded");
                byte[] content = ((ICellAccessor)cell).ToByteArray();
                int len = content.Length;
                cellid = cell.CellId;
                buffer = (long)Memory.malloc((ulong)len);
                Memory.Copy(content, (void*)buffer, len);
                return TrinityErrorCode.E_SUCCESS;
            }
            catch
            {
                Console.WriteLine("to binary failed");
                buffer = 0;
                cellid = 0;
                
                return TrinityErrorCode.E_FAILURE;
            }
        }

        [FFIExport]
        public static long getid(IntPtr cell)
        {
            ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
            return c.CellId;
        }

        [FFIExport]
        public static void setid(IntPtr cell, long cellId)
        {
            ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
            c.CellId = cellId;
        }

        [FFIExport]
        public static string get(IntPtr cell, string field)
        {
            ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
            return c.GetField<string>(field);
        }

        [FFIExport]
        public static int has(IntPtr cell, string field)
        {
            ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
            return c.ContainsField(field) ? 1 : 0;
        }
        [FFIExport]
        public static void set(IntPtr cell, string field, string content)
        {
            ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
            c.SetField(field, content);
        }

        [FFIExport]
        public static void append(IntPtr cell, string field, string content)
        {
            ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
            c.AppendToField<string>(field, content);
        }

        [FFIExport]
        public static void delete(IntPtr cell, string field)
        {
            ICell c = (ICell)GCHandle.FromIntPtr(cell).Target;
            c.SetField<string>(field, null);
        }
    }
}

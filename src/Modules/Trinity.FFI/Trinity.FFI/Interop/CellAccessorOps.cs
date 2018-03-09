using System;
using System.Collections.Generic;
using System.Text;
using Trinity.Storage;
using Trinity.TSL.Lib;

namespace Trinity.FFI.Interop
{
    internal static class CellAccessorOps
    {
        private static DisposableStore<ICellAccessor> Cache = new DisposableStore<ICellAccessor>();

        [FFIExport]
        public static int CA_Use_1(long cellId)
        {
            ICellAccessor c = Global.LocalStorage.UseGenericCell(cellId);
            return Cache.Put(c);
        }

        [FFIExport]
        public static int CA_Use_2(long cellId, CellAccessOptions options)
        {
            ICellAccessor c = Global.LocalStorage.UseGenericCell(cellId, options);
            return Cache.Put(c);
        }

        [FFIExport]
        public static int CA_Use_3(long cellId, CellAccessOptions options, string cellType)
        {
            ICellAccessor c = Global.LocalStorage.UseGenericCell(cellId, options, cellType);
            return Cache.Put(c);
        }

        [FFIExport]
        // cell accessor cellId cannot be changed
        public static long CA_GetId(int index)
        {
            return Cache.Get(index).CellID;
        }

        [FFIExport]
        public static string CA_GetField(int index, string fieldName)
        {
            return Cache.Get(index).GetField<string>(fieldName);
        }

        [FFIExport]
        public static void CA_SetField(int index, string fieldName, string value)
        {
            Cache.Get(index).SetField(fieldName, value);
        }

        [FFIExport]
        public static void CA_AppendField(int index, string field, string content)
        {
            Cache.Get(index).AppendToField(field, content);
        }

        [FFIExport]
        public static void CA_RemoveField(int index, string fieldName)
        {
            Cache.Get(index).SetField<string>(fieldName, null);
        }

        [FFIExport]
        public static void CA_Del(int index)
        {
            Cache.Del(index);
        }
    }
}

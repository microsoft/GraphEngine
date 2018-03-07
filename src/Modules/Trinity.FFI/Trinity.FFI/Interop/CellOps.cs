using System;
using System.Collections.Generic;
using System.Text;
using Trinity.Storage;
using Trinity.TSL.Lib;

namespace Trinity.FFI.Interop
{
    internal static class CellOps
    {
        static ObjectStore<ICell> Cache = new ObjectStore<ICell> { };

        [FFIExport]
        public static int CO_Load(long cellId)
        {
            ICell c = Global.LocalStorage.LoadGenericCell(cellId);
            return Cache.Put(c);
        }

        [FFIExport]
        public static int CO_New_1(string cellType)
        {
            ICell c = Global.LocalStorage.NewGenericCell(cellType);
            return Cache.Put(c);
        }

        [FFIExport]
        public static int CO_New_2(string cellType, long cellId)
        {
            ICell c = Global.LocalStorage.NewGenericCell(cellId, cellType);
            return Cache.Put(c);
        }

        [FFIExport]
        public static int CO_New_3(string cellType, long cellId, string content)
        {
            ICell c = Global.LocalStorage.NewGenericCell(cellType, content);
            c.CellID = cellId;
            return Cache.Put(c);
        }

        [FFIExport]
        public static void CO_Save_1(int index)
        {
            ICell c = Cache.Get(index);
            Global.LocalStorage.SaveGenericCell(c);
        }

        [FFIExport]
        public static void CO_Save_2(long cellId, int index)
        {
            ICell c = Cache.Get(index);
            Global.LocalStorage.SaveGenericCell(cellId, c);
        }

        [FFIExport]
        public static void CO_Save_3(CellAccessOptions writeAheadLogOptions, int index)
        {
            ICell c = Cache.Get(index);
            Global.LocalStorage.SaveGenericCell(writeAheadLogOptions, c);
        }

        [FFIExport]
        public static void CO_Save_4(CellAccessOptions writeAheadLogOptions, long cellId, int index)
        {
            ICell c = Cache.Get(index);
            Global.LocalStorage.SaveGenericCell(writeAheadLogOptions, cellId, c);
        }

        [FFIExport]
        public static TrinityErrorCode CO_Remove(long cellId)
        {
            return Global.LocalStorage.RemoveCell(cellId);
        }

        [FFIExport]
        public static long CO_GetId(int index)
        {
            return Cache.Get(index).CellID;
        }

        [FFIExport]
        public static void CO_SetId(int index, long value)
        {
            Cache.Get(index).CellID = value;
        }

        [FFIExport]
        public static string CO_GetField(int index, string fieldName)
        {
            return Cache.Get(index).GetField<string>(fieldName);
        }

        [FFIExport]
        public static void CO_SetField(int index, string fieldName, string value)
        {
            Cache.Get(index).SetField(fieldName, value);
        }

        [FFIExport]
        public static void CO_AppendField(int index, string field, string content)
        {
            Cache.Get(index).AppendToField(field, content);
        }

        [FFIExport]
        public static void CO_RemoveField(int index, string fieldName)
        {
            Cache.Get(index).SetField<string>(fieldName, null);
        }

        [FFIExport]
        public static void CO_Del(int index)
        {
            Cache.Del(index);
        }
    }
}

using Trinity.Storage;
using Trinity.TSL.Lib;
using System;
using System.Linq;

namespace Trinity.FFI.Agent
{

    public static partial class Storage
    {
        public class CellAccessorManager : IDisposable
        {
            private int moduleId;
            public int ModuleId { get => moduleId;}
            DisposableStore<ICellAccessor> Cache;

            public CellAccessorManager()
            {
                Cache = new DisposableStore<ICellAccessor> { };
                if (_cellAccessorModules == null)
                {
                    _cellAccessorModules = new ObjectStore<DisposableStore<ICellAccessor>>();
                    moduleId = _cellAccessorModules.Put(Cache);
                }
            }
            public int LoadCell(long cellId)
            {
                ICellAccessor c = Global.LocalStorage.UseGenericCell(cellId);
                return Cache.Put(c);
            }

            public int UseCell(long cellId)
            {
                ICellAccessor c = Global.LocalStorage.UseGenericCell(cellId);
                return Cache.Put(c);
            }
            public int UseCell(long cellId, CellAccessOptions options)
            {
                ICellAccessor c = Global.LocalStorage.UseGenericCell(cellId, options);
                return Cache.Put(c);
            }
            public int UseCell(long cellId, CellAccessOptions options, string cellType)
            {
                ICellAccessor c = Global.LocalStorage.UseGenericCell(cellId, options, cellType);
                return Cache.Put(c);
            }
            public void SaveCell(int index)
            {
                ICellAccessor c = Cache.Get(index);
                Global.LocalStorage.SaveGenericCell(c);
            }
            public void SaveCell(long cellId, int index)
            {
                ICellAccessor c = Cache.Get(index);
                Global.LocalStorage.SaveGenericCell(cellId, c);
            }

            public void SaveCell(CellAccessOptions writeAheadLogOptions, int index)
            {
                ICellAccessor c = Cache.Get(index);
                Global.LocalStorage.SaveGenericCell(writeAheadLogOptions, c);
            }
            public void SaveCell(CellAccessOptions writeAheadLogOptions, long cellId, int index)
            {
                ICellAccessor c = Cache.Get(index);
                Global.LocalStorage.SaveGenericCell(writeAheadLogOptions, cellId, c);
            }

            public long CellGetId(int index)
            {
                return Cache.Get(index).CellID;
            }
            public string CellGetField(int index, string fieldName)
            {
                return Cache.Get(index).GetField<string>(fieldName);
            }

            public void CellSetField(int index, string fieldName, string value)
            {
                Cache.Get(index).SetField(fieldName, value);
            }

            public void CellAppendField(int index, string field, string content)
            {
                Cache.Get(index).AppendToField(field, content);
            }

            public void CellRemoveField(int index, string fieldName)
            {
                Cache.Get(index).SetField<string>(fieldName, null);
            }

            public void Del(int index)
            {
                Cache.Del(index);
            }

            public void Dispose()
            {
                Cache.Dispose();
                moduleId = -1;
            }
        }
    }
}

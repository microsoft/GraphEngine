using System;
using Trinity.Storage;
using Trinity.TSL.Lib;
using System.Collections.Generic;
using System.Text;

namespace Trinity.FFI.Agent
{
   public static partial class Storage
    {
        public class CellManager : IDisposable
        {
            /// <summary>
            /// CacheStorageManager is to manage global cell caches, 
            /// whose context is shared for all applications in the same process.
            /// </summary>
            private int moduleId;
            public int ModuleId { get => moduleId;}

            static ObjectStore<ICell> Cache = new ObjectStore<ICell> { };

            

            public CellManager()
            {
                Cache = new ObjectStore<ICell> { };
                if (_cellModules == null)
                {
                    _cellModules = new ObjectStore<ObjectStore<ICell>>();
                    moduleId = _cellModules.Put(Cache);
                }
            }
            public int LoadCell(long cellId)
            {
                ICell c = Global.LocalStorage.LoadGenericCell(cellId);
                return Cache.Put(c);
            }
            public int NewCell(string cellType)
            {
                ICell c = Global.LocalStorage.NewGenericCell(cellType);
                return Cache.Put(c);
            }
            public int NewCell(long cellId, string cellType)
            {
                ICell c = Global.LocalStorage.NewGenericCell(cellId, cellType);
                return Cache.Put(c);
            }
            public int NewCell(string cellType, string content)
            {
                ICell c = Global.LocalStorage.NewGenericCell(cellType, content);
                return Cache.Put(c);
            }

            public void SaveCell(int index)
            {
                ICell c = Cache.Get(index);
                Global.LocalStorage.SaveGenericCell(c);
            }
            public void SaveCell(long cellId, int index)
            {
                ICell c = Cache.Get(index);
                Global.LocalStorage.SaveGenericCell(cellId, c);
            }

            public void SaveCell(CellAccessOptions writeAheadLogOptions, int index)
            {
                ICell c = Cache.Get(index);
                Global.LocalStorage.SaveGenericCell(writeAheadLogOptions, c);
            }
            public void SaveCell(CellAccessOptions writeAheadLogOptions, long cellId, int index)
            {
                ICell c = Cache.Get(index);
                Global.LocalStorage.SaveGenericCell(writeAheadLogOptions, cellId, c);
            }

            public long CellGetIdByIndex(int index)
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
                Cache.DeAlloc();
                moduleId = -1;
            }
        }
    }
}

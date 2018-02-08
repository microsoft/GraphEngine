using System.Collections.Generic;
using Trinity.Storage.CompositeExtension;
using Trinity.Storage;
using Trinity.TSL.Lib;
using System;
using System.Linq;

namespace Trinity.FFI
{
    public static class Agent
    {
        public static void Configure(string IncludeDirectory, string StorageRoot,
                                     string TSLCodeGenExeLocation, string DotNetExeLocation,
                                     int avgMaxAsmNum, int avgCellNum, int avgFieldNum) =>
            Setting.Configure(IncludeDirectory,
                              StorageRoot,
                              TSLCodeGenExeLocation,
                              DotNetExeLocation,
                              avgMaxAsmNum,
                              avgCellNum,
                              avgFieldNum);


        public static void Initialize()
        {
            if (!Controller.Initialized)
                Controller.Initialize();
        }
        public static void Uninitialize()
        {
            if (Controller.Initialized)
                Controller.Uninitialize();
        }
        public static void LoadTSL(string tslSrcDir, string tslBuildDir, string moduleName, string versionName) => Controller.LoadFrom(tslSrcDir, tslBuildDir, moduleName, versionName);

        public static void SetCmdPath(string TSLCodeGenExeLocation, string DotNetExeLocation)
        {
            Cmd.TSLCodeGenExeLocation = TSLCodeGenExeLocation;
            Cmd.DotNetExeLocation = DotNetExeLocation;
        }
        public static void CompositeParamSettings(int avgMaxAsmNum, int avgCellNum, int avgFieldNum)
        {
            ConfigConstant.AvgMaxAsmNum = avgMaxAsmNum;
            ConfigConstant.AvgCellNum = avgCellNum;
            ConfigConstant.AvgFieldNum = avgFieldNum;
        }
        public static List<IStorageSchema> StorageSchema => CompositeStorage.StorageSchema;

        public static List<IGenericCellOperations> GenericCellOperations => CompositeStorage.GenericCellOperations;

        public static List<int> IDIntervals => CompositeStorage.IDIntervals;

        public static Dictionary<string, int> CellTypeIDs => CompositeStorage.CellTypeIDs;

        public static List<VersionRecorder> VersionRecorders => CompositeStorage.VersionRecorders;

        public static PathHelper PathHelper = default(PathHelper);

    }


    public class AccessorManager : IDisposable
    {
        DisposableStore<ICellAccessor> CellAccessorCache;
        public AccessorManager(bool isAccessor)
        {
            CellAccessorCache = new DisposableStore<ICellAccessor> { };
        }
        public int LoadCell(long cellId)
        {
            ICellAccessor c = Global.LocalStorage.UseGenericCell(cellId);
            return CellAccessorCache.Put(c);
        }

        public int UseCellById(long cellId)
        {
            ICellAccessor c = Global.LocalStorage.UseGenericCell(cellId);
            return CellAccessorCache.Put(c);
        }
        public int UseCellByIdOps(long cellId, CellAccessOptions options)
        {
            ICellAccessor c = Global.LocalStorage.UseGenericCell(cellId, options);
            return CellAccessorCache.Put(c);
        }
        public int UseCellByIdOpsType(long cellId, CellAccessOptions options, string cellType)
        {
            ICellAccessor c = Global.LocalStorage.UseGenericCell(cellId, options, cellType);
            return CellAccessorCache.Put(c);
        }
        public void SaveCellByIndex(int cellIdx)
        {
            ICellAccessor c = CellAccessorCache.Get(cellIdx);
            Global.LocalStorage.SaveGenericCell(c);
        }
        public void SaveCellByIdIndex(long cellId, int cellIdx)
        {
            ICellAccessor c = CellAccessorCache.Get(cellIdx);
            Global.LocalStorage.SaveGenericCell(cellId, c);
        }

        public void SaveCellByOpsIndex(CellAccessOptions writeAheadLogOptions, int cellIdx)
        {
            ICellAccessor c = CellAccessorCache.Get(cellIdx);
            Global.LocalStorage.SaveGenericCell(writeAheadLogOptions, c);
        }
        public void SaveCellByOpsIdIndex(CellAccessOptions writeAheadLogOptions, long cellId, int cellIdx)
        {
            ICellAccessor c = CellAccessorCache.Get(cellIdx);
            Global.LocalStorage.SaveGenericCell(writeAheadLogOptions, cellId, c);
        }

        public long CellGetIdByIndex(int cellIdx)
        {
            return CellAccessorCache.Get(cellIdx).CellID;
        }
        public string CellGetField(int cellIdx, string fieldName)
        {
            return CellAccessorCache.Get(cellIdx).GetField<string>(fieldName);
        }

        public void CellSetField(int cellIdx, string fieldName, string value)
        {
            CellAccessorCache.Get(cellIdx).SetField(fieldName, value);
        }


        public void CellAppendField(int cellIdx, string field, string content)
        {
            CellAccessorCache.Get(cellIdx).AppendToField(field, content);
        }

        public void CellRemoveField(int cellIdx, string fieldName)
        {
            CellAccessorCache.Get(cellIdx).SetField<string>(fieldName, null);
        }

        public void Del(int cellIdx)
        {
            CellAccessorCache.Del(cellIdx);
        }

        public void Dispose()
        {
            CellAccessorCache.Dispose();
        }
    }

    public class CacheStorageManager : IDisposable
    {
        static ObjectStore<ICell> CellCache = new ObjectStore<ICell> { };
        public CacheStorageManager()
        {
        }
        public int LoadCell(long cellId)
        {
            ICell c = Global.LocalStorage.LoadGenericCell(cellId);
            return CellCache.Put(c);
        }
        public int NewCellByType(string cellType)
        {
            ICell c = Global.LocalStorage.NewGenericCell(cellType);
            return CellCache.Put(c);
        }
        public int NewCellByIdType(long cellId, string cellType)
        {
            ICell c = Global.LocalStorage.NewGenericCell(cellId, cellType);
            return CellCache.Put(c);
        }
        public int NewCellByTypeContent(string cellType, string content)
        {
            ICell c = Global.LocalStorage.NewGenericCell(cellType, content);
            return CellCache.Put(c);
        }



        public void SaveCellByIndex(int cellIdx)
        {
            ICell c = CellCache.Get(cellIdx);
            Global.LocalStorage.SaveGenericCell(c);
        }
        public void SaveCellByIdIndex(long cellId, int cellIdx)
        {
            ICell c = CellCache.Get(cellIdx);
            Global.LocalStorage.SaveGenericCell(cellId, c);
        }

        public void SaveCellByOpsIndex(CellAccessOptions writeAheadLogOptions, int cellIdx)
        {
            ICell c = CellCache.Get(cellIdx);
            Global.LocalStorage.SaveGenericCell(writeAheadLogOptions, c);
        }
        public void SaveCellByOpsIdIndex(CellAccessOptions writeAheadLogOptions, long cellId, int cellIdx)
        {

            ICell c = CellCache.Get(cellIdx);
            Global.LocalStorage.SaveGenericCell(writeAheadLogOptions, cellId, c);

        }

        public long CellGetIdByIndex(int cellIdx)
        {
            return CellCache.Get(cellIdx).CellID;
        }
        public string CellGetField(int cellIdx, string fieldName)
        {
            return CellCache.Get(cellIdx).GetField<string>(fieldName);
        }

        public void CellSetField(int cellIdx, string fieldName, string value)
        {
            CellCache.Get(cellIdx).SetField(fieldName, value);
        }


        public void CellAppendField(int cellIdx, string field, string content)
        {
            CellCache.Get(cellIdx).AppendToField(field, content);
        }

        public void CellRemoveField(int cellIdx, string fieldName)
        {
            CellCache.Get(cellIdx).SetField<string>(fieldName, null);
        }

        public void Del(int cellIdx)
        {
            CellCache.Del(cellIdx);
        }

        public void Dispose()
        {
            CellCache.DeAlloc();
        }
    }
}

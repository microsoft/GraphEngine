using System.Collections.Generic;
using Trinity.Storage.CompositeExtension;
using Trinity.Storage;

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
}

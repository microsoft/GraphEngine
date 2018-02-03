using System.Collections.Generic;
using Trinity.Extension;
using Trinity.Utilities;
using System.IO;
using System.Linq;
using System.Reflection;
using Trinity.Storage;
using Trinity.Diagnostics;

[assembly: GraphEngineExtension]
namespace CompositeStorageExtension
{
    public static class PathHelper
    {
        const string FolderName = "composite-helper";
        public static string Directory => FileUtility.CompletePath(Path.Combine(Trinity.TrinityConfig.StorageRoot, FolderName));
        public static string VersionRecorders => Path.Combine(Directory, "VersionRecorders.bin");
        public static string CellTypeIDs => Path.Combine(Directory, "CellTypeIDs.bin");
        public static string IDIntervals => Path.Combine(Directory, "IDIntervals.bin");
        public static string DLL(string dllName) => Path.Combine(Directory, dllName);

    }
    class Initializer : IStartupTask
    {
        public void Run()
        {
            Trinity.Global.LocalStorage.StorageBeforeLoad += LocalStorage_StorageBeforeLoad;
            Trinity.Global.LocalStorage.StorageSaved += LocalStorage_StorageSaved;
            Trinity.Global.LocalStorage.StorageLoaded += LocalStorage_StorageLoaded;
            Trinity.Global.LocalStorage.StorageReset += LocalStorage_StorageReset;
        }

        private void LocalStorage_StorageReset()
        {
            Log.WriteLine("Reset");
            Controller.CleanAll();
            Controller.Init(Trinity.TrinityConfig.StorageRoot);
        }

        private void LocalStorage_StorageLoaded()
        {
            Log.WriteLine("Loaded");

        }

        private void LocalStorage_StorageSaved()
        {
            Log.WriteLine("Saved");
            Utilities.Serialize(CompositeStorage.VersionRecorders, PathHelper.VersionRecorders);
            Utilities.Serialize(CompositeStorage.IDIntervals, PathHelper.IDIntervals);
            Utilities.Serialize(CompositeStorage.CellTypeIDs, PathHelper.CellTypeIDs);

        }

        private void LocalStorage_StorageBeforeLoad()
        {
            Log.WriteLine("BeforeLoad");
            Utilities.Deserialize<List<VersionRecorder>>(PathHelper.VersionRecorders)
                     .WhenNotDefault(_ => CompositeStorage.VersionRecorders = _);

            Utilities.Deserialize<Dictionary<string, int>>(PathHelper.CellTypeIDs)
                     .WhenNotDefault(_ => CompositeStorage.CellTypeIDs = _);

            Utilities.Deserialize<List<int>>(PathHelper.IDIntervals)
                     .WhenNotDefault(_ => CompositeStorage.IDIntervals = _);

            if (CompositeStorage.VersionRecorders != default(List<VersionRecorder>))
            {
                var asm = CompositeStorage.VersionRecorders
                            .Select(each => $"{each.Namespace}.dll"
                                                .Apply(PathHelper.DLL)
                                                .Apply(Assembly.LoadFrom))
                            .ToList();

                CompositeStorage.StorageSchema
                        = asm.Select(_ => AssemblyUtility.GetAllClassInstances<IStorageSchema>(assembly: _).First()).ToList();

                CompositeStorage.GenericCellOperations
                        = asm.Select(_ => AssemblyUtility.GetAllClassInstances<IGenericCellOperations>(assembly: _).First()).ToList();
            }
            Controller.Init(Trinity.TrinityConfig.StorageRoot);
        }
        
    }
}

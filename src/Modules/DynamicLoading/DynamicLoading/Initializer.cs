using System.Collections.Generic;
using Trinity.Extension;
using Trinity.Utilities;
using System.IO;
using System.Linq;
using System.Reflection;
using Trinity.Storage;
[assembly: GraphEngineExtension]
namespace CompositeStorageExtension
{
    class Initializer : IStartupTask
    {
        private static class DumplingHelper
        {
            const string FolderName = "composite-helper";
            public static string Directory => FileUtility.CompletePath(Path.Combine(Trinity.TrinityConfig.StorageRoot, FolderName));
            public static string VersionRecorders => Path.Combine(Directory, "VersionRecorders.bin");
            public static string CellTypeIDs => Path.Combine(Directory, "CellTypeIDs.bin");
            public static string IDIntervals => Path.Combine(Directory, "IDIntervals.bin");

        }

        public void Run()
        {
            Trinity.Global.LocalStorage.StorageBeforeLoad += LocalStorage_StorageBeforeLoad;
            Trinity.Global.LocalStorage.StorageSaved += LocalStorage_StorageSaved;
            Trinity.Global.LocalStorage.StorageLoaded += LocalStorage_StorageLoaded;
            Trinity.Global.LocalStorage.StorageReset += LocalStorage_StorageReset;
        }

        private void LocalStorage_StorageReset()
        {
            CompositeStorage.CellTypeIDs = null;
            CompositeStorage.GenericCellOperations = null;
            CompositeStorage.IDIntervals = null;
            CompositeStorage.StorageSchema = null;
            CompositeStorage.VersionRecorders = null;
            Controller.Init(Trinity.TrinityConfig.StorageRoot);
        }

        private void LocalStorage_StorageLoaded()
        {

            Utilities.Deserialize<List<VersionRecorder>>(DumplingHelper.VersionRecorders)
                     .WhenNotDefault(_ => CompositeStorage.VersionRecorders = _);

            Utilities.Deserialize<Dictionary<string, int>>(DumplingHelper.CellTypeIDs)
                     .WhenNotDefault(_ => CompositeStorage.CellTypeIDs = _);

            Utilities.Deserialize<List<int>>(DumplingHelper.IDIntervals)
                     .WhenNotDefault(_ => CompositeStorage.IDIntervals = _);

            if (CompositeStorage.VersionRecorders != default(List<VersionRecorder>))
            {
                var asm = CompositeStorage.VersionRecorders.Select(each => Assembly.LoadFrom($"{each.Namespace}.dll")).ToList();

                CompositeStorage.StorageSchema
                        = asm.Select(_ => AssemblyUtility.GetAllClassInstances<IStorageSchema>(assembly: _).First()).ToList();

                CompositeStorage.GenericCellOperations
                        = asm.Select(_ => AssemblyUtility.GetAllClassInstances<IGenericCellOperations>(assembly: _).First()).ToList();
            }

            Controller.Init(Trinity.TrinityConfig.StorageRoot);

        }

        private void LocalStorage_StorageSaved()
        {
            Utilities.Serialize(CompositeStorage.VersionRecorders, DumplingHelper.VersionRecorders);
            Utilities.Serialize(CompositeStorage.IDIntervals, DumplingHelper.IDIntervals);
            Utilities.Serialize(CompositeStorage.CellTypeIDs, DumplingHelper.CellTypeIDs);

        }

        private void LocalStorage_StorageBeforeLoad()
        {
            // we can do nothing.
        }
    }
}

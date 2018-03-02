using System.Collections.Generic;
using Trinity.Extension;
using Trinity.Utilities;
using System.IO;
using System.Linq;
using System.Reflection;
using Trinity.Storage;
using Trinity.Diagnostics;

[assembly: GraphEngineExtension]
namespace Trinity.Storage.CompositeExtension
{

    class Startup : IStartupTask
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
            Log.WriteLine($"{nameof(CompositeExtension)}: Reset");

            Controller.Uninitialize();
            Controller.Initialize();
        }

        private void LocalStorage_StorageLoaded()
        {
            Log.WriteLine($"{nameof(CompositeExtension)}: Loaded");
        }

        private void LocalStorage_StorageSaved()
        {
            LogisticHandler.Session(
                start: () => Log.WriteLine($"{nameof(CompositeExtension)}: Saving"),
                err: (e) => Log.WriteLine(LogLevel.Error, $"{nameof(CompositeExtension)}: {{0}}", e.Message),
                end: () => Log.WriteLine($"{nameof(CompositeExtension)}: Saved"),
                behavior: () =>
                {
                    Serialization.Serialize(CompositeStorage.VersionRecorders, PathHelper.VersionRecorders);
                    Serialization.Serialize(CompositeStorage.IDIntervals, PathHelper.IDIntervals);
                    Serialization.Serialize(CompositeStorage.CellTypeIDs, PathHelper.CellTypeIDs);
                }
            );
        }

        private void LocalStorage_StorageBeforeLoad()
        {
            Log.WriteLine($"{nameof(CompositeExtension)}: BeforeLoad");
            Serialization.Deserialize<List<VersionRecord>>(PathHelper.VersionRecorders)
                     .WhenNotDefault(_ => CompositeStorage.VersionRecorders = _);

            Serialization.Deserialize<Dictionary<string, int>>(PathHelper.CellTypeIDs)
                     .WhenNotDefault(_ => CompositeStorage.CellTypeIDs = _);

            Serialization.Deserialize<List<int>>(PathHelper.IDIntervals)
                     .WhenNotDefault(_ => CompositeStorage.IDIntervals = _);

            if (CompositeStorage.VersionRecorders != null)
            {
                var asm = CompositeStorage.VersionRecorders
                            .Select(each => $"{each.Namespace}.dll"
                                                .By(PathHelper.DLL)
                                                .By(Assembly.LoadFrom))
                            .ToList();

                CompositeStorage.StorageSchema
                        = asm.Select(_ => AssemblyUtility.GetAllClassInstances<IStorageSchema>(assembly: _).First()).ToList();

                CompositeStorage.GenericCellOperations
                        = asm.Select(_ => AssemblyUtility.GetAllClassInstances<IGenericCellOperations>(assembly: _).First()).ToList();
            }
        }

    }
}

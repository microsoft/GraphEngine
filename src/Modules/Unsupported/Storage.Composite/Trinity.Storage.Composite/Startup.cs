using System.Collections.Generic;
using Trinity.Extension;
using Trinity.Utilities;
using System.IO;
using System.Linq;
using System.Reflection;
using Trinity.Storage;
using Trinity.Diagnostics;

[assembly: GraphEngineExtension]
namespace Trinity.Storage.Composite
{
    class Startup : IStartupTask
    {
        public void Run()
        {
            Trinity.Global.LocalStorage.StorageBeforeLoad += CompositeStorage.LoadMetadata;
            Trinity.Global.LocalStorage.StorageSaved += CompositeStorage.SaveMetadata;
            //Trinity.Global.LocalStorage.StorageLoaded += LocalStorage_StorageLoaded;
            Trinity.Global.LocalStorage.StorageReset += CompositeStorage.ResetMetadata;
            Log.WriteLine("Storage.Composite: extension loaded.");
        }
    }
}

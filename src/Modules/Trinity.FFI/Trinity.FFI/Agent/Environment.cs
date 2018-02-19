using System;
using System.Collections.Generic;
using System.Text;
using Trinity.Storage;
using Trinity.Storage.CompositeExtension;
namespace Trinity.FFI.Agent
{
    public static class Environment
    {
        // for the convenience of FFI bindings, do not use property here.

        public static List<IStorageSchema> StorageSchema()
        {
            return CompositeStorage.StorageSchema;
        }

        public static List<IGenericCellOperations> GenericCellOperations()
        {
            return CompositeStorage.GenericCellOperations;
        }
        public static List<int> IDIntervals()
        {
            return CompositeStorage.IDIntervals;
        }

        public static Dictionary<string, int> CellTypeIDs()
        {
            return CompositeStorage.CellTypeIDs;
        }

        public static List<VersionRecorder> VersionRecorders()
        {
            return CompositeStorage.VersionRecorders;
        }

        public static PathHelper PathHelper()
        {
            return default(PathHelper);
        }
    }
}

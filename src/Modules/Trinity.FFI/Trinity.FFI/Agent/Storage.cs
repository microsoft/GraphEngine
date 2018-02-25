using System;
using System.Collections.Generic;
using System.Text;
using Trinity.FFI;
using Trinity.Storage;
using Trinity.Storage.CompositeExtension;
using Trinity.TSL.Lib;

namespace Trinity.FFI.Agent
{
    public static partial class Storage
    {
        // use modules to manage cellManagers and the accessors.
        private static ObjectStore<ObjectStore<ICell>> _cellModules;
        private static ObjectStore<DisposableStore<ICellAccessor>> _cellAccessorModules;

        public static TrinityErrorCode RemoveCellFromStorage(long cellId)
        {
            return Global.LocalStorage.RemoveCell(cellId);
        }
    }
}

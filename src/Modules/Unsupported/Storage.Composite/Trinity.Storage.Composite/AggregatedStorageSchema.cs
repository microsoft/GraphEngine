using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Extension;

namespace Trinity.Storage.Composite
{
    [ExtensionPriority(int.MaxValue)]
    public class AggregatedStorageSchema : IStorageSchema, IStorageSchemaUpdateNotifier
    {
        internal static AggregatedStorageSchema s_singleton = null;

        public AggregatedStorageSchema()
        {
            s_singleton = this;
        }

        public IEnumerable<ICellDescriptor> CellDescriptors
            => CompositeStorage.s_StorageSchemas.SelectMany(_ => _.CellDescriptors);

        public IEnumerable<string> CellTypeSignatures => 
            CompositeStorage.s_StorageSchemas.SelectMany(_ => _.CellTypeSignatures);

        public event StorageSchemaUpdatedHandler StorageSchemaUpdated = delegate { };

        public ushort GetCellType(string cellTypeString)
        {
            int seg = CompositeStorage.GetIntervalIndexByCellTypeName(cellTypeString);
            return CompositeStorage.s_StorageSchemas[seg].GetCellType(cellTypeString);
        }

        internal void Update()
        {
            StorageSchemaUpdated();
        }
    }
}

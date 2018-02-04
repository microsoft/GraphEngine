using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Trinity.Storage;
using Trinity.Extension;
namespace Trinity.Storage.CompositeExtension 
{
    [ExtensionPriority(int.MaxValue)]
    public class StorageSchema : IStorageSchema
    {
        public IEnumerable<ICellDescriptor> CellDescriptors
            => CompositeStorage.StorageSchema.SelectMany(_ => _.CellDescriptors);

        public IEnumerable<string> CellTypeSignatures => 
            CompositeStorage.StorageSchema.Select(_ => _.CellTypeSignatures).Aggregate((last, next) => last.Concat(next));

        public ushort GetCellType(string cellTypeString)
        {
            if (!CompositeStorage.CellTypeIDs.Keys.Contains(cellTypeString))
                throw new CellTypeNotMatchException("Unrecognized cell type string.");

            int seg = GetIntervalIndex.ByCellTypeName(cellTypeString);
            return CompositeStorage.StorageSchema[seg].GetCellType(cellTypeString);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Trinity.Storage;
using Trinity.Extension;
namespace DynamicLoading 
{
    [ExtensionPriority(int.MaxValue)]
    class StorageSchema : IStorageSchema
    {
        public IEnumerable<ICellDescriptor> CellDescriptors
            => Center.Leader.StorageSchema.SelectMany(_ => _.CellDescriptors);

        public IEnumerable<string> CellTypeSignatures => 
            Center.Leader.StorageSchema.Select(_ => _.CellTypeSignatures).Aggregate((last, next) => last.Concat(next));

        public ushort GetCellType(string cellTypeString)
        {
            if (!Center.CellTypeIDs.Keys.Contains(cellTypeString))
                throw new CellTypeNotMatchException("Unrecognized cell type string.");
            var tid = Center.CellTypeIDs[cellTypeString];
            int seg = Center.IDIntervals.FindLastIndex(seg_head => seg_head < tid);
            return Center.Leader.StorageSchema[seg].GetCellType(cellTypeString);
        }
    }
}

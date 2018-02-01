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
        public IEnumerable<ICellDescriptor> CellDescriptors {
            get {
                foreach (var storageSchema in Center.Leader.StorageSchema)
                    foreach (var cellDesc in storageSchema.CellDescriptors)
                        yield return cellDesc;
                yield break;
            }
        }

        public IEnumerable<string> CellTypeSignatures => 
            Center.Leader.StorageSchema.Select(_ => _.CellTypeSignatures).Aggregate((last, next) => last.Concat(next));


        public ushort GetCellType(string cellTypeString)
        {
            if (!Center.CellTypeIDs.Keys.Contains(cellTypeString))
                throw new Exception("Unrecognized cell type string.");
            var id = Center.CellTypeIDs[cellTypeString];
            int indexOfInterval = new Center.IntervalSearch(id).Call();
            return Center.Leader.StorageSchema[indexOfInterval].GetCellType(cellTypeString);
        }
    }
}

// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System.Collections.Generic;
using System.Linq;

namespace Trinity.Modules.Spark
{
    public interface ICellRepository
    {
        StructType GetCellSchema(string cellType);

        IEnumerable<long> FindCells(string cellType);

        string LoadCell(string cellType, long cellId);
    }

    public class DefaultCellRepository : ICellRepository
    {
        public virtual StructType GetCellSchema(string cellType)
        {
            var cellDesc = Global.StorageSchema.CellDescriptors.FirstOrDefault(_ => _.TypeName == cellType);
            return StructType.ConvertFromCellDescriptor(cellDesc);
        }

        public virtual IEnumerable<long> FindCells(string cellType)
        {
            return Global.LocalStorage.GenericCellAccessor_Selector().Where(c => c.TypeName == cellType).Select(c => c.CellID);
        }

        public virtual string LoadCell(string cellType, long cellId)
        {
            using (var cell = Global.LocalStorage.UseGenericCell(cellId))
            {
                return cell.ToString();
            }
        }
    }
}

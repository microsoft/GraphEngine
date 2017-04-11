// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Trinity.Storage;

namespace Trinity.Modules.Spark
{
    public interface ICellRepository
    {
        StructType GetCellSchema(string cellType);

        IEnumerable<long> FindCells(string cellType, IEnumerable<JObject> filters);

        string LoadCell(string cellType, long cellId, IEnumerable<string> fields);
    }

    public class DefaultCellRepository : ICellRepository
    {
        public virtual StructType GetCellSchema(string cellType)
        {
            var cellDesc = Global.StorageSchema.CellDescriptors.FirstOrDefault(_ => _.TypeName == cellType);
            return StructType.ConvertFromCellDescriptor(cellDesc);
        }

        public virtual IEnumerable<long> FindCells(string cellType, IEnumerable<JObject> filters)
        {
            var cellDesc = Global.StorageSchema.CellDescriptors.FirstOrDefault(_ => _.TypeName == cellType);
            if (cellDesc == null)
                return new List<long>();

            var cells = Global.LocalStorage.GenericCellAccessor_Selector().AsQueryable();
            var filtersExpr = FilterExpressionBuilder.BuildExpression(cells, cellDesc, filters);
            var cellsFiltered = cells.Provider.CreateQuery<ICell>(filtersExpr);
            return cellsFiltered.Select(_ => _.CellID);
        }

        public virtual string LoadCell(string cellType, long cellId, IEnumerable<string> fields)
        {
            using (var cell = Global.LocalStorage.UseGenericCell(cellId))
            {
                if (fields == null)
                    return cell.ToString();

                var jobj = new JObject();
                foreach (var field in fields)
                {
                    if (field == "CellID")
                        jobj[field] = cell.CellID;
                    else
                        jobj[field] = JToken.FromObject(cell.GetField<object>(field));
                }
                return JsonConvert.SerializeObject(jobj);
            }
        }
    }
}

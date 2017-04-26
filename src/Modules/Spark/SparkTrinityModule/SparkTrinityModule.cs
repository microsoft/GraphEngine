// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using Newtonsoft.Json;
using Trinity.Modules.Spark.Protocols.TSL;
using System.Linq;
using Newtonsoft.Json.Linq;
using Trinity.Storage;
using Trinity.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trinity.TSL.Lib;
using System.Collections.Concurrent;

namespace Trinity.Modules.Spark
{
    public class SparkTrinityModule : SparkTrinityBase
    {
        public static SparkTrinityModule GetClientModule()
        {
            var module = new SparkTrinityModule();
            module.ClientInitialize(RunningMode.Server, Global.CloudStorage);
            return module;
        }

        public override string GetModuleName()
        {
            return "Spark";
        }

        public override void FindCellsHandler(FindCellsRequestReader request, FindCellsResponseWriter response)
        {
            var cellType = request.CellType.ToString();
            var filters = request.Contains_Filters ? request.Filters.Select(f => (JsonConvert.DeserializeObject(f.ToString()) as JObject)).ToList() : null;

            var cellDesc = Global.StorageSchema.CellDescriptors.FirstOrDefault(_ => _.TypeName == cellType);
            if (cellDesc == null)
                return;

            var cells = Global.LocalStorage.GenericCellAccessor_Selector().AsQueryable();
            var filtersExpr = FilterExpressionBuilder.BuildExpression(cells, cellDesc, filters);
            var cellsFiltered = cells.Provider.CreateQuery<ICell>(filtersExpr);
            var cellIds = cellsFiltered.Select(_ => _.CellID).ToList();

            response.CellIds = cellIds;
            Log.WriteLine("FindCells succeeded: {0} cells found", cellIds.Count);
        }

        public override void LoadCellsHandler(LoadCellsRequestReader request, LoadCellsResponseWriter response)
        {
            IEnumerable<string> fields = request.Contains_Fields ? request.Fields.Select(f => f.ToString()).ToList() : null;
            var cells = new ConcurrentQueue<string>();

            Parallel.ForEach(request.CellIds.ToList(), cellId =>
            {
                using (var cell = Global.LocalStorage.UseGenericCell(cellId, CellAccessOptions.ReturnNullOnCellNotFound))
                {
                    if (cell == null)
                    {
                        Log.WriteLine(LogLevel.Warning, "LoadCells warning: cell#{0} does not exist.", cellId);
                        return;
                    }

                    if (fields == null)
                    {
                        cells.Enqueue(cell.ToString());
                        return;
                    }

                    var jobj = new JObject();
                    foreach (var field in fields)
                        jobj[field] = field == "CellID" ? cell.CellID : JToken.FromObject(cell.GetField<object>(field));
                    cells.Enqueue(JsonConvert.SerializeObject(jobj));
                }
            });

            response.Cells = cells.ToList();
            Log.WriteLine("LoadCells succeeded: {0} cells returned", cells.Count);
        }
    }
}

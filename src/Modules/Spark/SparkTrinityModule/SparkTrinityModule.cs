// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using Trinity.Diagnostics;
using System.Diagnostics;
using Trinity.Modules.Spark.Protocols.TSL;
using Newtonsoft.Json.Linq;
using Trinity.Storage;

namespace Trinity.Modules.Spark
{
    public class SparkTrinityModule : SparkTrinityBase
    {
        public override string GetModuleName()
        {
            return "Spark";
        }

        public override void SchemaHandler(SchemaRequestStruct request, HttpListenerResponse response)
        {
            var schema = GetSchema(request.cellType);
            var jsonResponse = JsonConvert.SerializeObject(schema);
            using (var sw = new StreamWriter(response.OutputStream))
            {
                sw.WriteLine(jsonResponse);
            }
        }

        public override void PartitionsHandler(PartitionsRequestStruct request, HttpListenerResponse response)
        {
            IEnumerable<JObject> filters = null;
            try
            {
                var filterArray = JsonConvert.DeserializeObject(request.filters) as JArray;
                if (filterArray != null)
                    filters = filterArray.Select(_ => _ as JObject);
            }
            catch
            {
            }

            var partitions = GetPartitions(request.cellType, request.batchSize, filters);
            var jsonResponse = JsonConvert.SerializeObject(partitions);
            using (var sw = new StreamWriter(response.OutputStream))
            {
                sw.WriteLine(jsonResponse);
            }
        }

        public override void GetPartitionHandler(GetPartitionRequestStruct request, HttpListenerResponse response)
        {
            var cells = GetPartition(request.cellType, request.partition);
            var jsonResponse = JsonConvert.SerializeObject(cells);
            using (var sw = new StreamWriter(response.OutputStream))
            {
                sw.WriteLine(jsonResponse);
            }
        }

        public StructType GetSchema(string cellType)
        {
            var cellDesc = Global.StorageSchema.CellDescriptors.FirstOrDefault(_ => _.TypeName == cellType);
            return StructType.ConvertFromCellDescriptor(cellDesc);
        }

        public object GetPartitions(string cellType, int batchSize, IEnumerable<JObject> filters)
        {
            var partitions = new List<List<long>>();

            if (batchSize <= 0)
                return partitions;

            var cellDesc = Global.StorageSchema.CellDescriptors.FirstOrDefault(_ => _.TypeName == cellType);
            if (cellDesc == null)
                return partitions;

            var cells = Global.LocalStorage.GenericCellAccessor_Selector().AsQueryable();
            var filtersExpr = FilterExpressionBuilder.BuildExpression(cells, cellDesc, filters);
            var cellsFiltered = cells.Provider.CreateQuery<ICellAccessor>(filtersExpr);

            var part = new List<long>();
            var count = 0;

            foreach (var cell in cellsFiltered)
            {
                part.Add(cell.CellID);
                count++;
                if (count == batchSize)
                {
                    partitions.Add(part);
                    part = new List<long>();
                    count = 0;
                }
            }

            if (part.Count() > 0)
                partitions.Add(part);

            return partitions;
        }

        public IEnumerable<object> GetPartition(string cellType, IEnumerable<long> cellIds)
        {
            var timer = Stopwatch.StartNew();

            var cells = cellIds.Select(id => {
                using (var cell = Global.LocalStorage.UseGenericCell(id))
                {
                    return cell.ToString();
                }
            });

            timer.Stop();
            Log.WriteLine(LogLevel.Info, $"GetPartition[cellType={cellType}, cellIds={cellIds.Count()} succeeded: count={cells.Count()}, timer={(long)timer.Elapsed.TotalMilliseconds}");

            return cells;
        }
    }
}

// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using SparkTrinity.Protocols.TSL;
using Trinity;
using Newtonsoft.Json;
using System.IO;
using Trinity.Diagnostics;
using System.Diagnostics;

namespace SparkTrinity
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
            var partitions = GetPartitions(request.cellType, request.batchSize);
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

        public List<object> GetSchema(string cellType)
        {
            var cellDesc = Global.StorageSchema.CellDescriptors.FirstOrDefault(_ => _.TypeName == cellType);
            if (cellDesc == null)
                return null;

            var schema = new List<object>();
            schema.Add(new
            {
                name = "CellID",
                dataType = typeof(long).Name,
                nullable = false,
                isList = false
            });

            var fields = cellDesc.GetFieldDescriptors();
            foreach (var fd in fields)
            {
                schema.Add(new
                {
                    name = fd.Name,
                    dataType = fd.IsList() ? fd.Type.GenericTypeArguments[0].Name : fd.Type.Name,
                    nullable = fd.Optional,
                    isList = fd.IsList()
                });
            }

            Log.WriteLine(LogLevel.Info, $"GetSchema[{cellType}] succeeded");

            return schema;
        }

        public object GetPartitions(string cellType, int batchSize)
        {
            var partitions = new List<List<long>>();

            if (batchSize <= 0)
                return partitions;

            var cellIds = Global.LocalStorage.GenericCellAccessor_Selector().Where(c => c.TypeName == cellType).Select(c => c.CellID);
            var part = new List<long>();
            var count = 0;

            foreach (var id in cellIds)
            {
                part.Add(id);
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

        public IEnumerable<object> GetPartition(string cellType, int partitions, int partitionId)
        {
            if (partitions <= 0 || partitionId >= partitions)
                return new List<object>();

            var timer = Stopwatch.StartNew();

            var cells = Global.LocalStorage.GenericCellAccessor_Selector()
                .Where(c => c.TypeName == cellType && Math.Abs(c.CellID % partitions) == partitionId)
                .Select(c => c.ToString()).ToList();

            timer.Stop();
            Log.WriteLine(LogLevel.Info, $"GetPartition[cellType={cellType}, partitions={partitions}, partitionId={partitionId}] succeeded: count={cells.Count}, timer={(long)timer.Elapsed.TotalMilliseconds}");

            return cells;
        }
    }
}

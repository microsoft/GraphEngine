// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Diagnostics;

namespace Trinity.Modules.Spark
{
    public interface ISparkTrinityConnector
    {
        StructType GetSchema(string jsonstr);

        object GetPartitions(string jsonstr);

        IEnumerable<object> GetPartition(string jsonstr);
    }

    public class DefaultSparkTrinityConnector : ISparkTrinityConnector
    {
        public ITrinityStorage Storage { get; set; }

        public Func<long, int> GetServerIdFromCellId { get; set; }

        public DefaultSparkTrinityConnector(SparkTrinityModule module)
        {
            Storage = new DefaultTrinityStorage(module);
        }

        public StructType GetSchema(string jsonstr)
        {
            JObject json;
            string cellType;
            if (!Utilities.TryDeserializeObject(jsonstr, out json) ||
                !Utilities.TryGetValue(json, "cellType", out cellType))
            {
                return null;
            }

            return Storage.GetCellSchema(cellType);
        }

        public object GetPartitions(string jsonstr)
        {
            JObject json;
            string cellType;
            int batchSize;
            if (!Utilities.TryDeserializeObject(jsonstr, out json) ||
                !Utilities.TryGetValue(json, "cellType", out cellType) ||
                !Utilities.TryGetValue(json, "batchSize", out batchSize))
            {
                return null;
            }

            var partitions = new List<List<long>>();

            if (batchSize <= 0)
                return partitions;

            IEnumerable<JObject> filters = null;
            Utilities.TryGetEnumerable(json, "filters", out filters);

            var cellIds = Storage.FindCells(cellType, filters == null ? null : filters.Select(f => JsonConvert.SerializeObject(f)));
            foreach(var ids in cellIds)
            {
                if (ids == null)
                    continue;

                var part = new List<long>();
                foreach (var id in ids)
                {
                    part.Add(id);
                    if (part.Count == batchSize)
                    {
                        partitions.Add(part);
                        part = new List<long>();
                    }
                }

                if (part.Count > 0)
                    partitions.Add(part);
            }

            return partitions;
        }

        public IEnumerable<object> GetPartition(string jsonstr)
        {
            if (GetServerIdFromCellId == null)
            {
                Log.WriteLine(LogLevel.Warning, "Func<long, int> GetServerIdFromCellId not set");
                return null;
            }

            JObject json;
            string cellType;
            IEnumerable<long> cellIds;
            if (!Utilities.TryDeserializeObject(jsonstr, out json) ||
                !Utilities.TryGetValue(json, "cellType", out cellType) ||
                !Utilities.TryGetEnumerable(json, "partition", out cellIds))
            {
                return null;
            }

            var cellDesc = Global.StorageSchema.CellDescriptors.FirstOrDefault(_ => _.TypeName == cellType);
            if (cellDesc == null)
                return null;

            IEnumerable<string> fieldNames = null;
            Utilities.TryGetEnumerable(json, "fields", out fieldNames);

            var timer = Stopwatch.StartNew();
            var ids = new List<long>[Global.ServerCount];
            for (int i = 0; i < Global.ServerCount; i++) ids[i] = new List<long>();
            foreach (var id in cellIds) ids[GetServerIdFromCellId(id)].Add(id);

            var cells = new List<string>();
            Parallel.For(0, Global.ServerCount, (serverId) =>
            {
                var res = Storage.LoadCells(serverId, cellType, ids[serverId], fieldNames);
                if (res != null && res.Count() > 0)
                {
                    lock (cells)
                    {
                        cells.AddRange(res);
                    }
                }
            });

            timer.Stop();
            Log.WriteLine(LogLevel.Info, $"GetPartition[cellType={cellType}, cellIds={cellIds.Count()}] succeeded: count={cells.Count()}, timer={(long)timer.Elapsed.TotalMilliseconds}");

            return cells;
        }
    }
}

// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.Modules.Spark.Protocols.TSL;

namespace Trinity.Modules.Spark
{
    public interface ITrinityStorage
    {
        StructType GetCellSchema(string cellType);

        IEnumerable<long>[] FindCells(string cellType, IEnumerable<string> filters);

        IEnumerable<string> LoadCells(int serverId, string cellType, IEnumerable<long> cellIds, IEnumerable<string> fields);
    }

    public class DefaultTrinityStorage : ITrinityStorage
    {
        public SparkTrinityModule SparkTrinityModule { get; set; }

        public Func<long, int> GetServerIdFromCellId { get; set; }

        public DefaultTrinityStorage(SparkTrinityModule module)
        {
            SparkTrinityModule = module;
        }

        public virtual StructType GetCellSchema(string cellType)
        {
            var cellDesc = Global.StorageSchema.CellDescriptors.FirstOrDefault(_ => _.TypeName == cellType);
            return StructType.ConvertFromCellDescriptor(cellDesc);
        }

        public virtual IEnumerable<long>[] FindCells(string cellType, IEnumerable<string> filters)
        {
            var cells = new IEnumerable<long>[Global.ServerCount];
            Parallel.For(0, Global.ServerCount, (serverId) =>
            {
                using (var request = new FindCellsRequestWriter(cellType, filters == null ? null : filters.ToList()))
                {
                    using (var response = SparkTrinityModule.FindCells(serverId, request))
                    {
                        cells[serverId] = response.CellIds.ToList();
                    }
                }
            });
            return cells;
        }

        public virtual IEnumerable<string> LoadCells(int serverId, string cellType, IEnumerable<long> cellIds, IEnumerable<string> fields)
        {
            if (serverId < 0 || serverId >= Global.ServerCount)
            {
                Log.WriteLine(LogLevel.Warning, "Server id {0} out of range [0, {1}].", serverId, Global.ServerCount - 1);
                return null;
            }

            if (cellIds == null || cellIds.Count() == 0)
                return null;

            using (var request = new LoadCellsRequestWriter(cellIds.ToList(), fields == null ? null : fields.ToList()))
            {
                using (var response = SparkTrinityModule.LoadCells(serverId, request))
                {
                    return response.Cells.Select(c => c.ToString()).ToList();
                }
            }
        }
    }
}

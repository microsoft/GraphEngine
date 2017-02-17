// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using FanoutSearch.Protocols.TSL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Storage;

namespace FanoutSearch
{
    public partial class FanoutSearchModule : FanoutSearchBase
    {
        public static string ToJsonArray(IEnumerable<string> array)
        {
            return "[" + string.Join(",", array.Select(element => "\"" + element + "\"")) + "]";
        }

        private IEnumerable<string> _GetEdgeType(ICellAccessor cell, long to)
        {
                foreach(var fd in cell.GetFieldDescriptors())
            {
                    if (fd.Attributes.ContainsKey("GraphEdge") && cell.EnumerateField<long>(fd.Name).Contains(to))
                {
                        yield return fd.Name;
                }
            }
        }

        public override void GetEdgeTypeHandler(EdgeStructReader request, StringStructWriter response)
        {
            using (var cell = s_useICellFunc(request.from))
            {
                response.queryString = ToJsonArray(_GetEdgeType(cell, request.to));
            }
        }

        public override void _GetNodesInfo_implHandler(GetNodesInfoRequestReader request, GetNodesInfoResponseWriter response)
        {
            List<string> fields            = request.fields;
            int          field_cnt         = fields.Count;
            List<long>   secondary_ids     = request.Contains_secondary_ids ? request.secondary_ids : null;
            NodeInfo[]   infos             = new NodeInfo[request.ids.Count];

            Parallel.ForEach(request.ids, (id, state, idx) =>
            {
                try
                {
                    using (var cell = s_useICellFunc(id))
                    {
                        infos[idx] = new NodeInfo
                        {
                            id    = id,
                            values = fields.Select(f =>
                            {
                                switch (f)
                                {
                                    case JsonDSL.graph_outlinks:
                                        return ToJsonArray(_GetEdgeType(cell, secondary_ids[(int)idx]));
                                    case "*":
                                        return cell.ToString();
                                    default:
                                        return cell.get(f);
                                }
                            }).ToList(),
                        };
                    }
                }
                catch // use cell failed. populate the list with an empty NodeInfo.
                {
                    infos[idx] = _CreateEmptyNodeInfo(id, field_cnt);
                }
            });
            response.infoList = infos.ToList();
        }

        private NodeInfo _CreateEmptyNodeInfo(long id, int fieldCount)
        {
            return new NodeInfo { id = id, values = Enumerable.Range(0, fieldCount).Select(f => "").ToList() };
        }
    }
}

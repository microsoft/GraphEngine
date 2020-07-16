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
using System.Threading;
using Trinity.Network.Messaging;
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
            foreach (var field in cell.SelectFields<List<long>>("GraphEdge"))
            {
                if (field.Value.Contains(to))
                {
                    yield return field.Key;
                }
            }
        }

        public override Task GetEdgeTypeHandlerAsync(EdgeStructReader request, StringStructWriter response)
        {
            using (var cell = s_useICellFunc(request.from))
            {
                response.queryString = ToJsonArray(_GetEdgeType(cell, request.to));
            }

            return Task.CompletedTask;
        }

        private NodeInfo _GetNodeInfo(long id, List<string> fields, long secondary_id)
        {
            try
            {
                using (var cell = s_useICellFunc(id))
                {
                    return new NodeInfo
                    {
                        id    = id,
                        values = fields.Select(f =>
                        {
                            switch (f)
                            {
                                case JsonDSL.graph_outlinks:
                                return ToJsonArray(_GetEdgeType(cell, secondary_id));
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
                return _CreateEmptyNodeInfo(id, fields.Count);
            }
        }

        public override Task _GetNodesInfo_implHandlerAsync(GetNodesInfoRequestReader request, GetNodesInfoResponseWriter response)
        {
            List<string> fields = request.fields;
            IEnumerable<long> secondary_ids = request.Contains_secondary_ids ? request.secondary_ids : Enumerable.Repeat(0L, request.ids.Count);

            try
            {
                long msg_approx_len = 0;
                foreach ((long a, long b) in request.ids.Zip(secondary_ids, (a, b) => (a, b)))
                {
                    NodeInfo info = _GetNodeInfo(a, fields, b);
                    msg_approx_len += info.values.Sum(_ => _.Length);
                    if (msg_approx_len > FanoutSearchModule.s_max_rsp_size)
                    {
                        throw new MessageTooLongException($"{nameof(_GetNodesInfo_implHandlerAsync)}: Message too long");
                    }
                    response.infoList.Add(info);
                }

                return Task.CompletedTask;
            }
            catch (AccessorResizeException ex)
            {
                throw new MessageTooLongException($"{nameof(_GetNodesInfo_implHandlerAsync)}: Message too long", ex);
            }
        }

        private NodeInfo _CreateEmptyNodeInfo(long id, int fieldCount)
        {
            return new NodeInfo { id = id, values = Enumerable.Repeat("", fieldCount).ToList() };
        }
    }
}

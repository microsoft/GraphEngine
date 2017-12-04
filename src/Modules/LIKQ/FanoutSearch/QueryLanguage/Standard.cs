// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Trinity.Storage;

namespace FanoutSearch.Standard
{
    public static class g
    {
        public static FanoutSearchDescriptor v(long cellId, IEnumerable<string> select = null)
        {
            return v(new List<long> { cellId }, select);
        }

        public static FanoutSearchDescriptor v(IEnumerable<long> cellId, IEnumerable<string> select = null)
        {
            var ret = new FanoutSearchDescriptor(cellId);
            if (select == null)
                select = new List<string>();
            ret.m_selectFields.Add(select.ToList());
            return ret;
        }

        public static FanoutSearchDescriptor v(object search_body, IEnumerable<string> select = null)
        {
            string json_search_body = Newtonsoft.Json.JsonConvert.SerializeObject(search_body);
            var ret = new FanoutSearchDescriptor(json_search_body);
            if (select == null)
                select = new List<string>();
            ret.m_selectFields.Add(select.ToList());
            return ret;
        }

        public static FanoutSearchDescriptor outV(this FanoutSearchDescriptor fs_desc, Expression<Func<ICellAccessor, Action>> action, IEnumerable<string> select = null)
        {
            fs_desc.AddTraverseStep(action, select);
            return fs_desc;
        }

        public static FanoutSearchDescriptor outV<T>(this FanoutSearchDescriptor fs_desc, Expression<Func<T, Action>> action, IEnumerable<string> select = null) where T : ICellAccessor
        {
            Expression<Func<ICellAccessor, Action>> strongly_typed_action = ExpressionBuilder.GenerateStronglyTypedTraverseAction(action);
            fs_desc.AddTraverseStep(strongly_typed_action, select);
            return fs_desc;
        }

        public static FanoutSearchDescriptor outV(this FanoutSearchDescriptor search, Action action)
        {
            return search.outV(ExpressionBuilder.WrapAction(action));
        }

        public static FanoutSearchDescriptor outV(this EdgeTypeDescriptor ets, Action action)
        {
            return ets.outV(ExpressionBuilder.WrapAction(action));
        }

        public static EdgeTypeDescriptor outE(this FanoutSearchDescriptor fs_desc, params string[] edge_types)
        {
            return new EdgeTypeDescriptor(fs_desc, edge_types);
        }

        public static EdgeTypeDescriptor outE(this EdgeTypeDescriptor ets, params string[] edge_types)
        {
            var fanout_search = ets.outV(ExpressionBuilder.WrapAction(Action.Continue));
            return fanout_search.outE(edge_types);
        }
    }
}

// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Trinity.Storage;

namespace FanoutSearch.LIKQ
{
    using FanoutSearch.Standard;
    public static class KnowledgeGraph
    {
        public static FanoutSearchDescriptor StartFrom(long cellId, IEnumerable<string> select = null)
        {
            return g.v(cellId, select);
        }

        public static FanoutSearchDescriptor StartFrom(IEnumerable<long> cellIds, IEnumerable<string> select = null)
        {
            return g.v(cellIds, select);
        }

        public static FanoutSearchDescriptor StartFrom(object queryObject, IEnumerable<string> select = null)
        {
            return g.v(queryObject, select);
        }
        
        public static FanoutSearchDescriptor VisitNode(this FanoutSearchDescriptor search, Expression<Func<ICellAccessor, Action>> action, IEnumerable<string> select = null)
        {
            return search.outV(action, select);
        }

        public static FanoutSearchDescriptor VisitNode(this FanoutSearchDescriptor search, Action action, IEnumerable<string> select = null)
        {
            return search.outV(ExpressionBuilder.WrapAction(action), select);
        }

        public static FanoutSearchDescriptor VisitNode(this EdgeTypeDescriptor ets, Expression<Func<ICellAccessor, Action>> action, IEnumerable<string> select = null)
        {
            return ets.outV(action, select);
        }

        public static FanoutSearchDescriptor VisitNode(this EdgeTypeDescriptor ets, Action action, IEnumerable<string> select = null)
        {
            return ets.outV(ExpressionBuilder.WrapAction(action), select);
        }

        public static EdgeTypeDescriptor FollowEdge(this FanoutSearchDescriptor search, params string[] edge_types)
        {
            return search.outE(edge_types);
        }

        public static EdgeTypeDescriptor FollowEdge(this EdgeTypeDescriptor ets, params string[] edge_types)
        {
            return ets.outE(edge_types);
        }
    }
}

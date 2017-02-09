// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using FanoutSearch.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Trinity.Storage;

namespace FanoutSearch
{
    public class EdgeTypeDescriptor
    {
        internal EdgeTypeDescriptor(FanoutSearchDescriptor search, params string[] edge_types)
        {
            this.edge_types = edge_types;
            this.fanout_search = search;
        }

        internal string[] edge_types;
        internal FanoutSearchDescriptor fanout_search;

        public FanoutSearchDescriptor outV(Expression<Func<ICellAccessor, Action>> action, IEnumerable<string> select = null)
        {
            fanout_search.m_edgeTypes.Add(this);
            return fanout_search.outV(action, select);
        }

        public FanoutSearchDescriptor outV<T>(Expression<Func<T, Action>> action, IEnumerable<string> select = null) where T : ICellAccessor
        {
            fanout_search.m_edgeTypes.Add(this);
            return fanout_search.outV(action, select);
        }
    }
}

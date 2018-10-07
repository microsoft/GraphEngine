// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using Trinity;
using Trinity.Storage;
using System.Linq.Expressions;

using Newtonsoft.Json.Linq;
using System.Diagnostics;
using FanoutSearch.Protocols.TSL;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections;
using FanoutSearch.Protocols.TSL.FanoutSearch;

namespace FanoutSearch
{
    public class FanoutSearchDescriptor : IEnumerable<PathDescriptor>
    {
        #region Fields
        private static readonly List<string>       s_empty_selection               = new List<string>();

        internal List<Expression>                  m_traverseActions               = new List<Expression>();        //  Starting at 1st hop.
        internal List<EdgeTypeDescriptor>          m_edgeTypes                     = new List<EdgeTypeDescriptor>();//  Starting at edge from origin to 1st hop.
        internal List<List<string>>                m_selectFields                  = new List<List<string>>();      //  Starting at origin.
        internal List<long>                        m_origin;                                                        //  Mutual exclusive: only one of m_origin and m_origin_query
        internal string                            m_origin_query;                                                  //  will be recognized, and the other one should be set to null.
        internal readonly bool                     m_generated_from_json_dsl;

        //  Paginator fields
        internal int                               m_skipCount                     = 0;
        internal int                               m_takeCount                     = 0;

        private IEnumerable<PathDescriptor>        m_results                       = null;
        private Dictionary<string, string>         m_result_metadata               = null;
        #endregion

        private FanoutSearchModule Module
        {
            get
            {
                if (FanoutSearchModule.s_force_run_as_client)
                {
                    return FanoutSearchModule.GetClientModule();
                }
                else
                {
                    return Global.CloudStorage.GetCommunicationModule<FanoutSearchModule>() ?? FanoutSearchModule.GetClientModule();
                }
            }
        }

        internal FanoutSearchDescriptor(IEnumerable<long> cellIds)
        {
            m_origin                  = cellIds.ToList();
            m_origin_query            = null;
            m_generated_from_json_dsl = false;
        }

        internal FanoutSearchDescriptor(string query)
        {
            m_origin_query = query;
            m_origin = null;
            m_generated_from_json_dsl = false;
        }

        internal FanoutSearchDescriptor(string queryPath, JObject queryObject)
        {
            m_origin                  = null;
            m_origin_query            = null;
            m_generated_from_json_dsl = true;

            JsonDSL.ProcessJsonQueryObject(this, queryPath, queryObject);
        }

        internal void AddTraverseStep(Expression<Func<ICellAccessor, Action>> action, IEnumerable<string> select)
        {
            m_traverseActions.Add(action);
            if (m_edgeTypes.Count < m_traverseActions.Count)
                m_edgeTypes.Add(new EdgeTypeDescriptor(this));
            if (select != null)
            {
                /* Ensure m_selectFields.Count == m_traverseActions.Count */
                while (m_selectFields.Count < m_traverseActions.Count)
                    m_selectFields.Add(s_empty_selection);
                /* Then push the current selection */
                m_selectFields.Add(select.ToList());
            }
        }

        public FanoutSearchDescriptor Take(int count)
        {
            return this.Take_impl(0, count);
        }

        public FanoutSearchDescriptor Take(int startIndex, int count)
        {
            return this.Take_impl(startIndex, count);
        }

        private FanoutSearchDescriptor Take_impl(int startIndex, int count)
        {
            Debug.Assert(startIndex >= 0);
            Debug.Assert(count >= 0);
            FanoutSearchDescriptor that = this.MemberwiseClone() as FanoutSearchDescriptor;
            that.m_takeCount = count;
            that.m_skipCount = startIndex;
            that.m_results = null;

            return that;
        }

        public IReadOnlyDictionary<string, string> Metadata
        {
            get
            {
                if (m_results == null)
                {
                    _ExecuteQuery();
                }

                return m_result_metadata;
            }
        }

        internal FanoutQueryMessage Serialize()
        {
            while (m_selectFields.Count <= m_traverseActions.Count) { m_selectFields.Add(new List<string>()); }

            QueryExpressionSecurityCheck();
            if (!m_generated_from_json_dsl) { EvaluateQueryParameters(); }
            if (!m_generated_from_json_dsl && m_origin_query != null)
            {
                try
                {
                    JObject jobj = JsonConvert.DeserializeObject<JObject>(m_origin_query);
                    JsonDSL.ProcessJsonOriginNodeObject(this, jobj);
                    JsonDSL.ProcessJsonSelectField(m_selectFields.First(), jobj);
                }
                catch { throw new FanoutSearchQueryException("Invalid starting node query object"); }
            }

            List<string>       predicates = m_traverseActions.Select(pred => ExpressionSerializer.Serialize(pred)).ToList();
            List<List<string>> edge_types = m_edgeTypes.Select(_ => _.edge_types.ToList()).ToList();

            return new FanoutQueryMessage((byte)m_traverseActions.Count, m_origin, m_origin_query, predicates, edge_types, m_selectFields, m_skipCount, m_takeCount);
        }

        private void _ExecuteQuery()
        {
            var query = Serialize();
            using (var query_msg = new Protocols.TSL.FanoutQueryMessageWriter(query.maxHop, query.origin, query.originQuery, query.predicates, query.edge_types, query.return_selection, query.skip_count, query.take_count))
            {
                var results = Global.CloudStorage[0].FanoutSearchQuery(query_msg);

                switch (results.transaction_id)
                {
                    case FanoutSearchErrorCode.Timeout:
                        // Timeout is an error, not a fault.
                        // So we throw timeout exception.
                        throw new FanoutSearchQueryTimeoutException();
                    case FanoutSearchErrorCode.Error:
                        // If propagated back to a REST client,
                        // result in a 500 internal server error.
                        throw new IOException("FanoutSearch server error.");
                    default:
                        // If propagated back to a REST client,
                        // result in a 500 internal server error.
                        if (results.transaction_id < 0) throw new IOException("FanoutSearch server error.");
                        break;
                }

                m_results = results.paths.Select(pda => new PathDescriptor(pda, m_selectFields)).ToList();
                m_result_metadata = new Dictionary<string, string>();
                for (int i = 0; i<results.metadata_keys.Count; ++i)
                {
                    m_result_metadata[results.metadata_keys[i]] = results.metadata_values[i];
                }
            }
        }

        private void EvaluateQueryParameters()
        {
            TraverseActionRewriter rewriter = new TraverseActionRewriter();
            m_traverseActions = m_traverseActions.Select(_ => rewriter.Visit(_)).ToList();
        }

        private void QueryExpressionSecurityCheck()
        {
            TraverseActionSecurityChecker checker = new TraverseActionSecurityChecker();
            m_traverseActions.ForEach(_ => checker.Visit(_));
        }

        public IEnumerator<PathDescriptor> GetEnumerator()
        {
            if (m_results == null)
            {
                _ExecuteQuery();
            }

            return m_results.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.Storage;

namespace FanoutSearch
{
    class QueryPredicateCompiler
    {
        private MemoryCache m_predicate_cache;
        private readonly CacheItemPolicy m_predicate_cache_policy;

        public QueryPredicateCompiler()
        {
            m_predicate_cache = MemoryCache.Default;
            m_predicate_cache_policy = new CacheItemPolicy();
            m_predicate_cache_policy.SlidingExpiration = TimeSpan.FromMinutes(30);
        }

        internal TimeSpan GetExperationTime()
        {
            return m_predicate_cache_policy.SlidingExpiration;
        }

        /// <returns>returns null if predicate is not compiled and cached.</returns>
        internal Func<ICellAccessor, Action> GetCachedPredicate(string predicate)
        {
            return (Func<ICellAccessor, Action>)m_predicate_cache[GetCachedPredicateKey(predicate)];
        }

        internal List<Func<ICellAccessor, Action>> CompileQueryPredicates(List<string> list)
        {
            var result     = new List<Func<ICellAccessor, Action>>();
            Stopwatch compile_timer = Stopwatch.StartNew();
            var cached_cnt = 0;

            foreach (var pred in list)
            {
                Func<ICellAccessor, Action> func = null;
                string cache_key = GetCachedPredicateKey(pred);

                func = (Func<ICellAccessor, Action>)m_predicate_cache.Get(cache_key);

                if (func == null)
                {
                    func = ExpressionSerializer.DeserializeTraverseAction(pred);
                    m_predicate_cache.Set(cache_key, func, m_predicate_cache_policy);
                }
                else
                {
                    ++cached_cnt;
                }

                result.Add(func);
            }

            compile_timer.Stop();
            Log.WriteLine(LogLevel.Debug, "QueryPredicateCompiler: Predicate compiled. {0} hit in cache. Time = {1}ms.", cached_cnt, compile_timer.ElapsedMilliseconds);

            return result;
        }

        private string GetCachedPredicateKey(string pred)
        {
            return "prediate:" + pred;
        }
    }
}

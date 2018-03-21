// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
#if NETCOREAPP2_0
using Microsoft.Extensions.Caching.Memory;
#else
using System.Runtime.Caching;
#endif
using System.Text;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.Storage;

namespace FanoutSearch
{
    class QueryPredicateCompiler
    {
        private MemoryCache m_predicate_cache;
#if NETCOREAPP2_0
        private readonly MemoryCacheEntryOptions m_entry_options = new MemoryCacheEntryOptions { SlidingExpiration= TimeSpan.FromMinutes(30) };
#else
        private readonly CacheItemPolicy m_predicate_cache_policy;
#endif


        public QueryPredicateCompiler()
        {
#if NETCOREAPP2_0
            m_predicate_cache = new MemoryCache(new MemoryCacheOptions());
#else
            m_predicate_cache = MemoryCache.Default;
            m_predicate_cache_policy = new CacheItemPolicy();
            m_predicate_cache_policy.SlidingExpiration = TimeSpan.FromMinutes(30);
#endif

        }

        internal TimeSpan GetExperationTime()
        {
#if NETCOREAPP2_0
            return m_entry_options.SlidingExpiration.Value; // it does have value here
#else
            return m_predicate_cache_policy.SlidingExpiration;
#endif
        }

        internal List<Func<ICellAccessor, Action>> CompileQueryPredicates(List<string> list)
        {
            var result = new List<Func<ICellAccessor, Action>>();
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
#if NETCOREAPP2_0
                    m_predicate_cache.Set(cache_key, func, m_entry_options);
#else
                    m_predicate_cache.Set(cache_key, func, m_predicate_cache_policy);
#endif
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

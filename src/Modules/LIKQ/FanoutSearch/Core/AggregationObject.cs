// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using FanoutSearch.Protocols.TSL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Storage;

namespace FanoutSearch
{
    class AggregationObject : IDisposable
    {
        #region Local-remote common fields
        public List<FanoutPathDescriptor> results;
        public int aggregationServer;
        public int maxHop;
        public List<Func<ICellAccessor, Action>> predicates;
        public List<List<string>> edgeTypes;
        #endregion

        #region Local aggregation facility
        public SemaphoreSlim local_signals;
        #endregion

        #region Remote aggregation facility
        public int remote_packedMessageCount = 0;
        #endregion

        #region Timing facility
        public Stopwatch stopwatch;
        public Timer commit_latch = null;
        public bool timed_out = false;
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AddPathDescs(FanoutAggregationMessageReader descs)
        {
            if (descs.results.Count != 0)
            {
                lock (this)
                {
                    foreach (var result_pd in descs.results)
                    {
                        results.Add(result_pd);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void CommitAggregationResults(IEnumerable<FanoutPathDescriptor> descs)
        {
            lock (this)
            {
                if (commit_latch != null)
                {
                    commit_latch.Dispose();
                    commit_latch = null;
                }

                if (descs.Count() != 0)
                {
                    results.AddRange(descs);
                }

                ++remote_packedMessageCount;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ReleaseLocalSignal(int count)
        {
            lock (this)
            {
                if (local_signals != null)
                    local_signals.Release(count);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetCommitLatch(System.Action p)
        {
            lock (this)
            {
                if (commit_latch == null)
                {
                    commit_latch = new Timer((state) => { p(); }, null, 20, Timeout.Infinite);
                }
            }
        }

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        private void _Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // our IDispose interface is called
                    lock (this)
                    {
                        local_signals.Dispose();
                        local_signals = null;
                    }
                }
                else
                {
                    // we're in the finalizer, don't lock
                    local_signals.Dispose();
                    local_signals = null;
                }

                _disposedValue = true;
            }
        }

        ~AggregationObject()
        {
            _Dispose(false);
        }

        public void Dispose()
        {
            _Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }


}

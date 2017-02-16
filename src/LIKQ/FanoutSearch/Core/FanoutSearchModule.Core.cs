// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using FanoutSearch.Protocols.TSL;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity;
using Trinity.Diagnostics;
using Trinity.Network;
using Trinity.Storage;
using System.Runtime.Serialization;
using Trinity.Network.Messaging;

namespace FanoutSearch
{
    public partial class FanoutSearchModule : FanoutSearchBase
    {
        /// <summary>
        /// Register the query and initialize the aggregation object (the context),
        /// so that it is ready for fan-out search.
        /// </summary>
        /// <returns>
        /// true on a successful registration. It is guaranteed that the return value only depends
        /// on the validity of the predicates, and the same queryMessage will have the same return
        /// value on all the servers, so that we only have to check on one server for the status.
        /// </returns>
        private bool RegisterQuery(FanoutQueryMessage queryMessage, int transaction_id, int aggregate_server_id)
        {
            try
            {
                var obj = new AggregationObject
                {
                    results = new List<FanoutPathDescriptor>(),
                    local_signals = new SemaphoreSlim(0),
                    aggregationServer = aggregate_server_id,
                    maxHop = queryMessage.maxHop,
                    predicates = m_compiler.CompileQueryPredicates(queryMessage.predicates),
                    edgeTypes = queryMessage.edge_types,
                    stopwatch = new Stopwatch(),
                };

                obj.stopwatch.Start();
                m_aggregationObjects[transaction_id] = obj;
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogLevel.Error, ex.ToString());
                return false;
            }

            return true;
        }

        #region Handlers
        public override void GetTransactionIdHandler(TransactionIdMessageWriter response)
        {
            response.transaction_id = Interlocked.Increment(ref m_globalTransactionId);
        }

        public override void QueryInitializationHandler(QueryInitializationMessageReader request)
        {
            Log.WriteLine(LogLevel.Debug, "Initializing query #{0}", request.transaction_id);
            RegisterQuery(request.message, request.transaction_id, request.aggregate_server_id);
            Log.WriteLine(LogLevel.Debug, "Finished initializing query #{0}", request.transaction_id);
        }

        public override void QueryUninitializationHandler(TransactionIdMessageReader request)
        {
            Log.WriteLine(LogLevel.Debug, "Uninitializing query #{0}", request.transaction_id);
            AggregationObject aggregation_obj;
            if (m_aggregationObjects.TryRemove(request.transaction_id, out aggregation_obj))
                aggregation_obj.Dispose();

            Log.WriteLine(LogLevel.Debug, "Finished uninitializing query #{0}", request.transaction_id);
        }

        /// <summary>
        /// The main query handler.
        /// </summary>
        public override void FanoutSearchQueryHandler(FanoutQueryMessageReader request, FanoutResultMessageWriter response)
        {
            int my_transaction = -1;
            List<ResultPathDescriptor> rpaths = null;
            Stopwatch query_timer = new Stopwatch();
            AggregationObject aggregation_obj = null;
            bool cached = false;
            int eresult = FanoutSearchErrorCode.OK;

            //obtain a transaction id atomically
            using (var _transaction = GetTransactionId(c_master_server_id))
            {
                my_transaction = _transaction.transaction_id;
            }

            Log.WriteLine("Transaction #{0} begins.", my_transaction);
            query_timer.Start();

            if (s_cache_enabled) { aggregation_obj = m_cache.GetCachedQueryResult(request); }

            if (aggregation_obj != null) { cached = true; }
            else { aggregation_obj = _DoFanoutSearch(my_transaction, request); }

            if (aggregation_obj == null) { eresult = FanoutSearchErrorCode.Error; }
            if (aggregation_obj != null && aggregation_obj.timed_out && !s_timeout_return_partial_results) { eresult = FanoutSearchErrorCode.Timeout; aggregation_obj = null; }

            if (aggregation_obj != null) { rpaths = _PullSelectionsAndAssembleResults(my_transaction, request, aggregation_obj); }
            else { rpaths = new List<ResultPathDescriptor>(); }

            response.transaction_id = (aggregation_obj != null) ? my_transaction : eresult;
            response.paths = rpaths;

            if (aggregation_obj != null && s_cache_enabled && !aggregation_obj.timed_out) { m_cache.RegisterQueryResult(my_transaction, request, aggregation_obj); }

            response.metadata_keys.Add("results_pulled_from_cache");
            response.metadata_values.Add(cached.ToString());
            s_metadataUpdateFunc(request, response);

            query_timer.Stop();
            Log.WriteLine("Transaction #{0} finished. Time = {1}ms.", my_transaction, query_timer.ElapsedMilliseconds);
        }

        private AggregationObject _DoFanoutSearch(int transaction_id, FanoutQueryMessageReader request)
        {
            AggregationObject aggregation_obj = null;
            Stopwatch fanout_timer = new Stopwatch();
            bool query_registered = false;
            do
            {
                fanout_timer.Start();
                List<FanoutPathDescriptor> origin_path_descriptors = _GetOriginPathDescriptors(request);

                if (origin_path_descriptors == null)
                {
                    aggregation_obj = null;
                    break;
                }

                //  Broadcast initialization message
                Parallel.For(0, Global.ServerCount, i =>
                {
                    using (var init_msg = new QueryInitializationMessageWriter(request, transaction_id, Global.MyServerID))
                    {
                        QueryInitialization(i, init_msg);
                    }
                });

                /* From this point on, we cannot abort this query without uninitializing our peers. */
                Log.WriteLine(LogLevel.Debug, "Transaction #{0} initialization synchronization complete, time = {1}ms.", transaction_id, fanout_timer.ElapsedMilliseconds);

                query_registered = m_aggregationObjects.TryGetValue(transaction_id, out aggregation_obj);
                if (!query_registered)
                {
                    Log.WriteLine(LogLevel.Error, "Transaction #{0}: Query registration failed.", transaction_id);
                    aggregation_obj = null;
                    break;
                }

                // For 0-hop queries, we simply return what we have in origin_path_descriptors
                if (request.maxHop == 0)
                {
                    aggregation_obj.results = origin_path_descriptors;
                    break;
                }

                _SendSeedMessagesAndWaitForResults(request, transaction_id, aggregation_obj, origin_path_descriptors);

            } while (false);

            // Query complete. Clean it up.
            if (query_registered)
            {
                Parallel.For(0, Global.ServerCount, i =>
                {
                    using (var uninit_msg = new TransactionIdMessageWriter(transaction_id))
                    {
                        QueryUninitialization(i, uninit_msg);
                    }
                });
            }

            fanout_timer.Stop();
            Log.WriteLine("Transaction #{0} Fanout finished. Time = {1}ms.", transaction_id, fanout_timer.ElapsedMilliseconds);
            return aggregation_obj;
        }

        private List<FanoutPathDescriptor> _GetOriginPathDescriptors(FanoutQueryMessageReader request)
        {
            List<FanoutPathDescriptor> origins = new List<FanoutPathDescriptor>();
            if (request.originQuery.Length == 0) // no query string for origin. use the provided origin IDs.
            {
                origins.AddRange(request.origin.Select(_ => new FanoutPathDescriptor(_)));
                Log.WriteLine(LogLevel.Debug, "FanoutSearchQueryHandler: origin = {0}", string.Join(",", request.origin));
            }
            else // use the query string to get origin IDs.
            {
                try
                {
                    Log.WriteLine(LogLevel.Debug, "FanoutSearchQueryHandler: origin query string = {0}", request.originQuery);

                    JObject query_object = JObject.Parse(request.originQuery);
                    object match_object = query_object[JsonDSL.Match];
                    string type_string = (string)query_object[JsonDSL.Type];

                    origins.AddRange(s_indexServiceFunc(match_object, type_string).Select(_ => new FanoutPathDescriptor(_)));
                }
                catch (IndexingServiceNotRegisteredException)
                {
                    Log.WriteLine(LogLevel.Error, "FanoutSearchQueryHandler: index service not registered.");
                    return null;
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "Failed to query index service: {0}", ex.ToString());
                    return null;
                }
            }

            return origins;
        }

        private void _SendSeedMessagesAndWaitForResults(FanoutQueryMessageReader request, int my_transaction, AggregationObject aggregation_obj, List<FanoutPathDescriptor> origin_path_descriptors)
        {
            Debug.Assert(origin_path_descriptors.Count != 0);

            // send the first(seed) search messages out.
            var grouped_origin_path_descs = from pd in origin_path_descriptors
                                            group pd by Global.CloudStorage.GetServerIdByCellId(pd.hop_0);
            var seed_message_cnt = grouped_origin_path_descs.Count();
            var wait_count_per_seed = GetWaitCount(request.maxHop);

            foreach (var g in grouped_origin_path_descs)
            {
                MessageDispatcher.DispatchOriginMessage(g.Key, my_transaction, g);
            }

            var quota_stopwatch = Stopwatch.StartNew();

            int minimum_nowait_result_count = MinimalRequiredResultCount(request);
            if (minimum_nowait_result_count == 0) { minimum_nowait_result_count = int.MaxValue; }

            long waitCount = 0;
            long waitMax = seed_message_cnt * wait_count_per_seed;

            for (; waitCount < waitMax; ++waitCount)
            {
                var time_left = s_query_time_quota - quota_stopwatch.ElapsedMilliseconds;

                if (!_QueryTimeoutEnabled()) { aggregation_obj.local_signals.Wait(); }
                else if (time_left > 0) { if (!aggregation_obj.local_signals.Wait((int)time_left)) break; }
                else { /*time out*/ break; }

                if (aggregation_obj.results.Count >= minimum_nowait_result_count)
                    break;
            }

            if (_QueryTimeoutEnabled() && quota_stopwatch.ElapsedMilliseconds >= s_query_time_quota)
            {
                Log.WriteLine(LogLevel.Warning, "Transaction #{0} timed out. Returning partial results. Signal: {1}/{2}", my_transaction, waitCount, waitMax);
                aggregation_obj.timed_out = true;
            }
        }

        internal static int MinimalRequiredResultCount(FanoutQueryMessageReader request)
        {
            int minimum_result_count;
            if (request.take_count != 0)
            {
                minimum_result_count = request.take_count + request.skip_count;
            }
            else
            {
                minimum_result_count = 0;
            }

            return minimum_result_count;
        }

        private List<ResultPathDescriptor> _PullSelectionsAndAssembleResults(int transaction_id, FanoutQueryMessageReader request, AggregationObject aggregation_obj)
        {
            int result_set_capacity;
            IEnumerable<FanoutPathDescriptor> result_set;

            List<ResultPathDescriptor> rpaths;
            Protocols.TSL.NodeDescriptor r_desc;
            bool[] has_return_selections;
            bool[] has_outlink_selections;
            lock (aggregation_obj)
            {
                if (request.take_count == 0)
                {
                    result_set_capacity = aggregation_obj.results.Count - request.skip_count;
                    result_set = aggregation_obj.results.Skip(request.skip_count);
                }
                else
                {
                    result_set_capacity = request.take_count;
                    result_set = aggregation_obj.results.Skip(request.skip_count).Take(request.take_count);
                }

                if (result_set_capacity < 0) result_set_capacity = 0;
                // Assemble result message.
                rpaths = new List<ResultPathDescriptor>(capacity: result_set_capacity);
                r_desc = new Protocols.TSL.NodeDescriptor(field_selections: null);
                has_return_selections = request.return_selection.Select(_ => _.Count != 0).ToArray();
                has_outlink_selections = request.return_selection.Select(_ => _.Contains(JsonDSL.graph_outlinks)).ToArray();


                foreach (var fpath in result_set)
                {
                    ResultPathDescriptor rpath = new ResultPathDescriptor(nodes: new List<Protocols.TSL.NodeDescriptor>());

                    r_desc.id = fpath.hop_0;
                    r_desc.field_selections = has_return_selections[0] ? new List<string>() : null;
                    rpath.nodes.Add(r_desc);

                    if (fpath.hop_1.HasValue)
                    {
                        r_desc.id = fpath.hop_1.Value;
                        r_desc.field_selections = has_return_selections[1] ? new List<string>() : null;
                        rpath.nodes.Add(r_desc);
                    }

                    if (fpath.hop_2.HasValue)
                    {
                        r_desc.id = fpath.hop_2.Value;
                        r_desc.field_selections = has_return_selections[2] ? new List<string>() : null;
                        rpath.nodes.Add(r_desc);
                    }

                    if (fpath.hop_3.HasValue)
                    {
                        r_desc.id = fpath.hop_3.Value;
                        r_desc.field_selections = has_return_selections[3] ? new List<string>() : null;
                        rpath.nodes.Add(r_desc);
                    }

                    if (fpath.hop_n != null)
                    {
                        int n = 4;
                        foreach (var id in fpath.hop_n)
                        {
                            r_desc.id = id;
                            r_desc.field_selections = has_return_selections[n] ? new List<string>() : null;
                            rpath.nodes.Add(r_desc);
                            ++n;
                        }
                    }

                    rpaths.Add(rpath);
                }
            }

            if (request.return_selection.Any(_ => _.Count > 0))
            {
                Log.WriteLine("Transaction #{0}: pulling selections.", transaction_id);
                Stopwatch pull_selection_timer = Stopwatch.StartNew();

                int hop_count = request.maxHop + 1;
                List<List<string>> return_selections = request.return_selection.Select(s => s.Select(_ => (string)_).ToList()).ToList();
                GetNodesInfoRequestWriter[,] node_info_writers = new GetNodesInfoRequestWriter[hop_count, Global.ServerCount];
                GetNodesInfoResponse[,] node_info_readers = new GetNodesInfoResponse[hop_count, Global.ServerCount];
                int[,] reader_idx = new int[hop_count, Global.ServerCount];
                Func<long, int> hash_func = Global.CloudStorage.GetServerIdByCellId;

                Parallel.For(0, hop_count, i =>
                {
                    if (has_return_selections[i])
                    {
                        //  create msg
                        for (int j = 0; j < Global.ServerCount; ++j)
                        {
                            node_info_writers[i, j] = new GetNodesInfoRequestWriter(fields: return_selections[i]);
                            if (has_outlink_selections[i])
                            {
                                node_info_writers[i, j].secondary_ids = new List<long>();
                            }
                        }

                        //  populate msg
                        foreach (var rpath in rpaths)
                        {
                            if (i < rpath.nodes.Count)
                            {
                                var id = rpath.nodes[i].id;
                                node_info_writers[i, hash_func(id)].ids.Add(id);
                                if (has_outlink_selections[i])
                                {
                                    long edge_dest_id = (i < rpath.nodes.Count - 1) ? rpath.nodes[i + 1].id : -1;
                                    node_info_writers[i, hash_func(id)].secondary_ids.Add(edge_dest_id);
                                }
                            }
                        }

                        //  dispatch msg
                        Parallel.For(0, Global.ServerCount, j =>
                        {
                            var reader = _GetNodesInfo_impl(j, node_info_writers[i, j]);
                            node_info_readers[i, j] = reader;
                            reader.Dispose();
                        });

                        //  consume msg
                        foreach (var rpath in rpaths)
                        {
                            if (i < rpath.nodes.Count)
                            {
                                var id = rpath.nodes[i].id;
                                var j = hash_func(id);
                                var idx = reader_idx[i, j]++;
                                rpath.nodes[i].field_selections.AddRange(node_info_readers[i, j].infoList[idx].values);
                            }
                        }

                        //  destruct msg
                        for (int j = 0; j < Global.ServerCount; ++j)
                        {
                            node_info_writers[i, j].Dispose();
                        }
                    }
                });

                pull_selection_timer.Stop();
                Log.WriteLine("Transaction #{0}: pulling selections complete. Time = {1}ms.", transaction_id, pull_selection_timer.ElapsedMilliseconds);
            }
            return rpaths;
        }

        internal unsafe void FanoutSearch_impl_Send(int moduleId, byte* bufferPtr, int length)
        {
            this.SendMessage(
                moduleId,
                bufferPtr,
                length);
        }

        public override void FanoutSearch_implHandler(FanoutSearchMessagePackageReader request)
        {
            throw new NotImplementedException("Overridden by raw handler");
        }

        //  The original message package:
        //  struct FanoutSearchMessagePackage
        //  {
        //  	int hop;
        //  	int transaction_id;
        //  	List<FanoutPathDescriptor> paths;
        //  }
        //  The binary replication:
        //  [header_len|msg_type|msg_id|hop(4B)|transaction(4B)|paths(array of long)]
        //  The # of paths can be calculated:
        //  (header_len-12)/8/(hop+1)
        //  - or -
        //  (AsynReqArgs.Size - 8)/8/(hop+1)

        private unsafe void FanoutSearch_impl_Recv(AsynReqArgs request_args)
        {
            int request_size = request_args.Size;
            byte* request_buffer = request_args.Buffer + request_args.Offset;
            int current_hop = *(int*)(request_buffer);
            int request_transaction_id = *(int*)(request_buffer + 4);
            int request_path_cnt = (request_size - 2 * sizeof(int)) / (sizeof(long) * (current_hop + 1));
            long* request_paths_ptr = (long*)(request_buffer + 2 * sizeof(int));
            int single_path_len = current_hop + 1;

            AggregationObject aggregation_obj;

            //Console.WriteLine("hop: {0}", current_hop);
            //Console.WriteLine("tx: {0}", request_transaction_id);

            if (!m_aggregationObjects.TryGetValue(request_transaction_id, out aggregation_obj)
                ||
                (_QueryTimeoutEnabled() && aggregation_obj.stopwatch.ElapsedMilliseconds > s_query_time_quota))
            { /* Timeout. */ return; }

            var predicate = current_hop == 0 ?
                p => Action.Continue :
                aggregation_obj.predicates[current_hop - 1];

            if (current_hop == aggregation_obj.maxHop)
            {
                System.Collections.Concurrent.ConcurrentBag<int> result_indices = new System.Collections.Concurrent.ConcurrentBag<int>();

                Parallel.For(0, request_path_cnt, i =>
                {
                    long cell_id = request_paths_ptr[i * single_path_len + current_hop];
                    try
                    {
                        Verbs.m_path_ptr = request_paths_ptr + i * single_path_len;
                        Verbs.m_path_len = single_path_len;
                        using (var cell = s_useICellFunc(cell_id))
                        {
                            if ((~predicate(cell)).HasFlag(c_action_inv_return))
                                result_indices.Add(i);
                        }
                    }
                    catch { }
                });

                var results = result_indices.Select(i => GetPathDescriptor(&request_paths_ptr[i * single_path_len], current_hop)).ToList();
                CommitAggregationResults(request_transaction_id, aggregation_obj, results);
                return;
            }

            var edgeType_list = aggregation_obj.edgeTypes[current_hop];
            bool enumerate_all_edges = edgeType_list.Count == 0;
            var negate_edge_types = new HashSet<string>(edgeType_list.Where(_ => _.FirstOrDefault() == '!').Select(_ => _.Substring(1)));

            if (negate_edge_types.Count == 0)
                negate_edge_types = null;

            int enumerated_path_cnt = 0;

            using (var dispatcher = new MessageDispatcher(current_hop + 1, request_transaction_id))
            {
                Lazy<List<FanoutPathDescriptor>> intermediate_result_paths = new Lazy<List<FanoutPathDescriptor>>(isThreadSafe: true);
                Parallel.For(0, request_path_cnt, (i, loopstate) =>
                {
                    long cell_id = request_paths_ptr[i * single_path_len + current_hop];
                    long last_id = -1;
                    if (current_hop > 0)
                        last_id = request_paths_ptr[i * single_path_len + (current_hop - 1)];
                    try
                    {
                        Verbs.m_path_ptr = request_paths_ptr + i * single_path_len;
                        Verbs.m_path_len = single_path_len;
                        using (var cell = s_useICellFunc(cell_id))
                        {
                            var action = ~predicate(cell);

                            if (action.HasFlag(c_action_inv_return))
                            {
                                var intermediate_paths = intermediate_result_paths.Value;
                                lock (intermediate_paths)
                                {
                                    intermediate_paths.Add(GetPathDescriptor(&request_paths_ptr[i * single_path_len], current_hop));
                                }
                            }

                            if (action.HasFlag(c_action_inv_continue))
                            {
                                VisitNeighbors(current_hop, edgeType_list, enumerate_all_edges, negate_edge_types, dispatcher, &request_paths_ptr[i * single_path_len], last_id, cell);
                            }
                        }
                    }
                    catch { }

                    if (0 == (enumerated_path_cnt & 0xFF) && _QueryTimeoutEnabled() && aggregation_obj.stopwatch.ElapsedMilliseconds > s_query_time_quota)
                    {
                        loopstate.Break();
                        return;
                    }

                    ++enumerated_path_cnt;
                });//END Parallel.For
                if (intermediate_result_paths.IsValueCreated)
                {
                    using (var intermediate_results = new FanoutAggregationMessageWriter(intermediate_result_paths.Value, request_transaction_id))
                    {
                        IntermediateResult(aggregation_obj.aggregationServer, intermediate_results);
                    }
                }
            }
        }

        /// <summary>
        /// Commit = the step before pushing the results to the aggregator
        /// </summary>
        private void CommitAggregationResults(int transaction_id, AggregationObject aggregation_obj, IEnumerable<FanoutPathDescriptor> results)
        {
            aggregation_obj.CommitAggregationResults(results);

            if (aggregation_obj.aggregationServer == Global.MyServerId)
            {
                aggregation_obj.ReleaseLocalSignal(1);
            }
            else
            {
                aggregation_obj.SetCommitLatch(() =>
                {
                    lock (aggregation_obj)
                    {
                        using (var result_msg = new FanoutAggregationMessageWriter(aggregation_obj.results, transaction_id, packaged_message_cnt: aggregation_obj.remote_packedMessageCount))
                        {
                            AggregateResult(aggregation_obj.aggregationServer, result_msg);
                        }
                        Console.WriteLine("Sending {0} packed messages", aggregation_obj.remote_packedMessageCount);
                        aggregation_obj.results.Clear();
                        aggregation_obj.remote_packedMessageCount = 0;
                    }
                });

            }
        }

        public override void AggregateResultHandler(FanoutAggregationMessageReader request)
        {
            AggregationObject obj;

            if (m_aggregationObjects.TryGetValue(request.transaction_id, out obj))
            {
                obj.AddPathDescs(request);

                if (request.packaged_message_cnt != 0)
                    obj.ReleaseLocalSignal(request.packaged_message_cnt);
                else
                    obj.ReleaseLocalSignal(1);
            }
            // !if we don't get aggregate object, it means time out. we don't want
            // incomplete results in our cache, so we ignore.
        }

        public override void IntermediateResultHandler(FanoutAggregationMessageReader request)
        {
            AggregationObject obj;

            if (m_aggregationObjects.TryGetValue(request.transaction_id, out obj))
            {
                obj.AddPathDescs(request);
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void VisitNeighbors(int current_hop, List<string> edgeType_list, bool enumerate_all_edges, HashSet<string> negate_edge_types, MessageDispatcher dispatcher, long* pathptr, long last_id, ICellAccessor cell)
        {
            if (enumerate_all_edges)
            {
                foreach (var neighbor_node in cell.EnumerateValues<long>("GraphEdge"))
                {
                    if (neighbor_node != last_id)
                        VisitNeighbor(current_hop, dispatcher, pathptr, neighbor_node);
                }
            }
            else if (negate_edge_types == null)
            {
                foreach (var edge_type in edgeType_list)
                {
                    try
                    {
                        foreach (var neighbor_node in cell.GetField<List<long>>(edge_type))
                        {
                            if (neighbor_node != last_id)
                                VisitNeighbor(current_hop, dispatcher, pathptr, neighbor_node);
                        }
                    }
                    catch { }
                }
            }
            else
            {
                foreach (var edges in cell.SelectFields<List<long>>("GraphEdge"))
                {
                    if (negate_edge_types.Contains(edges.Key))
                        continue;
                    foreach (var neighbor_node in edges.Value)
                    {
                        if (neighbor_node != last_id)
                            VisitNeighbor(current_hop, dispatcher, pathptr, neighbor_node);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void VisitNeighbor(int current_hop, MessageDispatcher dispatcher, long* path, long neighbor_node)
        {
            dispatcher.addAugmentedPath(path, current_hop, neighbor_node);
        }
        #endregion

    }

    [Serializable]
    internal class IndexingServiceNotRegisteredException : Exception { }
}

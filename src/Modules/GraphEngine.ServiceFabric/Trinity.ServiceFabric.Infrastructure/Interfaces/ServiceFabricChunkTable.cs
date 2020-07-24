// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Trinity.Diagnostics;
using Trinity.DynamicCluster;
using Trinity.DynamicCluster.Consensus;
using Trinity.Storage;
using Newtonsoft.Json;

namespace Trinity.ServiceFabric.Infrastructure.Interfaces
{
    class ServiceFabricChunkTable : IChunkTable
    {
        private CancellationToken                               m_cancel;
        private IReliableDictionary<Guid, byte[]>               m_chunktable     = null;
        private Task                                            m_inittask       = null;

        /// <inheritdocs/>
        public void Start(CancellationToken cancellationToken)
        {
            m_cancel   = cancellationToken;
            m_inittask = InitChunkTablesAsync();
        }

        private async Task InitChunkTablesAsync()
        {
            m_chunktable = await ServiceFabricUtils.CreateReliableStateAsync<IReliableDictionary<Guid, byte[]>>("Trinity.ServiceFabric.GarphEngine.Infrastructure.ChunkTable");
        }

        private async Task EnsureChunkTables()
        {
            var task = m_inittask;
            if (task != null)
            {
                await task;
                m_inittask = null;
            }
        }

        /// <inheritdocs/>
        public async Task DeleteEntry(Guid replicaId)
        {
            await EnsureChunkTables();
            using (var tx = ServiceFabricUtils.CreateTransaction())
            {
                await ServiceFabricUtils.DoWithTimeoutRetry(
                    async () => await m_chunktable.TryRemoveAsync(tx, replicaId));
            }
        }

        /// <inheritdocs/>
        public void Dispose() { }

        /// <inheritdocs/>
        public async Task<IEnumerable<Chunk>> GetChunks(Guid replicaId)
        {
            await EnsureChunkTables();
            return await GetChunks_impl(replicaId) ?? Enumerable.Empty<Chunk>();
        }

        /// <inheritdocs/>
        public async Task<IEnumerable<Chunk>> GetChunks()
        {
            await EnsureChunkTables();
            return await GetChunks_impl(null) ?? Enumerable.Empty<Chunk>();
        }

        private async Task<IEnumerable<Chunk>> GetChunks_impl(Guid? replicaId)
        {
            using (var tx = ServiceFabricUtils.CreateTransaction())
            {
retry:
                try
                {
                    if (replicaId.HasValue)
                    {
                        var res = await m_chunktable.TryGetValueAsync(tx, replicaId.Value);
                        if (res.HasValue) { return Utils.Deserialize<Chunk[]>(res.Value); }
                    }
                    else
                    {
                        var res = await m_chunktable.CreateEnumerableAsync(tx, EnumerationMode.Unordered);
                        var enumerator = res.GetAsyncEnumerator();
                        List<Chunk> chunks = new List<Chunk>();
                        while(await enumerator.MoveNextAsync(m_cancel))
                        {
                            chunks.AddRange(Utils.Deserialize<Chunk[]>(enumerator.Current.Value));
                        }
                        return chunks;
                    }
                }
                catch (TimeoutException) { await Task.Delay(1000, m_cancel); goto retry; }
                catch (Exception ex) { Log.WriteLine(LogLevel.Error, "{0}", $"ServiceFabricChunkTable: {ex.ToString()}"); }
                finally { await tx.CommitAsync(); }
            }
            return null;
        }

        /// <inheritdocs/>
        public async Task SetChunks(Guid replicaId, IEnumerable<Chunk> chunks)
        {
            var payload = Utils.Serialize(chunks.ToArray());
            await EnsureChunkTables();
            using (var tx = ServiceFabricUtils.CreateTransaction())
            {
                await ServiceFabricUtils.DoWithTimeoutRetry(
                    async () => await m_chunktable.SetAsync(tx, replicaId, payload),
                    async () => await tx.CommitAsync());
            }
        }
    }
}

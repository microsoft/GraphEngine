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

namespace Trinity.ServiceFabric.GarphEngine.Infrastructure.Interfaces
{
    using ReplicaInformation = DynamicCluster.Storage.ReplicaInformation;
    class ServiceFabricChunkTable : IChunkTable
    {
        private CancellationToken                               m_cancel;
        private IReliableDictionary<Guid, byte[]>               m_chunktable     = null;
        private IReliableDictionary<Guid, byte[]>[]             m_allchunktables = null;
        private Task                                            m_inittask       = null;

        public void Start(CancellationToken cancellationToken)
        {
            m_cancel   = cancellationToken;
            m_inittask = InitChunkTablesAsync();
        }

        private async Task InitChunkTablesAsync()
        {
            m_allchunktables = await Utils.Integers(GraphEngineStatefulServiceRuntime.Instance.PartitionCount)
                              .Select(p => ServiceFabricUtils.CreateReliableStateAsync<IReliableDictionary<Guid, byte[]>>
                                  (this, "Trinity.ServiceFabric.GarphEngine.Infrastructure.ChunkTable", p))
                              .Unwrap();
            m_chunktable = m_allchunktables[GraphEngineStatefulServiceRuntime.Instance.PartitionId];
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

        public bool IsMaster => GraphEngineStatefulServiceRuntime.Instance?.Role == ReplicaRole.Primary;

        public async Task DeleteEntry(Guid replicaId)
        {
            await EnsureChunkTables();
            using (var tx = ServiceFabricUtils.CreateTransaction())
            {
                await ServiceFabricUtils.DoWithTimeoutRetry(
                    async () => await m_chunktable.TryRemoveAsync(tx, replicaId));
            }
        }

        public void Dispose() { }

        public async Task<IEnumerable<Chunk>> GetChunks(ReplicaInformation replicaInfo)
        {
            await EnsureChunkTables();
            return await GetChunks_impl(replicaInfo.PartitionId, replicaInfo.Id) ?? Enumerable.Empty<Chunk>();
        }

        private async Task<IEnumerable<Chunk>> GetChunks_impl(int p, Guid replicaId)
        {
            using (var tx = ServiceFabricUtils.CreateTransaction())
            {
retry:
                try
                {
                    var res = await m_allchunktables[p].TryGetValueAsync(tx, replicaId);
                    if (res.HasValue) { return Utils.Deserialize<Chunk[]>(res.Value); }
                }
                catch (TimeoutException) { await Task.Delay(1000); goto retry; }
                catch (Exception ex) { Log.WriteLine(LogLevel.Error, "{0}", $"ServiceFabricChunkTable: {ex.ToString()}"); }
                finally { await tx.CommitAsync(); }
            }
            return null;
        }

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

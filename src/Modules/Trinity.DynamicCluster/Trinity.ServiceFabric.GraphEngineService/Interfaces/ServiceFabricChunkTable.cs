using System;
using System.Collections.Generic;
using System.Fabric;
using Trinity.DynamicCluster.Consensus;
using Trinity.Storage;
using System.Threading;
using System.Linq;
using Trinity.Network;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Trinity.DynamicCluster.Storage;
using Microsoft.ServiceFabric.Data;
using Trinity.DynamicCluster;
using System.Diagnostics;
using Trinity.Diagnostics;

namespace Trinity.ServiceFabric.Interfaces
{
    class ServiceFabricChunkTable : IChunkTable
    {
        private CancellationToken                               m_cancel;
        private IReliableDictionary<Guid, IEnumerable<Chunk>>   m_chunktable     = null;
        private IReliableDictionary<Guid, IEnumerable<Chunk>>[] m_allchunktables = null;
        private IReliableStateManager                           m_statemgr       = null;
        private Task                                            m_inittask       = null;
        private Dictionary<Guid, int>                           m_partitionCache = null;

        public void Start(CancellationToken cancellationToken)
        {
            m_cancel   = cancellationToken;
            m_statemgr = GraphEngineService.Instance.StateManager;
            m_partitionCache = new Dictionary<Guid, int>();
            m_inittask = InitChunkTablesAsync();
        }

        private async Task InitChunkTablesAsync()
        {
            var ctasks = Utils.Integers(GraphEngineService.Instance.PartitionCount).Select(CreateChunkTableAsync).ToArray();
            await Task.Factory.ContinueWhenAll(ctasks, ts => m_allchunktables = ts.Select(_ => _.Result).ToArray());
            m_chunktable = m_allchunktables[GraphEngineService.Instance.PartitionId];
        }

        private async Task<IReliableDictionary<Guid, IEnumerable<Chunk>>> CreateChunkTableAsync(int p)
        {
            var ctname = $"GraphEngine.ChunkTable-P{p}";
            await GraphEngineService.Instance.GetRoleAsync();

retry:

            try
            {
                if (IsMaster)
                {
                    return await m_statemgr.GetOrAddAsync<IReliableDictionary<Guid, IEnumerable<Chunk>>>(ctname);
                }
                else
                {
                    var result = await m_statemgr.TryGetAsync<IReliableDictionary<Guid, IEnumerable<Chunk>>>(ctname);
                    if (result.HasValue) return result.Value;
                    else
                    {
                        await Task.Delay(1000);
                        goto retry;
                    }
                }
            }
            catch (TimeoutException) { goto retry; }
            catch (FabricNotReadableException) { Debug.Assert(false, "Fabric not readable from Primary/ActiveSecondary"); throw; }
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

        public bool IsMaster => GraphEngineService.Instance?.Role == ReplicaRole.Primary;

        public async Task DeleteEntry(Guid replicaId)
        {
            await EnsureChunkTables();
            using (var tx = m_statemgr.CreateTransaction())
            {
                await m_chunktable.TryRemoveAsync(tx, replicaId);
            }
        }

        public void Dispose() { }

        public async Task<IEnumerable<Chunk>> GetChunks(Guid replicaId)
        {
            await EnsureChunkTables();
            if (m_partitionCache.TryGetValue(replicaId, out var p))
            {
                return await GetChunks_impl(p, replicaId);
            }

            for (int i = 0; i<m_allchunktables.Length; ++i)
            {
                var res = await GetChunks_impl(i, replicaId);
                if (res != null)
                {
                    m_partitionCache[replicaId] = i;
                    return res;
                }
            }

            return Enumerable.Empty<Chunk>();
        }

        private async Task<IEnumerable<Chunk>> GetChunks_impl(int p, Guid replicaId)
        {
            using (var tx = m_statemgr.CreateTransaction())
            {
retry:
                try
                {
                    var res = await m_allchunktables[p].TryGetValueAsync(tx, replicaId);
                    if (res.HasValue) return res.Value;
                }
                catch (TimeoutException) { await Task.Delay(1000); goto retry; }
                catch (Exception ex) { Log.WriteLine(LogLevel.Error, "{0}", $"ServiceFabricChunkTable: {ex.ToString()}"); }
                finally { await tx.CommitAsync(); }
            }
            return null;
        }

        public async Task SetChunks(Guid replicaId, IEnumerable<Chunk> chunks)
        {
            await EnsureChunkTables();
            using (var tx = m_statemgr.CreateTransaction())
            {
retry:
                try
                {
                    await m_chunktable.SetAsync(tx, replicaId, chunks);
                    await tx.CommitAsync();
                }
                catch (TimeoutException) { await Task.Delay(1000); goto retry; }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "{0}", $"ServiceFabricChunkTable: {ex.ToString()}");
                    tx.Abort();
                    throw;
                }
            }
        }
    }
}

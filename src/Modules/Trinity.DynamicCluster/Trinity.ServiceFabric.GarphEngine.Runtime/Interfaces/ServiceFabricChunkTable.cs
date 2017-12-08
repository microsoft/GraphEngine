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
using Trinity.ServiceFabric.GarphEngine.Infrastructure;

namespace Trinity.ServiceFabric.Interfaces
{
    using ReplicaInformation = Trinity.DynamicCluster.Storage.ReplicaInformation;
    class ServiceFabricChunkTable : IChunkTable
    {
        private CancellationToken                               m_cancel;
        private IReliableDictionary<Guid, IEnumerable<Chunk>>   m_chunktable     = null;
        private IReliableDictionary<Guid, IEnumerable<Chunk>>[] m_allchunktables = null;
        private IReliableStateManager                           m_statemgr       = null;
        private Task                                            m_inittask       = null;

        public void Start(CancellationToken cancellationToken)
        {
            m_cancel   = cancellationToken;
            m_statemgr = GraphEngineStatefulServiceRuntime.Instance.StateManager;
            m_inittask = InitChunkTablesAsync();
        }

        private async Task InitChunkTablesAsync()
        {
            var ctasks = Utils.Integers(GraphEngineStatefulServiceRuntime.Instance.PartitionCount).Select(CreateChunkTableAsync).ToArray();
            await Task.Factory.ContinueWhenAll(ctasks, ts => m_allchunktables = ts.Select(_ => _.Result).ToArray());
            m_chunktable = m_allchunktables[GraphEngineStatefulServiceRuntime.Instance.PartitionId];
        }

        private async Task<IReliableDictionary<Guid, IEnumerable<Chunk>>> CreateChunkTableAsync(int p)
        {
            var ctname = $"GraphEngine.ChunkTable-P{p}";
            await GraphEngineStatefulServiceRuntime.Instance.GetRoleAsync();

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
            catch (FabricNotReadableException)
            { Log.WriteLine("Fabric not readable from Primary/ActiveSecondary, retrying."); await Task.Delay(1000); goto retry; }
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
            using (var tx = m_statemgr.CreateTransaction())
            {
                await m_chunktable.TryRemoveAsync(tx, replicaId);
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

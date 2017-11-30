using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Consensus;
using Trinity.DynamicCluster.Tasks;
using Microsoft.ServiceFabric.Data;
using Trinity.DynamicCluster;

namespace Trinity.ServiceFabric.Interfaces
{
    class ReliableTaskQueue : ITaskQueue
    {
        private CancellationToken m_cancel;
        private IReliableQueue<ITask> m_queue = null;
        private IReliableQueue<ITask>[] m_allqueues = null;
        private IReliableStateManager m_statemgr = null;
        private ITransaction m_tx = null;

        public void Start(CancellationToken cancellationToken)
        {
            m_cancel = cancellationToken;
            m_statemgr = GraphEngineService.Instance.StateManager;
            EnsureQueues().Wait();
        }

        private async Task EnsureQueues()
        {
            if (m_queue == null)
            {
                var qtasks = Utils.Integers(GraphEngineService.Instance.PartitionCount).Select(CreatePartitionQueue).ToArray();
                await Task.Factory.ContinueWhenAll(qtasks, ts => m_allqueues = ts.Select(_ => _.Result).ToArray());
                m_queue = m_allqueues[GraphEngineService.Instance.PartitionId];
            }
        }


        private async Task<IReliableQueue<ITask>> CreatePartitionQueue(int p)
        {
            if (!IsMaster) return null;
            var qname = $"GraphEngine.TaskQueue-P{p}";
            retry:
            try { return await m_statemgr.GetOrAddAsync<IReliableQueue<ITask>>(qname); }
            catch (TimeoutException) { goto retry; }
        }

        public void Dispose() { if (m_tx != null) ReleaseTx(); }

        public bool IsMaster => GraphEngineService.Instance?.Role == ReplicaRole.Primary;

        public async Task<ITask> GetTask(CancellationToken token)
        {
            await EnsureQueues();

            if (m_tx != null) throw new InvalidOperationException("ReliableTaskQueue: only one task allowed in a transaction.");
            m_tx = m_statemgr.CreateTransaction();

            try
            {
                var result = await m_queue.TryDequeueAsync(m_tx, TimeSpan.FromSeconds(10), m_cancel);
                if (result.HasValue) return result.Value;
            }
            catch (TimeoutException) { }
            catch (TaskCanceledException) { }
            finally { ReleaseTx(); }

            return null;
        }

        private void ReleaseTx()
        {
            try { m_tx.Dispose(); }
            finally { m_tx = null; }
        }

        public async Task TaskCompleted(ITask task)
        {
            await m_tx.CommitAsync();
            ReleaseTx();
        }

        public Task TaskFailed(ITask task)
        {
            if (m_tx == null) throw new NullReferenceException("ReliableTaskQueue: transaction not found");
            ReleaseTx();
            return Task.FromResult(0);
        }

        public async Task PostTask(ITask task, int partitionId)
        {
            await EnsureQueues();
            using (var tx = m_statemgr.CreateTransaction())
            {
                retry:
                try
                {
                    await m_allqueues[partitionId].EnqueueAsync(tx, task, TimeSpan.FromSeconds(10), m_cancel);
                    await tx.CommitAsync();
                }
                catch (TimeoutException) { goto retry; }
                catch (OperationCanceledException) { }
            }
        }
    }
}

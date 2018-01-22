// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Trinity.DynamicCluster;
using Trinity.DynamicCluster.Consensus;
using Trinity.DynamicCluster.Tasks;
using System.Collections.Concurrent;

namespace Trinity.ServiceFabric.GarphEngine.Infrastructure.Interfaces
{
    class ReliableTaskQueue : ITaskQueue
    {
        private CancellationToken              m_cancel;
        private IReliableQueue<byte[]>         m_queue          = null;
        private IReliableDictionary<Guid, int> m_tagCounter     = null;
        private Task                           m_initqueue_task = null;
        private ConcurrentDictionary<ITask, ITransaction> m_tx  = null;

        public void Start(CancellationToken cancellationToken)
        {
            m_cancel         = cancellationToken;
            m_tx             = new ConcurrentDictionary<ITask, ITransaction>();
            m_initqueue_task = InitAsync();
        }

        private async Task InitAsync()
        {
            m_queue      = await ServiceFabricUtils.CreateReliableStateAsync<IReliableQueue<byte[]>>
                ("Trinity.ServiceFabric.GarphEngine.Infrastructure.TaskQueue");
            m_tagCounter = await ServiceFabricUtils.CreateReliableStateAsync<IReliableDictionary<Guid, int>>
                ("Trinity.ServiceFabric.GarphEngine.Infrastructure.TaskTagCounter");
        }

        private async Task EnsureInit()
        {
            var task = m_initqueue_task;
            if (task != null)
            {
                await task;
                m_initqueue_task = null;
            }
        }

        public void Dispose()
        {
            if (m_tx != null) foreach (var tx in m_tx.Values)
                {
                    tx.Dispose();
                }
        }

        public async Task<ITask> GetTask(CancellationToken token)
        {
            await EnsureInit();
            ITransaction tx = null;
            ITask ret = null;
            try
            {
                tx = ServiceFabricUtils.CreateTransaction();
                await ServiceFabricUtils.DoWithTimeoutRetry(async () =>
                {
                    var result = await m_queue.TryDequeueAsync(tx, TimeSpan.FromSeconds(10), m_cancel);
                    if (result.HasValue) ret = Utils.Deserialize<ITask>(result.Value);
                });
                if (ret == null) { tx.Dispose(); return null; }
                m_tx[ret] = tx;
                return ret;
            }
            catch
            {
                ReleaseTx(ret, tx);
                throw;
            }
        }

        private void ReleaseTx(ITask task, ITransaction tx)
        {
            tx?.Dispose();
            if (task != null) m_tx?.TryRemove(task, out _);
        }

        public async Task RemoveTask(ITask task)
        {
            if (!m_tx.TryGetValue(task, out var tx)) return;
            try
            {
                await ServiceFabricUtils.DoWithTimeoutRetry(
                    async () => await m_tagCounter.AddOrUpdateAsync(tx, task.Tag, _ => 0, (k, v) => v-1, TimeSpan.FromSeconds(10), m_cancel),
                    async () => await tx.CommitAsync());
            }
            finally
            {
                ReleaseTx(task, tx);
            }
        }

        public async Task UpdateTask(ITask task)
        {
            if (!m_tx.TryGetValue(task, out var tx)) return;
            try
            {
                await ServiceFabricUtils.DoWithTimeoutRetry(
                    async () => await m_queue.EnqueueAsync(tx, Utils.Serialize(task), TimeSpan.FromSeconds(10), m_cancel),
                    async () => await tx.CommitAsync());
            }
            finally
            {
                ReleaseTx(task, tx);
            }
        }

        public async Task PostTask(ITask task)
        {
            await EnsureInit();
            using (var tx = ServiceFabricUtils.CreateTransaction())
            {
                await ServiceFabricUtils.DoWithTimeoutRetry(
                    async () => await m_queue.EnqueueAsync(tx, Utils.Serialize(task), TimeSpan.FromSeconds(10), m_cancel),
                    async () => await m_tagCounter.AddOrUpdateAsync(tx, task.Tag, 1, (k, v) => v+1),
                    async () => await tx.CommitAsync());
            }
        }

        public async Task Wait(Guid tag)
        {
            await EnsureInit();
            await ServiceFabricUtils.DoWithTimeoutRetry(
                async () =>
                {
                    using (var tx = ServiceFabricUtils.CreateTransaction())
                    {
                        int cnt = 0;
                        var result = await m_tagCounter.TryGetValueAsync(tx, tag, TimeSpan.FromSeconds(10), m_cancel);
                        if (result.HasValue) cnt = result.Value;
                        if (cnt != 0) throw new TimeoutException();
                    }
                });
        }
    }
}

using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.DynamicCluster.Consensus;
using Trinity.DynamicCluster.DynamicCluster;
using Trinity.DynamicCluster.Storage;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Health
{
    class HealthMonitor : IDisposable
    {
        private CancellationToken     m_cancel;
        private INameService          m_namesvc;
        private IHealthManager        m_healthmgr;
        private Task                  m_parthealthproc;
        private Task                  m_rephealthproc;
        private Task m_cloudhealthproc;
        private CloudIndex            m_idx;
        private int                   m_redundancy;
        private AsyncManualResetEvent m_part_ev;
        private AsyncManualResetEvent m_cloud_ev;
        private DynamicMemoryCloud    m_mc;

        public HealthMonitor(CancellationToken token, INameService namesvc, CloudIndex idx, IHealthManager healthmgr, int redundancy)
        {
            m_cancel          = token;
            m_namesvc         = namesvc;
            m_healthmgr       = healthmgr;
            m_idx             = idx;
            m_redundancy      = redundancy;
            m_mc              = DynamicMemoryCloud.Instance;
            m_part_ev         = new AsyncManualResetEvent(false);
            m_cloud_ev        = new AsyncManualResetEvent(false);
            m_parthealthproc  = Utils.Daemon(m_cancel, "PartitionHealthMonitor", 20000, PartitionHealthMonitorProc);
            m_rephealthproc   = Utils.Daemon(m_cancel, "ReplicaHealthMonitor", 20000, ReplicaHealthMonitorProc);
            m_cloudhealthproc = Utils.Daemon(m_cancel, "MemoryCloudHealthMonitor", 20000, MemoryCloudHealthMonitorProc);
        }

        private (HealthStatus, int, string) _CheckPartitionHealth()
        {
            var rpg = m_idx.MyPartitionReplicas;
            var cached_chunks = rpg.SelectMany(r => m_idx.GetChunks(r.Id)).OrderBy(c => c.LowKey);
            var cnt_dict = cached_chunks.Aggregate(new Dictionary<Chunk, int>(), (dict, c) =>
            {
                if (!dict.ContainsKey(c)) dict.Add(c, 1);
                else dict[c]++;
                return dict;
            });

            // 0. Partition init check -- whether the chunktable is initialized.
            // 1. Range coverage check -- whether the whole address space is covered by the chunks.
            // 2. Chunk overlap check  -- overlapped chunks indicate corruptions.
            // 3. Redundancy check     -- whether the chunks meet the redundancy requirements.
            var chunks = cnt_dict.Keys.OrderBy(_ => _.LowKey).Distinct();
            if (!chunks.Any())
            {
                return (HealthStatus.Warning, m_idx.MyPartitionId, $"Partition {m_idx.MyPartitionId} has not initialized the chunk table.");
            }
            // By ensuring neighborhood between two chunks we achieve both 1) and 2).
            foreach (var (prev, cur) in chunks.Skip(1).Concat(chunks.Take(1)).ZipWith(chunks))
            {
                if (prev.HighKey + 1 != cur.LowKey)
                {
                    return (
                        Health.HealthStatus.Error,
                        m_idx.MyPartitionId,
                        $"Partition {m_idx.MyPartitionId} chunk table misalignment between " +
                        $"[{prev.LowKey}-{prev.HighKey}] and [{cur.LowKey}-{cur.HighKey}]");
                }
                if (cur.HighKey < cur.LowKey)
                {
                    return (
                        Health.HealthStatus.Error,
                        m_idx.MyPartitionId,
                        $"Partition {m_idx.MyPartitionId} chunk information corrupted:" +
                        $"[{cur.LowKey}-{cur.HighKey}]");
                }
            }

            if (cnt_dict.Values.Min() < m_redundancy)
            {
                return (HealthStatus.Warning, m_idx.MyPartitionId, $"Partition {m_idx.MyPartitionId} has not reached required redundancy.");
            }
            else
            {
                return (HealthStatus.Healthy, m_idx.MyPartitionId, $"Partition {m_idx.MyPartitionId} is healthy.");
            }
        }

        private async Task ReplicaHealthMonitorProc()
        {
            if (!m_namesvc.IsMaster) return;

            // TODO report master down if more than 1/2 reps are not reachable
            await m_healthmgr.ReportReplicaStatus(HealthStatus.Healthy, m_namesvc.InstanceId, $"Replica is healthy");
            foreach (var replica in m_mc.MyPartition.OfType<DynamicRemoteStorage>())
            {
                if (m_cancel.IsCancellationRequested) break;
                var repid = replica.ReplicaInformation.Id;
                try
                {
                    CancellationTokenSource timeout_src = new CancellationTokenSource(10000);
                    using (var rsp = await replica.QueryReplicaHealth().WaitAsync(timeout_src.Token))
                    {
                        if (rsp.errno != Errno.E_OK) { throw new NoSuitableReplicaException(); }
                        await m_healthmgr.ReportReplicaStatus(HealthStatus.Healthy, repid, $"Replica is healthy");
                    }
                }
                catch (NoSuitableReplicaException)
                {
                    await m_healthmgr.ReportReplicaStatus(HealthStatus.Error, repid, $"Replica reports itself unhealthy");
                }
                catch (TaskCanceledException)
                {
                    await m_healthmgr.ReportReplicaStatus(HealthStatus.Error, repid, $"Replica health query timed out");
                }
                catch (IOException)
                {
                    await m_healthmgr.ReportReplicaStatus(HealthStatus.Error, repid, $"Replica is not reachable");
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, $"{nameof(ReplicaHealthMonitorProc)}: an error occured: {{0}}", ex.ToString());
                }
            }
        }

        private async Task PartitionHealthMonitorProc()
        {
            if (!m_namesvc.IsMaster) return;

            var (health, id, msg) = _CheckPartitionHealth();

            if (health == HealthStatus.Healthy)
            {
                if (!m_part_ev.IsSet) Log.WriteLine(LogLevel.Info, $"{nameof(PartitionHealthMonitorProc)}: health status changed to [Healthy]");
                m_part_ev.Set();
            }
            else
            {
                if (m_part_ev.IsSet) Log.WriteLine(LogLevel.Error, $"{nameof(PartitionHealthMonitorProc)}: health status changed to [Error]");
                m_part_ev.Reset();
            }

            await m_healthmgr.ReportPartitionStatus(health, id, msg);
        }

        private Errno _ExtractErrno(Task<ErrnoResponseReader> rsp)
        {
            if (rsp.IsCompleted)
            {
                var errno = rsp.Result.errno;
                rsp.Result.Dispose();
                return errno;
            }
            else
            {
                return Errno.E_FAIL;
            }
        }

        private async Task MemoryCloudHealthMonitorProc()
        {
            if (!m_namesvc.IsMaster) return;
            bool cloud_healthy = false;
            try
            {
                var rsps = m_mc.m_cloudidx.GetMasters()
                    .Select(_ => _.QueryPartitionHealth())
                    .Select(t => t.ContinueWith(_ExtractErrno));

                using (var tsrc = new CancellationTokenSource(60000))
                {
                    var results = await Task.WhenAll(rsps).WaitAsync(tsrc.Token);
                    if (results.All(_ => _ == Errno.E_OK)) cloud_healthy = true;
                }
            }
            catch { }

            if (cloud_healthy)
            {
                if (!m_cloud_ev.IsSet) Log.WriteLine(LogLevel.Info, $"{nameof(MemoryCloudHealthMonitorProc)}: health status changed to [Healthy]");
                m_cloud_ev.Set();
                await m_healthmgr.ReportMemoryCloudStatus(HealthStatus.Healthy, "MemoryCloud is healthy");
            }
            else
            {
                if (m_cloud_ev.IsSet) Log.WriteLine(LogLevel.Error, $"{nameof(MemoryCloudHealthMonitorProc)}: health status changed to [Error]");
                m_cloud_ev.Reset();
                await m_healthmgr.ReportMemoryCloudStatus(HealthStatus.Error, "MemoryCloud is not healthy");
            }
        }

        internal Task WaitPartitionHealthyAsync() => m_part_ev.WaitAsync(m_cancel);

        internal Task WaitMemoryCloudHealthyAsync() => m_cloud_ev.WaitAsync(m_cancel);

        internal bool IsPartitionHealthy() => m_part_ev.IsSet;

        public void Dispose()
        {
            m_cloudhealthproc.Wait();
            m_cloud_ev.Reset();
            m_rephealthproc.Wait();
            m_parthealthproc.Wait();
            m_part_ev.Reset();
        }
    }
}

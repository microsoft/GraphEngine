using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Consensus;
using Trinity.DynamicCluster.Storage;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Health
{
    class HealthMonitor : IDisposable
    {
        private CancellationToken m_cancel;
        private IHealthManager    m_healthmgr;
        private Task              m_parthealthproc;
        private CloudIndex        m_idx;
        private int               m_redundancy;

        public HealthMonitor(CancellationToken token, CloudIndex idx, IHealthManager healthmgr, int redundancy)
        {
            m_cancel         = token;
            m_healthmgr      = healthmgr;
            m_idx            = idx;
            m_redundancy     = redundancy;
            m_parthealthproc = Utils.Daemon(m_cancel, "PartitionHealthMonitor", 20000, PartitionHealthMonitorProc);
        }

        private async Task PartitionHealthMonitorProc()
        {
            if (!m_healthmgr.IsMaster) return;
            var rpg = m_idx.MyPartitionReplicas;
            var cached_chunks = rpg.SelectMany(r => m_idx.GetChunks(r.Id)).OrderBy(c => c.LowKey);
            var cnt_dict = cached_chunks.Aggregate(new Dictionary<Chunk, int>(), (dict, c) =>
            {
                if (!dict.ContainsKey(c)) dict.Add(c, 1);
                else dict[c]++;
                return dict;
            });

            // 1. Range coverage check -- whether the whole address space is covered by the chunks.
            // 2. Chunk overlap check  -- overlapped chunks indicate corruptions.
            // 3. Redundancy check     -- whether the chunks meet the redundancy requirements.
            var chunks = cnt_dict.Keys.OrderBy(_ => _.LowKey).Distinct();
            if (!chunks.Any()) { return; }
            // By ensuring neighborhood between two chunks we achieve both 1) and 2).
            foreach(var (prev, cur) in chunks.Skip(1).Concat(chunks.Take(1)).ZipWith(chunks))
            {
                if(prev.HighKey + 1 != cur.LowKey)
                {
                    await m_healthmgr.ReportPartitionStatus(
                        Health.HealthStatus.Error,
                        m_idx.MyPartitionId,
                        $"Partition {m_idx.MyPartitionId} chunk table misalignment between " + 
                        $"[{prev.LowKey}-{prev.HighKey}] and [{cur.LowKey}-{cur.HighKey}]");
                    return;
                }
                if(cur.HighKey < cur.LowKey)
                {
                    await m_healthmgr.ReportPartitionStatus(
                        Health.HealthStatus.Error, 
                        m_idx.MyPartitionId,
                        $"Partition {m_idx.MyPartitionId} chunk information corrupted:" + 
                        $"[{cur.LowKey}-{cur.HighKey}]");
                    return;
                }
            }
            if(cnt_dict.Values.Min() < m_redundancy)
            {
                await m_healthmgr.ReportPartitionStatus(HealthStatus.Warning, m_idx.MyPartitionId, $"Partition {m_idx.MyPartitionId} has not reached required redundancy.");
                return;
            }

            await m_healthmgr.ReportPartitionStatus(HealthStatus.Healthy, m_idx.MyPartitionId, $"Partition {m_idx.MyPartitionId} is healthy.");
        }

        public void Dispose()
        {
            m_parthealthproc.Wait();
        }
    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Storage;
using Trinity.Storage;
using Trinity.Diagnostics;
using Trinity.DynamicCluster.Persistency;
using Trinity.DynamicCluster.Communication;

namespace Trinity.DynamicCluster.Tasks
{
    [Serializable]
    internal class ShrinkDataTask : ITask
    {
        public static readonly Guid Guid = new Guid("DF3AE902-78C7-49E0-B99B-8D4279D62E53");
        private Guid m_guid = Guid.NewGuid();
        private ReplicaInformation m_target;
        private List<(Chunk from, Chunk to)> m_plan;
        [NonSerialized]
        private DynamicMemoryCloud m_dmc;

        public ShrinkDataTask(ReplicaInformation target, IEnumerable<(Chunk from, Chunk to)> plan)
        {
            m_target = target;
            m_plan = plan.ToList();
        }

        public Guid Id => m_guid;

        public Guid Tag => Guid;

        public async Task Execute(CancellationToken cancel)
        {
            m_dmc = DynamicMemoryCloud.Instance;
            await DoShrinkData();
            await DoUpdateChunkTable();
        }

        private async Task DoShrinkData()
        {
            var target_id = m_dmc.GetInstanceId(m_target.Id);
            var mod = m_dmc.GetCommunicationModule<DynamicClusterCommModule>();
            using (var msg = new ShrinkDataTaskInformationWriter(
                       task_id: m_guid,
                       remove_target: m_plan.Select(_ => _diff(_.from, _.to)).ToList()))
            using (var rsp = await mod.ShrinkData(target_id, msg))
            {
                if (rsp.errno != Errno.E_OK) throw new Exception();
            }
        }

        private ChunkInformation _diff(Chunk from, Chunk to)
        {
            if (from.LowKey == to.LowKey) return new ChunkInformation(to.HighKey + 1, from.HighKey);
            else if (from.HighKey == to.HighKey) return new ChunkInformation(to.LowKey, from.HighKey);
            else throw new ArgumentException();
        }

        private async Task DoUpdateChunkTable()
        {
            var ct  = m_dmc.m_chunktable;
            //TODO race condition?
            var cks = (await ct.GetChunks(m_target)).ToList();
            foreach(var (from, to) in m_plan)
            {
                cks.Remove(from);
                cks.Add(to);
            }
            await m_dmc.m_chunktable.SetChunks(m_target.Id, cks.Distinct());
        }
    }
}

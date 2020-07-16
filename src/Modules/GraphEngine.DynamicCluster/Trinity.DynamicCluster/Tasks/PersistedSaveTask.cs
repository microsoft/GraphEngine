using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Communication;
using Trinity.DynamicCluster.Storage;

namespace Trinity.DynamicCluster.Tasks
{
    [Serializable]
    internal class PersistedSaveTask : ITask
    {
        public static readonly Guid Guid = new Guid("01818198-FF59-4292-B734-F8243D133B72");
        private Guid m_guid = Guid.NewGuid();
        private List<(ReplicaInformation, PersistedSlice)> m_plan;
        public PersistedSaveTask(IEnumerable<(ReplicaInformation, PersistedSlice)> plan)
        {
            m_plan = plan.ToList();
        }

        public Guid Id => m_guid;

        public Guid Tag => Guid;

        public async Task Execute(CancellationToken cancel)
        {
            await Task.WhenAll(m_plan.Select(t => SendUploadCommand(t.Item1, t.Item2)));
        }

        private async Task SendUploadCommand(ReplicaInformation r, PersistedSlice s)
        {
            var dmc = DynamicMemoryCloud.Instance;
            var mod = dmc.GetCommunicationModule<DynamicClusterCommModule>();
            int id = dmc.GetInstanceId(r.Id);
            using (var cmd = new PersistedSliceWriter(s.version, s.lowkey, s.highkey))
            using (var rsp = await mod.PersistedUploadAsync(id, cmd))
            {
                if (rsp.errno != Errno.E_OK) throw new Exception();
            }
        }
    }
}

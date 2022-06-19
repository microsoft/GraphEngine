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
    internal class PersistedLoadTask : ITask
    {
        public static readonly Guid Guid = new Guid("EEC84D67-FCE9-4CCF-B872-C4B090A0CE8A");
        private Guid m_guid = Guid.NewGuid();
        private List<(ReplicaInformation, PersistedSlice)> m_plan;
        public PersistedLoadTask(IEnumerable<(ReplicaInformation, PersistedSlice)> plan)
        {
            m_plan = plan.ToList();
        }

        public Guid Id => m_guid;

        public Guid Tag => Guid;

        public async Task Execute(CancellationToken cancel)
        {
            await Task.WhenAll(m_plan.Select(t => SendDownloadCommand(t.Item1, t.Item2)));
        }

        private async Task SendDownloadCommand(ReplicaInformation r, PersistedSlice s)
        {
            var dmc = DynamicMemoryCloud.Instance;
            var mod = dmc.GetCommunicationModule<DynamicClusterCommModule>();
            int id = dmc.GetInstanceId(r.Id);
            using (var cmd = new PersistedSliceWriter(s.version, s.lowkey, s.highkey))
            using (var rsp = await mod.PersistedDownload(id, cmd))
            {
                if (rsp.errno != Errno.E_OK) throw new Exception();
            }
        }
    }
}

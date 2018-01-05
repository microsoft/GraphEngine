using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Tasks
{
    [Serializable]
    class GroupedTask : ITask
    {
        private Guid m_tag;
        private Guid m_guid = Guid.NewGuid();
        private List<ITask> m_tgroup;

        public Guid Id => m_guid;
        public Guid Tag => m_tag;

        public GroupedTask(IEnumerable<ITask> tasks, Guid tag)
        {
            m_tgroup = tasks.ToList();
            m_tag = tag;
        }

        public Task Execute(CancellationToken cancel)
            => Task.WhenAll(m_tgroup.Select(_ => _.Execute(cancel)));
    }
}

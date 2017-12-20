using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Tasks
{
    class GroupedTask : ITask
    {
        private Guid m_tag;
        private Guid m_guid = Guid.NewGuid();
        private IEnumerable<ITask> m_tgroup;

        public Guid Id => m_guid;
        public Guid Tag => m_tag;

        public GroupedTask(IEnumerable<ITask> tasks)
        {
            var tags = tasks.Select(_ => _.Tag).Distinct();
            if(tags.Count() != 1)
            {
                throw new ArgumentException($"{nameof(GroupedTask)}: tag mismatch. Only one tag allowed.");
            }
            m_tgroup = tasks;
            m_tag = tags.First();
        }

        public Task Execute(CancellationToken cancel) => Task.WhenAll(m_tgroup.Select(_ => _.Execute(cancel)));
    }
}

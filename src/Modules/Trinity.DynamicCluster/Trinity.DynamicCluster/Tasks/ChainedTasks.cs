using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Tasks
{
    class ChainedTasks : ITask
    {
        private static Guid s_tag = new Guid("56EB7016-F926-43C2-84F8-E0070B6C8B0A");
        private Guid m_guid = Guid.NewGuid();
        private IEnumerable<ITask> m_tchain;

        public Guid Id => m_guid;
        public Guid Tag => s_tag;

        public ChainedTasks(IEnumerable<ITask> tasks)
        {
            m_tchain = tasks;
        }

        public Task Execute(CancellationToken cancel) => 
            m_tchain.Aggregate(Task.CompletedTask, (prev, cur) => 
            prev.ContinueWith(_ => 
            cur.Execute(cancel)));
    }
}

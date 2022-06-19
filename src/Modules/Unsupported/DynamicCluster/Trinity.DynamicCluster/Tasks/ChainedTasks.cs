using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Tasks
{
    /// <summary>
    /// Represents stateful, multi-stage task.
    /// </summary>
    [Serializable]
    class ChainedTasks : ITask
    {
        private Guid m_tag;
        private Guid m_guid = Guid.NewGuid();
        private Queue<ITask> m_tchain;

        public Guid Id => m_guid;
        public Guid Tag => m_tag;

        public ChainedTasks(IEnumerable<ITask> tasks, Guid tag)
        {
            m_tchain = new Queue<ITask>(tasks);
            m_tag    = tag;
        }

        public async Task Execute(CancellationToken cancel)
        {
            await m_tchain.Peek().Execute(cancel);
            m_tchain.Dequeue();
        }

        public bool Finished => m_tchain.Count == 0;
    }
}

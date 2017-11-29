using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Tasks
{
    public class NopTask : ITask
    {
        private Guid m_guid = Guid.NewGuid();
        public Guid Id => m_guid;

        public async Task Execute(CancellationToken cancel)
        {
            await Task.Delay(1000, cancel);
        }
    }
}

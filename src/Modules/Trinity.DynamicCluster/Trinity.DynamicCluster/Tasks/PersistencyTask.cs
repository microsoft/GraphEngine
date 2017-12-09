using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Tasks
{
    [Serializable]
    internal class PersistencyTask : ITask
    {
        public static readonly Guid Guid = new Guid("01818198-FF59-4292-B734-F8243D133B72");
        private Guid m_guid = Guid.NewGuid();

        public Guid Id => m_guid;

        public Guid Tag => Guid;

        public async Task Execute(CancellationToken cancel)
        {
        }
    }
}

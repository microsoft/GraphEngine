using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Tasks
{
    [Serializable]
    public class NopTask : ITask
    {
        public static readonly Guid Guid  = new Guid("509B35C6-A6FD-4A33-A034-26BADB4D0D06");
        private Guid m_guid = Guid.NewGuid();
        public Guid Id => m_guid;

        public Guid Tag => Guid;

        public async Task Execute(CancellationToken cancel)
        {
            await Task.Delay(1000, cancel);
        }
    }
}

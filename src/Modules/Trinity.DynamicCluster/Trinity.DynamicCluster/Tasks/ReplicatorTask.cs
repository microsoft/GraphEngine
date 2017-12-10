using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Tasks
{
    [Serializable]
    internal class ReplicatorTask: ITask
    {
        public static readonly Guid Guid = new Guid("0ADA7058-AAD6-4383-95C3-3022E4DBBD50");
        private Guid m_guid = Guid.NewGuid();

        public Guid Id => m_guid;

        public Guid Tag => Guid;

        public async Task Execute(CancellationToken cancel)
        {
        }
    }
}

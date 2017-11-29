using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Tasks
{
    /// <summary>
    /// Represents a cluster management task.
    /// </summary>
    public interface ITask
    {
        Guid Id { get; }
        Task Execute(CancellationToken cancel);
    }
}

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
        /// <summary>
        /// The Id of this task.
        /// </summary>
        Guid Id { get; }
        /// <summary>
        /// The tag of this task, indicating the task group.
        /// An implementation should generate a constant for
        /// all instances of this type.
        /// </summary>
        Guid Tag { get; }
        /// <summary>
        /// Execute the task asynchronously.
        /// </summary>
        /// <param name="cancel">The task should be cancelled if the cancellation token is fired.</param>
        Task Execute(CancellationToken cancel);
    }
}

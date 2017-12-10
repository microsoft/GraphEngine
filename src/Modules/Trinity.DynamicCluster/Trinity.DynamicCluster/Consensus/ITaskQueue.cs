using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Tasks;

namespace Trinity.DynamicCluster.Consensus
{
    /// <summary>
    /// Reliable, transactional task queue.
    /// Each partition serves as an AvailabilityGroup(AG),
    /// and thus is regarded as whole as a storage
    /// (see <see cref="Trinity.DynamicCluster.Storage.Partition"/>).
    /// An ITaskQueue is responsible to provide the following guarantees:
    /// 
    /// <list type="bullet">
    ///   <item><description>
    ///   Task submission should be resilient, that once submitted,
    ///   unless it is completed, it should remain in the system.
    ///   </description></item> 
    ///   
    ///   <item><description>
    ///   It is up to the implementation whether to use master-master
    ///   or master-slave replication for the queued tasks. However,
    ///   task execution should be done on master replicas. 
    ///   The dynamic memory cloud will fetch tasks accordingly from the replicas.
    ///   </description></item> 
    ///   
    ///   <item><description>
    ///   Task distribution should be atomic. A task cannot be fetched
    ///   on two masters simultaneously. However if the executing master fails
    ///   during task execution, the task is rolled back into the queue.
    ///   </description></item> 
    ///   
    ///   <item><description>
    ///   Task completion should be strongly consistent. Once marked complete,
    ///   the item should be removed from the system. Afterwards, it cannot be
    ///   retrieved again regardless of failures.
    ///   </description></item> 
    /// </list>
    /// </summary>
    public interface ITaskQueue : IService
    {
        /// <summary>
        /// Gets a task from the queue. The behavior
        /// is undefined when executed on a task queue slave.
        /// </summary>
        /// <param name="token">A cancellation token. When fired, GetTask should throw <see cref="TaskCanceledException"/>.</param>
        /// <returns>
        /// The task that is fetched from the queue. After the task is fetched, no other masters
        /// should be able to fetch it again unless the current instance crashed without marking
        /// the task completed.
        /// </returns>
        Task<ITask> GetTask(CancellationToken token);
        /// <summary>
        /// Reports completion of a task, and removes it from the system.
        /// </summary>
        Task TaskCompleted(ITask task);
        /// <summary>
        /// Reports failure of a task, and put it back into the queue.
        /// </summary>
        Task TaskFailed(ITask task);
        /// <summary>
        /// Post a new task to a partition. The behavior
        /// is undefined when executed on a task queue slave.
        /// </summary>
        /// <param name="task">The task to be posted.</param>
        Task PostTask(ITask task);
        /// <summary>
        /// Waits until tasks of the given tag to be all completed.
        /// </summary>
        /// <param name="tag">The tag to be waited. See <see cref="ITask.Tag"/>.</param>
        Task Wait(Guid tag);
    }
}

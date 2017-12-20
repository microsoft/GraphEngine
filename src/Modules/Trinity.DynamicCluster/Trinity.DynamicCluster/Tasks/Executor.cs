using System;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.DynamicCluster.Consensus;

namespace Trinity.DynamicCluster.Tasks
{
    internal class Executor : IDisposable
    {
        private ITaskQueue m_taskqueue;
        private CancellationToken m_cancel;
        private Task m_taskexecproc;

        public Executor(ITaskQueue queue, CancellationToken token)
        {
            m_taskqueue = queue;
            m_cancel = token;
            m_taskexecproc = Utils.Daemon(m_cancel, "TaskExecutorProc", 1000, TaskExecutionProc);
        }

        public void Dispose()
        {
            m_taskexecproc.Wait();
        }

        private async Task TaskExecutionProc()
        {
            ITask task;
            Exception exception;
            if (!m_taskqueue.IsMaster) return;
            task = await m_taskqueue.GetTask(m_cancel);
            if (task == null) return;
            try
            {
                await task.Execute(m_cancel);
                exception = null;
            }
            catch (Exception ex) { exception = ex; }
            if (null == exception)
            {
                await m_taskqueue.TaskCompleted(task);
            }
            else
            {
                Log.WriteLine(LogLevel.Error, $"TaskExecutionProc: task {task.Id} failed with exception: {exception.ToString()}");
                await m_taskqueue.TaskFailed(task);
            }
        }
    }
}

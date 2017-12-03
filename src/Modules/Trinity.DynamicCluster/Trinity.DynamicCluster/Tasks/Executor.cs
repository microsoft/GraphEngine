using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            m_taskexecproc = TaskExecutionProc();
        }

        public void Dispose()
        {
            m_taskexecproc.Wait();
        }

        private async Task TaskExecutionProc()
        {
            ITask task;
            Exception exception;
            while (!m_cancel.IsCancellationRequested)
            {
                try
                {
                    if (!m_taskqueue.IsMaster)
                    {
                        await Task.Delay(1000, m_cancel);
                        continue;
                    }
                    task = await m_taskqueue.GetTask(m_cancel);
                    if(task == null)
                    {
                        await Task.Delay(1000, m_cancel);
                        continue;
                    }
                    try
                    {
                        await task.Execute(m_cancel);
                        exception = null;
                    }
                    catch(Exception ex) { exception = ex; }
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
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, $"TaskExecutionProc: {ex.ToString()}");
                    await Task.Delay(1000, m_cancel);
                }
            }
        }
    }
}

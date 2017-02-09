// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Trinity.Daemon
{
    /// <summary>
    /// Represents a background operation.
    /// </summary>
    /// <returns>An updated execution interval.</returns>
    public delegate int BackgroundTaskRoutine();

    /// <summary>
    /// Represents a background task.
    /// </summary>
    public class BackgroundTask
    {
        /// <summary>
        /// Constructs a background task with the specified background operation and execution interval.
        /// </summary>
        /// <param name="task">An operation to be executed in background.</param>
        /// <param name="interval">The time interval between invocations of <paramref name="task"/>, in milliseconds.</param>
        public BackgroundTask(BackgroundTaskRoutine task, int interval)
        {
            ExecutionInterval = interval;
            LastExecutedTime = 0;
            action = task;
        }
        /// <summary>
        /// Executes the task once it passes its execution time interval since last execution.
        /// This routine is thread-safe.
        /// </summary>
        public int Execute(long CurrentTime)
        {
            bool MyJobToExecuteThis = false;
            lock (TimerLock)
            {
                if (CurrentTime - LastExecutedTime > ExecutionInterval && !Executing)
                {
                    LastExecutedTime = CurrentTime;
                    Executing = true;
                    MyJobToExecuteThis = true;
                }
            }
            if (MyJobToExecuteThis)
            {
                int ret = action();
                lock (TimerLock)
                {
                    Thread.MemoryBarrier();
                    Executing = false;          //Execution finished
                    if (ret != 0)
                        ExecutionInterval = ret;//update the interval
                }
                return ret;
            }
            return 0;
        }
        private long LastExecutedTime;
        private int ExecutionInterval;
        private bool Executing;
        private object TimerLock = new object();
        BackgroundTaskRoutine action;
    }
}

// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Trinity;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Trinity.Storage;
using Trinity.Core.Lib;
using System.Runtime.CompilerServices;

namespace Trinity.Daemon
{
    /// <summary>
    /// Represents a background thread that executes tasks in background.
    /// </summary>
    public class BackgroundThread
    {
        private static bool started = false;
        private static object _lock = new object();

        static BackgroundThread()
        {
            Start();
        }

        static int TimerInterval = 100; //Default frequency = 10HZ
        static long CurrentTime;
        static Timer timer;

        /// <summary>
        /// Starts the background thread.
        /// </summary>
        public static void Start()
        {
            if (!started)
            {
                lock (_lock)
                {
                    if (!started)
                    {
                        _thread_();

                        Thread.MemoryBarrier();
                        started = true;
                    }
                }
            }
        }

        internal static void Stop()
        {
            lock (_lock)
            {
                if (started)
                {
                    timer.Dispose();
                    ClearAllTasks();
                    started = false;
                }
            }
        }

        private static List<BackgroundTask> s_tasklist = new List<BackgroundTask>();

        internal static void ClearAllTasks()
        {
            s_tasklist.Clear();
        }

        /// <summary>
        /// Adds a background task.
        /// </summary>
        /// <param name="task">A background task described by a <see cref="T:Trinity.Daemon.BackgroundTask"/> object.</param>
        public static void AddBackgroundTask(BackgroundTask task)
        {
            lock (s_tasklist)
            {
                s_tasklist.Add(task);
            }
        }

        /// <summary>
        /// Removes a background task.
        /// </summary>
        /// <param name="task">A background task previously added to the background thread.</param>
        public static void RemoveBackgroundTask(BackgroundTask task)
        {
            lock (s_tasklist)
            {
                s_tasklist.Remove(task);
            }
        }

        private static void _thread_()
        {
            timer = new Timer((x) =>
            {
                long myCurrentTime = Interlocked.Add(ref CurrentTime, TimerInterval);
                List<BackgroundTask> tasklistShadow = null;
                lock (s_tasklist)
                {
                    tasklistShadow = new List<BackgroundTask>(s_tasklist);
                }

                foreach (var task in tasklistShadow)
                {
                    int ret = task.Execute(myCurrentTime);
                    if (ret != 0 && ret < TimerInterval)
                    {
                        lock (_lock)
                        {
                            TimerInterval = ret;
                            timer.Change(0, ret);
                        }
                    }
                }
            }, null, 0, TimerInterval);
        }
    }
}

// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include "TrinityCommon.h"
#include <thread>
#include <mutex>
#include <chrono>
#include <cassert>

#include <diagnostics>
#include <Trinity/Diagnostics/Log.h>
#include <Trinity/ref.h>


namespace BackgroundThread
{
    class BackgroundTask
    {
    public:
        virtual DWORD Execute() = 0;
    private:
        friend class TaskScheduler;

        //return: time after executing current task
        uint64_t _execute_task(uint64_t current_time)
        {
            Stopwatch sw = Stopwatch::StartNew();
            _waitTime = Execute();
            sw.Stop();
            assert(_waitTime >= 0);
            uint64_t execution_time = static_cast<uint64_t>(sw.GetElapsedMilliseconds());
            _lastExecution          = current_time + execution_time;
            Trinity::Diagnostics::WriteLine(LogLevel::Verbose, "BackgroundThread: task {0} execution time: {1}ms. last executed time: {2}", this, execution_time, _lastExecution);
            return _lastExecution;
        }

        uint64_t _lastExecution = -1;
        uint64_t _waitTime = -1;
    };

    class TaskScheduler
    {
    public:
        static void AddTask(BackgroundTask* const task)
        {
            _lock();
            auto refptr = ref<BackgroundTask>(task);
            _taskList.push_back(refptr);
            _unlock();
        }
        static void ClearAllTasks()
        {
            _lock();
            _taskList.clear();
            _unlock();
        }
        /* Should be called on system initialization */
        static void Start()
        {
            _lock();
            if (_thread == nullptr)
            {
                _thread = new std::thread(BackgroundExecutionLoop);
            }
            _unlock();
        }
    private:
        static std::vector<ReferencePointer<BackgroundTask>> GetTasks()
        {
            /**
             * Changes will take effect after the current iteration
             * of the background workers disposed the cleared tasks.
             */
            _lock();
            auto ret = _taskList;
            _unlock();
            return ret;
        }

        static void BackgroundExecutionLoop()
        {
            while (true)
            {
                //TODO in the future we might want to
                //make GetTasks() to take the tasks out
                //from _taskList so that different
                //BG workers can fetch simultaneously
                auto taskList = GetTasks();
                if (taskList.empty())
                {
                    std::this_thread::sleep_for(std::chrono::milliseconds(500));
                    continue;
                }

                /* phase 1: process Overdue tasks */

                for (auto& task : taskList)
                {
                    if (Overdue(task.Pointer()))
                        _current_time = task->_execute_task(_current_time);
                }

                /* phase 2: calculate sleep time */

                uint64_t sleep_time = UINT64_MAX;
                for (auto& task : taskList)
                {
                    if (!Overdue(task.Pointer()))
                    {
                        uint64_t time_remained = task->_lastExecution + task->_waitTime - _current_time;
                        sleep_time = std::min(sleep_time, time_remained);
                    }
                    else
                        sleep_time = 0;
                }


                Trinity::Diagnostics::WriteLine(LogLevel::Verbose, "BackgroundThread: current time = {0}, sleep time = {1}", _current_time, sleep_time);

                assert(sleep_time != UINT64_MAX);

                std::this_thread::sleep_for(std::chrono::milliseconds(sleep_time));
                _current_time += sleep_time;
            }
        }

        static inline bool Overdue(BackgroundTask* task)
        {
            return (task->_lastExecution == -1 || (task->_lastExecution + task->_waitTime) <= _current_time);
        }


        static inline void _lock() { _mutex.lock(); }
        static inline void _unlock() { _mutex.unlock(); }

        static std::mutex _mutex;
        static std::thread* _thread;
        static std::vector<ReferencePointer<BackgroundTask>> _taskList;
        static uint64_t _current_time;
        static struct _TaskSchedulerConfig
        {
        public:
            _TaskSchedulerConfig()
            {
                _lock();
                _current_time = 0;
                _unlock();
            }

        } _config;
    };
}
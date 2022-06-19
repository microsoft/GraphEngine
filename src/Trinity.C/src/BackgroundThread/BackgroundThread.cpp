// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "TrinityCommon.h"
#include "BackgroundThread/BackgroundThread.h"


namespace BackgroundThread
{
    std::mutex TaskScheduler::_mutex;
    std::thread* TaskScheduler::_thread = nullptr;
    std::vector<TaskScheduler::_refTask> TaskScheduler::_taskList;
    uint64_t TaskScheduler::_current_time;
    std::atomic<bool> TaskScheduler::_stopped;

    TaskScheduler::_TaskSchedulerConfig TaskScheduler::_config;
}

DLL_EXPORT void CStartBackgroundThread() { BackgroundThread::TaskScheduler::Start(); }
DLL_EXPORT void CStopBackgroundThread() { BackgroundThread::TaskScheduler::Stop(); }

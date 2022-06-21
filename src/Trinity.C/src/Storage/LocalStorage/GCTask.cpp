// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "TrinityCommon.h"
#include "Storage/LocalStorage/GCTask.h"

namespace Storage
{
    std::atomic<bool> GCTask::DefragmentationPaused(false);
    std::atomic<bool> GCTask::DefragInProcess(false);
    std::atomic<bool> GCTask::AggressiveStop(false);
    int32_t GCTask::GC_task_groups;
    int32_t GCTask::current_gc_taskgroup;
    Array<Array<int32_t>> GCTask::GCTrunkIDs;
}

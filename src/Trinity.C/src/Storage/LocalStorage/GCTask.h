// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include "TrinityCommon.h"
#include "BackgroundThread/BackgroundThread.h"
#include "Trinity/Configuration/TrinityConfig.h"
#include "Storage/LocalStorage/LocalMemoryStorage.h"
#include <atomic>

namespace Storage
{
    class GCTask : public BackgroundThread::BackgroundTask
    {
    private:
        static std::atomic<bool> DefragmentationPaused;
        static std::atomic<bool> DefragInProcess;
        static std::atomic<bool> AggressiveStop;
        static int32_t GC_task_groups;
        static int32_t current_gc_taskgroup;
        static Array<Array<int32_t>> GCTrunkIDs;

        static void ReconfigGC()
        {
            DefragmentationPaused.store(false);
            DefragInProcess.store(false);
            AggressiveStop.store(false);

            int current_trunk_id  = 0;
            int trunk_cnt         = TrinityConfig::TrunkCount();
            int trunks_per_gc_run = TrinityConfig::GCParallelism;
            GC_task_groups        = trunk_cnt / trunks_per_gc_run;

            if (GC_task_groups == 0)
            {
                GC_task_groups = 1;
            }

            current_gc_taskgroup = 0;
            GCTrunkIDs = Array<Array<int32_t>>(GC_task_groups);
            for (int32_t i = 0; i < GC_task_groups; i++)
            {
                GCTrunkIDs[i] = Array<int32_t>(TrinityConfig::GCParallelism);

                for (int32_t j = 0; j < TrinityConfig::GCParallelism; j++)
                {
                    if (current_trunk_id < trunk_cnt)
                    {
                        GCTrunkIDs[i][j] = current_trunk_id++;
                    }
                    else
                    {
                        GCTrunkIDs[i][j] = -1; // mark the entry non-effective (will be skipped)
                    }
                }
            }
        }

        static int32_t MemoryTrunkDefragThread()
        {
            if (AggressiveStop.load())
                return 600;
            DefragInProcess.store(true);

            if (AggressiveStop.load() /*double check*/ || TrinityConfig::ReadOnly() || DefragmentationPaused.load())
            {
                DefragInProcess.store(false);
                return 600;
            }

            Trinity::Diagnostics::WriteLine(LogLevel::Verbose, "GCTask: Starting defragmentation, DefragmentationPaused = {0}.", DefragmentationPaused.load());

            if (Storage::LocalMemoryStorage::initialized.load())
            {
                for (int32_t i : GCTrunkIDs[current_gc_taskgroup])
                {
                    if (i < 0) continue; // non-effective entries should be skipped
                    Storage::LocalMemoryStorage::Defragment(i);
                }

                current_gc_taskgroup = (current_gc_taskgroup + 1) % GC_task_groups;
            }
            DefragInProcess.store(false);
            Trinity::Diagnostics::WriteLine(LogLevel::Verbose, "GCTask: Finishing defragmentation, waiting for {0}ms.", TrinityConfig::DefragInterval);
            return TrinityConfig::DefragInterval;
        }

    public:
        inline GCTask()
        {
            GCTask::ReconfigGC();
        }
        DWORD Execute() override
        {
            return GCTask::MemoryTrunkDefragThread();
        }

        static bool GetDefragmentationPaused()
        {
            return DefragmentationPaused.load();
        }
        static void SetDefragmentationPaused(bool value)
        {
            DefragmentationPaused.store(value);
            Trinity::Diagnostics::WriteLine(LogLevel::Debug, "GCTask: DefragmentationPaused set to {0}", DefragmentationPaused.load());
        }
        static void StopDefragAndAwaitCeased()
        {
            AggressiveStop.store(true);
            while (DefragInProcess)
                YieldProcessor();
        }
        static void RestartDefragmentation()
        {
            AggressiveStop.store(false);
        }
    };
}

// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <thread>
#include <functional>
#include <vector>
#include <atomic>
namespace Trinity
{
    namespace Threading
    {
        namespace Parallel
        {
            inline void For(int32_t inclusiveFrom, int32_t exclusiveTo, std::function<void(int32_t)> action)
            {
                if (inclusiveFrom >= exclusiveTo)
                    return;

                uint32_t thread_cnt = std::thread::hardware_concurrency() + 1;
                uint32_t entry_per_thread = (exclusiveTo - inclusiveFrom) / thread_cnt;

                if (entry_per_thread < 1)
                {
                    for (int32_t i = inclusiveFrom; i < exclusiveTo; ++i)
                    {
                        action(i);
                    }
                    return;
                }

                std::vector<std::thread> threads;

                /* Chunk the payloads into segments */
                int32_t start_idx, end_idx;

                for (start_idx = inclusiveFrom; start_idx < exclusiveTo; start_idx += entry_per_thread)
                {
                    end_idx = start_idx + entry_per_thread;
                    if (end_idx > exclusiveTo)
                        end_idx = exclusiveTo;

                    threads.emplace_back([&](int32_t from, int32_t to)
                    {
                        for (int32_t entry_idx = from; entry_idx < to; ++entry_idx)
                            action(entry_idx);
                    }, start_idx, end_idx);
                }

                for (auto& t : threads)
                {
                    t.join();
                }

                std::atomic_thread_fence(std::memory_order::memory_order_seq_cst);
            }
        }
    }
}

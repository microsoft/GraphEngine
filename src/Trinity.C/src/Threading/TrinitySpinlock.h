// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <cstdint>
#include <atomic>

const int32_t COUNTER_THRESHOLD    = 20000;

class TrinitySpinlock
{
public:
    inline TrinitySpinlock()
    {
        value.store(0);
    }
    inline void lock()
    {
        int32_t zero = 0;
        if (!std::atomic_compare_exchange_strong_explicit(&value, &zero, 1, std::memory_order_acquire, std::memory_order_relaxed))
        {
            int32_t counter = 0;
            while (true)
            {
                zero = 0;
                if ((value.load(std::memory_order_relaxed) == 0) && std::atomic_compare_exchange_strong_explicit(&value, &zero, 1, std::memory_order_acquire, std::memory_order_relaxed))
                    break;
                if (counter < COUNTER_THRESHOLD)
                {
                    ++counter;
                }
                else
                {
                    YieldProcessor();
                }
            }
        }
    }

    inline void unlock()
    {
        value.store(0, std::memory_order_release);
    }

    inline void lock(std::atomic<int32_t> & pending_flag)
    {
        int32_t zero = 0;
        if (!std::atomic_compare_exchange_strong(&value, &zero, 1))
        {
            pending_flag.store(1);
            int32_t counter = 0;
            while (true)
            {
                zero = 0;
                if ((value.load(std::memory_order_relaxed) == 0) && std::atomic_compare_exchange_strong(&value, &zero, 1))
                {
                    pending_flag.store(0);
                    break;
                }
                if (counter < COUNTER_THRESHOLD)
                {
                    ++counter;
                }
                else
                {
                    YieldProcessor();
                }
            }
        }
    }

    inline bool trylock()
    {
        int32_t zero = 0;
        return std::atomic_compare_exchange_strong(&value, &zero, 1);
    }

    inline bool trylock(int32_t retry)
    {
        int32_t counter = retry;
        int32_t zero = 0;
        while (!std::atomic_compare_exchange_strong(&value, &zero, 1))
        {
            if (--counter < 0)
                return false;
            zero = 0;
        }
        return true;
    }

private:
    std::atomic<int32_t> value;
};

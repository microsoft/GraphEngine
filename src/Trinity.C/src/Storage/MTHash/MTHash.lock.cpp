// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "Storage/MTHash/MTHash.h"
#include "Storage/MemoryTrunk/MemoryTrunk.h"
#include <threading>

namespace Storage
{
    void MTHash::GetAllEntryLocksExceptArena()
    {
        EntryAllocLock.lock();
        for (uint32_t i = 0; i < EntryCount.load(); ++i)
        {
            char cmpxchg_val = ENTRYLOCK_NOLOCK;
            if (!std::atomic_compare_exchange_strong_explicit(&MTEntries[i].EntryLock, &cmpxchg_val, char(ENTRYLOCK_LOCK), std::memory_order_acquire, std::memory_order_relaxed))
            {
                int32_t counter = 0;
                while (cmpxchg_val != ENTRYLOCK_ARENA)
                {
                    cmpxchg_val = ENTRYLOCK_NOLOCK;
                    if (std::atomic_compare_exchange_strong_explicit(&MTEntries[i].EntryLock, &cmpxchg_val, char(ENTRYLOCK_LOCK), std::memory_order_acquire, std::memory_order_relaxed))
                        break;

                    ++counter;
                    if (counter < COUNTER_THRESHOLD)
                        continue;
                    else
                    {
                        Runtime::TransitionSleep(0);
                    }
                }
            }
        }
    }

    void MTHash::ReleaseAllEntryLocksExceptArena()
    {
        for (uint32_t i = 0; i < EntryCount.load(); ++i)
        {
            if (MTEntries[i].EntryLock.load() == ENTRYLOCK_ARENA)
                continue;
            MTEntries[i].EntryLock.store(ENTRYLOCK_NOLOCK);
        }
        EntryAllocLock.unlock();
    }

    /**
     *  Takes all bucket locks and wait for all entry lock holders to exit.
     */
    void MTHash::Lock()
    {
        if (TrinityConfig::ReadOnly())
            return;

        Trinity::Diagnostics::WriteLine(LogLevel::Verbose, "MTHash {0}: Attempting to lock.", this->memory_trunk->TrunkId);

        /** Take all bucket locks. */
        std::atomic<uint64_t>* bucket_ptr            = reinterpret_cast<std::atomic<uint64_t>*>(BucketLockers);
        uint32_t               batch_entry_cnt       = BucketCount / sizeof(uint64_t);
        
        auto                   batch_get_lock_action = [&](std::atomic<uint64_t>* ptr)
        {
            uint64_t comprand = BUCKETLOCK_EIGHT_NOLOCKS;
            /* Opportunistic batch lock acquire, falling back to individual lock acquiring actions. */
            if (!std::atomic_compare_exchange_strong(ptr, &comprand, uint64_t(BUCKETLOCK_EIGHT_LOCKS)))
            {
                uint32_t bucket_idx = (uint32_t) (reinterpret_cast<std::atomic<char>*>(ptr) - BucketLockers);
                for (uint32_t offset = 0; offset < sizeof(uint64_t); ++offset)
                    GetBucketLock(bucket_idx++);
            }
            return;
        };

        for (uint32_t i = 0; i < batch_entry_cnt; ++i)
        {
            batch_get_lock_action(bucket_ptr++);
        }

        /* Sequentially process the remainders */
        for (uint32_t i = batch_entry_cnt * sizeof(uint64_t); i < BucketCount; ++i)
        {
            GetBucketLock(i);
        }
        ////////////////////////////

        Trinity::Diagnostics::WriteLine(LogLevel::Verbose, "MTHash {0}: Bucket locks acquired, waiting for cell locks to release.", this->memory_trunk->TrunkId);

        /**
         *  Wait all entry locks to release. Now that all bucket locks
         *  are in possess, we can slow down a bit to make lock holders
         *  to run faster.
         */
        EntryAllocLock.lock();
        for (int32_t i = 0; i < NonEmptyEntryCount; ++i)
        {
            while (MTEntries[i].EntryLock.load(std::memory_order_relaxed) != ENTRYLOCK_NOLOCK)
                Runtime::TransitionSleep(1);
        }
        EntryAllocLock.unlock();

        Trinity::Diagnostics::WriteLine(LogLevel::Verbose, "MTHash {0}: Lock acquired.", this->memory_trunk->TrunkId);

        ////////////////////////////
    }

    /**
     *  !Make sure to stop GC before calling this
     */
    void MTHash::Unlock()
    {
        if (TrinityConfig::ReadOnly())
            return;

        Trinity::Diagnostics::WriteLine(LogLevel::Verbose, "MTHash {0}: Attempting to unlock.", this->memory_trunk->TrunkId);

        /** Release all bucket locks. */
        std::atomic<uint64_t>* bucket_ptr            = reinterpret_cast<std::atomic<uint64_t>*>(BucketLockers);
        uint32_t               batch_entry_cnt       = BucketCount / sizeof(uint64_t);
        for (uint32_t i = 0; i < batch_entry_cnt; ++i)
        {
            bucket_ptr++->store(BUCKETLOCK_EIGHT_NOLOCKS, std::memory_order_release);
        }

        /* Sequentially process the remainders */
        for (uint32_t i = batch_entry_cnt * sizeof(uint64_t); i < BucketCount; ++i)
        {
            ReleaseBucketLock(i);
        }

        Trinity::Diagnostics::WriteLine(LogLevel::Verbose, "MTHash {0}: Unlock complete.", this->memory_trunk->TrunkId);
    }

    bool MTHash::TryGetEntryLockForDefragment(int32_t index)
    {
        char cmpxchg_val = ENTRYLOCK_NOLOCK;
        if (!std::atomic_compare_exchange_strong_explicit(&MTEntries[index].EntryLock, &cmpxchg_val, char(1), std::memory_order_acquire, std::memory_order_relaxed))
        {
            int32_t counter = 0;
            while (true)
            {
                cmpxchg_val = ENTRYLOCK_NOLOCK;
                if ((MTEntries[index].EntryLock.load(std::memory_order_relaxed) == ENTRYLOCK_NOLOCK) && 
                    std::atomic_compare_exchange_strong_explicit(&MTEntries[index].EntryLock, &cmpxchg_val, char(ENTRYLOCK_LOCK), std::memory_order_acquire, std::memory_order_relaxed))
                    break;

                //Quit if a high priority task is taking place
                if (memory_trunk->pending_flag > 0)
                {
                    return false;
                }

                ++counter;
                if (counter < COUNTER_THRESHOLD)
                    continue;
                else
                {
                    if (counter > TIMEOUT_THRESHOLD)
                    {
                        return false;
                    }
                    Runtime::TransitionSleep(0);
                }
            }
        }
        return true;
    }
}
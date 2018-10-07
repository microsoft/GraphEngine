// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "Storage/MTHash/MTHash.h"
#include "Storage/MemoryTrunk/MemoryTrunk.h"
#include "Storage/LocalStorage/ThreadContext.h"
#include "Runtime/Spinwait.h"
#include <threading>

#define COUNTER_THRESHOLD 4000

namespace Storage
{
    void MTHash::GetAllEntryLocksExceptArena()
    {
        PTHREAD_CONTEXT pctx = EnsureCurrentThreadContext();
        pctx->ReloadingMemoryTrunk = true;
        EntryAllocLock->lock();
        auto arena_cells = ReadMemoryAllocationArena();

        for (uint32_t i = 0; i < ExtendedInfo->EntryCount.load(); ++i)
        {
            char cmpxchg_val = ENTRYLOCK_NOLOCK;
            if (!std::atomic_compare_exchange_strong_explicit(&MTEntries[i].EntryLock, &cmpxchg_val, char(ENTRYLOCK_LOCK), std::memory_order_acquire, std::memory_order_relaxed))
            {
                int32_t counter = 0;
                while (arena_cells.find(MTEntries[i].Key) == arena_cells.end())
                {
                    cmpxchg_val = ENTRYLOCK_NOLOCK;
                    if (std::atomic_compare_exchange_strong_explicit(&MTEntries[i].EntryLock, &cmpxchg_val, char(ENTRYLOCK_LOCK), std::memory_order_acquire, std::memory_order_relaxed))
                        break;

                    ++counter;
                    if (counter < COUNTER_THRESHOLD)
                        continue;
                    else
                    {
                        TrinityErrorCode eResult = Arbitrate();
                        //we're reloading. nobody should fail us.
                        assert(eResult == TrinityErrorCode::E_SUCCESS);
                        //update the arena, in case new threads come in
                        arena_cells = ReadMemoryAllocationArena();
                        counter = 0;
                    }
                }
            }
        }
    }

    void MTHash::ReleaseAllEntryLocksExceptArena()
    {
        PTHREAD_CONTEXT pctx = EnsureCurrentThreadContext();
        // !Note, we only read once when we release all locks
        //  except arena. This is different from acquiring the
        //  locks, because when we have acquired all locks,
        //  the entries are either acquired by us, or in the
        //  arena, blocked by the alloc_lock waiting for us
        //  to release the spinlock. So it is impossible for
        //  a new thread to hold a lock, and join the arena.
        auto arena_cells = ReadMemoryAllocationArena();
        for (uint32_t i = 0; i < ExtendedInfo->EntryCount.load(); ++i)
        {
            if (arena_cells.find(MTEntries[i].Key) != arena_cells.end())
                continue;
            MTEntries[i].EntryLock.store(ENTRYLOCK_NOLOCK);
        }
        pctx->ReloadingMemoryTrunk = false;
        EntryAllocLock->unlock();
    }

    TrinityErrorCode MTHash::TryGetBucketLock(const uint32_t index)
    {
        if (TrinityConfig::ReadOnly())
            return TrinityErrorCode::E_SUCCESS;

        char cmpxchg_val = BUCKETLOCK_NOLOCK;
        if (!std::atomic_compare_exchange_strong(BucketLockers + index, &cmpxchg_val, char(BUCKETLOCK_LOCK)))
        {
            int32_t fail_counter = 0;
            while (true)
            {
                cmpxchg_val = BUCKETLOCK_NOLOCK;
                if (BucketLockers[index].load(std::memory_order_relaxed) == BUCKETLOCK_NOLOCK &&
                    std::atomic_compare_exchange_strong(BucketLockers + index, &cmpxchg_val, char(BUCKETLOCK_LOCK)))
                {
                    return TrinityErrorCode::E_SUCCESS;
                }

                if (++fail_counter < BUCKET_LOCK_SPIN_COUNT)
                {
                    Runtime::Spinwait(fail_counter / BUCKET_LOCK_SPIN_DIVISOR);
                }
                else
                {
                    Runtime::Spinwait(BUCKET_LOCK_SPIN_COUNT / BUCKET_LOCK_SPIN_DIVISOR);
                    if (fail_counter > BUCKET_LOCK_SPIN_TIMEOUT)
                    {
                        return TrinityErrorCode::E_TIMEOUT;
                    }
                }
            }
        }
        return TrinityErrorCode::E_SUCCESS;
    }

    void MTHash::_GetBucketLock(const uint32_t index)
    {
        while (TrinityErrorCode::E_SUCCESS != TryGetBucketLock(index));
    }

    void MTHash::ReleaseBucketLock(const uint32_t index)
    {
        if (TrinityConfig::ReadOnly())
            return;

        BucketLockers[index].store(BUCKETLOCK_NOLOCK, std::memory_order_release);
    }

    TrinityErrorCode MTHash::TryGetEntryLock(const int32_t index)
    {
        if (TrinityConfig::ReadOnly())
            return TrinityErrorCode::E_SUCCESS;

        char cmpxchg_val = ENTRYLOCK_NOLOCK;
        if (!std::atomic_compare_exchange_strong(&MTEntries[index].EntryLock, &cmpxchg_val, char(ENTRYLOCK_LOCK)))
        {
            int32_t         fail_counter = 0;
            PTHREAD_CONTEXT p_ctx = EnsureCurrentThreadContext();
            cellid_t        cellid = MTEntries[index].Key;
            assert(p_ctx != nullptr);
            //  Am I locking the cell already?
            if (p_ctx->LockedCells.Contains(cellid))
            {
                uint8_t lock_val = MTEntries[index].EntryLock.fetch_add(1, std::memory_order_acquire) + 1;
                if (lock_val < ENTRYLOCK_MAX_LOCK_CNT)
                {
                    return TrinityErrorCode::E_SUCCESS;
                }
                else
                {
                    --MTEntries[index].EntryLock;
                    return TrinityErrorCode::E_CELL_LOCK_OVERFLOW;
                }
            }

            while (true)
            {
                cmpxchg_val = ENTRYLOCK_NOLOCK;
                if (MTEntries[index].EntryLock.load(std::memory_order_relaxed) == ENTRYLOCK_NOLOCK &&
                    std::atomic_compare_exchange_strong(&MTEntries[index].EntryLock, &cmpxchg_val, char(ENTRYLOCK_LOCK)))
                    return TrinityErrorCode::E_SUCCESS;

                Runtime::Spinwait(fail_counter / ENTRY_LOCK_SPIN_DIVISOR);

                if (++fail_counter > ENTRY_LOCK_SPIN_COUNT)
                {
                    return TrinityErrorCode::E_TIMEOUT;
                }
            }
        }
        return TrinityErrorCode::E_SUCCESS;
    }

    uint8_t MTHash::ReleaseEntryLock(const int32_t index)
    {
        if (TrinityConfig::ReadOnly())
            return 0;

        return (MTEntries[index].EntryLock.fetch_add(-1, std::memory_order_release) - 1);
    }

    /**
     *  Takes all bucket locks and wait for all entry lock holders to exit.
     *  Returns E_SUCCESS when the hash table is successfully locked.
     *  Returns E_DEADLOCK when there're deadlocks.
     *  It is guaranteed that, when this rountine returns, all bucket
     *  locks are obtained.
     */
    ALLOC_THREAD_CTX TrinityErrorCode MTHash::Lock()
    {
        if (TrinityConfig::ReadOnly())
            return TrinityErrorCode::E_SUCCESS;

        Trinity::Diagnostics::WriteLine(LogLevel::Verbose, "MTHash {0}: Attempting to lock.", this->memory_trunk->TrunkId);
        PTHREAD_CONTEXT p_ctx = EnsureCurrentThreadContext();
        assert(p_ctx != nullptr);

        p_ctx->LockingMTHash = this->memory_trunk->TrunkId;

        /** Take all bucket locks. */
        std::atomic<uint64_t>* bucket_ptr            = reinterpret_cast<std::atomic<uint64_t>*>(BucketLockers);
        uint32_t               batch_entry_cnt       = BucketCount / sizeof(uint64_t);

        auto                   batch_get_lock_action = [&](std::atomic<uint64_t>* ptr)
        {
            uint64_t comparand = BUCKETLOCK_EIGHT_NOLOCKS;
            /* Opportunistic batch lock acquire, falling back to individual lock acquiring actions. */
            if (!std::atomic_compare_exchange_strong(ptr, &comparand, uint64_t(BUCKETLOCK_EIGHT_LOCKS)))
            {
                uint32_t bucket_idx = (uint32_t)(reinterpret_cast<std::atomic<char>*>(ptr) - BucketLockers);
                for (uint32_t offset = 0; offset < sizeof(uint64_t); ++offset)
                    _GetBucketLock(bucket_idx++);
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
            _GetBucketLock(i);
        }
        ////////////////////////////

        Trinity::Diagnostics::WriteLine(LogLevel::Verbose, "MTHash {0}: Bucket locks acquired, waiting for cell locks to release.", this->memory_trunk->TrunkId);

        /**
         *  Wait all entry locks to release. Now that all bucket locks
         *  are in possess, we can slow down a bit to make lock holders
         *  to run faster.
         */
        EntryAllocLock->lock();
        for (int32_t i = 0; i < ExtendedInfo->NonEmptyEntryCount; ++i)
        {
            int retry_cnt = 0;
            while (MTEntries[i].EntryLock.load(std::memory_order_relaxed) != ENTRYLOCK_NOLOCK)
            {
                // If the current thread has locked the cell, we can skip it.
                if (p_ctx->LockedCells.Contains(MTEntries[i].Key)) { break; }
                Runtime::Spinwait(retry_cnt / ENTRY_LOCK_SPIN_DIVISOR);
                if (++retry_cnt > ENTRY_LOCK_SPIN_COUNT)
                {
                    TrinityErrorCode eResult = Storage::Arbitrate();
                    if (eResult != TrinityErrorCode::E_SUCCESS)
                    {
                        EntryAllocLock->unlock();
                        return eResult;
                    }
                    retry_cnt = 0;
                }
            }
        }
        EntryAllocLock->unlock();

        Trinity::Diagnostics::WriteLine(LogLevel::Verbose, "MTHash {0}: Lock acquired.", this->memory_trunk->TrunkId);

        ////////////////////////////
        return TrinityErrorCode::E_SUCCESS;
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


        PTHREAD_CONTEXT p_ctx = EnsureCurrentThreadContext();
        assert(p_ctx != nullptr);
        p_ctx->LockingMTHash = -1;
        Trinity::Diagnostics::WriteLine(LogLevel::Verbose, "MTHash {0}: Unlock complete.", this->memory_trunk->TrunkId);
    }

    bool MTHash::TryGetEntryLockForDefragment(const int32_t index)
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
                    Runtime::Spinwait(0);
                }
            }
        }
        return true;
    }
}

// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <functional>
#include <corelib>
#include <diagnostics>
#include "TrinityCommon.h"
#include "Trinity/Configuration/TrinityConfig.h"
#include "Memory/Memory.h"
#include "CellEntry.h"
#include "Trinity/Threading/TrinityLock.h"
#include "Utility/HashHelper.h"
#include "Trinity/Diagnostics/Log.h"
#include "Storage/LocalStorage/ThreadContext.h"

#define CELL_ACQUIRE_LOCK
#define CELL_ATOMIC
#define CELL_LOCK_PROTECTED

#define ENTRYLOCK_NOLOCK            0
#define ENTRYLOCK_LOCK              1
#define ENTRYLOCK_MAX_LOCK_CNT      255

#define BUCKETLOCK_NOLOCK           0
#define BUCKETLOCK_LOCK             1
#define BUCKETLOCK_EIGHT_NOLOCKS    0x0ULL
#define BUCKETLOCK_EIGHT_LOCKS      0x0101010101010101ULL

#define BUCKET_LOCK_SPIN_TIMEOUT    30000
#define BUCKET_LOCK_SPIN_DIVISOR    300
#define BUCKET_LOCK_SPIN_COUNT      20000
#define ENTRY_LOCK_SPIN_DIVISOR     300
#define ENTRY_LOCK_SPIN_COUNT       50000
#define LOOKUP_IDLE_SPIN_COUNT      10000
#define MTHASH_LOOKUP_MAX_RETRY     128
#define MTHASH_ALL_ENTRY_SPIN_COUNT 900000

namespace Storage
{
    namespace LocalMemoryStorage
    {
        extern int32_t* dirty_flags;
    }

    class MemoryTrunk;

    struct MTHashAllocationInfo
    {
        /// Freed entry list head
        int32_t FreeEntryList;
        /// Freed entry count
        std::atomic<int32_t> FreeEntryCount;
        /// Actually allocated entry count, not counting guard entries
        std::atomic<uint32_t> EntryCount;
        /// Non empty entries count, including free entries
        std::atomic<int32_t> NonEmptyEntryCount;
    };

    class MTHash
    {
    public:
        enum Int32_Constants : int32_t
        {
            /// Must be constant 1 for back compatibility
            VERSION             = 1,

            TIMEOUT_THRESHOLD   = 21044,

            CellTypeEnabled     = 1,

            /// To enable PhysicalMemoryLocking,
            /// Memory::SetWorkingSetProfile(MemoryAllocationProfile::Aggressive) must be first called.
            PhysicalMemoryLocking = 0,
        };

        enum UInt32_Contants :uint32_t
        {
            /// Value   = 560689, a prime number that is close to 524288 (512K)
            BucketCount = 560689,
            EntryExpandUnit = 8192,
            GuardedEntryCount = 1024,
        };

        static uint64_t MTEntryOffset()
        {
            return TrinityConfig::MemoryReserveUnit() / sizeof(CellEntries[0]);
        }
        static uint64_t BucketMemoryOffset()
        {
            return MTEntryOffset() + TrinityConfig::MemoryReserveUnit() / sizeof(MTEntries[0]);
        }
        static uint64_t BucketLockerMemoryOffset()
        {
            return BucketMemoryOffset() + TrinityConfig::MemoryReserveUnit() / sizeof(Buckets[0]);
        }
        static uint64_t LookupLossyCounter;
        constexpr uint64_t LookupSlowPathThreshold() { return 8192; }

        /*****************************************************************/
        /**************************** 64-byte block **********************/
        /// Entry Elements
        int32_t* Buckets;
        std::atomic<char>* BucketLockers;

        CellEntry* CellEntries;
        MTEntry* MTEntries;
        ///////////////////////////// 32 bytes
        MemoryTrunk* memory_trunk;

        TrinityLock* FreeListLock;
        TrinityLock* EntryAllocLock;

        MTHashAllocationInfo* ExtendedInfo;
        /**************************** 64-byte block **********************/
        ///////////////////////////////////////////////////////////////////

        MTHash();
        void AllocateMTHash();
        void DeallocateMTHash();
        void InitMTHashAttributes(MemoryTrunk * mt);
        void Initialize(uint32_t capacity, MemoryTrunk * mt);
        bool Initialize(const String& input_file, MemoryTrunk* mt);

        void Clear();
        ~MTHash();
        void ResetSizeEntryUnsafe(int32_t index);

        CELL_LOCK_PROTECTED inline uint16_t CellType(const uint32_t index)
        {
            if (CellTypeEnabled)
                return MTEntries[index].CellType;
            else
                return -1;
        }

        CELL_LOCK_PROTECTED inline int32_t CellSize(const uint32_t index)
        {
            if (CellEntries[index].offset >= 0)
                return CellEntries[index].size & 0xFFFFFF; // Max size = (16 M - 1)
            else
                return CellEntries[index].size;
        }

        inline uint32_t GetBucketIndex(int64_t cellId)
        {
            return (((uint32_t)cellId) ^ ((uint32_t)(cellId >> 0x20))) % BucketCount;
        }


        void MarkTrunkDirty();

        void Expand(bool force);

        // Thread-safe
        void                         FreeEntry(int32_t entry_index);
        int32_t                      FindFreeEntry();
        int32_t                      Count();
        // Lock-related
        //  The locking policy: favor _GetBucketLock() over TryGetEntryLock().
        //  _GetBucketLock() will always succeed, while TryGetEntryLock() might fail.
        //  TryGetEntryLock() is always called with bucket lock acquired.
        //  When TryGetEntryLock() fails, bucket lock is released, and the attempt
        //  to get the bucket lock will be made later, so that others requiring
        //  this bucket lock can get a chance to proceed. Thus, MTHash::Lock
        //  has the higher priority in taking all bucket locks (it will always 
        //  succeed, because others fail, and provide a time window for it).
        ALLOC_THREAD_CTX void GetAllEntryLocksExceptArena();
        ALLOC_THREAD_CTX void ReleaseAllEntryLocksExceptArena();
        ALLOC_THREAD_CTX TrinityErrorCode Lock();                     // E_SUCCESS or E_DEADLOCK.
        TrinityErrorCode TryGetBucketLock(const uint32_t index);            // E_SUCCESS or E_TIMEOUT.
        TrinityErrorCode TryGetEntryLock(const int32_t index);              // E_SUCCESS or E_TIMEOUT.
        bool             TryGetEntryLockForDefragment(const int32_t index);
        ALLOC_THREAD_CTX void Unlock();
        void             ReleaseBucketLock(const uint32_t index);
        uint8_t          ReleaseEntryLock(const int32_t index);

        // Cell manipulation interfaces
        CELL_ATOMIC         TrinityErrorCode RemoveCell(IN cellid_t cellId);
        CELL_ATOMIC         TrinityErrorCode RemoveCell(IN cellid_t cellId, IN CellAccessOptions options);
        CELL_LOCK_PROTECTED TrinityErrorCode ResizeCell(IN int32_t  cellEntryIndex, IN int32_t offsetInCell, IN int32_t sizeDelta, OUT char*& cell_ptr);
        CELL_ATOMIC         TrinityErrorCode GetCellType(IN cellid_t cellId, OUT uint16_t &cellType);
        // XXX GetCellSize not exposed through LocalMemoryStorage.
        CELL_ATOMIC         TrinityErrorCode GetCellSize(IN cellid_t key, OUT int32_t &size);
        TrinityErrorCode                     ContainsKey(IN cellid_t key);

        /////////////////////////////////

        // GetLockedCellInfo interfaces
        CELL_ACQUIRE_LOCK TrinityErrorCode CGetLockedCellInfo4CellAccessor(IN const cellid_t cellId, OUT int32_t &cellSize, OUT uint16_t &type, OUT char* &cellPtr, OUT int32_t &entryIndex);
        CELL_ACQUIRE_LOCK TrinityErrorCode CGetLockedCellInfo4SaveCell(IN const cellid_t cellId, IN const int32_t cellSize, IN const uint16_t type, OUT char* &cellPtr, OUT int32_t &entryIndex);
        CELL_ACQUIRE_LOCK TrinityErrorCode CGetLockedCellInfo4AddCell(IN const cellid_t cellId, IN const int32_t cellSize, IN const uint16_t type, OUT char* &cellPtr, OUT int32_t &entryIndex);
        CELL_ACQUIRE_LOCK TrinityErrorCode CGetLockedCellInfo4UpdateCell(IN const cellid_t cellId, IN const int32_t cellSize, OUT char* &cellPtr, OUT int32_t &entryIndex);
        CELL_ACQUIRE_LOCK TrinityErrorCode CGetLockedCellInfo4LoadCell(IN const cellid_t cellId, OUT int32_t &cellSize, OUT char* &cellPtr, OUT int32_t &entryIndex);
        CELL_ACQUIRE_LOCK TrinityErrorCode CGetLockedCellInfo4AddOrUseCell(IN const cellid_t cellId, IN OUT int32_t &cellSize, IN const uint16_t type, OUT char* &cellPtr, OUT int32_t &entryIndex);
        ///////////////////////////////////

        // DiskIO
        bool Save(const String& output_file);
        bool Reload(const String& input_file);

        // Performance counters
        uint64_t CommittedMemorySize();
        uint64_t TotalCellSize();

        // Checksum
        TrinityErrorCode GetMD5Hash(OUT char* hash);
    private:
        CELL_LOCK_PROTECTED void _discard_locked_free_entry(IN uint32_t bucket_index, IN int32_t entry_index);

        void _GetBucketLock(uint32_t index);              // High priority. Guarantees success.

        ALLOC_THREAD_CTX template<typename TLockAction, typename TLookupFound, typename TLookupNotFound, typename TSetupTx>
            TrinityErrorCode _Lookup_impl
            (const cellid_t cellId,
             const TLockAction& entry_lock_action,
             const TLookupFound& found_action,
             const TLookupNotFound& not_found_action,
             const TSetupTx& setup_tx)
        {
            int32_t          backoff_attempts = 0;
            uint32_t         bucket_index     = GetBucketIndex(cellId);
            bool             tx_setup         = false;
            int32_t          previous_entry_index;

        start:
            previous_entry_index = -1;
            if (TryGetBucketLock(bucket_index) != TrinityErrorCode::E_SUCCESS)
            {
                goto backoff;
            }

            for (int32_t entry_index = Buckets[bucket_index]; entry_index >= 0; entry_index = MTEntries[entry_index].NextEntry)
            {
                if (MTEntries[entry_index].Key == cellId)
                {
                    switch (TrinityErrorCode err = entry_lock_action(entry_index))
                    {
                    case TrinityErrorCode::E_SUCCESS:
                        return found_action(entry_index, bucket_index, previous_entry_index);
                    case TrinityErrorCode::E_TIMEOUT:
                        ReleaseBucketLock(bucket_index);
                        goto backoff;
                    default: /* E_CELL_LOCK_OVERFLOW or other errors */
                        ReleaseBucketLock(bucket_index);
                        return err;
                    }
                }
                previous_entry_index = entry_index;
            }

        notfound:
            return not_found_action(bucket_index);

        backoff:
            if (++backoff_attempts > MTHASH_LOOKUP_MAX_RETRY)
            {
                if (!tx_setup)
                {
                    setup_tx();
                    tx_setup = true;
                }

                if (TrinityErrorCode::E_DEADLOCK == Arbitrate())
                {
                    return TrinityErrorCode::E_DEADLOCK;
                }
                else
                {
                    backoff_attempts = 0;
                }
            }

            if (++LookupLossyCounter < LookupSlowPathThreshold())
            {
                goto start;
            }

            //  !slow path = fast path + collect free entries in bucket chain
            LookupLossyCounter = 0;

            previous_entry_index = -1;
            if (TryGetBucketLock(bucket_index) != TrinityErrorCode::E_SUCCESS)
            {
                goto backoff;
            }

            for (int32_t entry_index = Buckets[bucket_index]; entry_index >= 0; entry_index = MTEntries[entry_index].NextEntry)
            {
                if (CellEntries[entry_index].location == -1)
                {
                    if (previous_entry_index < 0)
                    {
                        Buckets[bucket_index] = MTEntries[entry_index].NextEntry;
                    }
                    else
                    {
                        MTEntries[previous_entry_index].NextEntry = MTEntries[entry_index].NextEntry;
                    }
                    FreeEntry(entry_index);
                    // roll back to previous and jump over
                    entry_index = previous_entry_index;
                }
                else if (MTEntries[entry_index].Key == cellId)
                {
                    switch (TrinityErrorCode err = entry_lock_action(entry_index))
                    {
                    case TrinityErrorCode::E_SUCCESS:
                        return found_action(entry_index, bucket_index, previous_entry_index);
                    case TrinityErrorCode::E_TIMEOUT:
                        ReleaseBucketLock(bucket_index);
                        goto backoff;
                    default: /* E_CELL_LOCK_OVERFLOW or other errors */
                        ReleaseBucketLock(bucket_index);
                        return err;
                    }
                }
                previous_entry_index = entry_index;
            }

            goto notfound;
        }

        template<typename TLookupFound, typename TLookupNotFound>
        inline TrinityErrorCode _Lookup_LockEntry_Or_NotFound
        (const cellid_t cellId,
         const TLookupFound& found_action,
         const TLookupNotFound& not_found_action)
        {
            return _Lookup_impl(
                cellId,
                [this](const int32_t entry_idx) {return TryGetEntryLock(entry_idx); },
                found_action,
                not_found_action,
                [=] { PTHREAD_CONTEXT pctx = EnsureCurrentThreadContext(); pctx->SetLockingCell(cellId); });
        }

        template<typename TLookupFound, typename TLookupNotFound>
        inline TrinityErrorCode _Lookup_NoLockEntry_Or_NotFound
        (const cellid_t cellId,
         const TLookupFound& found_action,
         const TLookupNotFound& not_found_action)
        {
            return _Lookup_impl(
                cellId,
                [](const int32_t _) {return TrinityErrorCode::E_SUCCESS; },
                found_action,
                not_found_action,
                [] {});
        }
    };

#define ENTER_ALLOCMEM_CELLENTRY_UPDATE_CRITICAL_SECTION() \
    memory_trunk->add_memory_entry_flag.fetch_add(1);      \
                                                while ((memory_trunk->add_memory_entry_flag.load() & 0x80000000) == 0x80000000);

#define LEAVE_ALLOCMEM_CELLENTRY_UPDATE_CRITICAL_SECTION() \
    memory_trunk->add_memory_entry_flag.fetch_sub(1);

}

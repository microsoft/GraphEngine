// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include "TrinityCommon.h"
#include "Trinity/Configuration/TrinityConfig.h"
#include "Memory/Memory.h"
#include "CellEntry.h"
#include "Threading/TrinitySpinlock.h"
#include "Utility/HashHelper.h"
#include "MT_ENUMERATOR.h"
#include <corelib>
#include <diagnostics>

#define CELL_ACQUIRE_LOCK
#define CELL_ATOMIC
#define CELL_LOCK_PROTECTED

#define ENTRYLOCK_NOLOCK         0
#define ENTRYLOCK_LOCK           1
#define ENTRYLOCK_ARENA          3

#define BUCKETLOCK_NOLOCK        0
#define BUCKETLOCK_LOCK          1
#define BUCKETLOCK_EIGHT_NOLOCKS 0x0ULL
#define BUCKETLOCK_EIGHT_LOCKS   0x0101010101010101ULL

namespace Storage
{
    namespace LocalMemoryStorage
    {
        enum   CellAccessOptions : int32_t;
        extern int32_t* dirty_flags;
    }

    using LocalMemoryStorage::CellAccessOptions;
    class MemoryTrunk;
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

        static uint64_t MTEntryOffset;
        static uint64_t BucketMemoryOffset;
        static uint64_t BucketLockerMemoryOffset;

        /*****************************************************************/
        /**************************** 64-byte block **********************/
        /// Entry Elements
        int32_t* Buckets;
        std::atomic<char>* BucketLockers;

        CellEntry* CellEntries;
        MTEntry* MTEntries;
        /////////////////////////////
        MemoryTrunk * memory_trunk;

        int32_t FreeEntryList;
        TrinitySpinlock FreeListLock;
        TrinitySpinlock EntryAllocLock;

        std::atomic<uint32_t> EntryCount;

        /// Non empty entries count, including free entries
        std::atomic<int32_t> NonEmptyEntryCount;

        /// Freed entry count
        std::atomic<int32_t> FreeEntryCount;
        /**************************** 64-byte block **********************/
        ///////////////////////////////////////////////////////////////////

        inline MTHash(){ BucketLockers = nullptr; };
        void AllocateMTHash();
        void DeallocateMTHash(bool deallocateBucketLockers);
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
            return (((uint32_t) cellId) ^ ((uint32_t) (cellId >> 0x20))) % BucketCount;
        }

        inline void GetBucketLock(uint32_t index)
        {
            if (TrinityConfig::ReadOnly())
                return;

            char cmpxchg_val = BUCKETLOCK_NOLOCK;
            if (!std::atomic_compare_exchange_strong(BucketLockers + index, &cmpxchg_val, char(BUCKETLOCK_LOCK)))
            {
                int32_t fail_counter = 0;
                while (true)
                {
                    cmpxchg_val = BUCKETLOCK_NOLOCK;
                    if (BucketLockers[index].load(std::memory_order_relaxed) == BUCKETLOCK_NOLOCK &&
                        std::atomic_compare_exchange_strong(BucketLockers + index, &cmpxchg_val, char(BUCKETLOCK_LOCK)))
                    { return; }
                    if (++fail_counter >= 10000)
                    { Runtime::TransitionSleep(0); }
                }
            }
            return;
        }

        inline void ReleaseBucketLock(uint32_t index)
        {
            if (TrinityConfig::ReadOnly())
                return;

            BucketLockers[index].store(BUCKETLOCK_NOLOCK, std::memory_order_release);
        }

        inline void GetEntryLock(int32_t index)
        {
            if (TrinityConfig::ReadOnly())
                return;

            char cmpxchg_val = ENTRYLOCK_NOLOCK;
            if (!std::atomic_compare_exchange_strong(&MTEntries[index].EntryLock, &cmpxchg_val, char(ENTRYLOCK_LOCK)))
            {
                int32_t fail_counter = 0;
                while (true)
                {
                    cmpxchg_val = ENTRYLOCK_NOLOCK;
                    if (MTEntries[index].EntryLock.load(std::memory_order_relaxed) == ENTRYLOCK_NOLOCK && 
                        std::atomic_compare_exchange_strong(&MTEntries[index].EntryLock, &cmpxchg_val, char(ENTRYLOCK_LOCK)))
                        return;
                    if (++fail_counter >= 10000)
                    {
                        Runtime::TransitionSleep(0);
                    }
                }
            }
            return;
        }

        inline void ReleaseEntryLock(int32_t index)
        {
            if (TrinityConfig::ReadOnly())
                return;

            MTEntries[index].EntryLock.store(ENTRYLOCK_NOLOCK, std::memory_order_release);
        }

        void Expand(bool force);

        void MarkTrunkDirty();

        // Thread-safe
        void                         FreeEntry    (int32_t entry_index);
        int32_t                      FindFreeEntry();
        int32_t                      Count        ();
        bool                         ContainsKey  (cellid_t key);
        CELL_ATOMIC TrinityErrorCode GetCellType  (IN cellid_t cellId, OUT uint16_t &cellType);
        CELL_ATOMIC int32_t          GetCellSize  (IN cellid_t key);

        // lock
        void GetAllEntryLocksExceptArena();
        void ReleaseAllEntryLocksExceptArena();
        void Lock();
        void Unlock();
        bool TryGetEntryLockForDefragment(int32_t index);

        // Cell manipulation interfaces
        CELL_ATOMIC         TrinityErrorCode RemoveCell            (IN cellid_t cellId);
        CELL_ATOMIC         TrinityErrorCode RemoveCell            (IN cellid_t cellId,         IN CellAccessOptions options);
        CELL_LOCK_PROTECTED TrinityErrorCode ResizeCell            (IN int32_t  cellEntryIndex, IN int32_t offsetInCell, IN int32_t sizeDelta, OUT char*& cell_ptr);
        /////////////////////////////////

        // GetLockedCellInfo interfaces
        CELL_ACQUIRE_LOCK TrinityErrorCode CGetLockedCellInfo4CellAccessor(IN cellid_t cellId, OUT int32_t &cellSize   , OUT uint16_t &type, OUT char* &cellPtr, OUT int32_t &entryIndex);
        CELL_ACQUIRE_LOCK TrinityErrorCode CGetLockedCellInfo4SaveCell    (IN cellid_t cellId, IN int32_t cellSize     , IN uint16_t type  , OUT char* &cellPtr, OUT int32_t &entryIndex);
        CELL_ACQUIRE_LOCK TrinityErrorCode CGetLockedCellInfo4AddCell     (IN cellid_t cellId, IN int32_t cellSize     , IN uint16_t type  , OUT char* &cellPtr, OUT int32_t &entryIndex);
        CELL_ACQUIRE_LOCK TrinityErrorCode CGetLockedCellInfo4UpdateCell  (IN cellid_t cellId, IN int32_t cellSize     , OUT char* &cellPtr, OUT int32_t &entryIndex);
        CELL_ACQUIRE_LOCK TrinityErrorCode CGetLockedCellInfo4LoadCell    (IN cellid_t cellId, OUT int32_t &cellSize   , OUT char* &cellPtr, OUT int32_t &entryIndex);
        CELL_ACQUIRE_LOCK TrinityErrorCode CGetLockedCellInfo4AddOrUseCell(IN cellid_t cellId, IN OUT int32_t &cellSize, IN uint16_t type  , OUT char* &cellPtr, OUT int32_t &entryIndex);
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

    };

#define ENTER_ALLOCMEM_CELLENTRY_UPDATE_CRITICAL_SECTION() \
    memory_trunk->add_memory_entry_flag.fetch_add(1);      \
                                                while ((memory_trunk->add_memory_entry_flag.load() & 0x80000000) == 0x80000000);

#define LEAVE_ALLOCMEM_CELLENTRY_UPDATE_CRITICAL_SECTION() \
    memory_trunk->add_memory_entry_flag.fetch_sub(1);

}
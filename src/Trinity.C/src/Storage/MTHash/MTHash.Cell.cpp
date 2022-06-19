// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "Storage/MTHash/MTHash.h"
#include "Storage/MemoryTrunk/MemoryTrunk.h"
#include "Storage/LocalStorage/LocalMemoryStorage.h"
#include <climits>

using namespace Storage::LocalMemoryStorage::Logging;

namespace Storage
{
    CELL_ATOMIC TrinityErrorCode MTHash::RemoveCell(IN cellid_t cellId)
    {
        return _Lookup_LockEntry_Or_NotFound(
            cellId,
            [this](const int32_t entry_index, const uint32_t bucket_index, const int32_t previous_entry_index)
        {
            int32_t offset = CellEntries[entry_index].offset;
            std::atomic<int64_t> * CellEntryAtomicPtr = (std::atomic<int64_t>*) CellEntries;
            (CellEntryAtomicPtr + entry_index)->store(-1, std::memory_order_relaxed);

            if (previous_entry_index < 0)
            {
                Buckets[bucket_index] = MTEntries[entry_index].NextEntry;
            }
            else
            {
                MTEntries[previous_entry_index].NextEntry = MTEntries[entry_index].NextEntry;
            }

            ReleaseBucketLock(bucket_index);
            FreeEntry(entry_index);
            ReleaseEntryLock(entry_index);

            if (offset < 0)
            {
                memory_trunk->lo_lock->lock();
                memory_trunk->DisposeLargeObject(-offset);
                memory_trunk->lo_lock->unlock();
            }
            else
            {
                MarkTrunkDirty();
            }

            return TrinityErrorCode::E_SUCCESS;
        },
            [this](const uint32_t bucket_index)
        {
            ReleaseBucketLock(bucket_index);
            return TrinityErrorCode::E_CELL_NOT_FOUND;
        });
    }

    CELL_ATOMIC TrinityErrorCode MTHash::RemoveCell(IN cellid_t cellId, IN CellAccessOptions options)
    {
        return _Lookup_LockEntry_Or_NotFound(
            cellId,
            [this, /* OUT params */ &cellId, &options](const int32_t entry_index, const uint32_t bucket_index, const int32_t previous_entry_index)
        {
            int32_t  offset = CellEntries[entry_index].offset;
            std::atomic<int64_t> * CellEntryAtomicPtr = (std::atomic<int64_t>*) CellEntries;
            (CellEntryAtomicPtr + entry_index)->store(-1, std::memory_order_relaxed);

            if (previous_entry_index < 0)
            {
                Buckets[bucket_index] = MTEntries[entry_index].NextEntry;
            }
            else
            {
                MTEntries[previous_entry_index].NextEntry = MTEntries[entry_index].NextEntry;
            }

            ReleaseBucketLock(bucket_index);
            FreeEntry(entry_index);
            WriteAheadLog(cellId, nullptr, -1, -1, options);
            ReleaseEntryLock(entry_index);

            if (offset < 0)
            {
                memory_trunk->lo_lock->lock();
                memory_trunk->DisposeLargeObject(-offset);
                memory_trunk->lo_lock->unlock();
            }
            else
            {
                MarkTrunkDirty();
            }

            return TrinityErrorCode::E_SUCCESS;
        },
            [this](const uint32_t bucket_index)
        {
            ReleaseBucketLock(bucket_index);
            return TrinityErrorCode::E_CELL_NOT_FOUND;
        });
    }

    TrinityErrorCode MTHash::ContainsKey(IN cellid_t key)
    {
		//  !Note, must be _Lookup_NoLockEntry_Or_NotFound, 
		//   because we do not require THREAD_CTX
        return _Lookup_NoLockEntry_Or_NotFound(
            key,
            [this](const int32_t entry_index, const uint32_t bucket_index, const int32_t _) 
        {
            ReleaseBucketLock(bucket_index);
            return TrinityErrorCode::E_CELL_FOUND;
        },
            [this](const uint32_t bucket_index) {
            ReleaseBucketLock(bucket_index);
            return TrinityErrorCode::E_CELL_NOT_FOUND;
        });
    }

    CELL_ATOMIC TrinityErrorCode MTHash::GetCellType(IN cellid_t cellId, OUT uint16_t &cellType)
    {
        return _Lookup_LockEntry_Or_NotFound(cellId,
            [this, /* OUT params */ &cellType](const int32_t entry_index, const uint32_t bucket_index, const int32_t _)
        {
            ReleaseBucketLock(bucket_index);
            // output
            if (CellTypeEnabled)
                cellType = MTEntries[entry_index].CellType;
            /////////////////////////////////////
            ReleaseEntryLock(entry_index);
            return TrinityErrorCode::E_SUCCESS;
        },
            [this](const uint32_t bucket_index)
        {
            ReleaseBucketLock(bucket_index);
            return TrinityErrorCode::E_CELL_NOT_FOUND;
        });
    }

    TrinityErrorCode MTHash::GetCellSize(cellid_t key, OUT int32_t &size)
    {
        return _Lookup_LockEntry_Or_NotFound(key,
            [this, /* OUT params */ &size](const int32_t entry_index, const uint32_t bucket_index, const int32_t _)
        {
            ReleaseBucketLock(bucket_index);
            size = CellSize(entry_index);
            ReleaseEntryLock(entry_index);
            return TrinityErrorCode::E_CELL_FOUND;
        },
            [this, /* OUT params */ &size](const uint32_t bucket_index)
        {
            ReleaseBucketLock(bucket_index);
            size = -1;
            return TrinityErrorCode::E_CELL_NOT_FOUND;
        });
    }

    // List
    // -----------------------------------------------------------------------
    // |length|xxxxxxxx|xxxxxxxx|xxxxxxxx|xxxxxxxx|xxxxxxxx|xxxxxxxx|xxxxxxxx|
    // -----------------------------------------------------------------------
    // ptr                      offset
    //                          [----------delta-----------]
    // if delta>0,
    // --------------------------------------------------------------------------------------------------
    // |length|xxxxxxxx|xxxxxxxx|      delta length        |xxxxxxxx|xxxxxxxx|xxxxxxxx|xxxxxxxx|xxxxxxxx|
    // --------------------------------------------------------------------------------------------------
    // if delta is less than 0,
    // -----------------------------------------------------------------------
    // |length|xxxxxxxx|xxxxxxxx| erased delta length   ---|xxxxxxxx|xxxxxxxx|
    // -----------------------------------------------------------------------
    // --------------------------------------------
    // |length|xxxxxxxx|xxxxxxxx|xxxxxxxx|xxxxxxxx|
    // --------------------------------------------
    CELL_LOCK_PROTECTED TrinityErrorCode MTHash::ResizeCell(int32_t cellEntryIndex, int32_t offsetInCell, int32_t sizeDelta, OUT char*& _cell_ptr)
    {
        _cell_ptr = nullptr;
        TrinityErrorCode eResult = TrinityErrorCode::E_SUCCESS;
        int32_t currentOffset = CellEntries[cellEntryIndex].offset;

        if (currentOffset < 0)//Large object
        {
            // Large Object
            _cell_ptr = memory_trunk->LOPtrs[-currentOffset];

            if (sizeDelta > 0)
            {
                eResult = memory_trunk->ExpandLargeObject(-currentOffset, CellEntries[cellEntryIndex].size, CellEntries[cellEntryIndex].size + sizeDelta);
                if (TrinityErrorCode::E_SUCCESS != eResult) { return eResult; }
                memmove(
                    _cell_ptr + offsetInCell + sizeDelta,
                    _cell_ptr + offsetInCell,
                    (uint64_t)(CellEntries[cellEntryIndex].size - offsetInCell));
            }
            else
            {
                memmove(
                    _cell_ptr + offsetInCell,
                    _cell_ptr + offsetInCell - sizeDelta,
                    (uint64_t)(CellEntries[cellEntryIndex].size - offsetInCell + sizeDelta));
                eResult = memory_trunk->ShrinkLargeObject(-currentOffset, CellEntries[cellEntryIndex].size, CellEntries[cellEntryIndex].size + sizeDelta);
                if (TrinityErrorCode::E_SUCCESS != eResult) { return eResult; }
            }
            CellEntries[cellEntryIndex].size += sizeDelta;
            /////////////////////////////////////////////////////////
        }
        else// The cell is in Memory Trunk
        {
            if (sizeDelta > 0)
            {
                /**
                 * Size record              (SR) = 8-bit reservation factor (RF) + 24-bit real cell size (CS).
                 * We define reserved bytes (RB) = 1 << RF.
                 * Actual allocated space   (AS) = CS up-aligned to RB.
                 * Occupied bytes           (OB) = occupied reserved space, this is not AS.
                 * New size to allocate     (TA) = (CS + delta), TA is up-aligned to the boundary of reserved memory.
                 *								   If (OB + delta) >= RB, we have run out of reserved space.
                 *                                 In this case, we grow RF until it holds at least one delta size, or
                 *                                 until RF == 22 (in which case we may need multiple RB
                 *                                 to hold one delta).
                 */
                int32_t sizeRecord = CellEntries[cellEntryIndex].size;
                char reservation_factor = (char)(sizeRecord >> 24);
                int32_t reserved_bytes = 1 << reservation_factor;
                int32_t occupied_bytes = (reserved_bytes - 1) & sizeRecord;
                int32_t newOffset;

                if (occupied_bytes + sizeDelta >= reserved_bytes) //Have to allocate new space
                {
                    if (reservation_factor < 22)//4M max
                    {
                        do
                        {
                            ++reservation_factor;
                        } while (reservation_factor < 22 && (1 << reservation_factor) < sizeDelta);
                        // update RB according to grown RF.
                        reserved_bytes = 1 << reservation_factor;
                    }

                    int32_t new_cell_size = (CellSize(cellEntryIndex) + sizeDelta);
                    // size_to_alloc is up-aligned to the boundary of reserved memory
                    int32_t size_to_alloc = new_cell_size & ~(reserved_bytes - 1);
                    while (size_to_alloc <= new_cell_size)
                        size_to_alloc += reserved_bytes;

                    if (size_to_alloc >= TrinityConfig::LargeObjectThreshold())
                    {
                        //! Fall back to non-reserving
                        size_to_alloc = new_cell_size;
                    }

                    if (size_to_alloc == new_cell_size)
                        reservation_factor = 0; //! reset the reservation factor

                    /// add_memory_entry_flag prologue
                    ENTER_ALLOCMEM_CELLENTRY_UPDATE_CRITICAL_SECTION();
                    /// add_memory_entry_flag prologue

                    eResult = memory_trunk->AddMemoryCell(MTEntries[cellEntryIndex].Key, size_to_alloc, OUT newOffset);
                    if (eResult != TrinityErrorCode::E_SUCCESS) goto cleanup;

                    _cell_ptr = (newOffset >= 0) ? (memory_trunk->trunkPtr + newOffset) : (memory_trunk->LOPtrs[-newOffset]);
                    currentOffset = CellEntries[cellEntryIndex].offset;//currentOffset must be updated(as Reload may happen)

                    memmove(
                        _cell_ptr,
                        memory_trunk->trunkPtr + currentOffset,
                        (uint64_t)offsetInCell);
                    memmove(
                        _cell_ptr + offsetInCell + sizeDelta,
                        memory_trunk->trunkPtr + currentOffset + offsetInCell,
                        (uint64_t)(CellSize(cellEntryIndex) - offsetInCell));

                    if (newOffset >= 0)//Still in the trunk, update the preserve information and size
                    {
                        CellEntries[cellEntryIndex].size = new_cell_size;
                        CellEntries[cellEntryIndex].size |= (reservation_factor << 24);
                    }
                    else // Turned into a LO
                    {
                        CellEntries[cellEntryIndex].size = new_cell_size;//Overwrite the preserve data.
                    }
                    //This line will potentially turn the cell into a LO. Don't move it before the "if"
                    CellEntries[cellEntryIndex].offset = newOffset;

                cleanup:
                    /// add_memory_entry_flag epilogue
                    LEAVE_ALLOCMEM_CELLENTRY_UPDATE_CRITICAL_SECTION();
                    /// add_memory_entry_flag epilogue
                    //////////////////////////////////////////////////////////
                }
                else
                {
                    _cell_ptr = memory_trunk->trunkPtr + currentOffset;
                    memmove(
                        _cell_ptr + offsetInCell + sizeDelta,
                        _cell_ptr + offsetInCell,
                        (uint64_t)(CellSize(cellEntryIndex) - offsetInCell));
                    CellEntries[cellEntryIndex].size += sizeDelta;
                }
            }
            else
            {
                _cell_ptr = memory_trunk->trunkPtr + currentOffset;
                memmove(
                    _cell_ptr + offsetInCell,
                    _cell_ptr + offsetInCell - sizeDelta,
                    (uint64_t)(CellSize(cellEntryIndex) - offsetInCell + sizeDelta));
                CellEntries[cellEntryIndex].size += sizeDelta;
                MarkTrunkDirty();
            }
            ////////////////////////////////////////////////////////////////////////////////////
        }

        return eResult;
    }
}

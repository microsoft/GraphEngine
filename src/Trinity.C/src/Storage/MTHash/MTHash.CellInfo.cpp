// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "MTHash.h"
#include "Storage/MemoryTrunk/MemoryTrunk.h"

namespace Storage
{
    // !Called when a free entry is acquired, but memory allocation fails.
    // In this situation, we still hold the cell lock, but the bucket lock
    // is released. 
    // Note: deadlock may happen here, which means that it is not guaranteed
    // we can get the bucket lock while still holding the cell lock. Thus
    // we should take a pessimistic approach: we simply write -1 to the cell entry
    // so it appears to be a deleted entry (still in the bucket chain), which could
    // be collected later.
    void MTHash::_discard_locked_free_entry(uint32_t bucket_index, int32_t free_entry)
    {
        std::atomic<int64_t> * CellEntryAtomicPtr = (std::atomic<int64_t>*) CellEntries;
        (CellEntryAtomicPtr + free_entry)->store(-1, std::memory_order_relaxed);
        ReleaseEntryLock(free_entry);
    }

    CELL_ACQUIRE_LOCK TrinityErrorCode MTHash::CGetLockedCellInfo4CellAccessor(IN cellid_t cellId, OUT int32_t &cellSize, OUT uint16_t &type, OUT char* &cellPtr, OUT int32_t &entryIndex)
    {
        return _Lookup_LockEntry_Or_NotFound(
            cellId,
            [this, /* OUT */ &cellSize, &type, &cellPtr, &entryIndex](const int32_t entry_index, const uint32_t bucket_index, const int32_t _)
        {
            ReleaseBucketLock(bucket_index);
            int32_t cell_offset = CellEntries[entry_index].offset;

            // output
            cellSize = CellSize(entry_index);
            if (CellTypeEnabled)
                type = MTEntries[entry_index].CellType;

            if (cell_offset < 0)
                cellPtr = memory_trunk->LOPtrs[-cell_offset];
            else
                cellPtr = memory_trunk->trunkPtr + cell_offset;
            entryIndex = entry_index;
            //////////////////////////////

            return TrinityErrorCode::E_SUCCESS;
        },
            [this](uint32_t bucket_index)
        {
            ReleaseBucketLock(bucket_index);
            return TrinityErrorCode::E_CELL_NOT_FOUND;
        });
    }

    CELL_ACQUIRE_LOCK TrinityErrorCode MTHash::CGetLockedCellInfo4SaveCell(IN cellid_t cellId, IN int32_t size, IN uint16_t type, OUT char* &cellPtr, OUT int32_t &entryIndex)
    {
        return _Lookup_LockEntry_Or_NotFound(
            cellId,
            [this, type, size, /* OUT */ &cellPtr, &entryIndex](const int32_t entry_index, const uint32_t bucket_index, const int32_t _)
        {
            TrinityErrorCode eResult = TrinityErrorCode::E_SUCCESS;
            ReleaseBucketLock(bucket_index);
            int32_t updated_cell_offset = CellEntries[entry_index].offset;

            /// add_memory_entry_flag prologue
            ENTER_ALLOCMEM_CELLENTRY_UPDATE_CRITICAL_SECTION();
            /// add_memory_entry_flag prologue

            if (size > CellSize(entry_index))
            {
                if (updated_cell_offset < 0)
                    eResult = memory_trunk->ExpandLargeObject(-updated_cell_offset, CellSize(entry_index), size);
                else
                {
                    eResult = memory_trunk->AddMemoryCell(size, entry_index, OUT updated_cell_offset);
                    if (eResult == TrinityErrorCode::E_SUCCESS) { MarkTrunkDirty(); }
                }
            }

            /////////////////////////

            if (eResult == TrinityErrorCode::E_SUCCESS)
            {
                CellEntry entry = { updated_cell_offset, size };
                std::atomic<int64_t> * CellEntryAtomicPtr = (std::atomic<int64_t>*) CellEntries;
                (CellEntryAtomicPtr + entry_index)->store(entry.location);

                if (CellTypeEnabled)
                    MTEntries[entry_index].CellType = type;
                // output
                if (updated_cell_offset < 0)
                    cellPtr = memory_trunk->LOPtrs[-updated_cell_offset];
                else
                    cellPtr = memory_trunk->trunkPtr + updated_cell_offset;
                entryIndex = entry_index;
            }

            /// add_memory_entry_flag epilogue
            LEAVE_ALLOCMEM_CELLENTRY_UPDATE_CRITICAL_SECTION();
            /// add_memory_entry_flag epilogue

            return eResult;
        },
            [this, cellId, type, size, /* OUT */ &cellPtr, &entryIndex](const uint32_t bucket_index) 
        {
            // Add new cell
            int32_t free_entry = FindFreeEntry();
            TrinityErrorCode eResult = TryGetEntryLock(free_entry);
            assert(eResult == TrinityErrorCode::E_SUCCESS);

            MTEntries[free_entry].Key = cellId;
            MTEntries[free_entry].NextEntry = Buckets[bucket_index];
            Buckets[bucket_index] = free_entry;
            ReleaseBucketLock(bucket_index);

            /// add_memory_entry_flag prologue
            ENTER_ALLOCMEM_CELLENTRY_UPDATE_CRITICAL_SECTION();
            /// add_memory_entry_flag prologue

            int32_t cell_offset;
            eResult = memory_trunk->AddMemoryCell(size, free_entry, OUT cell_offset);

            if (eResult == TrinityErrorCode::E_SUCCESS)
            {
                CellEntry entry = { cell_offset, size };
                std::atomic<int64_t> * CellEntryAtomicPtr = (std::atomic<int64_t>*) CellEntries;
                (CellEntryAtomicPtr + free_entry)->store(entry.location);

                if (CellTypeEnabled)
                    MTEntries[free_entry].CellType = type;
                // output
                if (cell_offset < 0)
                    cellPtr = memory_trunk->LOPtrs[-cell_offset];
                else
                    cellPtr = memory_trunk->trunkPtr + cell_offset;
                entryIndex = free_entry;
            }
            else
            {
                _discard_locked_free_entry(bucket_index, free_entry);
            }

            /// add_memory_entry_flag epilogue
            LEAVE_ALLOCMEM_CELLENTRY_UPDATE_CRITICAL_SECTION();
            /// add_memory_entry_flag epilogue

            return eResult;
            ////////////////////////////////////////////////////////////
        });
    }

    CELL_ACQUIRE_LOCK TrinityErrorCode MTHash::CGetLockedCellInfo4AddCell(IN cellid_t cellId, IN int32_t size, IN uint16_t type, OUT char* &cellPtr, OUT int32_t &entryIndex)
    {
        return _Lookup_NoLockEntry_Or_NotFound(
            cellId,
            [this](const int32_t entry_index, const uint32_t bucket_index, const int32_t _) 
        {
            ReleaseBucketLock(bucket_index);
            return TrinityErrorCode::E_DUPLICATED_CELL;
        },
            [this, cellId, type, size, /* OUT */ &cellPtr, &entryIndex](const uint32_t bucket_index) 
        {
            int32_t free_entry = FindFreeEntry();
            TrinityErrorCode eResult = TryGetEntryLock(free_entry);
            assert(eResult == TrinityErrorCode::E_SUCCESS);

            MTEntries[free_entry].Key = cellId;
            MTEntries[free_entry].NextEntry = Buckets[bucket_index];
            Buckets[bucket_index] = free_entry;
            ReleaseBucketLock(bucket_index);

            /// add_memory_entry_flag prologue
            ENTER_ALLOCMEM_CELLENTRY_UPDATE_CRITICAL_SECTION();
            /// add_memory_entry_flag prologue
            int32_t cell_offset;
            eResult = memory_trunk->AddMemoryCell(size, free_entry, OUT cell_offset);


            if (eResult == TrinityErrorCode::E_SUCCESS)
            {
                CellEntry entry = { cell_offset, size };
                std::atomic<int64_t> * CellEntryAtomicPtr = (std::atomic<int64_t>*) CellEntries;
                (CellEntryAtomicPtr + free_entry)->store(entry.location);

                if (CellTypeEnabled)
                    MTEntries[free_entry].CellType = type;
                // output
                if (cell_offset < 0)
                    cellPtr = memory_trunk->LOPtrs[-cell_offset];
                else
                    cellPtr = memory_trunk->trunkPtr + cell_offset;
                entryIndex = free_entry;
            }
            else
            {
                _discard_locked_free_entry(bucket_index, free_entry);
            }
            /// add_memory_entry_flag epilogue
            LEAVE_ALLOCMEM_CELLENTRY_UPDATE_CRITICAL_SECTION();
            /// add_memory_entry_flag epilogue

            return eResult;
        });
    }

    CELL_ACQUIRE_LOCK TrinityErrorCode MTHash::CGetLockedCellInfo4UpdateCell(IN cellid_t cellId, IN int32_t size, OUT char* &cellPtr, OUT int32_t &entryIndex)
    {
        return _Lookup_LockEntry_Or_NotFound(
            cellId,
            [this, size, /* OUT params */ &cellPtr, &entryIndex](const int32_t entry_index, const uint32_t bucket_index, const int32_t _) 
        {
            ReleaseBucketLock(bucket_index);
            int32_t updated_cell_offset = CellEntries[entry_index].offset;
            TrinityErrorCode eResult = TrinityErrorCode::E_SUCCESS;

            /// add_memory_entry_flag prologue
            ENTER_ALLOCMEM_CELLENTRY_UPDATE_CRITICAL_SECTION();
            /// add_memory_entry_flag prologue
            if (size > CellSize(entry_index))
            {
                if (updated_cell_offset < 0)
                    eResult = memory_trunk->ExpandLargeObject(-updated_cell_offset, CellSize(entry_index), size);
                else
                {
                    eResult = memory_trunk->AddMemoryCell(size, entry_index, OUT updated_cell_offset);
                    if (eResult == TrinityErrorCode::E_SUCCESS) { MarkTrunkDirty(); }
                }
            }

            if (eResult == TrinityErrorCode::E_SUCCESS)
            {
                CellEntry entry = { updated_cell_offset, size };
                std::atomic<int64_t> * CellEntryAtomicPtr = (std::atomic<int64_t>*) CellEntries;
                (CellEntryAtomicPtr + entry_index)->store(entry.location);

                // output
                if (updated_cell_offset < 0)
                    cellPtr = memory_trunk->LOPtrs[-updated_cell_offset];
                else
                    cellPtr = memory_trunk->trunkPtr + updated_cell_offset;
                entryIndex = entry_index;
            }
            /// add_memory_entry_flag epilogue
            LEAVE_ALLOCMEM_CELLENTRY_UPDATE_CRITICAL_SECTION();
            /// add_memory_entry_flag epilogue

            return eResult;

        },
            [this](const uint32_t bucket_index) 
        {
            ReleaseBucketLock(bucket_index);
            return TrinityErrorCode::E_CELL_NOT_FOUND;
        });
    }

    CELL_ACQUIRE_LOCK TrinityErrorCode MTHash::CGetLockedCellInfo4LoadCell(IN cellid_t cellId, OUT int32_t &size, OUT char* &cellPtr, OUT int32_t &entryIndex)
    {
        return _Lookup_LockEntry_Or_NotFound(
            cellId,
            [this, /* OUT params */ &size, &cellPtr, &entryIndex](const int32_t entry_index, const uint32_t bucket_index, const int32_t _) 
        {
            ReleaseBucketLock(bucket_index);
            int32_t cell_offset = CellEntries[entry_index].offset;
            // output
            size = CellSize(entry_index);
            if (cell_offset < 0)
                cellPtr = memory_trunk->LOPtrs[-cell_offset];
            else
                cellPtr = memory_trunk->trunkPtr + cell_offset;
            entryIndex = entry_index;
            ///////////////////////////////

            return TrinityErrorCode::E_SUCCESS;
        },
            [this](uint32_t bucket_index) 
        {
            ReleaseBucketLock(bucket_index);
            return TrinityErrorCode::E_CELL_NOT_FOUND;
        });
    }

    CELL_ACQUIRE_LOCK TrinityErrorCode MTHash::CGetLockedCellInfo4AddOrUseCell(IN cellid_t cellId, IN OUT int32_t &size, IN uint16_t type, OUT char* &cellPtr, OUT int32_t &entryIndex)
    {
        return _Lookup_LockEntry_Or_NotFound(
            cellId,
            [this, type, /* OUT params */ &size, &cellPtr, &entryIndex](const int32_t entry_index, const uint32_t bucket_index, const int32_t _) 
        {
            ReleaseBucketLock(bucket_index);

            if (CellTypeEnabled && MTEntries[entry_index].CellType != type)
            {
                ReleaseEntryLock(entry_index);
                return TrinityErrorCode::E_WRONG_CELL_TYPE;
            }
            int32_t cell_offset = CellEntries[entry_index].offset;

            /* size is OUT */
            size = CellSize(entry_index);
            if (cell_offset < 0)
                cellPtr = memory_trunk->LOPtrs[-cell_offset];
            else
                cellPtr = memory_trunk->trunkPtr + cell_offset;
            entryIndex = entry_index;

            return TrinityErrorCode::E_CELL_FOUND;
        },
            [this, cellId, type, /* OUT params */ &size, &cellPtr, &entryIndex](const uint32_t bucket_index) 
        {
#pragma region add new cell
            int32_t free_entry = FindFreeEntry();
            TrinityErrorCode eResult = TryGetEntryLock(free_entry);
            assert(eResult == TrinityErrorCode::E_SUCCESS);

            MTEntries[free_entry].Key = cellId;
            MTEntries[free_entry].NextEntry = Buckets[bucket_index];
            Buckets[bucket_index] = free_entry;
            ReleaseBucketLock(bucket_index);

            /// add_memory_entry_flag prologue
            ENTER_ALLOCMEM_CELLENTRY_UPDATE_CRITICAL_SECTION();
            /// add_memory_entry_flag prologue
            int32_t cell_offset;
            eResult = memory_trunk->AddMemoryCell(size, free_entry, OUT cell_offset);

            if (eResult == TrinityErrorCode::E_SUCCESS)
            {
                CellEntry entry = { cell_offset, size };
                std::atomic<int64_t> * CellEntryAtomicPtr = (std::atomic<int64_t>*) CellEntries;
                (CellEntryAtomicPtr + free_entry)->store(entry.location);

                if (CellTypeEnabled)
                    MTEntries[free_entry].CellType = type;
                /* size is IN, cellPtr is OUT */
                if (cell_offset < 0)
                    cellPtr = memory_trunk->LOPtrs[-cell_offset];
                else
                    cellPtr = memory_trunk->trunkPtr + cell_offset;
                entryIndex = free_entry;
                // !Note, for AddOrUse, we return E_CELL_NOT_FOUND for 'add' case
                eResult = TrinityErrorCode::E_CELL_NOT_FOUND;
            }
            else
            {
                _discard_locked_free_entry(bucket_index, free_entry);
            }
            /// add_memory_entry_flag epilogue
            LEAVE_ALLOCMEM_CELLENTRY_UPDATE_CRITICAL_SECTION();
            /// add_memory_entry_flag epilogue

            return TrinityErrorCode::E_CELL_NOT_FOUND;
#pragma endregion
        });
    }
}
// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "MTHash.h"
#include "Storage/MemoryTrunk/MemoryTrunk.h"

namespace Storage
{
    CELL_ACQUIRE_LOCK TrinityErrorCode MTHash::CGetLockedCellInfo4CellAccessor(IN cellid_t cellId, OUT int32_t &cellSize, OUT uint16_t &type, OUT char* &cellPtr, OUT int32_t &entryIndex)
    {
        uint32_t bucket_index = GetBucketIndex(cellId);
        GetBucketLock(bucket_index);
        for (int32_t entry_index = Buckets[bucket_index]; entry_index >= 0; entry_index = MTEntries[entry_index].NextEntry)
        {
            if (MTEntries[entry_index].Key == cellId)
            {
                GetEntryLock(entry_index);
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
            }
        }
        ReleaseBucketLock(bucket_index);
        return TrinityErrorCode::E_CELL_NOT_FOUND;
    }

    CELL_ACQUIRE_LOCK TrinityErrorCode MTHash::CGetLockedCellInfo4SaveCell(IN cellid_t cellId, IN int32_t size, IN uint16_t type, OUT char* &cellPtr, OUT int32_t &entryIndex)
    {
        uint32_t bucket_index = GetBucketIndex(cellId);
        TrinityErrorCode eResult = TrinityErrorCode::E_SUCCESS;
        GetBucketLock(bucket_index);
        for (int32_t entry_index = Buckets[bucket_index]; entry_index >= 0; entry_index = MTEntries[entry_index].NextEntry)
        {
            if (MTEntries[entry_index].Key == cellId)
            {
                GetEntryLock(entry_index);
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
                        eResult = memory_trunk->AddMemoryCell(size, entry_index, updated_cell_offset);
                        if (eResult == TrinityErrorCode::E_SUCCESS) { MarkTrunkDirty(); }
                    }
                }

                // output
                if (updated_cell_offset < 0)
                    cellPtr = memory_trunk->LOPtrs[-updated_cell_offset];
                else
                    cellPtr = memory_trunk->trunkPtr + updated_cell_offset;
                entryIndex = entry_index;
                /////////////////////////

                CellEntry entry ={ updated_cell_offset, size };
                std::atomic<int64_t> * CellEntryAtomicPtr = (std::atomic<int64_t>*) CellEntries;

                if (eResult == TrinityErrorCode::E_SUCCESS)
                {
                    (CellEntryAtomicPtr + entry_index)->store(entry.location);
                    if (CellTypeEnabled)
                        MTEntries[entry_index].CellType = type;
                }

                /// add_memory_entry_flag epilogue
                LEAVE_ALLOCMEM_CELLENTRY_UPDATE_CRITICAL_SECTION();
                /// add_memory_entry_flag epilogue

                return eResult;
            }
            ///////////////////////////////////////////////////
        }

        // Add new cell
        int32_t free_entry = FindFreeEntry();
        GetEntryLock(free_entry);

        MTEntries[free_entry].Key = cellId;
        MTEntries[free_entry].NextEntry = Buckets[bucket_index];
        Buckets[bucket_index] = free_entry;
        ReleaseBucketLock(bucket_index);

        /// add_memory_entry_flag prologue
        ENTER_ALLOCMEM_CELLENTRY_UPDATE_CRITICAL_SECTION();
        /// add_memory_entry_flag prologue

        int32_t cell_offset;
        eResult = memory_trunk->AddMemoryCell(size, free_entry, cell_offset);

        // output
        if (cell_offset < 0)
            cellPtr = memory_trunk->LOPtrs[-cell_offset];
        else
            cellPtr = memory_trunk->trunkPtr + cell_offset;
        entryIndex = free_entry;
        /////////////////////////////

        CellEntry entry ={ cell_offset, size };
        std::atomic<int64_t> * CellEntryAtomicPtr = (std::atomic<int64_t>*) CellEntries;

        if (eResult == TrinityErrorCode::E_SUCCESS)
        {
            (CellEntryAtomicPtr + free_entry)->store(entry.location);
            if (CellTypeEnabled)
                MTEntries[free_entry].CellType = type;
        }
        else
        {
            FreeEntry(free_entry);
            ReleaseEntryLock(free_entry);
        }

        /// add_memory_entry_flag epilogue
        LEAVE_ALLOCMEM_CELLENTRY_UPDATE_CRITICAL_SECTION();
        /// add_memory_entry_flag epilogue

        return eResult;
        ////////////////////////////////////////////////////////////
    }

    CELL_ACQUIRE_LOCK TrinityErrorCode MTHash::CGetLockedCellInfo4AddCell(IN cellid_t cellId, IN int32_t size, IN uint16_t type, OUT char* &cellPtr, OUT int32_t &entryIndex)
    {
        uint32_t bucket_index = GetBucketIndex(cellId);
        TrinityErrorCode eResult = TrinityErrorCode::E_SUCCESS;
        GetBucketLock(bucket_index);
        for (int32_t entry_index = Buckets[bucket_index]; entry_index >= 0; entry_index = MTEntries[entry_index].NextEntry)
        {
            if (MTEntries[entry_index].Key == cellId)
            {
                ReleaseBucketLock(bucket_index);
                return TrinityErrorCode::E_DUPLICATED_CELL;
            }
        }

        int32_t free_entry = FindFreeEntry();
        GetEntryLock(free_entry);

        MTEntries[free_entry].Key = cellId;
        MTEntries[free_entry].NextEntry = Buckets[bucket_index];
        Buckets[bucket_index] = free_entry;
        ReleaseBucketLock(bucket_index);

        /// add_memory_entry_flag prologue
        ENTER_ALLOCMEM_CELLENTRY_UPDATE_CRITICAL_SECTION();
        /// add_memory_entry_flag prologue
        int32_t cell_offset;
        eResult = memory_trunk->AddMemoryCell(size, free_entry, cell_offset);

        // output
        if (cell_offset < 0)
            cellPtr = memory_trunk->LOPtrs[-cell_offset];
        else
            cellPtr = memory_trunk->trunkPtr + cell_offset;
        entryIndex = free_entry;
        //////////////////////////

        CellEntry entry ={ cell_offset, size };
        std::atomic<int64_t> * CellEntryAtomicPtr = (std::atomic<int64_t>*) CellEntries;

        if (eResult == TrinityErrorCode::E_SUCCESS)
        {
            (CellEntryAtomicPtr + free_entry)->store(entry.location);
            if (CellTypeEnabled)
                MTEntries[free_entry].CellType = type;
        }
        else
        {
            FreeEntry(free_entry);
            ReleaseEntryLock(free_entry);
        }
        /// add_memory_entry_flag epilogue
        LEAVE_ALLOCMEM_CELLENTRY_UPDATE_CRITICAL_SECTION();
        /// add_memory_entry_flag epilogue

        return eResult;
    }

    CELL_ACQUIRE_LOCK TrinityErrorCode MTHash::CGetLockedCellInfo4UpdateCell(IN cellid_t cellId, IN int32_t size, OUT char* &cellPtr, OUT int32_t &entryIndex)
    {
        uint32_t bucket_index = GetBucketIndex(cellId);
        GetBucketLock(bucket_index);
        TrinityErrorCode eResult = TrinityErrorCode::E_SUCCESS;
        for (int32_t entry_index = Buckets[bucket_index]; entry_index >= 0; entry_index = MTEntries[entry_index].NextEntry)
        {
            if (MTEntries[entry_index].Key == cellId)
            {
                GetEntryLock(entry_index);
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
                        eResult = memory_trunk->AddMemoryCell(size, entry_index, updated_cell_offset);
                        if (eResult == TrinityErrorCode::E_SUCCESS) { MarkTrunkDirty(); }
                    }
                }

                // output
                if (updated_cell_offset < 0)
                    cellPtr = memory_trunk->LOPtrs[-updated_cell_offset];
                else
                    cellPtr = memory_trunk->trunkPtr + updated_cell_offset;
                entryIndex = entry_index;
                ////////////////////////////

                CellEntry entry ={ updated_cell_offset, size };
                std::atomic<int64_t> * CellEntryAtomicPtr = (std::atomic<int64_t>*) CellEntries;
                if (eResult == TrinityErrorCode::E_SUCCESS)
                {
                    (CellEntryAtomicPtr + entry_index)->store(entry.location);
                }
                /// add_memory_entry_flag epilogue
                LEAVE_ALLOCMEM_CELLENTRY_UPDATE_CRITICAL_SECTION();
                /// add_memory_entry_flag epilogue

                return TrinityErrorCode::E_SUCCESS;
            }
        }
        ReleaseBucketLock(bucket_index);
        return TrinityErrorCode::E_CELL_NOT_FOUND;
    }

    CELL_ACQUIRE_LOCK TrinityErrorCode MTHash::CGetLockedCellInfo4LoadCell(IN cellid_t cellId, OUT int32_t &size, OUT char* &cellPtr, OUT int32_t &entryIndex)
    {
        uint32_t bucket_index = GetBucketIndex(cellId);
        GetBucketLock(bucket_index);
        for (int32_t entry_index = Buckets[bucket_index]; entry_index >= 0; entry_index = MTEntries[entry_index].NextEntry)
        {
            if (MTEntries[entry_index].Key == cellId)
            {
                GetEntryLock(entry_index);
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
            }
        }
        ReleaseBucketLock(bucket_index);
        return TrinityErrorCode::E_CELL_NOT_FOUND;
    }

    CELL_ACQUIRE_LOCK TrinityErrorCode MTHash::CGetLockedCellInfo4AddOrUseCell(IN cellid_t cellId, IN OUT int32_t &size, IN uint16_t type, OUT char* &cellPtr, OUT int32_t &entryIndex)
    {
        uint32_t bucket_index = GetBucketIndex(cellId);
        TrinityErrorCode eResult = TrinityErrorCode::E_SUCCESS;
        GetBucketLock(bucket_index);
        for (int32_t entry_index = Buckets[bucket_index]; entry_index >= 0; entry_index = MTEntries[entry_index].NextEntry)
        {
#pragma region existing cell found
            if (MTEntries[entry_index].Key == cellId)
            {
                GetEntryLock(entry_index);
                ReleaseBucketLock(bucket_index);

                int32_t cell_offset = CellEntries[entry_index].offset;

#pragma region output
                /* size is OUT */
                size = CellSize(entry_index);
                if (cell_offset < 0)
                    cellPtr = memory_trunk->LOPtrs[-cell_offset];
                else
                    cellPtr = memory_trunk->trunkPtr + cell_offset;
                entryIndex = entry_index;
#pragma endregion

                if (CellTypeEnabled && MTEntries[entry_index].CellType != type)
                {
                    ReleaseEntryLock(entry_index);
                    return TrinityErrorCode::E_WRONG_CELL_TYPE;
                }

                return TrinityErrorCode::E_CELL_FOUND;
            }
#pragma endregion
        }

#pragma region add new cell
        int32_t free_entry = FindFreeEntry();
        GetEntryLock(free_entry);

        MTEntries[free_entry].Key = cellId;
        MTEntries[free_entry].NextEntry = Buckets[bucket_index];
        Buckets[bucket_index] = free_entry;
        ReleaseBucketLock(bucket_index);

        /// add_memory_entry_flag prologue
        ENTER_ALLOCMEM_CELLENTRY_UPDATE_CRITICAL_SECTION();
        /// add_memory_entry_flag prologue
        int32_t cell_offset;
        eResult = memory_trunk->AddMemoryCell(size, free_entry, cell_offset);

#pragma region output
        /* size is IN */
        if (cell_offset < 0)
            cellPtr = memory_trunk->LOPtrs[-cell_offset];
        else
            cellPtr = memory_trunk->trunkPtr + cell_offset;
        entryIndex = free_entry;
#pragma endregion

        CellEntry entry ={ cell_offset, size };
        std::atomic<int64_t> * CellEntryAtomicPtr = (std::atomic<int64_t>*) CellEntries;

        if (eResult == TrinityErrorCode::E_SUCCESS)
        {
            (CellEntryAtomicPtr + free_entry)->store(entry.location);
            if (CellTypeEnabled)
                MTEntries[free_entry].CellType = type;
            // !Note, for AddOrUse, we return E_CELL_NOT_FOUND for 'add' case
            eResult = TrinityErrorCode::E_CELL_NOT_FOUND;
        }
        else
        {
            FreeEntry(free_entry);
            ReleaseEntryLock(free_entry);
        }
        /// add_memory_entry_flag epilogue
        LEAVE_ALLOCMEM_CELLENTRY_UPDATE_CRITICAL_SECTION();
        /// add_memory_entry_flag epilogue

        return eResult;
#pragma endregion
    }
}
// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "TrinityCommon.h"
#include "Storage/MemoryTrunk/MemoryTrunk.h"

namespace Storage
{
    using namespace Trinity::Diagnostics;

    TrinityErrorCode MemoryTrunk::AddMemoryCell(int32_t cell_length, int32_t cellEntryIndex, OUT int32_t& cell_offset)
    {
        cell_offset = 0;

        if (cell_length < 0)
        {
            return TrinityErrorCode::E_INVALID_ARGUMENTS;
        }

        if (cell_length >= TrinityConfig::LargeObjectThreshold())
        {
            char* cell_ptr = (char*)AllocateLargeObject(cell_length);
            if (cell_ptr == NULL) return TrinityErrorCode::E_NOMEM;
            lo_lock.lock();
            if (LOIndex >= LOCapacity)
            {
                ResizeLOContainer();
            }

            LOPtrs[LOIndex] = cell_ptr;
            LOPreservedSizeArray[LOIndex] = 0;
            cell_offset = -LOIndex;
            LOIndex++;
            LOCount++;
            lo_lock.unlock();
        }
        else
        {
            char* cell_ptr = CellAlloc((uint32_t)cell_length, cellEntryIndex);
            if (cell_ptr == NULL) return TrinityErrorCode::E_NOMEM;
            cell_offset = (int32_t)(cell_ptr - trunkPtr);
        }

        return TrinityErrorCode::E_SUCCESS;
    }

    TrinityErrorCode MemoryTrunk::ExpandLargeObject(int32_t lo_index, int32_t original_size, int32_t new_size)
    {
        if (new_size < 0) return TrinityErrorCode::E_INVALID_ARGUMENTS;

        if ((new_size - original_size) <= LOPreservedSizeArray[lo_index])
        {
            LOPreservedSizeArray[lo_index] -= (new_size - original_size);
            return TrinityErrorCode::E_SUCCESS;
        }

        int32_t roundup_newsize = Memory::RoundUpToPage(new_size);
        //Reserve 1MB more every time
        if (!Memory::ExpandMemoryRegion(LOPtrs[lo_index], (uint64_t)Memory::RoundUpToPage(original_size), (uint64_t)(roundup_newsize + TrinityConfig::LOReservationSize)))
        {
            Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "Memory Trunk {0}: Cannot expand large object. \n MemoryTrunk: ExpandLargeObject : Out of memory", TrunkId);
            return TrinityErrorCode::E_NOMEM;
        }
        LOPreservedSizeArray[lo_index] = (roundup_newsize - new_size) + TrinityConfig::LOReservationSize; 
        return TrinityErrorCode::E_SUCCESS;
    }

    TrinityErrorCode MemoryTrunk::ShrinkLargeObject(int32_t lo_index, int32_t original_size, int32_t new_size)
    {
        uint32_t roundedup_currentsize = Memory::RoundUpToPage(original_size);
        uint32_t roundedup_newsize = Memory::RoundUpToPage(new_size);

        LOPreservedSizeArray[lo_index] += (original_size - new_size);
        if (LOPreservedSizeArray[lo_index] > (TrinityConfig::LOReservationSize << 1))
        {
            Memory::ShrinkMemoryRegion(LOPtrs[lo_index], roundedup_currentsize, roundedup_newsize);
            LOPreservedSizeArray[lo_index] = Memory::RoundUpToPage(new_size) - new_size;
        }

        return TrinityErrorCode::E_SUCCESS;
    }
}
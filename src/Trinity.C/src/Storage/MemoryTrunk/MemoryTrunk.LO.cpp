// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "TrinityCommon.h"
#include "Storage/MemoryTrunk/MemoryTrunk.h"

namespace Storage
{
    void MemoryTrunk::InitLOContainer()
    {
        LOIndex = 1; //! Note here
        LOCount = 0;
        LOCapacity = TrinityConfig::MaxLargeObjectCount;

        LOPtrs = new char*[LOCapacity];
        LOPreservedSizeArray = new int32_t[LOCapacity];
    }

    void MemoryTrunk::EnsureLOCapacity(int32_t count)
    {
        LOCapacity = TrinityConfig::MaxLargeObjectCount;
        while (count >= LOCapacity)
            LOCapacity += TrinityConfig::MaxLargeObjectCount;
    }

#pragma warning(push)
#pragma warning(disable:6385)
#pragma warning(disable:6386)
    void MemoryTrunk::ShrinkLOContainer()
    {
        if (LOCount > 0 && LOIndex > (LOCount + 1))
        {
            LOCapacity = TrinityConfig::MaxLargeObjectCount;
            while (LOCount >= LOCapacity)
                LOCapacity += TrinityConfig::MaxLargeObjectCount;

            char** new_LOPtrs = new char*[LOCapacity];
            int32_t* new_LOPreservedSizeArray = new int[LOCapacity];
            int32_t new_lo_index = 1;

            int32_t entry_count = hashtable->ExtendedInfo->NonEmptyEntryCount;
            for (int32_t i = 0; i < entry_count; i++)
            {
                if (hashtable->CellEntries[i].location != -1)
                {
                    if (hashtable->CellEntries[i].offset < 0)
                    {
                        int32_t index = -hashtable->CellEntries[i].offset;
                        new_LOPtrs[new_lo_index] = LOPtrs[index];
                        new_LOPreservedSizeArray[new_lo_index] = LOPreservedSizeArray[index];
                        int32_t current_index = new_lo_index++;
                        hashtable->CellEntries[i].offset = -current_index;
                    }
                }
            }

            LOPtrs = new_LOPtrs;
            LOPreservedSizeArray = new_LOPreservedSizeArray;
        }
    }

    void MemoryTrunk::ResizeLOContainer()
    {
        LOCapacity = LOCapacity + TrinityConfig::MaxLargeObjectCount;
        char** new_LOPtrs = new char*[LOCapacity];
        int* new_LOPreservedSizeArray = new int[LOCapacity];
        for (int32_t i = 1; i < LOIndex; i++)
        {
            new_LOPtrs[i] = LOPtrs[i];
            new_LOPreservedSizeArray[i] = LOPreservedSizeArray[i];
        }

        LOPtrs = new_LOPtrs;
        LOPreservedSizeArray = new_LOPreservedSizeArray;
    }
#pragma warning(pop)

    void MemoryTrunk::DisposeLargeObjects()
    {
        for (int32_t i = 1; i < LOIndex; i++)
        {
            Memory::FreeMemoryRegion(LOPtrs[i], LOPreservedSizeArray[i]);
            LOPtrs[i] = nullptr;
            LOPreservedSizeArray[i] = 0;
        }

        if (LOPtrs != nullptr)
        {
            delete[] LOPtrs;
            LOPtrs = nullptr;
        }
        if (LOPreservedSizeArray != nullptr)
        {
            delete[] LOPreservedSizeArray;
            LOPreservedSizeArray = nullptr;
        }

        LOCount = 0;
        LOIndex = 1;
    }

    void MemoryTrunk::DisposeLargeObject(int32_t lo_index)
    {
        Memory::FreeMemoryRegion(LOPtrs[lo_index], LOPreservedSizeArray[lo_index]);
        LOPtrs[lo_index] = nullptr;
        LOPreservedSizeArray[lo_index] = 0;
        LOCount--;
    }
}

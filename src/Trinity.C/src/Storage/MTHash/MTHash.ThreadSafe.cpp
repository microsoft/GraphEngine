// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "Storage/MTHash/MTHash.h"
#include "Storage/MemoryTrunk/MemoryTrunk.h"

namespace Storage
{
    void MTHash::FreeEntry(int32_t entry_index)
    {
        FreeListLock.lock();
        MTEntries[entry_index].NextEntry = FreeEntryList;
        FreeEntryList = entry_index;
        FreeEntryCount.fetch_add(1, std::memory_order_relaxed);
        FreeListLock.unlock();
    }

    int32_t MTHash::FindFreeEntry()
    {
        int32_t free_entry = -1;
        FreeListLock.lock();

        if (FreeEntryList != -1)
        {
            free_entry = FreeEntryList;
            FreeEntryList = MTEntries[FreeEntryList].NextEntry;
            FreeEntryCount.fetch_sub(1, std::memory_order_relaxed);
            FreeListLock.unlock();
        }
        else
        {
            FreeListLock.unlock();
            free_entry = NonEmptyEntryCount.fetch_add(1, std::memory_order_relaxed);
            while (free_entry >= (int32_t)EntryCount.load())
            {
                Expand(false);
            }
        }

        return free_entry;
    }

    int32_t MTHash::Count()
    {
        EntryAllocLock.lock();
        int32_t _count = (NonEmptyEntryCount - FreeEntryCount);
        EntryAllocLock.unlock();
        return _count;
    }
}
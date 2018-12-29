// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "Storage/MTHash/MTHash.h"
#include "Storage/MemoryTrunk/MemoryTrunk.h"

namespace Storage
{
    void MTHash::Expand(bool force)
    {
        EntryAllocLock->lock();
        uint32_t CurrentEntryCount = ExtendedInfo->EntryCount.load();

        if ((!force) && ((uint32_t)ExtendedInfo->NonEmptyEntryCount.load() < CurrentEntryCount))
        {
            EntryAllocLock->unlock();
            return;
        }

        if ((CurrentEntryCount + UInt32_Contants::GuardedEntryCount) >= TrinityConfig::ReserveEntriesPerMTHash())
        {
			//XXX should not exit here
            Trinity::Diagnostics::FatalError(1, "Memory Trunk {0} is out of Memory::", memory_trunk->TrunkId);
        }

        uint32_t expanded_entry_count = CurrentEntryCount + UInt32_Contants::EntryExpandUnit;
        if ((expanded_entry_count + UInt32_Contants::GuardedEntryCount) >= TrinityConfig::ReserveEntriesPerMTHash())
        {
            expanded_entry_count = (uint32_t) (TrinityConfig::ReserveEntriesPerMTHash() - UInt32_Contants::GuardedEntryCount);
        }

        uint64_t size_expanded = expanded_entry_count - CurrentEntryCount;
		Trinity::Diagnostics::WriteLine(LogLevel::Verbose, "MemoryTrunk {0}: Expand: {1}->{2}", memory_trunk->TrunkId, CurrentEntryCount, expanded_entry_count);

        Memory::ExpandMemoryFromCurrentPosition((char*)CellEntries + ((CurrentEntryCount + UInt32_Contants::GuardedEntryCount) * sizeof(CellEntries[0])), size_expanded * sizeof(CellEntries[0]));
        memset((char*)CellEntries + ((CurrentEntryCount + UInt32_Contants::GuardedEntryCount) * sizeof(CellEntries)), -1, size_expanded * sizeof(CellEntries[0]));

        Memory::ExpandMemoryFromCurrentPosition((char*)MTEntries + ((CurrentEntryCount + UInt32_Contants::GuardedEntryCount) * sizeof(MTEntries[0])), size_expanded * sizeof(MTEntries[0]));

        ExtendedInfo->EntryCount.store(expanded_entry_count);
        EntryAllocLock->unlock();
    }
}
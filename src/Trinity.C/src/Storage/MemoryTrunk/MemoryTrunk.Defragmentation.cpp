// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "TrinityCommon.h"
#include "Storage/MemoryTrunk/MemoryTrunk.h"
#include "BackgroundThread/BackgroundThread.h"
#include "Storage/LocalStorage/GCTask.h"

namespace Storage
{
    namespace LocalMemoryStorage
    {
        extern int32_t* dirty_flags;
    }
    /**
     *  @param  calledByGCThread    True if called by GC thread, otherwise, it should be from SaveStorage,
     *                              where all the locks are taken and thus should not be taken again.
     */
    void MemoryTrunk::Defragment(bool calledByGCThread)
    {
        if (Storage::GCTask::GetDefragmentationPaused())
            return;

        // No memory expansion/contraction occurred since last GC
        if (head.append_head == LastAppendHead && !LocalMemoryStorage::dirty_flags[TrunkId])
            return;

        int free_entry_cnt = hashtable->ExtendedInfo->FreeEntryCount.load();
        int non_empty_cnt  = hashtable->ExtendedInfo->NonEmptyEntryCount.load();

        if (non_empty_cnt == 0 && free_entry_cnt == 0) return;

        Trinity::Diagnostics::WriteLine(LogLevel::Verbose, 
                                        "MemoryTrunk: Defragmenting memory trunk #{0}, FreeEntryCount={1}, NonEmptyEntryCount={2}.", 
                                        TrunkId, 
                                        free_entry_cnt, 
                                        non_empty_cnt);

        if (free_entry_cnt != 0 && free_entry_cnt == non_empty_cnt)
        {
            EmptyCommittedMemory(/*takeMTHashLock:*/calledByGCThread);
            return;
        }
        int32_t addressTableLength;
        bool AddressTableOneRegion;
        AddressTableEntry* addressTable = DumpAddressTable(OUT addressTableLength, OUT AddressTableOneRegion);
        if (addressTable == nullptr)
            return;

        if (!defrag_lock->trylock())
        {
            delete[] addressTable;
            return;
        }

        if (!IsAddressTableValid.load())
        {
            defrag_lock->unlock();
            delete[] addressTable;
            return;
        }

        //! Holding read lock during defragmentation to prevent modifying the MTHash table

        AddressTableEndPoint endpoint = SortAddressTable(addressTable, addressTableLength, AddressTableOneRegion);
        if (addressTableLength >= 2)
        {
            if (AddressTableOneRegion)
            {
                DefragmentOneRegion(addressTable, addressTableLength, endpoint);
            }
            else
            {
                DefragmentTwoRegion(addressTable, addressTableLength, endpoint);
            }
        }

        defrag_lock->unlock();

        if (addressTable != nullptr)
            delete[] addressTable;

        LastAppendHead.store(head.append_head);
        LocalMemoryStorage::dirty_flags[TrunkId] = 0;
    }
}

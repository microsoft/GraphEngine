// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "TrinityCommon.h"
#include "Storage/MemoryTrunk/MemoryTrunk.h"
#include "Utility/HashHelper.h"

#include <algorithm>

namespace Storage
{
    AddressTableEntry* MemoryTrunk::DumpAddressTable(int32_t& addressTableLength, bool& AddressTableOneRegion)
    {
        IsAddressTableValid.store(true);
        CellEntry* entry_shadow;
        int32_t entry_count;
        HeadGroup HeadGroupShadow;
        uint32_t CommittedTailShadow;

        if (split_lock->trylock())
        {
            HeadGroupShadow.head_group.store(head.head_group.load(std::memory_order_relaxed));
            CommittedTailShadow = committed_tail;

            entry_count = (hashtable->ExtendedInfo->NonEmptyEntryCount.load());

            /* **********************************************************************************************
             * After this snapshot point, existing cells may be deleted, updated and new cells may be added *
             * **********************************************************************************************/

            std::atomic<int64_t> * CellEntryAtomicPtr = (std::atomic<int64_t>*) (hashtable->CellEntries);
            /// add_memory_entry_flag prologue
            add_memory_entry_flag.fetch_or(0x80000000);
            int32_t retry = 64;
            while ((add_memory_entry_flag.load() & 0x7FFFFFFF) > 0)
            {
                if (--retry == 0)
                {
                    add_memory_entry_flag.fetch_and(0x7FFFFFFF);
                    split_lock->unlock();
                    return nullptr;
                }
            }

            // alternative prologue: less aggressive
            //if ((add_memory_entry_flag.load() & 0x7FFFFFFF) > 0)
            //{
            //    add_memory_entry_flag.fetch_and(0x7FFFFFFF);
            //    split_lock.unlock();
            //    return nullptr;
            //}
            /// add memory_entry_flag prologue

            entry_shadow = new CellEntry[entry_count];
            for (int32_t i = 0; i < entry_count; i++)
            {
                entry_shadow[i].location = CellEntryAtomicPtr[i].load();
            }

            /// add_memory_entry_flag epilogue
            add_memory_entry_flag.fetch_and(0x7FFFFFFF);
            /// add_memory_entry_flag epilogue

            split_lock->unlock();
        }
        else
            return nullptr;

        if (entry_count == 0)
        {
            if (entry_shadow != nullptr)
            {
                delete[] entry_shadow;
            }
            return nullptr;
        }

        AddressTableOneRegion = (CommittedTailShadow < HeadGroupShadow.committed_head) ||
            (CommittedTailShadow == HeadGroupShadow.committed_head && CommittedTailShadow == 0);

        AddressTableEntry* addressTable = new AddressTableEntry[entry_count];
        addressTableLength = 0;
        for (int32_t i = 0; i < entry_count; ++i)
        {
            if (entry_shadow[i].size > 0 && entry_shadow[i].offset >= 0 && CellBoundaryCheck(entry_shadow[i].offset, AddressTableOneRegion, HeadGroupShadow, CommittedTailShadow))
            {
                addressTable[addressTableLength].index = i;
                addressTable[addressTableLength].offset = (uint32_t) entry_shadow[i].offset;
                ++addressTableLength;
            }
        }

        delete[] entry_shadow;
        return addressTable;
    }

    bool MemoryTrunk::CellBoundaryCheck(int32_t cell_offset, bool AddressTableOneRegion, HeadGroup& HeadGroupShadow, uint32_t& CommittedTailShadow)
    {
        uint32_t offset = (uint32_t) cell_offset;
        if (AddressTableOneRegion)
        {
            if (offset >= CommittedTailShadow && offset < HeadGroupShadow.append_head)
                return true;
            else
                return false;
        }
        else
        {
            if (offset < CommittedTailShadow && offset >= HeadGroupShadow.append_head)
                return false;
            return true;
        }
    }

    void MemoryTrunk::EmptyCommittedMemory(bool takeMTHashLock)
    {
        TrinityErrorCode err = TrinityErrorCode::E_SUCCESS;
        Trinity::Diagnostics::WriteLine(LogLevel::Verbose, "MemoryTrunk: EmptyCommittedMemory on memory trunk #{0}.", TrunkId);

        if (takeMTHashLock)
            err = hashtable->Lock();

        /*! The assertion is true because:
         *  1. When this function is called by GC thread,
         *     it cannot hold any cell locks.
         *  2. When this function is called by a normal thread,
         *     if it holds a lock, it means that there's at least
         *     one cell remaining in the memory trunk, but this
         *     function is only called when there's nothing left
         *     in the trunk. Contradict.
         */
        assert(err == TrinityErrorCode::E_SUCCESS);

        HeadGroup HeadGroupShadow;
        uint32_t CommittedTailShadow;
        HeadGroupShadow.head_group.store(head.head_group.load(std::memory_order_relaxed));
        CommittedTailShadow = committed_tail;

        bool  AddressTableOneRegion = (CommittedTailShadow < HeadGroupShadow.committed_head) ||
            (CommittedTailShadow == HeadGroupShadow.committed_head && CommittedTailShadow == 0);

        if (CommittedTailShadow == 0 && HeadGroupShadow.committed_head == 0)
            goto exit;

        if (hashtable->ExtendedInfo->FreeEntryCount != 0 && hashtable->ExtendedInfo->FreeEntryCount == hashtable->ExtendedInfo->NonEmptyEntryCount) // Double checking
        {
            uint64_t mem_to_decommit = 0;
            if (AddressTableOneRegion)
            {
                mem_to_decommit = (uint64_t) ((HeadGroupShadow.committed_head - committed_tail) & Memory::PAGE_MASK_32);
                if (mem_to_decommit > 0 && committed_tail < TrunkLength)
                {
                    BufferedDecommitMemory(trunkPtr + committed_tail, mem_to_decommit);
                }
            }
            else
            {
                mem_to_decommit = (TrunkLength - committed_tail) & Memory::PAGE_MASK_32;

                if (mem_to_decommit > 0)
                {
                    BufferedDecommitMemory(trunkPtr + committed_tail, mem_to_decommit);
                }

                mem_to_decommit = (uint64_t) (HeadGroupShadow.committed_head & Memory::PAGE_MASK_32);

                if (mem_to_decommit > 0)
                {
                    BufferedDecommitMemory(trunkPtr, mem_to_decommit);
                }
            }
            committed_tail = 0;
            head.append_head = 0;
            head.committed_head = 0;
            for (int32_t i = 0; i < hashtable->ExtendedInfo->NonEmptyEntryCount; ++i)
            {
                hashtable->CellEntries[i].size = -1;
            }
        }

        exit:
        if (takeMTHashLock)
            hashtable->Unlock();
    }

    inline int32_t CompareAddressTableEntry(const void* a, const void* b)
    {
        if (((AddressTableEntry*) a)->offset < ((AddressTableEntry*) b)->offset) return -1;
        if (((AddressTableEntry*) a)->offset > ((AddressTableEntry*) b)->offset) return 1;
        return 0;
    }

    AddressTableEndPoint MemoryTrunk::SortAddressTable(AddressTableEntry* addressTable, int32_t addressTableLength, bool AddressTableOneRegion)
    {
        qsort(addressTable, addressTableLength, 8, CompareAddressTableEntry);

        AddressTableEndPoint endpoint;

        if (AddressTableOneRegion)//Easy case
        {
            endpoint.fwd_index = 0;
            endpoint.bwd_index = addressTableLength - 1;
        }
        else
        {
            if (addressTable[addressTableLength - 1].offset < committed_tail ||
                addressTable[0].offset >= committed_tail)
            {
                endpoint.fwd_index = 0;
                endpoint.bwd_index = addressTableLength - 1;
            }
            else
            {
                int32_t i = addressTableLength - 1;
                for (i = addressTableLength - 1; i >= 0;)
                {
                    if (addressTable[i].offset >= committed_tail)
                        i--;
                    else
                        break;
                }
                endpoint.bwd_index = i;
                endpoint.fwd_index = i + 1;
            }
        }
        return endpoint;
    }
}
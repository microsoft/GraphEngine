// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "TrinityCommon.h"
#include "Storage/MemoryTrunk/MemoryTrunk.h"
#include <io>
#include <diagnostics>

namespace Storage
{
    void MemoryTrunk::DisposeTrunkBuffer()
    {
        if (PhysicalMemoryPageLocking)
        {
            VirtualUnlock(trunkPtr, head.append_head);
        }
        Memory::DecommitMemory(trunkPtr, TrunkLength); //Decommit the whole region
        head.committed_head = 0;
        committed_tail = 0;
    }

    char* MemoryTrunk::CellAlloc(uint32_t cellSize, int32_t entryIndex)
    {
        char* cell_p = nullptr;
        hashtable->MTEntries[entryIndex].EntryLock += 2; //! Put myself to arena (the exclusion list of GetAllEntryLocksExceptArena in Reload)

        alloc_lock.lock();

        do
        {
            if (head.append_head + cellSize <= head.committed_head)
            {
                cell_p = trunkPtr + head.append_head;
                head.append_head += cellSize;
                hashtable->MTEntries[entryIndex].EntryLock -= 2; //! Release the exclusion flag
                alloc_lock.unlock();
                return cell_p;
            }

            if (CommittedMemoryExpand(cellSize))
            {
                continue;
            }
            alloc_lock.unlock();
            Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "CellAlloc: MemoryTrunk {0} is out of Memory.", TrunkId);
            return NULL;
        } while (true);
    }

    bool MemoryTrunk::CommittedMemoryExpand(uint32_t minimum_size)
    {
        /**
        * There are two major cases:
        * 1) committed_head >= committed_tail
        *      Committed region consists of one part
        * 2) committed_head < committed_tail
        *      Committed region consists of two parts
        * */

        uint32_t minimum_to_expand = ((head.append_head + minimum_size + Memory::PAGE_RANGE) & Memory::PAGE_MASK) - head.committed_head;
        uint32_t CurrentVMAllocUnit = TrinityConfig::VMAllocUnit < minimum_to_expand ? minimum_to_expand : TrinityConfig::VMAllocUnit;
        bool ret = true;
        uint32_t available_space = 0;

        uint32_t committed_tail_snapshot = committed_tail;

        bool two_region = (head.committed_head < committed_tail_snapshot) ||
            (head.committed_head == committed_tail_snapshot && committed_tail_snapshot != 0);

#pragma region Two Region
        if (two_region)
        {
            available_space = committed_tail_snapshot - head.committed_head;

            // large enough virtual memory between committed_head and committed_tail for allocating VMAllocUnit
            if (CurrentVMAllocUnit <= available_space)
            {
                ret = Memory::ExpandMemoryFromCurrentPosition(trunkPtr + head.committed_head, CurrentVMAllocUnit);
                head.committed_head += CurrentVMAllocUnit;
                goto SUCCESS_RETURN;
            }

            // Not large enough for VMAllocUnit, but enough for minimum_to_expand
            if (minimum_to_expand <= available_space)
            {
                ret = Memory::ExpandMemoryFromCurrentPosition(trunkPtr + head.committed_head, minimum_to_expand);
                head.committed_head += minimum_to_expand;
                goto SUCCESS_RETURN;
            }

            // Out of memory, try to reload later
            goto FAIL_RETURN;
        }
#pragma endregion

#pragma region One Region
        available_space = TrunkLength - head.committed_head;

        // There is enough virtual memory between committed_head to trunk_end for VMAllocUnit
        if (CurrentVMAllocUnit <= available_space)
        {
            ret = Memory::ExpandMemoryFromCurrentPosition(trunkPtr + head.committed_head, CurrentVMAllocUnit);
            head.committed_head += CurrentVMAllocUnit;
            goto SUCCESS_RETURN;
        }

        // Not enough for VMAllocUnit, but enough for minimum_to_expand
        if (minimum_to_expand <= available_space)
        {
            ret = Memory::ExpandMemoryFromCurrentPosition(trunkPtr + head.committed_head, minimum_to_expand);
            head.committed_head += minimum_to_expand;
            goto SUCCESS_RETURN;
        }

        // There is not enough virtual memory between committed_head to trunk_end
        //! Now check the space between trunk_ptr and committed_tail

        // Enough available memory for VMAllocUnit
        if (CurrentVMAllocUnit <= committed_tail) //available_space = committed_tail - 0
        {
            //First allocate the memory between committed_head to trunk_end
            uint32_t padding_size = available_space;
            Memory::ExpandMemoryFromCurrentPosition(trunkPtr + head.committed_head, padding_size);

            //Then allocate memory between trunk_ptr to committed_tail
            split_lock.lock();
            ret = Memory::ExpandMemoryFromCurrentPosition(trunkPtr, CurrentVMAllocUnit);
            head.append_head = 0; //warp back to head
            head.committed_head = CurrentVMAllocUnit;
            split_lock.unlock();
            goto SUCCESS_RETURN;
        }

        // Not enough for VMAllocUnit, but enough for minimum_to_expand
        if (minimum_to_expand <= committed_tail) //vailable_space = committed_tail - 0
        {
            //First allocate the memory between committed_head to trunk_end
            uint32_t padding_size = available_space;
            Memory::ExpandMemoryFromCurrentPosition(trunkPtr + head.committed_head, padding_size);

            //Then allocate memory between trunk_ptr to committed_tail
            split_lock.lock();
            ret = Memory::ExpandMemoryFromCurrentPosition(trunkPtr, minimum_to_expand);
            head.append_head = 0; //warp back to head
            head.committed_head = minimum_to_expand;
            split_lock.unlock();
            goto SUCCESS_RETURN;
        }

        // Out of memory
        goto FAIL_RETURN;

#pragma endregion

    SUCCESS_RETURN :
        if (ret == false)
        {
            Trinity::Diagnostics::FatalError("Trunk {0}: CommittedMemoryExpand failed.", TrunkId);
        }
        return ret;
    FAIL_RETURN:
        return Reload(minimum_size);
    }

    bool MemoryTrunk::Reload(uint32_t minimum_size)
    {
        if (hashtable->NonEmptyEntryCount == 0)
            return true;

        //! Use pending_flag to notify other threads a high-priority task is ongoing
        defrag_lock.lock(pending_flag);

        HeadGroup temp_head_group;

        hashtable->GetAllEntryLocksExceptArena();

        FlushDecommitBuffer(); //! Before the actual reload, decommit all buffered memory

        temp_head_group.append_head = (uint32_t)ReloadImpl(); // Update hashtable.CellEntries

        InfoLog(String("Memory trunk {0} reloaded."), TrunkId);

        if (temp_head_group.append_head == 0xFFFFFFFF) //Reload buffer allocation failed!
        {
            hashtable->ReleaseAllEntryLocksExceptArena();
            defrag_lock.unlock();

            Trinity::Diagnostics::FatalError("Trunk: {0}, run out of memory during trunk reloading, buffer allocation failed. \n MemoryTrunk: Reload: Out of memory", TrunkId);
        }

        uint32_t available_space = TrunkLength - temp_head_group.append_head;
        if (available_space < minimum_size)
        {
            hashtable->ReleaseAllEntryLocksExceptArena();
            defrag_lock.unlock();
            Trinity::Diagnostics::FatalError("Trunk: {0}, run out of memory during trunk reloading, available size: {1}, required size: {2}. \n MemoryTrunk: Reload: Out of memory", TrunkId, available_space, minimum_size);
        }

        temp_head_group.committed_head = (temp_head_group.append_head + Memory::PAGE_RANGE) & Memory::PAGE_MASK;
        committed_tail = 0;

        hashtable->ReleaseAllEntryLocksExceptArena();

        head.head_group.store(temp_head_group.head_group);
        //Console.WriteLine("Reloaded: append head: {0} committed_head: {1}", head.append_head, head.committed_head);
        IsAddressTableValid.store(false); //! Reloading will invalidate the address table
        defrag_lock.unlock();
        return true;
    }

    int32_t MemoryTrunk::ReloadImpl()
    {
        CellEntry* entries = hashtable->CellEntries;
        CellEntry* entriesEndPtr = entries + hashtable->NonEmptyEntryCount;

        char * buffer = (char*)malloc(TrunkLength);
        if (buffer == NULL)
            return -1;

        char * cp = buffer;

        for (; entries != entriesEndPtr; ++entries)
        {
            if ((entries->size != -1))
            {
                if (entries->offset >= 0) // Not large object
                {
                    entries->size &= 0xFFFFFF; // Drop all preserved space
                    memcpy(cp, ((char*)trunkPtr + entries->offset), entries->size);
                    entries->offset = (int32_t)(cp - buffer);
                    cp += entries->size;
                }
            }
        }

        if (cp == buffer)
        {
            free(buffer);
            return 0;
        }

        /*
        If the dwFreeType parameter is MEM_DECOMMIT.
        If lpAddress is the base address returned by VirtualAlloc and dwSize is 0 (zero),
        the function decommits the entire region that is allocated by VirtualAlloc.
        After that, the entire region is in the reserved state.
        */
#pragma warning(suppress: 6250)
        Memory::DecommitMemory(trunkPtr, TrunkLength);

        size_t append_head = cp - buffer;
        if (Memory::MemoryCommit(trunkPtr, append_head) == NULL)
        {
            free(buffer);
            return -1;
        }

        memcpy(trunkPtr, buffer, append_head);

        free(buffer);
        return (int32_t)(append_head);
    }

    char * MemoryTrunk::AllocateLargeObject(int32_t size)
    {
        return Memory::ReserveAlloc(MaxLargeObjectSize /* 1GB */, size);
    }
}
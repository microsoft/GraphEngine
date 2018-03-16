// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "TrinityCommon.h"
#include "Storage/MemoryTrunk/MemoryTrunk.h"
#include "Storage/LocalStorage/ThreadContext.h"
#include "Storage/MTHash/MTHash.h"
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

    ALLOC_THREAD_CTX char* MemoryTrunk::CellAlloc(cellid_t cellId, uint32_t cellSize)
    {
        //  !CellAlloc is the only path towards GetAllEntryLocksExceptArena --> ReadMemoryAllocationArena 
        char* cell_p         = nullptr;
		PTHREAD_CONTEXT pctx = nullptr; 

        if (!alloc_lock.trylock(100000))
        {
            //  !Note, we should setup the Tx so that the arena
            //  is aware that we are locking current cell.
			pctx = EnsureCurrentThreadContext();
            pctx->SetLockingCell(cellId);
            EnterMemoryAllocationArena(pctx);
            alloc_lock.lock();
        }

		while(true)
        {
            if (head.append_head + cellSize <= head.committed_head)
            {
                /**! Note, a special case here is that the trunk is
                 *   already full (all 2GB space are occupied), so that
                 *   append_head == committed_head == TrunkLength. If we
                 *   now save a cell with length 0, then the if-condition still holds but
                 *   we get a cell_ptr out of the range of the current trunk, which could 
                 *   make (cell_ptr - trunkPtr) negative (TrunkLength = INT_MAX + 1).
                 */

                cell_p = trunkPtr + (head.append_head % TrunkLength);
                head.append_head += cellSize;
                // This is the only path to ExitMemoryAllocationArena.
                // We hold alloc_lock, check if we entered arena
                // due to long blocking lock.
                if (pctx != nullptr)
                {
                    ExitMemoryAllocationArena(pctx);
                }
                //  !If it happens that pctx == nullptr, it means that
                //  we have not alerted the arena, and the memory trunk
                //  has sufficient space. A fast path without touching
                //  the thread context is thus achieved.
                alloc_lock.unlock();
                return cell_p;
            }

            // Not enough space. We have to do memory expansion.
            // Ensure that we are in the arena now.
            if (pctx == nullptr)
            {
				pctx = EnsureCurrentThreadContext();
                pctx->SetLockingCell(cellId);
                EnterMemoryAllocationArena(pctx);
            }

            if (CommittedMemoryExpand(cellSize))
            {
                continue;
            }
            alloc_lock.unlock();
            Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "CellAlloc: MemoryTrunk {0} is out of Memory.", TrunkId);
            return NULL;
        }
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
                if (ret) head.committed_head += CurrentVMAllocUnit;
                goto EXPAND_RETURN;
            }

            // Not large enough for VMAllocUnit, but enough for minimum_to_expand
            if (minimum_to_expand <= available_space)
            {
                ret = Memory::ExpandMemoryFromCurrentPosition(trunkPtr + head.committed_head, minimum_to_expand);
                if (ret) head.committed_head += minimum_to_expand;
                goto EXPAND_RETURN;
            }

            // Out of memory, try to reload later
            goto RELOAD_RETURN;
        }
#pragma endregion

#pragma region One Region
        available_space = TrunkLength - head.committed_head;

        // There is enough virtual memory between committed_head to trunk_end for VMAllocUnit
        if (CurrentVMAllocUnit <= available_space)
        {
            ret = Memory::ExpandMemoryFromCurrentPosition(trunkPtr + head.committed_head, CurrentVMAllocUnit);
            if (ret) head.committed_head += CurrentVMAllocUnit;
            goto EXPAND_RETURN;
        }

        // Not enough for VMAllocUnit, but enough for minimum_to_expand
        if (minimum_to_expand <= available_space)
        {
            ret = Memory::ExpandMemoryFromCurrentPosition(trunkPtr + head.committed_head, minimum_to_expand);
            if (ret) head.committed_head += minimum_to_expand;
            goto EXPAND_RETURN;
        }

        // There is not enough virtual memory between committed_head to trunk_end
        //! Now check the space between trunk_ptr and committed_tail

        // Enough available memory for VMAllocUnit
        if (CurrentVMAllocUnit <= committed_tail) //available_space = committed_tail - 0
        {
            //First allocate the memory between committed_head to trunk_end
            uint32_t padding_size = available_space;
            ret = Memory::ExpandMemoryFromCurrentPosition(trunkPtr + head.committed_head, padding_size);
            if (!ret) goto EXPAND_RETURN;

            //Then allocate memory between trunk_ptr to committed_tail
            split_lock.lock();
            ret = Memory::ExpandMemoryFromCurrentPosition(trunkPtr, CurrentVMAllocUnit);
            if (ret)
            {
                head.append_head = 0; //warp back to head
                head.committed_head = CurrentVMAllocUnit;
            }
            split_lock.unlock();
            goto EXPAND_RETURN;
        }

        // Not enough for VMAllocUnit, but enough for minimum_to_expand
        if (minimum_to_expand <= committed_tail) //vailable_space = committed_tail - 0
        {
            //First allocate the memory between committed_head to trunk_end
            uint32_t padding_size = available_space;
            ret = Memory::ExpandMemoryFromCurrentPosition(trunkPtr + head.committed_head, padding_size);
            if (!ret) goto EXPAND_RETURN;

            //Then allocate memory between trunk_ptr to committed_tail
            split_lock.lock();
            ret = Memory::ExpandMemoryFromCurrentPosition(trunkPtr, minimum_to_expand);
            if (ret)
            {
                head.append_head = 0; //warp back to head
                head.committed_head = minimum_to_expand;
            }
            split_lock.unlock();
            goto EXPAND_RETURN;
        }

        // Out of memory
        goto RELOAD_RETURN;
#pragma endregion
    EXPAND_RETURN:
        if (ret == false)
        {
            Trinity::Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "CommittedMemoryExpand: MemoryTrunk {0} failed to expand.", TrunkId);
        }
        return ret;
    RELOAD_RETURN:
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

        Diagnostics::WriteLine(Diagnostics::LogLevel::Info, "Reload: MemoryTrunk {0} reloaded.", TrunkId);

        if (temp_head_group.append_head == 0xFFFFFFFF) //Reload buffer allocation failed!
        {
            hashtable->ReleaseAllEntryLocksExceptArena();
            defrag_lock.unlock();

            Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "Reload: MemoryTrunk {0} temporary buffer allocation failed.", TrunkId);
            return false;
        }

        uint32_t available_space = TrunkLength - temp_head_group.append_head;
        if (available_space < minimum_size)
        {
            hashtable->ReleaseAllEntryLocksExceptArena();
            defrag_lock.unlock();
            Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "Reload: MemoryTrunk {0} run out of memory during trunk reloading, available size: {1}, required size: {2}.", TrunkId, available_space, minimum_size);
            return false;
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

    // return new append_head on reload success
    // return -1 when a temporary buffer cannot be allocated
    // panics and exits when there's data loss caused by memory allocation errors.
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
            Trinity::Diagnostics::FatalError("ReloadImpl: MemoryTrunk {0} fails to reclaim memory after data's been moved to a temporary buffer.", TrunkId);
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
// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "TrinityCommon.h"
#include "Storage/MemoryTrunk/MemoryTrunk.h"

namespace Storage
{
    using namespace Trinity::Diagnostics;
    void MemoryTrunk::Initialize(int32_t id, void* mem_ptr, uint64_t initial_size)
    {
        head.head_group.store(0, std::memory_order_relaxed);
        committed_tail.store(0, std::memory_order_relaxed);
        add_memory_entry_flag.store(0, std::memory_order_relaxed);
        pending_flag.store(0);

        LOIndex              = 0;
        LOCapacity           = 0;
        LOCount              = 0;

        LOPtrs               = nullptr;
        LOPreservedSizeArray = nullptr;

        IsAddressTableValid  = false;

        LastAppendHead.store(UINT32_MAX);

        this->TrunkId        = id;
        trunkPtr             = (char*)mem_ptr;
        AllocateTrunk(mem_ptr, initial_size, false);
        InitLOContainer();
    }

    MemoryTrunk::MemoryTrunk()
    {
        split_lock  = new TrinityLock();
        alloc_lock  = new TrinityLock();
        defrag_lock = new TrinityLock();
        lo_lock     = new TrinityLock();
    }

    MemoryTrunk::MemoryTrunk(int32_t id, void* mem_ptr, uint64_t initial_size): MemoryTrunk()
    {
        Initialize(id, mem_ptr, initial_size);
    }

    MemoryTrunk::MemoryTrunk(int32_t id, void* mem_ptr) :
        MemoryTrunk::MemoryTrunk(id, mem_ptr, TrinityConfig::MemoryPoolSize){}

    MemoryTrunk::~MemoryTrunk()
    {
        DeallocateTrunk();
        delete split_lock;
        delete alloc_lock;
        delete defrag_lock;
        delete lo_lock;
    }

    void MemoryTrunk::DeallocateTrunk()
    {
        DisposeTrunkBuffer();
        DisposeLargeObjects();
    }

    void MemoryTrunk::AllocateTrunk(void* mem_ptr, uint64_t size, bool LockPhysicalMemory)
    {
        size = size < (TrinityConfig::MemoryPoolSize >> 8) ? (TrinityConfig::MemoryPoolSize >> 8) : size;
        // align size to system page
        size = Memory::RoundUpToPage_64(size);

        trunkPtr = (char*)Memory::MemoryCommit(mem_ptr, size);
        if (trunkPtr == nullptr)
        {
            Trinity::Diagnostics::FatalError("Cannot allocate MemoryTrunk {0} , size = {1}", TrunkId, size);
        }

        if (LockPhysicalMemory && trunkPtr != nullptr)
        {
            if (!VirtualLock(trunkPtr, size))
            {
                Trinity::Diagnostics::WriteLine(Trinity::Diagnostics::LogLevel::Error, "Cannot lock MemoryTrunk {0} into the physical memory, LastErrorCode {1}", TrunkId, GetLastError());
            }
        }

        committed_tail      = 0;
        head.committed_head = (uint32_t)size;
    }

    int64_t MemoryTrunk::CellCount()
    {
        return hashtable->Count();
    }

    uint64_t MemoryTrunk::CommittedMemorySize()
    {
        HeadGroup headgroup_snapshot;
        headgroup_snapshot.head_group.store(head.head_group.load(std::memory_order_relaxed));
        uint64_t tail_snapshot = committed_tail;
        bool isOneRegion = (tail_snapshot < headgroup_snapshot.committed_head) ||
            (tail_snapshot == headgroup_snapshot.committed_head && tail_snapshot == 0);

        if (isOneRegion)
        {
            return headgroup_snapshot.committed_head - tail_snapshot;
        }
        else
        {
            return (TrunkLength - tail_snapshot) + headgroup_snapshot.committed_head;
        }
    }
}
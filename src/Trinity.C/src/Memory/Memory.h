// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include "TrinityCommon.h"

namespace Memory
{
    const uint32_t PAGE_MASK = 0xFFFFF000;
    const uint32_t PAGE_RANGE = 0xFFF; // PAGE_RANGE = ~PAGE_MASK = SYSTEM_PAGE_SIZE - 1

    const uint32_t WorkingSetDecreaseStep = 67108864; //64M
    const uint32_t MinWorkingSetSize = 16777216; //16M
    const uint32_t TrinityMinWorkingSet = 1048576;// 1M
    const uint64_t TrinityMaxWorkingSet = 68719476736; //64G

    extern uint64_t LargePageMinimum;
    extern uint64_t MinWorkingSet;
    extern uint64_t MaxWorkingSet;
    extern uint32_t WorkingSetFlag;

    BOOL MemoryInject(LPVOID targetPtr, uint64_t targetValue);
    void Copy(char* src, char* dest, int32_t len);
    uint64_t GetLargePageMinimumSize();
    void InitLargePageSize();
    void* LargePageAlloc(uint32_t page_num);

    bool SetReadOnly(void* address, uint64_t size);
    bool SetReadWrite(void* address, uint64_t size);
    
    char * ReserveAlloc(uint64_t reserved_size, uint64_t alloc_size);
    bool ExpandMemoryFromCurrentPosition(void* p, uint64_t size_to_expand);
    bool ExpandMemoryRegion(char*, uint64_t, uint64_t);
    void ShrinkMemoryRegion(char* p, uint32_t current_size, uint32_t desired_size);
    void FreeMemoryRegion(char* trunkPtr, uint64_t size);
    
    BOOL DecommitMemory(void* lpAddr, uint64_t size);
    void* LockedAlloc(uint64_t size);
    void LockedFree(void* p, uint64_t size);
    void* LockedReAlloc(void* p, uint64_t size, uint64_t new_size);
    void EliminateWorkingSetLimit();
    void* AlignedAlloc(uint64_t size, uint64_t alignment); // The memory will be zero-out after allocation
    void GetWorkingSetSize();
    void SetMaxWorkingSet(uint64_t max_size);
    void SetTrinityDefaultWorkingSet();
    void SetWorkingSetProfile(int32_t flag);

    inline uint32_t RoundUpToPage(uint32_t size)
    {
        return (size + Memory::PAGE_RANGE) & Memory::PAGE_MASK;
    }

    void * MemoryReserve(uint64_t size);
    void * MemoryCommit(void * buf, uint64_t size);
}

#if !defined(TRINITY_PLATFORM_WINDOWS)
    bool VirtualLock(void* addr, size_t size);
    bool VirtualUnlock(void* addr, size_t size);
#endif

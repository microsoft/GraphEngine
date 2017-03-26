// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "TrinityCommon.h"
#include <Trinity/Diagnostics/Log.h>
#include <os/os.h>
#include "Memory/Memory.h"
#if !defined(TRINITY_PLATFORM_WINDOWS)
#include <sys/types.h>
#include <unistd.h>
#include <sys/ptrace.h>
#endif

namespace Memory
{
    uint64_t LargePageMinimum = 2097152; //2M
    uint64_t MinWorkingSet = 0;
    uint64_t MaxWorkingSet = 0;
    uint32_t WorkingSetFlag = 0;

    /// Inject 8 bytes into memory.
    BOOL MemoryInject(LPVOID targetPtr, uint64_t targetValue)
    {
#if defined(TRINITY_PLATFORM_WINDOWS)
        SIZE_T bytesWritten = 0;
        DWORD old = 0;
        if (!VirtualProtectEx(GetCurrentProcess(), targetPtr, sizeof(uint64_t), PAGE_READWRITE, &old))
        {
            Console::WriteLine("Failed to change the memory page to PAGE_READWRITE\n");
            return FALSE;
        }
        if (WriteProcessMemory(GetCurrentProcess(), (LPVOID)targetPtr, (LPVOID)&targetValue, sizeof(LPVOID), &bytesWritten))
        {
            return TRUE;
        }
        else
        {
            Console::WriteLine("Failed to write to process memory.\n");
            return FALSE;
        }
#else
        /* align the address to page */
        int    pagesize             = sysconf(_SC_PAGE_SIZE);
        LPVOID targetPtr_aligned    = (LPVOID)((uint64_t)targetPtr & ~(pagesize - 1));

        if(mprotect(targetPtr_aligned, sizeof(uint64_t), PROT_READ | PROT_WRITE | PROT_EXEC) == 0)
        {
            *(uint64_t*)targetPtr = targetValue;
            return TRUE;
        }
        else
        {
            Console::WriteLine("Failed to change the memory page to PAGE_READWRITE\n");
            return FALSE;
        }
#endif
    }

    /// Warning: this is not memcpy!
    void Copy(char* src, char* dest, int32_t len)
    {
        memcpy(dest, src, len);
    }

    /**
     *  Query committed pages within the given memory space,
     *  then set the committed pages read-only.
     */
    bool SetReadOnly(void* address, uint64_t len)
    {
#if defined(TRINITY_PLATFORM_WINDOWS)
        DWORD unused;
        MEMORY_BASIC_INFORMATION mbi;
        void* address_end = (char*)address + len;

        while (address < address_end)
        {
            if (!VirtualQuery(address, &mbi, sizeof(mbi)))
            {
                return false;
            }

            if (mbi.State == MEM_COMMIT && !VirtualProtect(mbi.BaseAddress, mbi.RegionSize, PAGE_READONLY, &unused))
            {
                return false;
            }

            address = (char*)address + mbi.RegionSize;
            len    -= mbi.RegionSize;
        }

        return true;
#else
        // Update the protection flags to PROT_READ (readonly)
        return mprotect(address, len, PROT_READ) == 0;
#endif
    }

    /**
     *  Query committed pages within the given memory space,
     *  then set the committed pages read-write.
     */
    bool SetReadWrite(void* address, uint64_t len)
    {
#if defined(TRINITY_PLATFORM_WINDOWS)
        DWORD unused;
        MEMORY_BASIC_INFORMATION mbi;
        void* address_end = (char*)address + len;

        while (address < address_end)
        {
            if (!VirtualQuery(address, &mbi, sizeof(mbi)))
            {
                return false;
            }

            if (mbi.State == MEM_COMMIT && !VirtualProtect(mbi.BaseAddress, mbi.RegionSize, PAGE_READWRITE, &unused))
            {
                return false;
            }

            address = (char*)address + mbi.RegionSize;
            len    -= mbi.RegionSize;
        }

        return true;
#else
        // Update the protection flags to PROT_READ (readonly)
        return mprotect(address, len, PROT_READ | PROT_WRITE) == 0;
#endif
    }

    void * MemoryReserve(uint64_t size)
    {
#if defined(TRINITY_PLATFORM_WINDOWS)
        return VirtualAlloc(NULL, size, MEM_RESERVE /*| MEM_LARGE_PAGES */, PAGE_READWRITE);
#else
        void * p = mmap(NULL, size, PROT_NONE, MAP_PRIVATE | MAP_ANONYMOUS, -1, 0);
        return MAP_FAILED == p ? NULL : p;
#endif
    }

    /***************************************

    From MSDN:
    An attempt to commit a page that is
    already committed does not cause the
    function to fail. This means that you
    can commit pages without first determining
    the current commitment state of each page.

    ***************************************/
    void * MemoryCommit(void * buf, size_t size)
    {
#if defined(TRINITY_PLATFORM_WINDOWS)
        //Commit the desired size, the actually allocated space will be larger(up to a whole page) than the desired size.
        return VirtualAlloc(buf, size, MEM_COMMIT, PAGE_READWRITE);
#else
        return mprotect(buf, size, PROT_READ | PROT_WRITE) == 0 ? buf : NULL;
#endif
    }

    char * ReserveAlloc(uint64_t reserved_size, uint64_t alloc_size)
    {
#if defined(TRINITY_PLATFORM_WINDOWS)
        void* buf = VirtualAlloc(NULL, reserved_size, MEM_RESERVE/* | MEM_LARGE_PAGES */, PAGE_READWRITE);
        if (buf == NULL)//allocation failed
            return NULL;

        uint64_t size = (alloc_size + PAGE_RANGE) & PAGE_MASK;
        //Commit the desired size, the actually allocated space will be larger(up to a whole page) than the desired size.
        void* alloc_buf = VirtualAlloc(buf, size, MEM_COMMIT, PAGE_READWRITE);
        if (alloc_buf == NULL)
        {
            VirtualFree(buf, 0, MEM_RELEASE);
            return NULL;
        }
        return (char*)alloc_buf;
#else
        void * p = mmap(NULL, reserved_size, PROT_NONE, MAP_PRIVATE | MAP_ANONYMOUS, -1, 0);
        if (MAP_FAILED == p)
            return NULL;
        uint64_t size = (alloc_size + PAGE_RANGE) & PAGE_MASK;
        if (mprotect(p, size, PROT_READ | PROT_WRITE) == 0)
            return (char*)p;
        else
        {
            munmap(p, reserved_size);
            return NULL;
        }
#endif
    }

    void* LockedAlloc(uint64_t size)
    {
#if defined(TRINITY_PLATFORM_WINDOWS)
        void* buf = VirtualAlloc(NULL, size, MEM_COMMIT, PAGE_READWRITE);
        if (buf == NULL)//allocation failed
            return NULL;
        if (!VirtualLock(buf, size))
        {
            VirtualFree(buf, 0, MEM_RELEASE);
            return NULL;
        }
        return buf;
#else
        void * p = mmap(NULL, size, PROT_NONE, MAP_PRIVATE | MAP_ANONYMOUS, -1, 0);
        if (MAP_FAILED == p)
            return NULL;
        if (mlock(p, size) == 0)
            return p;
        else
        {
            munmap(p, size);
            return NULL;
        }
#endif
    }

    void* LockedReAlloc(void* p, uint64_t size, uint64_t new_size)
    {
#if defined(TRINITY_PLATFORM_WINDOWS)
        VirtualUnlock(p, size);
        VirtualFree(p, 0, MEM_RELEASE);
#else
        munlock(p, size);
        munmap(p, size);
#endif
        return LockedAlloc(new_size);
    }

    void LockedFree(void* p, uint64_t size)
    {
#if defined(TRINITY_PLATFORM_WINDOWS)
        VirtualUnlock(p, size);
        VirtualFree(p, 0, MEM_RELEASE);
#else
        munlock(p, size);
        munmap(p, size);
#endif
    }

    void EliminateWorkingSetLimit()
    {
#if defined(TRINITY_PLATFORM_WINDOWS)
        MEMORYSTATUSEX memoryStatus;
        memoryStatus.dwLength = sizeof(memoryStatus);
        if (!GlobalMemoryStatusEx(&memoryStatus))
            return;

        uint64_t max_wsz = std::min(memoryStatus.ullTotalPhys, memoryStatus.ullTotalVirtual) * 4 / 5;

        //max_wsz = min(max_wsz, TrinityMaxWorkingSet);

        //max_wsz -= WorkingSetDecreaseStep;

        DWORD dwQuotaFlags = QUOTA_LIMITS_HARDWS_MIN_DISABLE | QUOTA_LIMITS_HARDWS_MAX_DISABLE;
        while (!SetProcessWorkingSetSizeEx(::GetCurrentProcess(), max_wsz, max_wsz, dwQuotaFlags))
        {
            if (max_wsz <= WorkingSetDecreaseStep)
            {
                break;
            }

            if (GetLastError() == ERROR_NO_SYSTEM_RESOURCES)
            {
                max_wsz -= WorkingSetDecreaseStep;
                continue;
            }
            break;
        }
#endif
    }

    void SetTrinityDefaultWorkingSet()
    {
#if defined(TRINITY_PLATFORM_WINDOWS)
        MEMORYSTATUSEX memoryStatus;
        memoryStatus.dwLength = sizeof(memoryStatus);
        if (!GlobalMemoryStatusEx(&memoryStatus))
        {
            Console::WriteLine("Get GlobalMemoryStatus: {0}\n", GetLastError());
            return;
        }

        uint64_t max_wsz = std::min(memoryStatus.ullTotalPhys, memoryStatus.ullTotalVirtual) * 4 / 5;
        DWORD dwQuotaFlags = QUOTA_LIMITS_HARDWS_MIN_DISABLE | QUOTA_LIMITS_HARDWS_MAX_DISABLE;
        while (!SetProcessWorkingSetSizeEx(::GetCurrentProcess(), TrinityMinWorkingSet, max_wsz, dwQuotaFlags))
        {
            if (max_wsz <= WorkingSetDecreaseStep)
            {
                Console::WriteLine("Increasing working set size failed.\r\n");
                break;
            }

            if (GetLastError() == ERROR_NO_SYSTEM_RESOURCES)
            {
                max_wsz -= WorkingSetDecreaseStep;
                continue;
            }

            break;
        }
#endif
    }

    void SetMaxWorkingSet(uint64_t mem_size)
    {
#if defined(TRINITY_PLATFORM_WINDOWS)
        MEMORYSTATUSEX memoryStatus;
        memoryStatus.dwLength = sizeof(memoryStatus);
        if (!GlobalMemoryStatusEx(&memoryStatus))
        {
            return;
        }

        uint64_t max_wsz = std::min(memoryStatus.ullTotalPhys, memoryStatus.ullTotalVirtual);

        max_wsz = std::min(max_wsz, mem_size);

        DWORD dwQuotaFlags = QUOTA_LIMITS_HARDWS_MIN_DISABLE | QUOTA_LIMITS_HARDWS_MAX_DISABLE;
        while (!SetProcessWorkingSetSizeEx(::GetCurrentProcess(), max_wsz, max_wsz, dwQuotaFlags))
        {
            if (max_wsz <= WorkingSetDecreaseStep)
            {
                break;
            }

            if (GetLastError() == ERROR_NO_SYSTEM_RESOURCES)
            {
                //Need to decrease min request:
                max_wsz -= WorkingSetDecreaseStep;
                continue;
            }

            break;
        }

        /*uint64_t MinSize = 0;
        uint64_t MaxSize = 0;
        DWORD WSFlag = 0;

        GetProcessWorkingSetSizeEx(::GetCurrentProcess(), &MinSize, &MaxSize, &WSFlag);
        Console::WriteLine("%Iu, %Iu, %Iu", MinSize, MaxSize, WSFlag);*/
#endif
    }

    void ModestMemoryAllocation()
    {
#if defined(TRINITY_PLATFORM_WINDOWS)
        uint64_t min_wsz = std::min(MaxWorkingSet, static_cast<uint64_t>(TrinityMinWorkingSet));

        uint64_t max_wsz = std::max(MaxWorkingSet, static_cast<uint64_t>(TrinityMinWorkingSet));

        DWORD dwQuotaFlags = QUOTA_LIMITS_HARDWS_MIN_DISABLE | QUOTA_LIMITS_HARDWS_MAX_DISABLE;
        while (!SetProcessWorkingSetSizeEx(::GetCurrentProcess(), min_wsz, max_wsz, dwQuotaFlags))
        {
            if (max_wsz <= WorkingSetDecreaseStep)
            {
                Console::WriteLine("Increasing working set size failed.\r\n");
                break;
            }

            if (GetLastError() == ERROR_NO_SYSTEM_RESOURCES)
            {
                max_wsz -= WorkingSetDecreaseStep;
                continue;
            }

            break;
        }
#endif
    }

    void ResetWorkingSetSize()
    {
#if defined(TRINITY_PLATFORM_WINDOWS)
        SetProcessWorkingSetSizeEx(::GetCurrentProcess(), MinWorkingSet, MaxWorkingSet, WorkingSetFlag);
#endif
    }

    void GetWorkingSetSize()
    {
#if defined(TRINITY_PLATFORM_WINDOWS)
        GetProcessWorkingSetSizeEx(::GetCurrentProcess(), &MinWorkingSet, &MaxWorkingSet, (PDWORD)&WorkingSetFlag);
        //Console::WriteLine("MinWorkingSet: %Iu, MaxWorkingSet: %Iu, WorkingSetFlag: %Iu\n", MinWorkingSet, MaxWorkingSet, WorkingSetFlag);
#endif
    }

    void SetWorkingSetProfile(int32_t flag)
    {
#if defined(TRINITY_PLATFORM_WINDOWS)
        switch (flag)
        {
        case 0:
            EliminateWorkingSetLimit();
            break;
        case 1:
            SetTrinityDefaultWorkingSet();
            break;
        case 2:
            ModestMemoryAllocation();
            break;
        case 3:
            ResetWorkingSetSize();
            break;
        default:
            break;
        }

        /*uint64_t MinSize = 0;
        uint64_t MaxSize = 0;
        DWORD WSFlag = 0;

        GetProcessWorkingSetSizeEx(::GetCurrentProcess(), &MinSize, &MaxSize, &WSFlag);
        Console::WriteLine("%Iu, %Iu, %Iu", MinSize, MaxSize, WSFlag);*/
#endif
    }

    bool ExpandMemoryRegion(char* trunkPtr, uint64_t current_size, uint64_t desired_size)
    {
        return (NULL != MemoryCommit(trunkPtr + current_size, desired_size - current_size));
    }

    /// The current_size and desired_size are rounded-up size
    void ShrinkMemoryRegion(char* p, uint32_t current_size, uint32_t desired_size)
    {
        DecommitMemory(p + desired_size, current_size - desired_size);
    }

    bool ExpandMemoryFromCurrentPosition(void* p, uint64_t size_to_expand)
    {
        return (NULL != MemoryCommit(p, size_to_expand));
    }

    void FreeMemoryRegion(char* trunkPtr, uint64_t size)
    {
#if defined(TRINITY_PLATFORM_WINDOWS)
        /* size is unused */
        VirtualFree(trunkPtr, 0, MEM_RELEASE);
#else
        munmap(trunkPtr, size);
#endif
    }

    BOOL DecommitMemory(void* lpAddr, uint64_t size)
    {
#if defined(TRINITY_PLATFORM_WINDOWS)
#pragma warning(suppress: 6250)
        return VirtualFree(lpAddr, size, MEM_DECOMMIT);
#elif defined(TRINITY_PLATFORM_LINUX)
        do
        {
            if(mprotect(lpAddr, size, PROT_NONE) != 0) break;
            if(madvise(lpAddr, size, MADV_DONTNEED) != 0) break;
            return true;
        }while(0);

        return false;
#else
#error DecommitMemory: Not supported
#endif
    }

    //========================================== Large page related BEGIN ==================================================
    uint64_t GetLargePageMinimumSize()
    {
        return LargePageMinimum;
    }

    void InitLargePageSize()
    {
#if defined(TRINITY_PLATFORM_WINDOWS)
        LargePageMinimum = GetLargePageMinimum();
#else
        TRINITY_COMPILER_WARNING("InitLargePageSize: Linux support is absent")
#endif
    }

    void* LargePageAlloc(uint32_t page_num)
    {
#if defined(TRINITY_PLATFORM_WINDOWS)
        return VirtualAlloc(
            NULL,                                              // Let OS select address
            LargePageMinimum*page_num,                               // Size of allocation
            MEM_COMMIT | MEM_RESERVE | MEM_LARGE_PAGES,          // Allocate Large pages
            PAGE_READWRITE);
#else
        TRINITY_COMPILER_WARNING("LargePageAlloc: Linux support is absent")
#endif
    }

    //========================================== Large page related END  ==================================================

    // size must be 8x
    void* AlignedAlloc(uint64_t size, uint64_t alignment)
    {
#if defined(TRINITY_PLATFORM_WINDOWS)
        double* p = (double*)_aligned_malloc(size, alignment);
#else
        double* p = (double*)aligned_alloc(alignment, size);
#endif
        uint64_t count = size >> 3;
        int32_t loop = count & 0xfffffffe; // count/2 * 2;
        __m128d zero = _mm_setzero_pd();
        uint64_t i = 0;
        for (; i < loop; i += 2)
            _mm_store_pd(p + i, zero);
        if (i < count)
            _mm_store_sd(p + i, zero);
        return p;
    }
}

#if !defined(TRINITY_PLATFORM_WINDOWS)
    bool VirtualLock(void* addr, size_t size)
    {
        Trinity::Diagnostics::WriteLine(Trinity::Diagnostics::LogLevel::Error, "VirtualLock: Windows only.");
        return false;
    }
    bool VirtualUnlock(void* addr, size_t size) 
    { 
        Trinity::Diagnostics::WriteLine(Trinity::Diagnostics::LogLevel::Error, "VirtualUnlock: Windows only.");
        return false;
    }
#endif


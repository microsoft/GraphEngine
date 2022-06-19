// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include <os/os.h>
#if defined(TRINITY_PLATFORM_WINDOWS)
#include <Windows.h>

#include "Debugger.h"
#include <corelib>
#include <io>

#include <TrinityCommon.h>

#include "Storage/LocalStorage/LocalMemoryStorage.h"
#include "Storage/MTHash/MTHash.h"
#include "Storage/MemoryTrunk/MemoryTrunk.h"

#define MemChkGranularity (1<<20)

#define DEBUG_DUMP_HEX(disp, x) printf("\n%32s\t= %016llx", disp, (uint64_t)x)
#define DEBUG_DUMP_DEC(disp, x) printf("\n%32s\t= %016lld", disp, (int64_t)x)
#define CR printf("\n")
#define MSG(x) printf("%s\n", x);

static bool TWO_GB_CHK(String disp, uint64_t x)
{
    if (x >= 1 << 31) { Console::WriteLine(Console::ForegroundRed, "{0} out of range.", disp); return false; }
    return true;
}
static bool FOUR_K_CHK(String disp, uint64_t x)
{
    if (((uint64_t) x) & 0xFFF) { Console::WriteLine(Console::ForegroundRed, "{0} not aligned to 4K.", disp); return false; }
    return true;
}

namespace Storage
{
    namespace LocalMemoryStorage
    {
        extern MemoryTrunk* memory_trunks;
        extern MTHash* hashtables;

        void DebugDump()
        {
            printf("\nLocalMemoryStorage:\n");
            CR;
            DEBUG_DUMP_DEC("TrinityConfig::ReadOnly", TrinityConfig::ReadOnly());
        }

        static std::vector<LPVOID> MemoryValidityCheck(char* from, char* to, bool should_be_valid)
        {
            std::vector<LPVOID> ret;

            //if (!FOUR_K_CHK("Memchk range begin address", (uint64_t)from))
            //	return ret;
            //if (!FOUR_K_CHK("Memchk range end address", (uint64_t)to))
            //	return ret;

            MEMORY_BASIC_INFORMATION mbi;
            HANDLE hProcess = GetCurrentProcess();

            for (char* p = from; p < to; p += mbi.RegionSize)
            {
                SIZE_T result = VirtualQuery(p, &mbi, sizeof(mbi));

                if (result != sizeof(mbi))
                {
                    Trinity::Debugger::printError("VirtualQuery");
                    break;
                }

                char* seg_end = (char*) (mbi.BaseAddress) + mbi.RegionSize;
                if (seg_end > to)
                    seg_end = to;

                String dots((seg_end - p) / MemChkGranularity + 1, '.');

                if (should_be_valid)
                    if (!(mbi.State & MEM_COMMIT))
                    {
                        Console::Write(Console::ForegroundRed, dots);
                        ret.push_back(p);
                    }
                    else
                        Console::Write(Console::ForegroundCyan, dots);
                if (!should_be_valid)
                    if (mbi.State & MEM_COMMIT)
                    {
                        Console::Write(Console::ForegroundDarkRed, dots);
                        ret.push_back(p);
                    }
                    else
                        Console::Write(Console::ForegroundDarkBlue, dots);
            }
            return ret;
        }

#define CHECK_MEMORY(ptr, size) do{\
	Console::WriteLine(Console::ForegroundGreen, #ptr); \
	CR; \
	invalid_addr = MemoryValidityCheck((char*)(ptr), ((char*)(ptr)) + (size), true); \
	CR; \
		for (LPVOID p : invalid_addr)\
        		{\
		Console::WriteLine("Invalid address detected:\t{0}", p); \
		failure = true; \
        		}\
		CR; }while (0); \

        void MemoryTrunkDebugDump(int32_t idx)
        {
            printf("\n  MemoryTrunk[%d]:\n", idx);

            auto p_mt = memory_trunks + idx;

            CR;
            DEBUG_DUMP_HEX("append_head", p_mt->head.append_head);
            DEBUG_DUMP_HEX("committed_head", p_mt->head.committed_head);
            DEBUG_DUMP_HEX("committed_tail", p_mt->committed_tail);
            DEBUG_DUMP_HEX("trunkPtr", p_mt->trunkPtr);
            DEBUG_DUMP_HEX("lastAppendHead", p_mt->LastAppendHead);
            CR;
            TWO_GB_CHK("append_head", p_mt->head.append_head);
            TWO_GB_CHK("committed_head", p_mt->head.committed_head);
            TWO_GB_CHK("committed_tail", p_mt->committed_tail);
            CR;
            FOUR_K_CHK("committed_head", p_mt->head.committed_head);
            FOUR_K_CHK("committed_tail", p_mt->committed_tail);
            FOUR_K_CHK("trunkPtr", (uint64_t) p_mt->trunkPtr);
            CR;

            Console::WriteLine(Console::ForegroundGreen, "trunkPtr:");

            std::vector<LPVOID> invalid_addr, leaked_addr, vec;

            if (p_mt->head.committed_head > p_mt->committed_tail)
            {
                vec = MemoryValidityCheck(
                    p_mt->trunkPtr,
                    p_mt->trunkPtr + p_mt->committed_tail,
                    false);
                leaked_addr.insert(leaked_addr.end(), vec.begin(), vec.end());
                vec = MemoryValidityCheck(
                    p_mt->trunkPtr + p_mt->committed_tail,
                    p_mt->trunkPtr + p_mt->head.committed_head,
                    true);
                invalid_addr.insert(invalid_addr.end(), vec.begin(), vec.end());
                vec = MemoryValidityCheck(
                    p_mt->trunkPtr + p_mt->head.committed_head,
                    p_mt->trunkPtr + MemoryTrunk::TrunkLength,
                    false);
                leaked_addr.insert(leaked_addr.end(), vec.begin(), vec.end());
            }
            else
            {
                vec = MemoryValidityCheck(
                    p_mt->trunkPtr,
                    p_mt->trunkPtr + p_mt->head.committed_head,
                    true);
                invalid_addr.insert(invalid_addr.end(), vec.begin(), vec.end());
                vec = MemoryValidityCheck(
                    p_mt->trunkPtr + p_mt->head.committed_head,
                    p_mt->trunkPtr + p_mt->committed_tail,
                    false);
                leaked_addr.insert(leaked_addr.end(), vec.begin(), vec.end());
                vec = MemoryValidityCheck(
                    p_mt->trunkPtr + p_mt->committed_tail,
                    p_mt->trunkPtr + MemoryTrunk::TrunkLength,
                    true);
                invalid_addr.insert(invalid_addr.end(), vec.begin(), vec.end());
            }
            CR;

            for (LPVOID p : invalid_addr)
                Console::WriteLine("Invalid address detected:\t{0}", p);
            for (LPVOID p : leaked_addr)
                Console::WriteLine("Leaked address detected:\t{0}", p);

            MSG("LO:");
            DEBUG_DUMP_HEX("LOPtrs", p_mt->LOPtrs);
            DEBUG_DUMP_DEC("LOIndex", p_mt->LOIndex);
            DEBUG_DUMP_DEC("LOCount", p_mt->LOCount);
        }

        void DumpCellEntry(CellEntry* p_entry, cellid_t id, String msg)
        {
            Console::WriteLine("Cell {0}:\t{1}.", id, msg);
            DEBUG_DUMP_HEX("offset", p_entry->offset);
            DEBUG_DUMP_HEX("size", p_entry->size);
            CR;
        }

        void MTHashSearchCellEntry(cellid_t cell_id)
        {
            uint8_t trunk_id = cell_id & 0xFF;
            MTHash* p_hash = hashtables + trunk_id;
            bool found = false;

            DEBUG_DUMP_DEC("cell_id", cell_id);
            DEBUG_DUMP_DEC("TrunkID", trunk_id);
            CR;
            CR;
            MSG("Searching.");
            DEBUG_DUMP_DEC("NonEmptyEntryCount", p_hash->ExtendedInfo->NonEmptyEntryCount);
            DEBUG_DUMP_DEC("EntryCount", p_hash->ExtendedInfo->EntryCount);
            DEBUG_DUMP_DEC("FreeEntryCount", p_hash->ExtendedInfo->FreeEntryCount);
            DEBUG_DUMP_DEC("FreeEntryList", p_hash->ExtendedInfo->FreeEntryList);
            CR;

            for (uint32_t i = 0; i < p_hash->ExtendedInfo->EntryCount; ++i)
            {
                auto p_mtentry = p_hash->MTEntries + i;
                auto p_cellentry = p_hash->CellEntries + i;
                uint32_t committed_head = memory_trunks[trunk_id].head.committed_head;
                uint32_t committed_tail = memory_trunks[trunk_id].committed_tail;

                if (p_hash->CellEntries[i].location == -1 && i < (uint32_t)p_hash->ExtendedInfo->NonEmptyEntryCount && p_hash->ExtendedInfo->FreeEntryList == -1)
                {
                    /* An invalid entry is not supposed to be found here. */
                    DEBUG_DUMP_DEC("Invalid hole entry", i);
                    DEBUG_DUMP_DEC("CellEntryIndex", i);
                    DEBUG_DUMP_DEC("CellId", p_mtentry->Key);
                    DEBUG_DUMP_DEC("CellType", p_mtentry->CellType);
                    DEBUG_DUMP_DEC("EntryLock", (char) p_mtentry->EntryLock);
                    DEBUG_DUMP_DEC("Flag", p_mtentry->Flag);
                    DEBUG_DUMP_DEC("NextEntry", p_mtentry->NextEntry);

                    CR;
                    CR;
                }

                if (p_hash->MTEntries[i].Key == cell_id)
                {
                    found = true;
                    bool inrange = true;

                    MSG("Found entry.");
                    DEBUG_DUMP_DEC("CellEntryIndex", i);
                    DEBUG_DUMP_DEC("CellType", p_mtentry->CellType);
                    DEBUG_DUMP_DEC("EntryLock", (char) p_mtentry->EntryLock);
                    DEBUG_DUMP_DEC("Flag", p_mtentry->Flag);
                    DEBUG_DUMP_DEC("NextEntry", p_mtentry->NextEntry);

                    CR;
                    CR;
                    if (i >= (uint32_t)p_hash->ExtendedInfo->NonEmptyEntryCount)
                    {
                        MSG("Cell entry out of range!");
                    }

                    DEBUG_DUMP_HEX("CellOffset", p_cellentry->offset);
                    DEBUG_DUMP_HEX("CellSize", p_cellentry->size);
                    CR;
                    CR;

                    if (p_cellentry->offset >= 0)
                    {

                        MSG("In MemoryTrunk. Checking offset.");

                        DEBUG_DUMP_HEX("committed_head", committed_head);
                        DEBUG_DUMP_HEX("committed_tail", committed_tail);

                        if (committed_head > committed_tail)
                            inrange = (committed_head > (uint32_t)p_cellentry->offset && (uint32_t)p_cellentry->offset >= committed_tail);
                        else
                            inrange = ((uint32_t)p_cellentry->offset >= committed_tail || (uint32_t)p_cellentry->offset < committed_head);
                        if (!inrange)
                        {
                            DumpCellEntry(p_cellentry, p_hash->MTEntries[i].Key, "range check error");
                        }
                        else
                        {
                            MSG("In range. All good.");
                        }
                    }

                }
            }

            if (!found)
                MSG("No entry.");

        }

        void MTHashDebugDump(int32_t idx)
        {
            printf("\n  MTHash[%d]:\n", idx);

            MTHash* p_hash = hashtables + idx;

            MSG("variables:");
            DEBUG_DUMP_HEX("Buckets", p_hash->Buckets);
            DEBUG_DUMP_HEX("BucketLockers", p_hash->BucketLockers);
            //DEBUG_DUMP_HEX("NextEntryArray", p_hash->NextEntryArray);
            DEBUG_DUMP_HEX("MTEntries", p_hash->MTEntries);
            DEBUG_DUMP_HEX("CellEntries", p_hash->CellEntries);
            //DEBUG_DUMP_HEX("CellEntryAtomicPtr", p_hash->CellEntryAtomicPtr);
            //DEBUG_DUMP_HEX("EntryLockers", p_hash->EntryLockers);
            //DEBUG_DUMP_HEX("CellTypeArray", p_hash->CellTypeArray);
            //DEBUG_DUMP_HEX("CellEntryPtr", p_hash->CellEntryPtr);
            //DEBUG_DUMP_HEX("KeyPtr", p_hash->KeyPtr);
            //DEBUG_DUMP_HEX("BucketPtr", p_hash->BucketPtr);
            //DEBUG_DUMP_HEX("NextEntryArrayPtr", p_hash->NextEntryArrayPtr);
            //DEBUG_DUMP_HEX("EntryLockersPtr", p_hash->EntryLockersPtr);
            //DEBUG_DUMP_HEX("BucketLockersPtr", p_hash->BucketLockersPtr);
            //DEBUG_DUMP_HEX("CellTypeArrayPtr", p_hash->CellTypeArrayPtr);
            DEBUG_DUMP_HEX("memory_trunk", p_hash->memory_trunk);
            //DEBUG_DUMP_HEX("ExternOffsetArray", p_hash->ExternOffsetArray);
            //DEBUG_DUMP_HEX("ExternArrayPtr", p_hash->ExternArrayPtr);
            DEBUG_DUMP_DEC("FreeEntryList", p_hash->ExtendedInfo->FreeEntryList);
            DEBUG_DUMP_DEC("EntryCount", p_hash->ExtendedInfo->EntryCount);
            DEBUG_DUMP_DEC("NonEmptyEntryCount", p_hash->ExtendedInfo->NonEmptyEntryCount);
            CR;
            MSG("memory:");
            CR;
            std::vector<LPVOID> invalid_addr;
            bool failure = false;
            bool inrange = false;

            if (!TrinityConfig::ReadOnly())
            {
                CHECK_MEMORY(p_hash->BucketLockers, MTHash::BucketCount);
            }

            CHECK_MEMORY(p_hash->Buckets, MTHash::BucketCount << 2);
            CHECK_MEMORY(p_hash->CellEntries, p_hash->ExtendedInfo->EntryCount << 3);
            CHECK_MEMORY(p_hash->MTEntries, p_hash->ExtendedInfo->EntryCount << 4);
            //CHECK_MEMORY(p_hash->NextEntryArray, p_hash->EntryCount << 2);

            if (failure)
                return;

            CR;
            MSG("cell:");

            auto p_trunk = p_hash->memory_trunk;
            uint32_t confirmed_entry_count = 0;

            uint32_t committed_head = p_trunk->head.committed_head;
            uint32_t committed_tail = p_trunk->committed_tail;

            DEBUG_DUMP_HEX("committed_head", committed_head);
            DEBUG_DUMP_HEX("committed_tail", committed_tail);

            CR;

            for (int32_t i = 0; i < p_hash->ExtendedInfo->NonEmptyEntryCount; ++i)
            {
                auto p_entry = p_hash->CellEntries + i;
                if (p_entry->location == -1)
                    continue;
                ++confirmed_entry_count;
                if (p_entry->offset >= 0)
                {
                    if (committed_head > committed_tail)
                        inrange = (committed_head > (uint32_t)p_entry->offset && (uint32_t)p_entry->offset >= committed_tail);
                    else
                        inrange = ((uint32_t)p_entry->offset >= committed_tail || (uint32_t)p_entry->offset < committed_head);
                    if (!inrange)
                    {
                        DumpCellEntry(p_entry, p_hash->MTEntries[i].Key, "range check error");
                    }
                }
                else
                {
                    int32_t LOidx = -p_entry->offset;
                    failure = false;
                    CHECK_MEMORY(p_trunk->LOPtrs[LOidx], p_entry->size);
                    if (failure)
                    {
                        DumpCellEntry(p_entry, p_hash->MTEntries[i].Key, "LO memory corrupted");
                    }
                }
            }

            MSG("---");
            DEBUG_DUMP_DEC("Cell count", confirmed_entry_count);
        }
    }
}
#endif

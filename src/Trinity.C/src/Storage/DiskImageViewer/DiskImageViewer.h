#pragma once

#include "Storage/MemoryTrunk/MemoryTrunk.h"
#include "Storage/MTHash/MTHash.h"
#include "Storage/MTHash/MT_ENUMERATOR.h"

namespace Storage
{
    //  Loads a disk image (.mp, .index, .lo files)
    class DiskImageViewer
    {
        std::unique_ptr<MTHash>        p_hash;
        std::unique_ptr<MemoryTrunk>   p_trunk;
        std::unique_ptr<MT_ENUMERATOR> p_enumerator;
        void*                          p_memory;
        uint64_t                       n_reserved;

        void Destroy()
        {
            p_trunk.reset();
            p_hash.reset();
            p_enumerator.reset();

            if (p_memory)
            {
                Memory::FreeMemoryRegion(p_memory, n_reserved);
                p_memory = nullptr;
            }
        }

    public:

        DiskImageViewer(const String& trunkimg, const String& hashimg, const String& lofile)
            : p_memory(nullptr)
        {
            if (trunkimg.Empty() || hashimg.Empty() || lofile.Empty())
            {
                return;
            }

            n_reserved = TrinityConfig::ReservedSpacePerTrunk();
            p_memory   = Memory::MemoryReserve(n_reserved);
            p_trunk    = std::make_unique<MemoryTrunk>(-1, (char*)p_memory, TrinityConfig::MemoryPoolSize >> 8);
            p_hash     = std::make_unique<MTHash>();

            if (!p_hash->Initialize(hashimg, p_trunk.get()) || 
                !p_trunk->LoadMP(trunkimg) ||
                !p_trunk->LoadLOFile(lofile))
            {
                Destroy();
            }

            p_enumerator = std::make_unique<MT_ENUMERATOR>(p_hash.get());
        }

        bool Good()
        {
            return p_memory && p_hash.get() && p_trunk.get();
        }

        MT_ENUMERATOR* GetEnumerator()
        {
            return p_enumerator.get();
        }

        void Reset()
        {
            p_enumerator->Initialize(p_hash.get());
        }

        TrinityErrorCode MoveNext(cellid_t& cellId, char*& cellPtr, int32_t& cellSize, uint16_t& cellType)
        {
            auto error = p_enumerator->MoveNext();
            if (error == TrinityErrorCode::E_SUCCESS)
            {
                cellId   = p_enumerator->CellId();
                cellPtr  = p_enumerator->CellPtr();
                cellSize = p_enumerator->CellSize();
                cellType = p_enumerator->CellType();
            }
            return error;
        }

        ~DiskImageViewer()
        {
            Destroy();
        }
    };
}

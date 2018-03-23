// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include "TrinityCommon.h"
#include "Trinity/Configuration/TrinityConfig.h"
#include "Memory/Memory.h"
#include "Storage/MTHash/MTHash.h"
#include "Storage/MTHash/MT_ENUMERATOR.h"
#include "Storage/MTHash/CellEntry.h"
#include <Trinity/Diagnostics/Log.h>
#include <corelib>

/////////////////////// Forward declaration
namespace Storage
{
    namespace LocalMemoryStorage
    {
        String GetPrimaryStorageSlot();
        String GetSecondaryStorageSlot();
    }
}
//////////////////////////////////////////

namespace Storage
{
    typedef union
    {
#if defined(TRINITY_PLATFORM_WINDOWS)
        struct
        {
            std::atomic<uint32_t> append_head;
            std::atomic<uint32_t> committed_head;
        };
        struct
        {
            std::atomic<int64_t> head_group;
        };
#else
        //HACK
#undef committed_head
        std::atomic<uint32_t> append_head;
        struct
        {
            uint32_t _pad;
            std::atomic<uint32_t> committed_head;
        }______committed_head_struct______;

        std::atomic<int64_t> head_group;
#define committed_head ______committed_head_struct______.committed_head
#endif
    }HeadGroup;

    typedef struct
    {
        int32_t index;
        uint32_t offset;
    }AddressTableEntry;

    typedef struct
    {
        /// Backward index in the address table.
        int32_t bwd_index;
        /// Forward index in the address table.
        int32_t fwd_index;
    }AddressTableEndPoint;

    typedef struct
    {
        int64_t LowBits;
        int64_t HighBits;
    }MD5_SIGNATURE, *PMD5_SIGNATURE;

    class MemoryTrunk
    {
    private:
        enum Int32_Constants :int32_t
        {
            PhysicalMemoryPageLocking = 0,
        };

    public:
        enum UInt32_Constants :uint32_t
        {
            /// 2G
            TrunkLength = 0x80000000,

            /// 1GB
            MaxLargeObjectSize = 0x40000000,
        };

        /**************************** 64-byte block 1 ********************/
        char* trunkPtr;
        MTHash* hashtable;
        HeadGroup head;
        std::atomic<uint32_t> committed_tail;
        std::atomic<int32_t> pending_flag;
        //////////////////////////// 32bytes
        TrinityLock* split_lock;
        TrinityLock* alloc_lock;
        TrinityLock* defrag_lock;
        TrinityLock* lo_lock;
        /**************************** 64-byte block 2 ********************/
        char** LOPtrs;
        int32_t* LOPreservedSizeArray;
        /// Pointing to next free large object slot.
        /// Note: It starts from 1, so that it can be converted into a negative number (used by hash table)
        int32_t LOIndex;
        int32_t LOCapacity;
        ///Current number of large objects
        int32_t LOCount;
        int32_t TrunkId;
        ///////////////////////////// 32bytes
        std::atomic<uint32_t> add_memory_entry_flag;
        /********** Defragmentation *********/
        std::atomic<uint32_t> LastAppendHead;
        std::atomic<uint8_t> IsAddressTableValid;
        uint8_t padding[23];
        /**************************** 64-byte block 2 ******************/
        /////////////////////////////////////////////////////////////////

        void Initialize(int32_t id, void* mem_ptr, uint64_t initial_size);
        MemoryTrunk();
        MemoryTrunk(int32_t id, void* mem_ptr, uint64_t initial_size);
        MemoryTrunk(int32_t id, void* mem_ptr);
        ~MemoryTrunk();
        void DeallocateTrunk();
        void AllocateTrunk(void* mem_ptr, uint64_t size, bool LockPhysicalMemory);
        int64_t CellCount();

        /***************** Defragmentation *****************/
        bool CellBoundaryCheck(int32_t cell_offset, bool AddressTableOneRegion, HeadGroup& HeadGroupShadow, uint32_t& CommittedTailShadow);
        AddressTableEntry* DumpAddressTable(int32_t& addressTableLength, bool& AddressTableOneRegion);
        void EmptyCommittedMemory(bool takeMTHashLock = true);
        AddressTableEndPoint SortAddressTable(AddressTableEntry* addressTable, int32_t addressTableLength, bool AddressTableOneRegion);
        void Defragment(bool calledByGCThread = true);
        void DefragmentOneRegion(AddressTableEntry* addressTable, int32_t addressTableLength, AddressTableEndPoint endpoint);
        void DefragmentTwoRegion(AddressTableEntry* addressTable, int32_t addressTableLength, AddressTableEndPoint endpoint);
        /////////////////////////////////////////////////////

        /*********************** Memory ************************/
        TrinityErrorCode AddMemoryCell(cellid_t cellId, int32_t cell_length, OUT int32_t& cell_offset);
        TrinityErrorCode ExpandLargeObject(int32_t lo_index, int32_t original_size, int32_t new_size);
        TrinityErrorCode ShrinkLargeObject(int32_t lo_index, int32_t original_size, int32_t new_size);
        ////////////////////////////////////////////////////////
        
        char* AllocateLargeObject(int32_t);

        int32_t ReloadImpl();
        void DisposeTrunkBuffer();
        inline void BufferedDecommitMemory(void* lpAddr, uint64_t size)
        {
            Memory::DecommitMemory(lpAddr, size);
        };

        inline void FlushDecommitBuffer() {};

        /// Allocate a continuous memory from the committed memory region.
        /// Note after calling this function, all cell_offsets may be invalid due to a possible reload.
        /// @param cellId The target cellId.
        /// @param cellSize The bytes of memory to allocate.
        /// @return A memory pointer pointing to the allocated memory.
        ALLOC_THREAD_CTX char* CellAlloc(cellid_t cellId, uint32_t cellSize);

        /// Allocate virtual memory and move committed_head and append_head (when committed_region splits)
        /// Guarantee that after this call:
        /// there are at least minimum_size bytes between append_head and commited_head
        bool CommittedMemoryExpand(uint32_t minimum_size);

        /// Reload current memory trunk (all memory bubbles will be squeezed out)
        /// After the Reload, all cells will be arranged continuously starting from trunkPtr:
        /// committed_tail is reset to trunk_start;
        /// append_head will be placed right after the last cell;
        /// committed_head will be placed on the first byte of next page after append_head
        /// All unused pages will be de-committed.
        bool Reload(uint32_t minimum_size);
        ///////////////////////////////////////////////////////////////////////////////////

        /******************* LO ***********************************/
        void InitLOContainer();
        void ShrinkLOContainer();
        void EnsureLOCapacity(int32_t count);
        void ResizeLOContainer();
        void DisposeLargeObjects();
        void DisposeLargeObject(int32_t lo_index);
        int32_t ShrinkLOContainerAction(int32_t index);
        /////////////////////////////////////////////////////////////

        /****************** Performance Monitor ****************/
        uint64_t CommittedMemorySize();
        /////////////////////////////////////////////////////////

        /******************** DiskIO ****************************/
        inline String MemoryTrunkFilePath(const char* fileExtension, bool is_primary)
        {
            String mp_dir;
            mp_dir = is_primary ? LocalMemoryStorage::GetPrimaryStorageSlot() : LocalMemoryStorage::GetSecondaryStorageSlot();
            mp_dir = Path::Combine(mp_dir, "MemoryPool");
            mp_dir = Path::CompletePath(mp_dir, true);
            mp_dir = Path::Combine(mp_dir, String::Format("{0}.{1}", TrunkId, fileExtension));

            return mp_dir;
        }

        inline String MPFile(bool is_primary)
        {
            return MemoryTrunkFilePath("mp", is_primary);
        }

        inline String LO_File(bool is_primary)
        {
            return MemoryTrunkFilePath("lo", is_primary);
        }

        inline String IndexFile(bool is_primary)
        {
            return MemoryTrunkFilePath("index", is_primary);
        }

        bool Save(String storage_root = "");
        bool Load(String storage_root = "");

        bool SaveMP(String mp_file = "");
        bool LoadMP(String mp_file = "");
        bool SaveIndex(String index_file = "");
        bool LoadIndex(String index_file = "");
        bool SaveLOFile(String lo_file = "");
        bool LoadLOFile(String lo_file = "");
        //////////////////////////////////////////////////////////////
    };
    }

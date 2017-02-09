// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include "TrinityCommon.h"
#include "Threading/TrinitySpinlock.h"
#include "Storage/MTHash/MTHash.h"
#include "Storage/MemoryTrunk/MemoryTrunk.h"

#include <mutex>

namespace Storage
{
    namespace LocalMemoryStorage
    {
        enum Int32_Constants : int32_t
        {
            c_MaxTrunkCount = 256,
        };

        /**! Should be synchronized with Trinity.Core\TSL\Lib\Common.cs */
        enum CellAccessOptions : int32_t
        {
            /// <summary>
            /// No actions. This entry should not be used.
            /// </summary>
            None                         = 0x0,
            /// <summary>
            /// Throws an exception when a cell is not found.
            /// </summary>
            ThrowExceptionOnCellNotFound = 0x1,
            /// <summary>
            /// Creates a new cell when a cell is not found.
            /// </summary>
            CreateNewOnCellNotFound      = 0x2,
            /// <summary>
            /// Specifies that write-ahead-log should be performed with strong durability.
            /// </summary>
            StrongLogAhead               = 0x4,
            /// <summary>
            /// Specifies that write-ahead-log should be performed with weak durability. This option brings better performance,
            /// but the durability may be degraded when this option is used.
            /// </summary>
            WeakLogAhead                 = 0x8,
        };

        extern std::atomic<bool> initialized;
        extern int32_t trunk_id_mask;

        TrinityErrorCode Initialize();
        void Defragment(int32_t trunkIndex);
        uint64_t CellCount();
        void Dispose();

        inline int32_t GetTrunkId(cellid_t cellId)
        {
            return trunk_id_mask & cellId;
        }

        // Inter-operative cell interfaces
        CELL_ACQUIRE_LOCK   TrinityErrorCode CGetLockedCellInfo4CellAccessor(IN cellid_t cellId, OUT int32_t &size, OUT uint16_t &type, OUT char* &cellPtr, OUT int32_t &entryIndex);
        CELL_ACQUIRE_LOCK   TrinityErrorCode CGetLockedCellInfo4SaveCell(IN cellid_t cellId, IN int32_t size, IN uint16_t type, OUT char* &cellPtr, OUT int32_t &entryIndex);
        CELL_ACQUIRE_LOCK   TrinityErrorCode CGetLockedCellInfo4AddCell(IN cellid_t cellId, IN int32_t size, IN uint16_t type, OUT char* &cellPtr, OUT int32_t &entryIndex);
        CELL_ACQUIRE_LOCK   TrinityErrorCode CGetLockedCellInfo4UpdateCell(IN cellid_t cellId, IN int32_t size, OUT char* &cellPtr, OUT int32_t &entryIndex);
        CELL_ACQUIRE_LOCK   TrinityErrorCode CGetLockedCellInfo4LoadCell(IN cellid_t cellId, OUT int32_t &size, OUT char* &cellPtr, OUT int32_t &entryIndex);
        CELL_ACQUIRE_LOCK   TrinityErrorCode CGetLockedCellInfo4AddOrUseCell(IN cellid_t cellId, IN OUT int32_t &size, IN uint16_t type, OUT char* &cellPtr, OUT int32_t &entryIndex);
        CELL_LOCK_PROTECTED TrinityErrorCode CLockedGetCellSize(IN cellid_t cellId, IN int32_t entryIndex, OUT int32_t &size);
        ///////////////////////////////////

        // cell manipulation interfaces
        char* ResizeCell(cellid_t cellId, int32_t cellEntryIndex, int32_t offset, int32_t delta);

        //   non-logging interfaces
        CELL_ATOMIC TrinityErrorCode LoadCell(cellid_t cellId, Array<char>& cellBuff);
        CELL_ATOMIC TrinityErrorCode SaveCell(cellid_t cellId, char* buff, int32_t size, uint16_t cellType);
        CELL_ATOMIC TrinityErrorCode AddCell(cellid_t cellId, char* buff, int32_t size, uint16_t cellType);
        CELL_ATOMIC TrinityErrorCode UpdateCell(cellid_t cellId, char* buff, int32_t size);
        CELL_ATOMIC TrinityErrorCode RemoveCell(cellid_t cellId);

        //   logging interfaces
        CELL_ATOMIC TrinityErrorCode SaveCell(cellid_t cellId, char* buff, int32_t size, uint16_t cellType, CellAccessOptions options);
        CELL_ATOMIC TrinityErrorCode AddCell(cellid_t cellId, char* buff, int32_t size, uint16_t cellType, CellAccessOptions options);
        CELL_ATOMIC TrinityErrorCode UpdateCell(cellid_t cellId, char* buff, int32_t size, CellAccessOptions options);
        CELL_ATOMIC TrinityErrorCode RemoveCell(cellid_t cellId, CellAccessOptions options);

        CELL_ATOMIC TrinityErrorCode GetCellType(cellid_t cellId, uint16_t& cellType);
        CELL_LOCK_PROTECTED void ReleaseCellLock(cellid_t cellId, int32_t entryIndex);

        bool Contains(cellid_t cellId);
        /////////////////////////////////////

        // DiskIO

        typedef struct
        {
            /* version (ulong) + md5 * trunkCount */
            uint64_t         version;
            MD5_SIGNATURE    trunk_signatures[c_MaxTrunkCount];
        }TRINITY_IMAGE_SIGNATURE, *PTRINITY_IMAGE_SIGNATURE;

        String GetPrimaryStorageSlot();
        String GetSecondaryStorageSlot();

        bool LoadStorage();
        bool SaveStorage();
        bool ResetStorage();

        // Write-ahead logging

        namespace Logging
        {
#pragma pack(push, 1)
            typedef struct
            {
                cellid_t    CELL_ID;
                int32_t     CONTENT_LEN;
                uint16_t    CELL_TYPE;
                uint8_t     CHECKSUM; // 8-bit second-order check
            }LOG_RECORD_HEADER, *PLOG_RECORD_HEADER;
#pragma pack(pop)

            void ComputeChecksum(PLOG_RECORD_HEADER plog, char* bufferPtr);
            bool ValidateChecksum(PLOG_RECORD_HEADER plog, char* content);
            void WriteAheadLog(cellid_t cellId, char* cellPtr, int32_t cellSize, uint16_t cellType, CellAccessOptions options);
            void SetWriteAheadLogFile(FILE* fp);
        }

        // Performance counter
        uint64_t TrunkCommittedMemorySize();
        uint64_t MTHashCommittedMemorySize();
        uint64_t TotalCommittedMemorySize();
        uint64_t TotalCellSize();

        namespace Enumeration
        {
            typedef struct
            {
                /** Public members visible to C# */
                char*       CellPtr;
                cellid_t    CellId;
                int32_t     CellEntryIndex;
                uint16_t    CellType;
                /**         CellSize should be obtained from mt_enumerator if necessary */
                //////////////////////////////////
                /** Internal members */
                uint16_t        mt_enumerator_active;
                MT_ENUMERATOR   mt_enumerator;
                MTHash*         mt_hash;
                //////////////////////////////////
            }LOCAL_MEMORY_STORAGE_ENUMERATOR, *PLOCAL_MEMORY_STORAGE_ENUMERATOR;

            TrinityErrorCode Allocate(OUT LOCAL_MEMORY_STORAGE_ENUMERATOR* &pp_enum);
            TrinityErrorCode Deallocate(IN  LOCAL_MEMORY_STORAGE_ENUMERATOR* p_enum);
            TrinityErrorCode MoveNext(IN  LOCAL_MEMORY_STORAGE_ENUMERATOR* p_enum);
            TrinityErrorCode Reset(IN  LOCAL_MEMORY_STORAGE_ENUMERATOR* p_enum);
        }

        void DebugDump();
        void MemoryTrunkDebugDump(int32_t idx);
        void MTHashDebugDump(int32_t idx);
        void MTHashSearchCellEntry(cellid_t cell_id);

        // Checksum
        TrinityErrorCode GetMD5Hash(IN int32_t trunkIndex, OUT char* hash);
        TrinityErrorCode GetTrinityImageSignature(OUT PTRINITY_IMAGE_SIGNATURE pSignature);
    };
}

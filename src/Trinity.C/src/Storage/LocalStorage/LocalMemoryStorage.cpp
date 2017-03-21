// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "TrinityCommon.h"
#include "Storage/LocalStorage/LocalMemoryStorage.h"
#include "Trinity/Configuration/TrinityConfig.h"
#include "Storage/LocalStorage/GCTask.h"
#include "BackgroundThread/BackgroundThread.h"
#include <threading>

namespace Storage
{
    namespace LocalMemoryStorage
    {

        MemoryTrunk*             memory_trunks;
        MTHash*         	     hashtables;
        MTHash*         	     hashtables_end;
        int32_t*                 dirty_flags;
        int32_t                  trunk_count;
        int32_t                  trunk_id_mask;

        void*                    MemoryPtr = nullptr;
        std::atomic<bool>        disposed;
        std::mutex               disposal_lock;

        std::atomic<bool>        initialized(false);

        static TrinityErrorCode  ReadOnlyInitialize();

        const int32_t            c_ImageSignatureLength = sizeof(TRINITY_IMAGE_SIGNATURE);
        TRINITY_IMAGE_SIGNATURE *g_ImageSignature;
        static void              _calculate_storage_slot();
        static TrinityErrorCode  _load_signature(PTRINITY_IMAGE_SIGNATURE, bool=true);

        TrinityErrorCode Initialize()
        {
            uint64_t MemoryReserveUnit       = TrinityConfig::MemoryReserveUnit();
            MTHash::MTEntryOffset            = (MemoryReserveUnit << 3); // skip CellEntries
            MTHash::BucketMemoryOffset       = MTHash::MTEntryOffset + (MemoryReserveUnit << 4);
            MTHash::BucketLockerMemoryOffset = MTHash::BucketMemoryOffset + (MemoryReserveUnit << 2);

            trunk_count                      = TrinityConfig::TrunkCount();
            trunk_id_mask                    = trunk_count - 1;
            MemoryPtr                        = Memory::MemoryReserve(TrinityConfig::TrinityReservedSpace());
            memory_trunks                    = new MemoryTrunk[trunk_count];
            hashtables                       = new MTHash[trunk_count];
            hashtables_end                   = hashtables + trunk_count;
            dirty_flags                      = new int32_t[trunk_count];

            if (MemoryPtr == nullptr)
            {
                FatalError("LocalMemoryStorage::Initialize: Failed to reserve memory.");
            }

            for (int32_t i = 0; i < trunk_count; i++)
            {
                memory_trunks[i].Initialize(i, (char*)MemoryPtr + (TrinityConfig::ReservedSpacePerTrunk() * (uint64_t)i), TrinityConfig::MemoryPoolSize >> 8);
                hashtables[i].Initialize((uint32_t)(TrinityConfig::MemoryPoolSize >> 15), memory_trunks + i); // Default capacity = 256M >> 15 = 8192
            }

            memset(dirty_flags, 0, sizeof(int32_t) * trunk_count);

            disposed.store(false);
            initialized.store(true);

            g_ImageSignature          = new TRINITY_IMAGE_SIGNATURE();
            memset(g_ImageSignature, -1, c_ImageSignatureLength);
            _calculate_storage_slot();
            _load_signature(g_ImageSignature);
            /**! The semantic of Initialize is equivalent to ResetStorage() from the
             *   image of the highest version.
             */
            ++g_ImageSignature->version;

            if (TrinityConfig::ReadOnly())
            {
                auto readonly_init_result = ReadOnlyInitialize();
                if (readonly_init_result == TrinityErrorCode::E_SUCCESS)
                {
                    Diagnostics::WriteLine(LogLevel::Info, "LocalMemoryStorage is initialized in read-only mode");
                }
                else
                {
                    Diagnostics::WriteLine(LogLevel::Warning, "LocalMemoryStorage read-only initialization failed, code = {0}", readonly_init_result);
                    return readonly_init_result;
                }
            }
            else
            {
                BackgroundThread::TaskScheduler::AddTask(new GCTask());
                Diagnostics::WriteLine(LogLevel::Info, "LocalMemoryStorage is initialized in read-write mode");
            }

            return TrinityErrorCode::E_SUCCESS;
        }

        static TrinityErrorCode ReadOnlyInitialize()
        {
            if (FALSE == Memory::SetReadOnly(MemoryPtr, TrinityConfig::TrinityReservedSpace()))
                return TrinityErrorCode::E_FAILURE;

            return TrinityErrorCode::E_SUCCESS;
        }

        /**! Should only be called from GC thread. */
        void Defragment(int32_t trunkIndex)
        {
            memory_trunks[trunkIndex].Defragment(/*calledByGCThread:*/true);
        }

        static uint64_t _CellCount_impl()
        {
            uint64_t total = 0;
            for (int32_t i = 0; i < trunk_count; i++)
            {
                total += (uint64_t)memory_trunks[i].CellCount();
            }
            return total;
        }

        uint64_t CellCount()
        {
            TRINITY_INTEROP_ENTER_UNMANAGED();
            uint64_t total = _CellCount_impl();
            TRINITY_INTEROP_LEAVE_UNMANAGED();
            return total;
        }

        void Dispose()
        {
            TRINITY_INTEROP_ENTER_UNMANAGED();
            if (!disposed)
            {
                disposal_lock.lock();
                if (!disposed)
                {
                    GCTask::StopDefragAndAwaitCeased();
                    delete[] memory_trunks; //! This will call the MemoryTrunk deconstructor implicitly
                    delete[] hashtables; //! This will call the MTHash deconstructor implicitly
                    delete   g_ImageSignature;
                    delete[] dirty_flags;

                    memory_trunks    = nullptr;
                    hashtables       = nullptr;
                    hashtables_end   = nullptr;
                    g_ImageSignature = nullptr;

                    disposed.store(true);
                }
                disposal_lock.unlock();
            }
            TRINITY_INTEROP_LEAVE_UNMANAGED();
        }

        TrinityErrorCode ResizeCell(cellid_t cellId, int32_t cellEntryIndex, int32_t offset, int32_t delta, char*& cell_ptr)
        {
            TRINITY_INTEROP_ENTER_UNMANAGED();
            TrinityErrorCode ret = hashtables[GetTrunkId(cellId)].ResizeCell(cellEntryIndex, offset, delta, cell_ptr);
            TRINITY_INTEROP_LEAVE_UNMANAGED();
            return ret;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Whole DB scale operations
        ///////////////////////////////////////////////////////////////////////////////////////////

        static String s_primaryStorageSlot;
        static String s_secondaryStorageSlot;

        String GetPrimaryStorageSlot()
        {
            return Path::CompletePath(Path::Combine(TrinityConfig::StorageRoot(), s_primaryStorageSlot), true);
        }

        String GetSecondaryStorageSlot()
        {
            return Path::CompletePath(Path::Combine(TrinityConfig::StorageRoot(), s_secondaryStorageSlot), true);
        }

        static std::mutex s_critical_lock;

        static inline void _enter_db_critical()
        {
            s_critical_lock.lock();
            GCTask::StopDefragAndAwaitCeased();

            Parallel::For(0, trunk_count, [&](int32_t trunk_idx){hashtables[trunk_idx].Lock(); });

        }

        static inline void _exit_db_critical()
        {
            Parallel::For(0, trunk_count, [&](int32_t trunk_idx){hashtables[trunk_idx].Unlock(); });

            GCTask::RestartDefragmentation();
            s_critical_lock.unlock();
        }

        static inline TrinityErrorCode _save_signature(PTRINITY_IMAGE_SIGNATURE p_sig)
        {
            String dir_path  = GetSecondaryStorageSlot();
            String file_path = Path::Combine(dir_path, "image.sig");

            FILE* fp;

            if (0 != _wfopen_s(&fp, file_path.ToWcharArray(), _u("wb")))
            {
                Trinity::Diagnostics::WriteLine(LogLevel::Error, "Cannot open signature file '{0}' for write", file_path);
                return TrinityErrorCode::E_FAILURE;
            }

            if (1 != fwrite(p_sig, c_ImageSignatureLength, 1, fp))
            {
                Trinity::Diagnostics::WriteLine(LogLevel::Error, "Failed to write to signature file '{0}'", file_path);
                return TrinityErrorCode::E_FAILURE;
            }

            if (0 != fclose(fp))
            {
                Trinity::Diagnostics::WriteLine(LogLevel::Warning, "Failed to close signature file '{0}'", file_path);
            }

            return TrinityErrorCode::E_SUCCESS;
        }

        static inline TrinityErrorCode _load_signature(PTRINITY_IMAGE_SIGNATURE p_sig, bool is_primary)
        {
            String dir_path  = is_primary ? GetPrimaryStorageSlot() : GetSecondaryStorageSlot();
            String file_path = Path::Combine(dir_path, "image.sig");

            FILE* fp;

            if (0 != _wfopen_s(&fp, file_path.ToWcharArray(), _u("rb")))
            {
                // This message will be triggered when the signature does not exist yet,
                // so we lower its level to Debug to suppress it under the default Info logging level
                Trinity::Diagnostics::WriteLine(LogLevel::Debug, "Cannot open signature file '{0}' for read", file_path);
                return TrinityErrorCode::E_FAILURE;
            }

            if (1 != fread(p_sig, c_ImageSignatureLength, 1, fp))
            {
                Trinity::Diagnostics::WriteLine(LogLevel::Error, "Signature file '{0}' corrupted", file_path);
                return TrinityErrorCode::E_FAILURE;
            }

            if (0 != fclose(fp))
            {
                Trinity::Diagnostics::WriteLine(LogLevel::Warning, "Failed to close signature file '{0}'", file_path);
            }

            return TrinityErrorCode::E_SUCCESS;
        }

        static int64_t _compare_signature(PTRINITY_IMAGE_SIGNATURE lhs, PTRINITY_IMAGE_SIGNATURE rhs, int cmp_signature_cnt)
        {
            if (lhs->version != rhs->version)
                return lhs->version - rhs->version;

            int64_t* lp   = reinterpret_cast <int64_t*> (lhs->trunk_signatures);
            int64_t* rp   = reinterpret_cast <int64_t*> (rhs->trunk_signatures);
            int64_t* lend = reinterpret_cast <int64_t*> (&lhs->trunk_signatures[cmp_signature_cnt]);

            while (lp != lend)
            {
                int64_t diff = *lp - *rp;

                if (diff != 0)
                {
                    PMD5_SIGNATURE lp_trunk_signature = reinterpret_cast<PMD5_SIGNATURE>(lp);
                    Diagnostics::WriteLine(LogLevel::Error, "Storage signature mismatch on trunk #{0}.", lp_trunk_signature - lhs->trunk_signatures);
                    return diff;
                }

                ++lp;
                ++rp;
            }

            return 0;
        }

        static void _calculate_storage_slot()
        {
            PTRINITY_IMAGE_SIGNATURE p_signatures = new TRINITY_IMAGE_SIGNATURE[2];

            /**
             * The reason to set primary to "B" and secondary to "A" by default
             * is that, when saving the storage for the first time, by our logic,
             * we will save to the secondary slot. It may be a good idea to start
             * from "A", then "B" rather than saving the first copy into "B" slot.
             */
            s_primaryStorageSlot   = "B";
            s_secondaryStorageSlot = "A";

            p_signatures[0].version = p_signatures[1].version = UINT64_MAX;

            _load_signature(&p_signatures[0], true);
            _load_signature(&p_signatures[1], false);

            if (p_signatures[1].version != UINT64_MAX &&
                (p_signatures[0].version == UINT64_MAX || p_signatures[0].version < p_signatures[1].version))
            {
                std::swap(s_primaryStorageSlot, s_secondaryStorageSlot);
            }

            delete[] p_signatures;
        }

        bool LoadStorage()
        {
            TRINITY_INTEROP_ENTER_UNMANAGED();

            _enter_db_critical();

            _calculate_storage_slot();

            Trinity::Diagnostics::WriteLine(LogLevel::Info, "Loading storage from [SLOT {0}]", s_primaryStorageSlot);

            PTRINITY_IMAGE_SIGNATURE p_primary_sig      = new TRINITY_IMAGE_SIGNATURE;
            bool                     sig_load_fail      = false;
            bool                     image_load_success = true;

            if (TrinityErrorCode::E_SUCCESS == _load_signature(p_primary_sig))
            {
                /* Signature load successful. */
                memset(g_ImageSignature, -1, c_ImageSignatureLength);
                g_ImageSignature->version = p_primary_sig->version;
            }
            else
            {
                /* Cannot load signature. */
                Trinity::Diagnostics::WriteLine(LogLevel::Error, "LoadStorage: cannot load storage image signature. Reverting version to INIT");
                g_ImageSignature->version = UINT64_MAX;
                sig_load_fail             = true;
            }

            for (int32_t i = 0; i < trunk_count; i++)
            {
                Trinity::Diagnostics::WriteLine(LogLevel::Verbose, "Loading memory trunk #{0}", i);
                image_load_success = memory_trunks[i].Load() && image_load_success;
            }

            PMD5_SIGNATURE p_sig = g_ImageSignature->trunk_signatures;
            Parallel::For(0, trunk_count, [&](int32_t trunk_idx){
                memory_trunks[trunk_idx].hashtable->GetMD5Hash((char*)(p_sig + trunk_idx));
            });

            if (_compare_signature(g_ImageSignature, p_primary_sig, trunk_count))
            {
                /* Either there's no signature and we're loading from empty image, or there's signature mismatch. */

                if (sig_load_fail && !_CellCount_impl())
                {
                    Trinity::Diagnostics::WriteLine(LogLevel::Warning, "LoadStorage: loading from an empty storage.");
                }
                else
                {
                    /* Signature mismatch, exit now. */
                    Trinity::Diagnostics::FatalError("Failed to load storage: Image signature mismatch.");
                }

            }

            if (TrinityConfig::ReadOnly())
            {
                if (FALSE == Memory::SetReadOnly(MemoryPtr, TrinityConfig::TrinityReservedSpace()))
                {
                    Trinity::Diagnostics::WriteLine(LogLevel::Error, "Failed to enable write protection under read-only mode.");
                }
            }

            _calculate_storage_slot();

            delete p_primary_sig;

            _exit_db_critical();

            Trinity::Diagnostics::WriteLine(LogLevel::Info, "Load storage complete, image version = {0}", g_ImageSignature->version);

            TRINITY_INTEROP_LEAVE_UNMANAGED();

            return true;
        }

        bool SaveStorage()
        {
            TRINITY_INTEROP_ENTER_UNMANAGED();

            if (TrinityConfig::ReadOnly())
            {
                TRINITY_INTEROP_LEAVE_UNMANAGED();
                return false;
            }

            bool success = true;

            _enter_db_critical();

            _calculate_storage_slot();

            Trinity::Diagnostics::WriteLine(LogLevel::Info, "Saving storage to [SLOT {0}]", s_secondaryStorageSlot);

            for (int32_t i = 0; i < trunk_count; i++)
            {
                Trinity::Diagnostics::WriteLine(LogLevel::Verbose, "Saving memory trunk #{0}", i);
                memory_trunks[i].Defragment(/*calledByGCThread:*/false);
                success = memory_trunks[i].Save() && success;
            }

            if (success)
            {
                PMD5_SIGNATURE p_sig = g_ImageSignature->trunk_signatures;
                Parallel::For(0, trunk_count, [&](int32_t trunk_idx){
                    memory_trunks[trunk_idx].hashtable->GetMD5Hash((char*)(p_sig + trunk_idx));
                });

                /* Increase the version of the image by one. */
                ++g_ImageSignature->version;
                /* Save new signature to secondary slot. */
                success = (TrinityErrorCode::E_SUCCESS == _save_signature(g_ImageSignature));
                /* After saving signature, re-calculate to swap to the new primary storage. */
                _calculate_storage_slot();
            }

            _exit_db_critical();

            if (success)
            {
                Trinity::Diagnostics::WriteLine(LogLevel::Info, "Save storage complete, image version = {0}.", g_ImageSignature->version);
            }
            else
            {
                Trinity::Diagnostics::WriteLine(LogLevel::Error, "Save storage failed.");
            }

            TRINITY_INTEROP_LEAVE_UNMANAGED();

            return success;
        }

        bool ResetStorage()
        {
            TRINITY_INTEROP_ENTER_UNMANAGED();

            if (TrinityConfig::ReadOnly())
            {
                TRINITY_INTEROP_LEAVE_UNMANAGED();
                return false;
            }

            _enter_db_critical();

            Trinity::Diagnostics::WriteLine(LogLevel::Info, "Resetting storage.");

            for (int32_t i = 0; i < trunk_count; i++)
            {
                Trinity::Diagnostics::WriteLine(LogLevel::Verbose, "Resetting memory trunk #{0}", i);
                memory_trunks[i].DeallocateTrunk();
                hashtables[i].DeallocateMTHash(/** deallocateBucketLockers: */false);
                memory_trunks[i].Initialize(i, (char*)MemoryPtr + (TrinityConfig::ReservedSpacePerTrunk() * (uint64_t)i), TrinityConfig::MemoryPoolSize >> 8);
                hashtables[i].Initialize((uint32_t)(TrinityConfig::MemoryPoolSize >> 15), memory_trunks + i); // Default capacity = 256M >> 15 = 8192
            }
            disposed.store(false);
            initialized.store(true);

            /* Increase version by one, and reset the signature. */
            ++g_ImageSignature->version;
            memset(g_ImageSignature->trunk_signatures, -1, c_ImageSignatureLength - sizeof(uint64_t));

            _exit_db_critical();

            Trinity::Diagnostics::WriteLine(LogLevel::Info, "Reset storage complete, image version = {0}", g_ImageSignature->version);

            TRINITY_INTEROP_LEAVE_UNMANAGED();

            return true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Whole DB scale operations END
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Single cell scale operations
        ///////////////////////////////////////////////////////////////////////////////////////////

        // GeteLockedCellInfo interfaces
        CELL_ACQUIRE_LOCK TrinityErrorCode CGetLockedCellInfo4CellAccessor(IN cellid_t cellId, OUT int32_t &size, OUT uint16_t &type, OUT char* &cellPtr, OUT int32_t &entryIndex)
        {
            TRINITY_INTEROP_ENTER_UNMANAGED();
            auto eResult = hashtables[GetTrunkId(cellId)].CGetLockedCellInfo4CellAccessor(cellId, size, type, cellPtr, entryIndex);
            TRINITY_INTEROP_LEAVE_UNMANAGED();
            return eResult;
        }

        CELL_ACQUIRE_LOCK TrinityErrorCode CGetLockedCellInfo4SaveCell(IN cellid_t cellId, IN int32_t size, IN uint16_t type, OUT char* &cellPtr, OUT int32_t &entryIndex)
        {
            TRINITY_INTEROP_ENTER_UNMANAGED();
            auto eResult = hashtables[GetTrunkId(cellId)].CGetLockedCellInfo4SaveCell(cellId, size, type, cellPtr, entryIndex);
            TRINITY_INTEROP_LEAVE_UNMANAGED();
            return eResult;
        }

        CELL_ACQUIRE_LOCK TrinityErrorCode CGetLockedCellInfo4AddCell(IN cellid_t cellId, IN int32_t size, IN uint16_t type, OUT char* &cellPtr, OUT int32_t &entryIndex)
        {
            TRINITY_INTEROP_ENTER_UNMANAGED();
            auto eResult = hashtables[GetTrunkId(cellId)].CGetLockedCellInfo4AddCell(cellId, size, type, cellPtr, entryIndex);
            TRINITY_INTEROP_LEAVE_UNMANAGED();
            return eResult;
        }

        CELL_ACQUIRE_LOCK TrinityErrorCode CGetLockedCellInfo4UpdateCell(IN cellid_t cellId, IN int32_t size, OUT char* &cellPtr, OUT int32_t &entryIndex)
        {
            TRINITY_INTEROP_ENTER_UNMANAGED();
            auto eResult = hashtables[GetTrunkId(cellId)].CGetLockedCellInfo4UpdateCell(cellId, size, cellPtr, entryIndex);
            TRINITY_INTEROP_LEAVE_UNMANAGED();
            return eResult;
        }

        CELL_ACQUIRE_LOCK TrinityErrorCode CGetLockedCellInfo4LoadCell(IN cellid_t cellId, OUT int32_t &size, OUT char* &cellPtr, OUT int32_t &entryIndex)
        {
            TRINITY_INTEROP_ENTER_UNMANAGED();
            auto eResult = hashtables[GetTrunkId(cellId)].CGetLockedCellInfo4LoadCell(cellId, size, cellPtr, entryIndex);
            TRINITY_INTEROP_LEAVE_UNMANAGED();
            return eResult;
        }

        CELL_ACQUIRE_LOCK TrinityErrorCode CGetLockedCellInfo4AddOrUseCell(IN cellid_t cellId, IN OUT int32_t &size, IN uint16_t type, OUT char* &cellPtr, OUT int32_t &entryIndex)
        {
            TRINITY_INTEROP_ENTER_UNMANAGED();
            auto eResult = hashtables[GetTrunkId(cellId)].CGetLockedCellInfo4AddOrUseCell(cellId, size, type, cellPtr, entryIndex);
            TRINITY_INTEROP_LEAVE_UNMANAGED();
            return eResult;
        }

        CELL_LOCK_PROTECTED TrinityErrorCode CLockedGetCellSize(IN cellid_t cellId, IN int32_t entryIndex, OUT int32_t &size)
        {
            size = hashtables[GetTrunkId(cellId)].CellSize(entryIndex);
            return TrinityErrorCode::E_SUCCESS;
        }

        CELL_LOCK_PROTECTED void ReleaseCellLock(cellid_t cellId, int32_t entryIndex)
        {
            hashtables[GetTrunkId(cellId)].ReleaseEntryLock(entryIndex);
        }
        ////////////////////////////////////////

        TrinityErrorCode LoadCell(cellid_t cellId, Array<char>& cellBuff)
        {
            TRINITY_INTEROP_ENTER_UNMANAGED();

            MTHash * hashtable = hashtables + GetTrunkId(cellId);
            char* cellPtr;
            int32_t entryIndex;
            int32_t cellSize;
            TrinityErrorCode eResult = hashtable->CGetLockedCellInfo4LoadCell(cellId, cellSize, cellPtr, entryIndex);
            if (TrinityErrorCode::E_SUCCESS == eResult)
            {
                cellBuff = Array<char>(cellSize);
                memcpy(cellBuff, cellPtr, cellSize);
                hashtable->ReleaseEntryLock(entryIndex);
            }

            TRINITY_INTEROP_LEAVE_UNMANAGED();
            return eResult;
        }

        // Non-logging interfaces

        TrinityErrorCode SaveCell(cellid_t cellId, char* buff, int32_t cellSize, uint16_t cellType)
        {
            TRINITY_INTEROP_ENTER_UNMANAGED();
            MTHash * hashtable = hashtables + GetTrunkId(cellId);
            char* cellPtr;
            int32_t entryIndex;
            TrinityErrorCode eResult = hashtable->CGetLockedCellInfo4SaveCell(cellId, cellSize, cellType, cellPtr, entryIndex);
            if (TrinityErrorCode::E_SUCCESS == eResult)
            {
                memcpy(cellPtr, buff, cellSize);
                hashtable->ReleaseEntryLock(entryIndex);
            }
            TRINITY_INTEROP_LEAVE_UNMANAGED();
            return eResult;
        }

        TrinityErrorCode AddCell(cellid_t cellId, char* buff, int32_t cellSize, uint16_t cellType)
        {
            TRINITY_INTEROP_ENTER_UNMANAGED();
            MTHash * hashtable = hashtables + GetTrunkId(cellId);
            char* cellPtr;
            int32_t entryIndex;
            TrinityErrorCode eResult = hashtable->CGetLockedCellInfo4AddCell(cellId, cellSize, cellType, cellPtr, entryIndex);
            if (TrinityErrorCode::E_SUCCESS == eResult)
            {
                memcpy(cellPtr, buff, cellSize);
                hashtable->ReleaseEntryLock(entryIndex);
            }
            TRINITY_INTEROP_LEAVE_UNMANAGED();
            return eResult;
        }

        TrinityErrorCode UpdateCell(cellid_t cellId, char* buff, int32_t cellSize)
        {
            TRINITY_INTEROP_ENTER_UNMANAGED();
            MTHash * hashtable = hashtables + GetTrunkId(cellId);
            char* cellPtr;
            int32_t entryIndex;
            TrinityErrorCode eResult = hashtable->CGetLockedCellInfo4UpdateCell(cellId, cellSize, cellPtr, entryIndex);
            if (TrinityErrorCode::E_SUCCESS == eResult)
            {
                memcpy(cellPtr, buff, cellSize);
                hashtable->ReleaseEntryLock(entryIndex);
            }
            TRINITY_INTEROP_LEAVE_UNMANAGED();
            return eResult;
        }

        TrinityErrorCode RemoveCell(cellid_t cellId)
        {
            TRINITY_INTEROP_ENTER_UNMANAGED();
            TrinityErrorCode eResult = hashtables[GetTrunkId(cellId)].RemoveCell(cellId);
            TRINITY_INTEROP_LEAVE_UNMANAGED();
            return eResult;
        }

        // Logging interfaces

        TrinityErrorCode SaveCell(cellid_t cellId, char* buff, int32_t cellSize, uint16_t cellType, CellAccessOptions options)
        {
            TRINITY_INTEROP_ENTER_UNMANAGED();
            MTHash * hashtable = hashtables + GetTrunkId(cellId);
            char* cellPtr;
            int32_t entryIndex;
            TrinityErrorCode eResult = hashtable->CGetLockedCellInfo4SaveCell(cellId, cellSize, cellType, cellPtr, entryIndex);
            if (TrinityErrorCode::E_SUCCESS == eResult)
            {
                Logging::WriteAheadLog(cellId, buff, cellSize, cellType, options);
                memcpy(cellPtr, buff, cellSize);
                hashtable->ReleaseEntryLock(entryIndex);
            }
            TRINITY_INTEROP_LEAVE_UNMANAGED();
            return eResult;
        }

        TrinityErrorCode AddCell(cellid_t cellId, char* buff, int32_t cellSize, uint16_t cellType, CellAccessOptions options)
        {
            TRINITY_INTEROP_ENTER_UNMANAGED();
            MTHash * hashtable = hashtables + GetTrunkId(cellId);
            char* cellPtr;
            int32_t entryIndex;
            TrinityErrorCode eResult = hashtable->CGetLockedCellInfo4AddCell(cellId, cellSize, cellType, cellPtr, entryIndex);
            if (TrinityErrorCode::E_SUCCESS == eResult)
            {
                Logging::WriteAheadLog(cellId, buff, cellSize, cellType, options);
                memcpy(cellPtr, buff, cellSize);
                hashtable->ReleaseEntryLock(entryIndex);
            }
            TRINITY_INTEROP_LEAVE_UNMANAGED();
            return eResult;
        }

        TrinityErrorCode UpdateCell(cellid_t cellId, char* buff, int32_t cellSize, CellAccessOptions options)
        {
            TRINITY_INTEROP_ENTER_UNMANAGED();
            MTHash * hashtable = hashtables + GetTrunkId(cellId);
            char* cellPtr;
            int32_t entryIndex;
            TrinityErrorCode eResult = hashtable->CGetLockedCellInfo4UpdateCell(cellId, cellSize, cellPtr, entryIndex);
            if (TrinityErrorCode::E_SUCCESS == eResult)
            {
                uint16_t cellType = hashtable->CellType(entryIndex);
                Logging::WriteAheadLog(cellId, buff, cellSize, cellType, options);
                memcpy(cellPtr, buff, cellSize);
                hashtable->ReleaseEntryLock(entryIndex);
            }
            TRINITY_INTEROP_LEAVE_UNMANAGED();
            return eResult;
        }

        TrinityErrorCode RemoveCell(cellid_t cellId, CellAccessOptions options)
        {
            TRINITY_INTEROP_ENTER_UNMANAGED();
            TrinityErrorCode eResult = hashtables[GetTrunkId(cellId)].RemoveCell(cellId, options);
            TRINITY_INTEROP_LEAVE_UNMANAGED();
            return eResult;
        }

        ////////////////////////////////////////

        TrinityErrorCode GetCellType(cellid_t cellId, uint16_t& cellType)
        {
            TRINITY_INTEROP_ENTER_UNMANAGED();
            TrinityErrorCode eResult = hashtables[GetTrunkId(cellId)].GetCellType(cellId, cellType);
            TRINITY_INTEROP_LEAVE_UNMANAGED();
            return eResult;
        }

        bool Contains(cellid_t cellId)
        {
            TRINITY_INTEROP_ENTER_UNMANAGED();
            bool ret = hashtables[GetTrunkId(cellId)].ContainsKey(cellId);
            TRINITY_INTEROP_LEAVE_UNMANAGED();
            return ret;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Single cell scale operations END
        ///////////////////////////////////////////////////////////////////////////////////////////

        uint64_t TrunkCommittedMemorySize()
        {
            uint64_t total = 0;
            for (int32_t i = 0; i < trunk_count; i++)
            {
                total += memory_trunks[i].CommittedMemorySize();
            }
            return total;
        }

        uint64_t MTHashCommittedMemorySize()
        {
            uint64_t total = 0;
            for (int32_t i = 0; i < trunk_count; i++)
            {
                total += hashtables[i].CommittedMemorySize();
            }
            return total;
        }

        uint64_t TotalCellSize()
        {
            uint64_t total = 0;
            for (int32_t i = 0; i < trunk_count; i++)
            {
                total += hashtables[i].TotalCellSize();
            }
            return total;
        }

        uint64_t TotalCommittedMemorySize()
        {
            return TrunkCommittedMemorySize() + MTHashCommittedMemorySize();
        }

        TrinityErrorCode GetMD5Hash(IN int32_t trunkIndex, OUT char* hash)
        {
            return hashtables[trunkIndex].GetMD5Hash(hash);
        }

        TrinityErrorCode GetTrinityImageSignature(OUT PTRINITY_IMAGE_SIGNATURE pSignature)
        {
            *pSignature = *g_ImageSignature;
            return E_SUCCESS;
        }
    }
}

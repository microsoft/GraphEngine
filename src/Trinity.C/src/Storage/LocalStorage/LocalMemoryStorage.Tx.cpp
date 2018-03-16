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
#include "ThreadContext.h"

namespace Storage
{
    namespace LocalMemoryStorage 
    {
		extern MTHash* hashtables;

#define TRINITY_ENTER_TX() \
            TRINITY_INTEROP_ENTER_UNMANAGED(); \
            PTHREAD_CONTEXT p_tls_ctx = GetCurrentThreadContext();\
            SetCurrentThreadContext(ctx);

#define TRINITY_LEAVE_TX() \
            SetCurrentThreadContext(p_tls_ctx); \
            TRINITY_INTEROP_LEAVE_UNMANAGED();

        ///////////////////////////////////////////////////////////////////////////////////////////
        // TX Single cell operations
        ///////////////////////////////////////////////////////////////////////////////////////////

        // GetLockedCellInfo interfaces
        TrinityErrorCode TxCGetLockedCellInfo4CellAccessor(IN PTHREAD_CONTEXT ctx, IN cellid_t cellId, OUT int32_t &size, OUT uint16_t &type, OUT char* &cellPtr, OUT int32_t &entryIndex)
        {
            TRINITY_ENTER_TX();

            ctx->SetLockingCell(cellId);
            TrinityErrorCode eResult = hashtables[GetTrunkId(cellId)].CGetLockedCellInfo4CellAccessor(cellId, size, type, cellPtr, entryIndex);
            ctx->SetLockAcquired(eResult == TrinityErrorCode::E_SUCCESS);

            TRINITY_LEAVE_TX();
            return eResult;
        }

        TrinityErrorCode TxCGetLockedCellInfo4LoadCell(IN PTHREAD_CONTEXT ctx, IN cellid_t cellId, OUT int32_t &size, OUT char* &cellPtr, OUT int32_t &entryIndex)
        {
            TRINITY_ENTER_TX();

            ctx->SetLockingCell(cellId);
            TrinityErrorCode eResult = hashtables[GetTrunkId(cellId)].CGetLockedCellInfo4LoadCell(cellId, size, cellPtr, entryIndex);
            ctx->SetLockAcquired(eResult == TrinityErrorCode::E_SUCCESS);

            TRINITY_LEAVE_TX();
            return eResult;
        }

        TrinityErrorCode TxCGetLockedCellInfo4AddOrUseCell(IN PTHREAD_CONTEXT ctx, IN cellid_t cellId, IN OUT int32_t &size, IN uint16_t type, OUT char* &cellPtr, OUT int32_t &entryIndex)
        {
            TRINITY_ENTER_TX();
            ctx->SetLockingCell(cellId);
            TrinityErrorCode eResult = hashtables[GetTrunkId(cellId)].CGetLockedCellInfo4AddOrUseCell(cellId, size, type, cellPtr, entryIndex);
            ctx->SetLockAcquired(eResult == TrinityErrorCode::E_CELL_FOUND || eResult == TrinityErrorCode::E_CELL_NOT_FOUND);
            TRINITY_LEAVE_TX();
            return eResult;
        }

        void TxReleaseCellLock(IN PTHREAD_CONTEXT ctx, IN cellid_t cellId, IN int32_t entryIndex)
        {
            if (hashtables[GetTrunkId(cellId)].ReleaseEntryLock(entryIndex) == 0)
            {
                //Lock count decreased to 0. We no longer holds the lock.
                ctx->SetLockReleased(cellId);
            }
        }

        TrinityErrorCode TxResizeCell(IN PTHREAD_CONTEXT ctx, IN cellid_t cellId, IN int32_t cellEntryIndex, IN int32_t offset, IN int32_t delta, OUT char*& cell_ptr)
        {
            TRINITY_ENTER_TX();
            ctx->SetLockingCell(cellId);
            TrinityErrorCode result = hashtables[GetTrunkId(cellId)].ResizeCell(cellEntryIndex, offset, delta, cell_ptr);
            TRINITY_LEAVE_TX();
            return result;
        }

        ////////////////////////////////////////

        TrinityErrorCode TxLoadCell(PTHREAD_CONTEXT ctx, cellid_t cellId, Array<char>& cellBuff)
        {
            TRINITY_ENTER_TX();
            MTHash * hashtable = hashtables + GetTrunkId(cellId);
            char* cellPtr;
            int32_t entryIndex;
            int32_t cellSize;
            ctx->SetLockingCell(cellId);
            TrinityErrorCode eResult = hashtable->CGetLockedCellInfo4LoadCell(cellId, cellSize, cellPtr, entryIndex);
            if (TrinityErrorCode::E_SUCCESS == eResult)
            {
                cellBuff = Array<char>(cellSize);
                memcpy(cellBuff, cellPtr, cellSize);
                hashtable->ReleaseEntryLock(entryIndex);
            }

			TRINITY_LEAVE_TX();
			return eResult;
		}

		// Non-logging interfaces

        TrinityErrorCode TxSaveCell(PTHREAD_CONTEXT ctx, cellid_t cellId, char* buff, int32_t cellSize, uint16_t cellType)
        {
            TRINITY_ENTER_TX();
            MTHash * hashtable = hashtables + GetTrunkId(cellId);
            char* cellPtr;
            int32_t entryIndex;
            ctx->SetLockingCell(cellId);
            TrinityErrorCode eResult = hashtable->CGetLockedCellInfo4SaveCell(cellId, cellSize, cellType, cellPtr, entryIndex);
            if (TrinityErrorCode::E_SUCCESS == eResult)
            {
                memcpy(cellPtr, buff, cellSize);
                hashtable->ReleaseEntryLock(entryIndex);
            }
            TRINITY_LEAVE_TX();
            return eResult;
        }

        TrinityErrorCode TxAddCell(PTHREAD_CONTEXT ctx, cellid_t cellId, char* buff, int32_t cellSize, uint16_t cellType)
        {
            TRINITY_ENTER_TX();
            MTHash * hashtable = hashtables + GetTrunkId(cellId);
            char* cellPtr;
            int32_t entryIndex;
            ctx->SetLockingCell(cellId);
            TrinityErrorCode eResult = hashtable->CGetLockedCellInfo4AddCell(cellId, cellSize, cellType, cellPtr, entryIndex);
            if (TrinityErrorCode::E_SUCCESS == eResult)
            {
                memcpy(cellPtr, buff, cellSize);
                hashtable->ReleaseEntryLock(entryIndex);
            }
            TRINITY_LEAVE_TX();
            return eResult;
        }

        TrinityErrorCode TxUpdateCell(PTHREAD_CONTEXT ctx, cellid_t cellId, char* buff, int32_t cellSize)
        {
            TRINITY_ENTER_TX();
            ctx->SetLockingCell(cellId);
            MTHash * hashtable = hashtables + GetTrunkId(cellId);
            char* cellPtr;
            int32_t entryIndex;
            TrinityErrorCode eResult = hashtable->CGetLockedCellInfo4UpdateCell(cellId, cellSize, cellPtr, entryIndex);
            if (TrinityErrorCode::E_SUCCESS == eResult)
            {
                memcpy(cellPtr, buff, cellSize);
                hashtable->ReleaseEntryLock(entryIndex);
            }
            TRINITY_LEAVE_TX();
            return eResult;
        }

        TrinityErrorCode TxRemoveCell(PTHREAD_CONTEXT ctx, cellid_t cellId)
        {
            TRINITY_ENTER_TX();
            ctx->SetLockingCell(cellId);
            TrinityErrorCode eResult = hashtables[GetTrunkId(cellId)].RemoveCell(cellId);
            TRINITY_LEAVE_TX();
            return eResult;
        }

		// Logging interfaces

        TrinityErrorCode TxSaveCell(PTHREAD_CONTEXT ctx, cellid_t cellId, char* buff, int32_t cellSize, uint16_t cellType, CellAccessOptions options)
        {
            TRINITY_ENTER_TX();

            MTHash * hashtable = hashtables + GetTrunkId(cellId);
            char* cellPtr;
            int32_t entryIndex;
            ctx->SetLockingCell(cellId);
            TrinityErrorCode eResult = hashtable->CGetLockedCellInfo4SaveCell(cellId, cellSize, cellType, cellPtr, entryIndex);
            if (TrinityErrorCode::E_SUCCESS == eResult)
            {
                Logging::WriteAheadLog(cellId, buff, cellSize, cellType, options);
                memcpy(cellPtr, buff, cellSize);
                hashtable->ReleaseEntryLock(entryIndex);
            }
            TRINITY_LEAVE_TX();
            return eResult;
        }

        TrinityErrorCode TxAddCell(PTHREAD_CONTEXT ctx, cellid_t cellId, char* buff, int32_t cellSize, uint16_t cellType, CellAccessOptions options)
        {
            TRINITY_ENTER_TX();
            MTHash * hashtable = hashtables + GetTrunkId(cellId);
            char* cellPtr;
            int32_t entryIndex;
            ctx->SetLockingCell(cellId);
            TrinityErrorCode eResult = hashtable->CGetLockedCellInfo4AddCell(cellId, cellSize, cellType, cellPtr, entryIndex);
            if (TrinityErrorCode::E_SUCCESS == eResult)
            {
                Logging::WriteAheadLog(cellId, buff, cellSize, cellType, options);
                memcpy(cellPtr, buff, cellSize);
                hashtable->ReleaseEntryLock(entryIndex);
            }
            TRINITY_LEAVE_TX();
            return eResult;
        }

        TrinityErrorCode TxUpdateCell(PTHREAD_CONTEXT ctx, cellid_t cellId, char* buff, int32_t cellSize, CellAccessOptions options)
        {
            TRINITY_ENTER_TX();
            MTHash * hashtable = hashtables + GetTrunkId(cellId);
            char* cellPtr;
            int32_t entryIndex;
            ctx->SetLockingCell(cellId);
            TrinityErrorCode eResult = hashtable->CGetLockedCellInfo4UpdateCell(cellId, cellSize, cellPtr, entryIndex);
            if (TrinityErrorCode::E_SUCCESS == eResult)
            {
                uint16_t cellType = hashtable->CellType(entryIndex);
                Logging::WriteAheadLog(cellId, buff, cellSize, cellType, options);
                memcpy(cellPtr, buff, cellSize);
                hashtable->ReleaseEntryLock(entryIndex);
            }
            TRINITY_LEAVE_TX();
            return eResult;
        }

        TrinityErrorCode TxRemoveCell(PTHREAD_CONTEXT ctx, cellid_t cellId, CellAccessOptions options)
        {
            TRINITY_ENTER_TX();
            ctx->SetLockingCell(cellId);
            TrinityErrorCode eResult = hashtables[GetTrunkId(cellId)].RemoveCell(cellId, options);
            TRINITY_LEAVE_TX();
            return eResult;
        }

		////////////////////////////////////////
        TrinityErrorCode TxGetCellType(PTHREAD_CONTEXT ctx, cellid_t cellId, uint16_t& cellType)
        {
            TRINITY_ENTER_TX();
            ctx->SetLockingCell(cellId);
            TrinityErrorCode eResult = hashtables[GetTrunkId(cellId)].GetCellType(cellId, cellType);
            TRINITY_LEAVE_TX();
            return eResult;
        }

    }
}
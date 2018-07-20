// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
// Trinity.h: public interfaces

#pragma once
#include "TrinityCommon.h"
#include "../src/Storage/LocalStorage/ThreadContext.h"

typedef struct THREAD_CONTEXT* PTHREAD_CONTEXT;

//====================== LocalMemoryStorage ==========================//
//DLL_IMPORT TrinityErrorCode  CInitialize();
DLL_IMPORT uint64_t          CCellCount();
//DLL_IMPORT BOOL              CResetStorage();
//DLL_IMPORT void              CDispose();

//DLL_IMPORT BOOL              CSaveStorage();
//DLL_IMPORT BOOL              CLoadStorage();
//DLL_IMPORT TrinityErrorCode  CGetTrinityImageSignature(Storage::LocalMemoryStorage::PTRINITY_IMAGE_SIGNATURE pSignature);
/* Non-logging interfaces */
DLL_IMPORT TrinityErrorCode  CSaveCell(cellid_t cellid, char* buf, int32_t size, uint16_t type);
DLL_IMPORT TrinityErrorCode  CAddCell(cellid_t cellid, char* buf, int32_t size, uint16_t type);
DLL_IMPORT TrinityErrorCode  CUpdateCell(cellid_t cellid, char* buf, int32_t size);
DLL_IMPORT TrinityErrorCode  CRemoveCell(cellid_t cellid);
/* Logging interfaces */
//DLL_IMPORT TrinityErrorCode  CLoggedSaveCell(cellid_t cellid, char* buf, int32_t size, uint16_t type, CellAccessOptions options);
//DLL_IMPORT TrinityErrorCode  CLoggedAddCell(cellid_t cellid, char* buf, int32_t size, uint16_t type, CellAccessOptions options);
//DLL_IMPORT TrinityErrorCode  CLoggedUpdateCell(cellid_t cellid, char* buf, int32_t size, CellAccessOptions options);
//DLL_IMPORT TrinityErrorCode  CLoggedRemoveCell(cellid_t cellid, CellAccessOptions options);
//DLL_IMPORT void              CWriteAheadLog(cellid_t cellid, char* buf, int32_t size, uint16_t type, CellAccessOptions options);
//DLL_IMPORT void              CSetWriteAheadLogFile(FILE* fp);
//DLL_IMPORT void              CWriteAheadLogComputeChecksum(Storage::LocalMemoryStorage::Logging::PLOG_RECORD_HEADER plog, char* buffer);
//DLL_IMPORT BOOL              CWriteAheadLogValidateChecksum(Storage::LocalMemoryStorage::Logging::PLOG_RECORD_HEADER plog, char* buffer);

DLL_IMPORT TrinityErrorCode  CResizeCell(cellid_t cellId, int32_t cellEntryIndex, int32_t offset, int32_t delta, char*& cell_ptr);
DLL_IMPORT TrinityErrorCode  CGetCellType(cellid_t cellId, uint16_t& cellType);
DLL_IMPORT void              CReleaseCellLock(cellid_t cellId, int32_t cellEntryIndex);
DLL_IMPORT BOOL              CContains(cellid_t cellid);

//DLL_IMPORT TrinityErrorCode  CLocalMemoryStorageEnumeratorAllocate(Storage::LocalMemoryStorage::Enumeration::LOCAL_MEMORY_STORAGE_ENUMERATOR *& pp_enum);
//DLL_IMPORT TrinityErrorCode  CLocalMemoryStorageEnumeratorDeallocate(Storage::LocalMemoryStorage::Enumeration::LOCAL_MEMORY_STORAGE_ENUMERATOR * p_enum);
//DLL_IMPORT TrinityErrorCode  CLocalMemoryStorageEnumeratorMoveNext(Storage::LocalMemoryStorage::Enumeration::LOCAL_MEMORY_STORAGE_ENUMERATOR * p_enum);
//DLL_IMPORT TrinityErrorCode  CLocalMemoryStorageEnumeratorReset(Storage::LocalMemoryStorage::Enumeration::LOCAL_MEMORY_STORAGE_ENUMERATOR * p_enum);

DLL_IMPORT PTHREAD_CONTEXT   CThreadContextAllocate();
DLL_IMPORT void              CThreadContextDeallocate(PTHREAD_CONTEXT pctx);
DLL_IMPORT void              CThreadContextSet(PTHREAD_CONTEXT pctx);

DLL_IMPORT void              SetDefragmentationPaused(bool value);
DLL_IMPORT void              RestartDefragmentation();

DLL_IMPORT uint64_t          CTrunkCommittedMemorySize();
DLL_IMPORT uint64_t          CMTHashCommittedMemorySize();
DLL_IMPORT uint64_t          CTotalCommittedMemorySize();
DLL_IMPORT uint64_t          CTotalCellSize();

DLL_IMPORT TrinityErrorCode  CGetLockedCellInfo4CellAccessor(cellid_t cellId, int32_t &cellSize, uint16_t &type, char* &cellPtr, int32_t &entryIndex);
DLL_IMPORT TrinityErrorCode  CGetLockedCellInfo4LoadCell(cellid_t cellId, int32_t &cellSize, char* &cellPtr, int32_t &entryIndex);
DLL_IMPORT TrinityErrorCode  CGetLockedCellInfo4AddOrUseCell(cellid_t cellId, int32_t &cellSize, uint16_t type, char* &cellPtr, int32_t &entryIndex);
DLL_IMPORT TrinityErrorCode  CLockedGetCellSize(cellid_t cellId, int32_t entryIndex, int32_t &size);

//not exposed
//DLL_IMPORT void CStartDebugger(bool suspendOthers);
//DLL_IMPORT int32_t CGetTrunkId(cellid_t cellid);
//DLL_IMPORT void StopDefragAndAwaitCeased();

// Tx

/* Non-logging interfaces */
DLL_IMPORT TrinityErrorCode  TxCSaveCell(PTHREAD_CONTEXT ctx, cellid_t cellid, char* buf, int32_t size, uint16_t type);
DLL_IMPORT TrinityErrorCode  TxCAddCell(PTHREAD_CONTEXT ctx, cellid_t cellid, char* buf, int32_t size, uint16_t type);
DLL_IMPORT TrinityErrorCode  TxCUpdateCell(PTHREAD_CONTEXT ctx, cellid_t cellid, char* buf, int32_t size);
DLL_IMPORT TrinityErrorCode  TxCRemoveCell(PTHREAD_CONTEXT ctx, cellid_t cellid);
/* Logging interfaces */
//DLL_IMPORT TrinityErrorCode  TxCLoggedSaveCell(PTHREAD_CONTEXT ctx, cellid_t cellid, char* buf, int32_t size, uint16_t type, CellAccessOptions options);
//DLL_IMPORT TrinityErrorCode  TxCLoggedAddCell(PTHREAD_CONTEXT ctx, cellid_t cellid, char* buf, int32_t size, uint16_t type, CellAccessOptions options);
//DLL_IMPORT TrinityErrorCode  TxCLoggedUpdateCell(PTHREAD_CONTEXT ctx, cellid_t cellid, char* buf, int32_t size, CellAccessOptions options);
//DLL_IMPORT TrinityErrorCode  TxCLoggedRemoveCell(PTHREAD_CONTEXT ctx, cellid_t cellid, CellAccessOptions options);

DLL_IMPORT TrinityErrorCode  TxCResizeCell(PTHREAD_CONTEXT ctx, cellid_t cellId, int32_t cellEntryIndex, int32_t offset, int32_t delta, char*& cell_ptr);
DLL_IMPORT TrinityErrorCode  TxCGetCellType(PTHREAD_CONTEXT ctx, cellid_t cellId, uint16_t& cellType);
DLL_IMPORT void              TxCReleaseCellLock(PTHREAD_CONTEXT ctx, cellid_t cellId, int32_t cellEntryIndex);

DLL_IMPORT TrinityErrorCode  TxCGetLockedCellInfo4CellAccessor(PTHREAD_CONTEXT ctx, cellid_t cellId, int32_t &cellSize, uint16_t &type, char* &cellPtr, int32_t &entryIndex);
DLL_IMPORT TrinityErrorCode  TxCGetLockedCellInfo4LoadCell(PTHREAD_CONTEXT ctx, cellid_t cellId, int32_t &cellSize, char* &cellPtr, int32_t &entryIndex);
DLL_IMPORT TrinityErrorCode  TxCGetLockedCellInfo4AddOrUseCell(PTHREAD_CONTEXT ctx, cellid_t cellId, int32_t &cellSize, uint16_t type, char* &cellPtr, int32_t &entryIndex);


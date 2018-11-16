// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
// Trinity.h: public interfaces

#pragma once
#include "TrinityCommon.h"
#include "Trinity/String.h"
#include "Trinity/Log.h"
#include "Trinity/Events.h"

#if defined(Trinity_EXPORTS)
#define TRINITY_API DLL_EXPORT
#else
#define TRINITY_API DLL_IMPORT
#endif

typedef struct THREAD_CONTEXT* PTHREAD_CONTEXT;

//====================== LocalMemoryStorage ==========================//
TRINITY_API TrinityErrorCode  CInitialize();
TRINITY_API uint64_t          CCellCount();
TRINITY_API BOOL              CResetStorage();
TRINITY_API void              CDispose();

TRINITY_API BOOL              CSaveStorage();
TRINITY_API BOOL              CLoadStorage();
//TRINITY_API TrinityErrorCode  CGetTrinityImageSignature(Storage::LocalMemoryStorage::PTRINITY_IMAGE_SIGNATURE pSignature);
/* Non-logging interfaces */
TRINITY_API TrinityErrorCode  CSaveCell(cellid_t cellid, char* buf, int32_t size, uint16_t type);
TRINITY_API TrinityErrorCode  CAddCell(cellid_t cellid, char* buf, int32_t size, uint16_t type);
TRINITY_API TrinityErrorCode  CUpdateCell(cellid_t cellid, char* buf, int32_t size);
TRINITY_API TrinityErrorCode  CRemoveCell(cellid_t cellid);
/* Logging interfaces */
TRINITY_API TrinityErrorCode  CLoggedSaveCell(cellid_t cellid, char* buf, int32_t size, uint16_t type, CellAccessOptions options);
TRINITY_API TrinityErrorCode  CLoggedAddCell(cellid_t cellid, char* buf, int32_t size, uint16_t type, CellAccessOptions options);
TRINITY_API TrinityErrorCode  CLoggedUpdateCell(cellid_t cellid, char* buf, int32_t size, CellAccessOptions options);
TRINITY_API TrinityErrorCode  CLoggedRemoveCell(cellid_t cellid, CellAccessOptions options);
TRINITY_API void              CWriteAheadLog(cellid_t cellid, char* buf, int32_t size, uint16_t type, CellAccessOptions options);
TRINITY_API void              CSetWriteAheadLogFile(FILE* fp);
//TRINITY_API void              CWriteAheadLogComputeChecksum(Storage::LocalMemoryStorage::Logging::PLOG_RECORD_HEADER plog, char* buffer);
//TRINITY_API BOOL              CWriteAheadLogValidateChecksum(Storage::LocalMemoryStorage::Logging::PLOG_RECORD_HEADER plog, char* buffer);

TRINITY_API TrinityErrorCode  CResizeCell(cellid_t cellId, int32_t cellEntryIndex, int32_t offset, int32_t delta, char*& cell_ptr);
TRINITY_API TrinityErrorCode  CGetCellType(cellid_t cellId, uint16_t& cellType);
TRINITY_API void              CReleaseCellLock(cellid_t cellId, int32_t cellEntryIndex);
TRINITY_API BOOL              CContains(cellid_t cellid);

//TRINITY_API TrinityErrorCode  CLocalMemoryStorageEnumeratorAllocate(Storage::LocalMemoryStorage::Enumeration::LOCAL_MEMORY_STORAGE_ENUMERATOR *& pp_enum);
//TRINITY_API TrinityErrorCode  CLocalMemoryStorageEnumeratorDeallocate(Storage::LocalMemoryStorage::Enumeration::LOCAL_MEMORY_STORAGE_ENUMERATOR * p_enum);
//TRINITY_API TrinityErrorCode  CLocalMemoryStorageEnumeratorMoveNext(Storage::LocalMemoryStorage::Enumeration::LOCAL_MEMORY_STORAGE_ENUMERATOR * p_enum);
//TRINITY_API TrinityErrorCode  CLocalMemoryStorageEnumeratorReset(Storage::LocalMemoryStorage::Enumeration::LOCAL_MEMORY_STORAGE_ENUMERATOR * p_enum);

TRINITY_API PTHREAD_CONTEXT   CThreadContextAllocate();
TRINITY_API void              CThreadContextDeallocate(PTHREAD_CONTEXT pctx);
TRINITY_API void              CThreadContextSet(PTHREAD_CONTEXT pctx);

TRINITY_API void              SetDefragmentationPaused(bool value);
TRINITY_API void              RestartDefragmentation();

TRINITY_API uint64_t          CTrunkCommittedMemorySize();
TRINITY_API uint64_t          CMTHashCommittedMemorySize();
TRINITY_API uint64_t          CTotalCommittedMemorySize();
TRINITY_API uint64_t          CTotalCellSize();

TRINITY_API TrinityErrorCode  CGetLockedCellInfo4CellAccessor(cellid_t cellId, int32_t &cellSize, uint16_t &type, char* &cellPtr, int32_t &entryIndex);
TRINITY_API TrinityErrorCode  CGetLockedCellInfo4LoadCell(cellid_t cellId, int32_t &cellSize, char* &cellPtr, int32_t &entryIndex);
TRINITY_API TrinityErrorCode  CGetLockedCellInfo4AddOrUseCell(cellid_t cellId, int32_t &cellSize, uint16_t type, char* &cellPtr, int32_t &entryIndex);
TRINITY_API TrinityErrorCode  CLockedGetCellSize(cellid_t cellId, int32_t entryIndex, int32_t &size);

//not exposed
//TRINITY_API void CStartDebugger(bool suspendOthers);
//TRINITY_API int32_t CGetTrunkId(cellid_t cellid);
//TRINITY_API void StopDefragAndAwaitCeased();

// Tx

/* Non-logging interfaces */
TRINITY_API TrinityErrorCode  TxCSaveCell(PTHREAD_CONTEXT ctx, cellid_t cellid, char* buf, int32_t size, uint16_t type);
TRINITY_API TrinityErrorCode  TxCAddCell(PTHREAD_CONTEXT ctx, cellid_t cellid, char* buf, int32_t size, uint16_t type);
TRINITY_API TrinityErrorCode  TxCUpdateCell(PTHREAD_CONTEXT ctx, cellid_t cellid, char* buf, int32_t size);
TRINITY_API TrinityErrorCode  TxCRemoveCell(PTHREAD_CONTEXT ctx, cellid_t cellid);
/* Logging interfaces */
TRINITY_API TrinityErrorCode  TxCLoggedSaveCell(PTHREAD_CONTEXT ctx, cellid_t cellid, char* buf, int32_t size, uint16_t type, CellAccessOptions options);
TRINITY_API TrinityErrorCode  TxCLoggedAddCell(PTHREAD_CONTEXT ctx, cellid_t cellid, char* buf, int32_t size, uint16_t type, CellAccessOptions options);
TRINITY_API TrinityErrorCode  TxCLoggedUpdateCell(PTHREAD_CONTEXT ctx, cellid_t cellid, char* buf, int32_t size, CellAccessOptions options);
TRINITY_API TrinityErrorCode  TxCLoggedRemoveCell(PTHREAD_CONTEXT ctx, cellid_t cellid, CellAccessOptions options);

TRINITY_API TrinityErrorCode  TxCResizeCell(PTHREAD_CONTEXT ctx, cellid_t cellId, int32_t cellEntryIndex, int32_t offset, int32_t delta, char*& cell_ptr);
TRINITY_API TrinityErrorCode  TxCGetCellType(PTHREAD_CONTEXT ctx, cellid_t cellId, uint16_t& cellType);
TRINITY_API void              TxCReleaseCellLock(PTHREAD_CONTEXT ctx, cellid_t cellId, int32_t cellEntryIndex);

TRINITY_API TrinityErrorCode  TxCGetLockedCellInfo4CellAccessor(PTHREAD_CONTEXT ctx, cellid_t cellId, int32_t &cellSize, uint16_t &type, char* &cellPtr, int32_t &entryIndex);
TRINITY_API TrinityErrorCode  TxCGetLockedCellInfo4LoadCell(PTHREAD_CONTEXT ctx, cellid_t cellId, int32_t &cellSize, char* &cellPtr, int32_t &entryIndex);
TRINITY_API TrinityErrorCode  TxCGetLockedCellInfo4AddOrUseCell(PTHREAD_CONTEXT ctx, cellid_t cellId, int32_t &cellSize, uint16_t type, char* &cellPtr, int32_t &entryIndex);

//====================== TrinityConfig ==========================//
TRINITY_API BOOL              CHandshake();
TRINITY_API VOID              CSetHandshake(bool value);
TRINITY_API BOOL              CClientDisableSendBuffer();
TRINITY_API VOID              CSetClientDisableSendBuffer(bool value);
TRINITY_API VOID              SetStorageRoot(const u16char* buffer, int32_t length);
TRINITY_API BOOL              CReadOnly();
TRINITY_API void              CSetReadOnly(bool value);
TRINITY_API int32_t           CTrunkCount();
TRINITY_API void              CSetTrunkCount(int32_t value);
TRINITY_API int32_t           GetStorageCapacityProfile();
TRINITY_API void              SetStorageCapacityProfile(int32_t value);
TRINITY_API int32_t           CLargeObjectThreshold();
TRINITY_API void              CSetLargeObjectThreshold(int32_t value);
TRINITY_API void              CSetGCDefragInterval(int32_t value);

//====================== Networking Client ==========================//

TRINITY_API uint64_t CreateClientSocket();
TRINITY_API BOOL ClientSocketConnect(uint64_t socket, uint32_t ip, uint16_t port);
TRINITY_API BOOL ClientSend(uint64_t socket, char* buf, int32_t len);
TRINITY_API BOOL ClientSendMulti(uint64_t socket, char** bufs, int32_t* lens, int32_t cnt);
TRINITY_API TrinityErrorCode ClientReceive(uint64_t socket, char* &buf, int32_t &len);
TRINITY_API TrinityErrorCode WaitForAckPackage(uint64_t socket);
TRINITY_API void CloseClientSocket(uint64_t socket);

//====================== Networking Server ==========================//
TRINITY_API int StartSocketServer(uint16_t port);
TRINITY_API int StopSocketServer();

//====================== Logging ==========================//
TRINITY_API VOID CLogOpenFile(const u16char* logDir);
TRINITY_API VOID CLogCloseFile();
TRINITY_API VOID CLogInitialize();
TRINITY_API VOID CLogUninitialize();
TRINITY_API VOID CLogWriteLine(Trinity::Diagnostics::LogLevel logLevel, const u16char* str);

TRINITY_API VOID CLogFlush();
TRINITY_API VOID CLogSetLogLevel(Trinity::Diagnostics::LogLevel level);
TRINITY_API VOID CLogSetEchoOnConsole(bool is_set);
TRINITY_API TrinityErrorCode CLogCollectEntries(OUT size_t& arr_size, OUT Trinity::Diagnostics::PLOG_ENTRY& entries);

//====================== Background tasks ==========================//
TRINITY_API void CStartBackgroundThread();
TRINITY_API void CStopBackgroundThread();

//====================== Eventloop ==========================//
TRINITY_API TrinityErrorCode StartEventLoop();
TRINITY_API TrinityErrorCode StopEventLoop();
TRINITY_API TrinityErrorCode RegisterMessageHandler(uint16_t msgId, void * handler);
TRINITY_API void LocalSendMessage(Trinity::Events::message_t* message);
TRINITY_API TrinityErrorCode PostCompute(Trinity::Events::compute_handler_t* pcompute, void* pdata);

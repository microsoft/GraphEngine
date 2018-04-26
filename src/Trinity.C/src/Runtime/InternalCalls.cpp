// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "TrinityCommon.h"
#include "Memory/Memory.h"
#include "Mathematics/TrinityMath.h"
#include "Storage/LocalStorage/LocalMemoryStorage.h"
#include "Storage/LocalStorage/ThreadContext.h"
#include "Trinity/Configuration/TrinityConfig.h"
#include "Storage/LocalStorage/GCTask.h"
#include "Utility/FileIO.h"
#include <diagnostics>
#include "Debugger/Debugger.h"
#include "Trinity/Configuration/TrinityConfig.h"

using Storage::CellAccessOptions;
using Storage::PTHREAD_CONTEXT;

//====================== CStdio ==========================//
DLL_EXPORT int32_t           C_wfopen_s(FILE** _File, u16char* _Filename, u16char* _Mode)
{
#ifdef TRINITY_PLATFORM_WINDOWS
    return _wfopen_s(_File, _Filename, _Mode);
#else
    return _wfopen_cswrapper(_File, _Filename, _Mode);
#endif
}
DLL_EXPORT uint64_t          C_fread(void* _DstBuf, size_t _ElementSize, size_t _Count, FILE* _File) { return fread(_DstBuf, _ElementSize, _Count, _File); }
DLL_EXPORT uint64_t          C_fwrite(void* _DstBuf, size_t _ElementSize, size_t _Count, FILE* _File) { return fwrite(_DstBuf, _ElementSize, _Count, _File); }
DLL_EXPORT int               C_fflush(FILE* _File) { return fflush(_File); }
DLL_EXPORT int               C_fclose(FILE* _File) { return fclose(_File); }
DLL_EXPORT int               C_feof(FILE* _File) { return feof(_File); }
//====================== Memory ==========================//
DLL_EXPORT void              Copy(char* src, char* dst, int32_t size) { Memory::Copy(src, dst, size); }
DLL_EXPORT void*             C_malloc(uint64_t size) { return malloc(size); }
DLL_EXPORT void*             C_realloc(char* _Memory, uint64_t _NewSize) { return realloc(_Memory, _NewSize); }
DLL_EXPORT void*             C_memset(char* dst, int32_t val, uint64_t count) { return memset(dst, val, count); }
DLL_EXPORT void*             C_memcpy(void* dst, void* src, uint64_t count) { return memcpy(dst, src, count); }
DLL_EXPORT void              C_free(void* buff) { free(buff); }
DLL_EXPORT void*             C_memmove(void* dst, void* src, uint64_t count) { return memmove(dst, src, count); }
DLL_EXPORT int               C_memcmp(void* dst, void* src, uint64_t count) { return memcmp(dst, src, count); }
DLL_EXPORT void*             C_aligned_malloc(uint64_t size, uint64_t alignment)
{
#ifdef TRINITY_PLATFORM_WINDOWS
    return _aligned_malloc(size, alignment);
#else
    return aligned_alloc(size, alignment);
#endif
}
DLL_EXPORT void              C_aligned_free(void* buff)
{
#ifdef TRINITY_PLATFORM_WINDOWS
    _aligned_free(buff);
#else
    free(buff);
#endif
}
DLL_EXPORT void*             AlignedAlloc(uint64_t size, uint64_t alignment) { return Memory::AlignedAlloc(size, alignment); }
DLL_EXPORT void              SetWorkingSetProfile(int profile) { Memory::SetWorkingSetProfile(profile); }
DLL_EXPORT void              SetMaxWorkingSet(uint64_t size) { Memory::SetMaxWorkingSet(size); }
//void SetMemoryAllocationProfile(MemoryAllocationProfile profile) {Memory::}
//====================== Math ==========================//
DLL_EXPORT double            C_multiply_double_vector(double* dv1, double* dv2, int32_t count) { return multiply_double_vector(dv1, dv2, count); }
DLL_EXPORT double            C_multiply_sparse_double_vector(double* dv1, double* dv2, int32_t* index, int32_t count) { return multiply_sparse_double_vector(dv1, dv2, index, count); }
//====================== Error ==========================//
DLL_EXPORT int32_t           C_GetLastError()
{
#ifdef TRINITY_PLATFORM_WINDOWS
    return GetLastError();
#else
    return errno;
#endif
}
//====================== LocalMemoryStorage ==========================//
DLL_EXPORT TrinityErrorCode  CInitialize() { return Storage::LocalMemoryStorage::Initialize(); }
DLL_EXPORT uint64_t          CCellCount() { return Storage::LocalMemoryStorage::CellCount(); }
DLL_EXPORT BOOL              CResetStorage() { return Storage::LocalMemoryStorage::ResetStorage() ? TRUE : FALSE; }
DLL_EXPORT void              CDispose() { Storage::LocalMemoryStorage::Dispose(); }

DLL_EXPORT BOOL              CSaveStorage() { return Storage::LocalMemoryStorage::SaveStorage() ? TRUE : FALSE; }
DLL_EXPORT BOOL              CLoadStorage() { return Storage::LocalMemoryStorage::LoadStorage() ? TRUE : FALSE; }
DLL_EXPORT TrinityErrorCode  CGetTrinityImageSignature(Storage::LocalMemoryStorage::PTRINITY_IMAGE_SIGNATURE pSignature) { return Storage::LocalMemoryStorage::GetTrinityImageSignature(pSignature); }
/* Non-logging interfaces */
DLL_EXPORT TrinityErrorCode  CSaveCell(cellid_t cellid, char* buf, int32_t size, uint16_t type) { return Storage::LocalMemoryStorage::SaveCell(cellid, buf, size, type); }
DLL_EXPORT TrinityErrorCode  CAddCell(cellid_t cellid, char* buf, int32_t size, uint16_t type) { return Storage::LocalMemoryStorage::AddCell(cellid, buf, size, type); }
DLL_EXPORT TrinityErrorCode  CUpdateCell(cellid_t cellid, char* buf, int32_t size) { return Storage::LocalMemoryStorage::UpdateCell(cellid, buf, size); }
DLL_EXPORT TrinityErrorCode  CRemoveCell(cellid_t cellid) { return Storage::LocalMemoryStorage::RemoveCell(cellid); }
/* Logging interfaces */
DLL_EXPORT TrinityErrorCode  CLoggedSaveCell(cellid_t cellid, char* buf, int32_t size, uint16_t type, CellAccessOptions options) { return Storage::LocalMemoryStorage::SaveCell(cellid, buf, size, type, options); }
DLL_EXPORT TrinityErrorCode  CLoggedAddCell(cellid_t cellid, char* buf, int32_t size, uint16_t type, CellAccessOptions options) { return Storage::LocalMemoryStorage::AddCell(cellid, buf, size, type, options); }
DLL_EXPORT TrinityErrorCode  CLoggedUpdateCell(cellid_t cellid, char* buf, int32_t size, CellAccessOptions options) { return Storage::LocalMemoryStorage::UpdateCell(cellid, buf, size, options); }
DLL_EXPORT TrinityErrorCode  CLoggedRemoveCell(cellid_t cellid, CellAccessOptions options) { return Storage::LocalMemoryStorage::RemoveCell(cellid, options); }
DLL_EXPORT void              CWriteAheadLog(cellid_t cellid, char* buf, int32_t size, uint16_t type, CellAccessOptions options) { Storage::LocalMemoryStorage::Logging::WriteAheadLog(cellid, buf, size, type, options); }
DLL_EXPORT void              CSetWriteAheadLogFile(FILE* fp) { Storage::LocalMemoryStorage::Logging::SetWriteAheadLogFile(fp); }
DLL_EXPORT void              CWriteAheadLogComputeChecksum(Storage::LocalMemoryStorage::Logging::PLOG_RECORD_HEADER plog, char* buffer) { Storage::LocalMemoryStorage::Logging::ComputeChecksum(plog, buffer); }
DLL_EXPORT BOOL              CWriteAheadLogValidateChecksum(Storage::LocalMemoryStorage::Logging::PLOG_RECORD_HEADER plog, char* buffer) { return Storage::LocalMemoryStorage::Logging::ValidateChecksum(plog, buffer) ? TRUE : FALSE; }

DLL_EXPORT TrinityErrorCode  CResizeCell(cellid_t cellId, int32_t cellEntryIndex, int32_t offset, int32_t delta, char*& cell_ptr) { return Storage::LocalMemoryStorage::ResizeCell(cellId, cellEntryIndex, offset, delta, cell_ptr); }
DLL_EXPORT TrinityErrorCode  CGetCellType(cellid_t cellId, uint16_t& cellType) { return Storage::LocalMemoryStorage::GetCellType(cellId, cellType); }
DLL_EXPORT void              CReleaseCellLock(cellid_t cellId, int32_t cellEntryIndex) { Storage::LocalMemoryStorage::ReleaseCellLock(cellId, cellEntryIndex); }
DLL_EXPORT BOOL              CContains(cellid_t cellid) { return Storage::LocalMemoryStorage::Contains(cellid) ? TRUE : FALSE; }

DLL_EXPORT TrinityErrorCode  CLocalMemoryStorageEnumeratorAllocate(Storage::LocalMemoryStorage::Enumeration::LOCAL_MEMORY_STORAGE_ENUMERATOR *& pp_enum) { return Storage::LocalMemoryStorage::Enumeration::Allocate(pp_enum); }
DLL_EXPORT TrinityErrorCode  CLocalMemoryStorageEnumeratorDeallocate(Storage::LocalMemoryStorage::Enumeration::LOCAL_MEMORY_STORAGE_ENUMERATOR * p_enum) { return Storage::LocalMemoryStorage::Enumeration::Deallocate(p_enum); }
DLL_EXPORT TrinityErrorCode  CLocalMemoryStorageEnumeratorMoveNext(Storage::LocalMemoryStorage::Enumeration::LOCAL_MEMORY_STORAGE_ENUMERATOR * p_enum) { return Storage::LocalMemoryStorage::Enumeration::MoveNext(p_enum); }
DLL_EXPORT TrinityErrorCode  CLocalMemoryStorageEnumeratorReset(Storage::LocalMemoryStorage::Enumeration::LOCAL_MEMORY_STORAGE_ENUMERATOR * p_enum) { return Storage::LocalMemoryStorage::Enumeration::Reset(p_enum); }

DLL_EXPORT PTHREAD_CONTEXT   CThreadContextAllocate() { return Storage::AllocateThreadContext(); }
DLL_EXPORT void              CThreadContextDeallocate(PTHREAD_CONTEXT pctx) { Storage::DeallocateThreadContext(pctx); }
DLL_EXPORT void              CThreadContextSet(PTHREAD_CONTEXT pctx) { Storage::SetCurrentThreadContext(pctx); }

DLL_EXPORT void              SetDefragmentationPaused(bool value) { Storage::GCTask::SetDefragmentationPaused(value); }
DLL_EXPORT void              RestartDefragmentation() { Storage::GCTask::RestartDefragmentation(); }

DLL_EXPORT uint64_t          CTrunkCommittedMemorySize() { return Storage::LocalMemoryStorage::TrunkCommittedMemorySize(); }
DLL_EXPORT uint64_t          CMTHashCommittedMemorySize() { return Storage::LocalMemoryStorage::MTHashCommittedMemorySize(); }
DLL_EXPORT uint64_t          CTotalCommittedMemorySize() { return Storage::LocalMemoryStorage::TotalCommittedMemorySize(); }
DLL_EXPORT uint64_t          CTotalCellSize() { return Storage::LocalMemoryStorage::TotalCellSize(); }

DLL_EXPORT TrinityErrorCode  CGetLockedCellInfo4CellAccessor(cellid_t cellId, int32_t &cellSize, uint16_t &type, char* &cellPtr, int32_t &entryIndex) { return Storage::LocalMemoryStorage::CGetLockedCellInfo4CellAccessor(cellId, cellSize, type, cellPtr, entryIndex); }
DLL_EXPORT TrinityErrorCode  CGetLockedCellInfo4LoadCell(cellid_t cellId, int32_t &cellSize, char* &cellPtr, int32_t &entryIndex) { return Storage::LocalMemoryStorage::CGetLockedCellInfo4LoadCell(cellId, cellSize, cellPtr, entryIndex); }
DLL_EXPORT TrinityErrorCode  CGetLockedCellInfo4AddOrUseCell(cellid_t cellId, int32_t &cellSize, uint16_t type, char* &cellPtr, int32_t &entryIndex) { return Storage::LocalMemoryStorage::CGetLockedCellInfo4AddOrUseCell(cellId, cellSize, type, cellPtr, entryIndex); }
DLL_EXPORT TrinityErrorCode  CLockedGetCellSize(cellid_t cellId, int32_t entryIndex, int32_t &size) { return Storage::LocalMemoryStorage::CLockedGetCellSize(cellId, entryIndex, size); }

//not exposed
//DLL_EXPORT void CStartDebugger(bool suspendOthers) { Trinity::Debugger::TryStartDebugger(suspendOthers); }
//DLL_EXPORT int32_t CGetTrunkId(cellid_t cellid) { return Storage::LocalMemoryStorage::GetTrunkId(cellid); }
//DLL_EXPORT void StopDefragAndAwaitCeased() { return Storage::GCTask::StopDefragAndAwaitCeased(); }

// Tx

/* Non-logging interfaces */
DLL_EXPORT TrinityErrorCode  TxCSaveCell(PTHREAD_CONTEXT ctx, cellid_t cellid, char* buf, int32_t size, uint16_t type) { return Storage::LocalMemoryStorage::TxSaveCell(ctx, cellid, buf, size, type); }
DLL_EXPORT TrinityErrorCode  TxCAddCell(PTHREAD_CONTEXT ctx, cellid_t cellid, char* buf, int32_t size, uint16_t type) { return Storage::LocalMemoryStorage::TxAddCell(ctx, cellid, buf, size, type); }
DLL_EXPORT TrinityErrorCode  TxCUpdateCell(PTHREAD_CONTEXT ctx, cellid_t cellid, char* buf, int32_t size) { return Storage::LocalMemoryStorage::TxUpdateCell(ctx, cellid, buf, size); }
DLL_EXPORT TrinityErrorCode  TxCRemoveCell(PTHREAD_CONTEXT ctx, cellid_t cellid) { return Storage::LocalMemoryStorage::TxRemoveCell(ctx, cellid); }
/* Logging interfaces */
DLL_EXPORT TrinityErrorCode  TxCLoggedSaveCell(PTHREAD_CONTEXT ctx, cellid_t cellid, char* buf, int32_t size, uint16_t type, CellAccessOptions options) { return Storage::LocalMemoryStorage::TxSaveCell(ctx, cellid, buf, size, type, options); }
DLL_EXPORT TrinityErrorCode  TxCLoggedAddCell(PTHREAD_CONTEXT ctx, cellid_t cellid, char* buf, int32_t size, uint16_t type, CellAccessOptions options) { return Storage::LocalMemoryStorage::TxAddCell(ctx, cellid, buf, size, type, options); }
DLL_EXPORT TrinityErrorCode  TxCLoggedUpdateCell(PTHREAD_CONTEXT ctx, cellid_t cellid, char* buf, int32_t size, CellAccessOptions options) { return Storage::LocalMemoryStorage::TxUpdateCell(ctx, cellid, buf, size, options); }
DLL_EXPORT TrinityErrorCode  TxCLoggedRemoveCell(PTHREAD_CONTEXT ctx, cellid_t cellid, CellAccessOptions options) { return Storage::LocalMemoryStorage::TxRemoveCell(ctx, cellid, options); }

DLL_EXPORT TrinityErrorCode  TxCResizeCell(PTHREAD_CONTEXT ctx, cellid_t cellId, int32_t cellEntryIndex, int32_t offset, int32_t delta, char*& cell_ptr) { return Storage::LocalMemoryStorage::TxResizeCell(ctx, cellId, cellEntryIndex, offset, delta, cell_ptr); }
DLL_EXPORT TrinityErrorCode  TxCGetCellType(PTHREAD_CONTEXT ctx, cellid_t cellId, uint16_t& cellType) { return Storage::LocalMemoryStorage::TxGetCellType(ctx, cellId, cellType); }
DLL_EXPORT void              TxCReleaseCellLock(PTHREAD_CONTEXT ctx, cellid_t cellId, int32_t cellEntryIndex) { Storage::LocalMemoryStorage::TxReleaseCellLock(ctx, cellId, cellEntryIndex); }

DLL_EXPORT TrinityErrorCode  TxCGetLockedCellInfo4CellAccessor(PTHREAD_CONTEXT ctx, cellid_t cellId, int32_t &cellSize, uint16_t &type, char* &cellPtr, int32_t &entryIndex) { return Storage::LocalMemoryStorage::TxCGetLockedCellInfo4CellAccessor(ctx, cellId, cellSize, type, cellPtr, entryIndex); }
DLL_EXPORT TrinityErrorCode  TxCGetLockedCellInfo4LoadCell(PTHREAD_CONTEXT ctx, cellid_t cellId, int32_t &cellSize, char* &cellPtr, int32_t &entryIndex) { return Storage::LocalMemoryStorage::TxCGetLockedCellInfo4LoadCell(ctx, cellId, cellSize, cellPtr, entryIndex); }
DLL_EXPORT TrinityErrorCode  TxCGetLockedCellInfo4AddOrUseCell(PTHREAD_CONTEXT ctx, cellid_t cellId, int32_t &cellSize, uint16_t type, char* &cellPtr, int32_t &entryIndex) { return Storage::LocalMemoryStorage::TxCGetLockedCellInfo4AddOrUseCell(ctx, cellId, cellSize, type, cellPtr, entryIndex); }

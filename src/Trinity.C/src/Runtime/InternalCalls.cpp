// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "TrinityCommon.h"
#include "InternalCalls.h"
#include "Memory/Memory.h"
#include "Mathematics/TrinityMath.h"
#include "Storage/LocalStorage/LocalMemoryStorage.h"
#include "Trinity/Configuration/TrinityConfig.h"
#include "Storage/LocalStorage/GCTask.h"
#include "Utility/FileIO.h"
#include <diagnostics>
#include "Debugger/Debugger.h"
#include "Trinity/Configuration/TrinityConfig.h"

using Storage::LocalMemoryStorage::CellAccessOptions;

#ifndef CORECLR
//! See InternalCall.cs for explanation of entry names.
ICallEntry ICallTable[] =
{
    //====================== CStdio ==========================//

#ifdef TRINITY_PLATFORM_WINDOWS
    { "Trinity.CStdio::C_wfopen_s"                                                  , &_wfopen_s },
#else
    { "Trinity.CStdio::C_wfopen_s"                                                  , &_wfopen_cswrapper },
#endif
    { "Trinity.CStdio::C_fread"                                                     , &fread },
    { "Trinity.CStdio::C_fwrite"                                                    , &fwrite },
    { "Trinity.CStdio::C_fflush"                                                    , &fflush },
    { "Trinity.CStdio::C_fclose"                                                    , &fclose },
    { "Trinity.CStdio::C_feof"                                                      , &feof },

    //====================== Memory ==========================//

    { "Trinity.Core.Lib.CMemory::Copy(void*,void*,int)"                             , &Memory::Copy },
    { "Trinity.Core.Lib.CMemory::C_malloc"                                          , &malloc },
    { "Trinity.Core.Lib.CMemory::C_realloc"                                         , &realloc },
    { "Trinity.Core.Lib.CMemory::C_memset"                                          , &memset },
    { "Trinity.Core.Lib.CMemory::C_memcpy"                                          , &memcpy },
    { "Trinity.Core.Lib.CMemory::C_free"                                            , &free },
    { "Trinity.Core.Lib.CMemory::C_memmove"                                         , &memmove },
    { "Trinity.Core.Lib.CMemory::C_memcmp"                                          , &memcmp },
#if defined(TRINITY_PLATFORM_WINDOWS)
    { "Trinity.Core.Lib.CMemory::C_aligned_malloc"                                  , &_aligned_malloc },
    { "Trinity.Core.Lib.CMemory::C_aligned_free"                                    , &_aligned_free },
#else
    { "Trinity.Core.Lib.CMemory::C_aligned_malloc"                                  , &aligned_alloc },
    { "Trinity.Core.Lib.CMemory::C_aligned_free"                                    , &free },
#endif
    { "Trinity.Core.Lib.CMemory::AlignedAlloc"                                      , &Memory::AlignedAlloc },
    { "Trinity.Core.Lib.CMemory::SetWorkingSetProfile"                              , &Memory::SetWorkingSetProfile },
    { "Trinity.Core.Lib.CMemory::SetMaxWorkingSet"                                  , &Memory::SetMaxWorkingSet },

    //====================== Math ==========================//

    { "Trinity.Mathematics.CMathUtility::C_multiply_double_vector"                    , &multiply_double_vector },
    { "Trinity.Mathematics.CMathUtility::C_multiply_sparse_double_vector"             , &multiply_sparse_double_vector },

    //====================== LocalMemoryStorage ==========================//

    { "Trinity.Storage.CLocalMemoryStorage::CInitialize"                            , &Storage::LocalMemoryStorage::Initialize },
    { "Trinity.Storage.CLocalMemoryStorage::CCellCount"                             , &Storage::LocalMemoryStorage::CellCount },
    { "Trinity.Storage.CLocalMemoryStorage::CResetStorage"                          , &Storage::LocalMemoryStorage::ResetStorage },
    { "Trinity.Storage.CLocalMemoryStorage::CDispose"                               , &Storage::LocalMemoryStorage::Dispose },

    { "Trinity.Storage.CLocalMemoryStorage::CSaveStorage"                           , &Storage::LocalMemoryStorage::SaveStorage },
    { "Trinity.Storage.CLocalMemoryStorage::CLoadStorage"                           , &Storage::LocalMemoryStorage::LoadStorage },
    { "Trinity.Storage.CLocalMemoryStorage::CGetTrinityImageSignature"              , &Storage::LocalMemoryStorage::GetTrinityImageSignature },

    /* Non-logging interfaces */
    { "Trinity.Storage.CLocalMemoryStorage::CSaveCell(long,byte*,int,uint16)"       , static_cast<TrinityErrorCode(*)(cellid_t, char*, int32_t, uint16_t)>(&Storage::LocalMemoryStorage::SaveCell) },
    { "Trinity.Storage.CLocalMemoryStorage::CAddCell(long,byte*,int,uint16)"        , static_cast<TrinityErrorCode(*)(cellid_t, char*, int32_t, uint16_t)>(&Storage::LocalMemoryStorage::AddCell) },
    { "Trinity.Storage.CLocalMemoryStorage::CUpdateCell(long,byte*,int)"            , static_cast<TrinityErrorCode(*)(cellid_t, char*, int32_t)>(&Storage::LocalMemoryStorage::UpdateCell) },
    { "Trinity.Storage.CLocalMemoryStorage::CRemoveCell(long)"                      , static_cast<TrinityErrorCode(*)(cellid_t)>(&Storage::LocalMemoryStorage::RemoveCell) },

    /* Logging interfaces */
    { "Trinity.Storage.CLocalMemoryStorage::CLoggedSaveCell(long,byte*,int,uint16,Trinity.TSL.Lib.CellAccessOptions)", static_cast<TrinityErrorCode(*)(cellid_t, char*, int32_t, uint16_t, CellAccessOptions)>(&Storage::LocalMemoryStorage::SaveCell) },
    { "Trinity.Storage.CLocalMemoryStorage::CLoggedAddCell(long,byte*,int,uint16,Trinity.TSL.Lib.CellAccessOptions)", static_cast<TrinityErrorCode(*)(cellid_t, char*, int32_t, uint16_t, CellAccessOptions)>(&Storage::LocalMemoryStorage::AddCell) },
    { "Trinity.Storage.CLocalMemoryStorage::CLoggedUpdateCell(long,byte*,int,Trinity.TSL.Lib.CellAccessOptions)", static_cast<TrinityErrorCode(*)(cellid_t, char*, int32_t, CellAccessOptions)>(&Storage::LocalMemoryStorage::UpdateCell) },
    { "Trinity.Storage.CLocalMemoryStorage::CLoggedRemoveCell(long,Trinity.TSL.Lib.CellAccessOptions)", static_cast<TrinityErrorCode(*)(cellid_t, CellAccessOptions)>(&Storage::LocalMemoryStorage::RemoveCell) },

    { "Trinity.Storage.CLocalMemoryStorage::CWriteAheadLog"                         , &Storage::LocalMemoryStorage::Logging::WriteAheadLog },
    { "Trinity.Storage.CLocalMemoryStorage::CSetWriteAheadLogFile"                  , &Storage::LocalMemoryStorage::Logging::SetWriteAheadLogFile },
    { "Trinity.Storage.LOG_RECORD_HEADER::CWriteAheadLogComputeChecksum"            , &Storage::LocalMemoryStorage::Logging::ComputeChecksum },
    { "Trinity.Storage.LOG_RECORD_HEADER::CWriteAheadLogValidateChecksum"           , &Storage::LocalMemoryStorage::Logging::ValidateChecksum },

    { "Trinity.Storage.CLocalMemoryStorage::CResizeCell"                            , &Storage::LocalMemoryStorage::ResizeCell },
    { "Trinity.Storage.CLocalMemoryStorage::CGetCellType"                           , &Storage::LocalMemoryStorage::GetCellType },
    { "Trinity.Storage.CLocalMemoryStorage::CReleaseCellLock"                       , &Storage::LocalMemoryStorage::ReleaseCellLock },
    { "Trinity.Storage.CLocalMemoryStorage::CContains"                              , &Storage::LocalMemoryStorage::Contains },

    { "Trinity.Storage.CLocalMemoryStorage::CLocalMemoryStorageEnumeratorAllocate"  , &Storage::LocalMemoryStorage::Enumeration::Allocate },
    { "Trinity.Storage.CLocalMemoryStorage::CLocalMemoryStorageEnumeratorDeallocate", &Storage::LocalMemoryStorage::Enumeration::Deallocate },
    { "Trinity.Storage.CLocalMemoryStorage::CLocalMemoryStorageEnumeratorMoveNext"  , &Storage::LocalMemoryStorage::Enumeration::MoveNext },
    { "Trinity.Storage.CLocalMemoryStorage::CLocalMemoryStorageEnumeratorReset"     , &Storage::LocalMemoryStorage::Enumeration::Reset },

    { "Trinity.Storage.CLocalMemoryStorage::SetDefragmentationPaused"               , &Storage::GCTask::SetDefragmentationPaused },
    { "Trinity.Storage.CLocalMemoryStorage::RestartDefragmentation"                 , &Storage::GCTask::RestartDefragmentation },

    { "Trinity.Storage.CLocalMemoryStorage::CTrunkCommittedMemorySize"              , &Storage::LocalMemoryStorage::TrunkCommittedMemorySize },
    { "Trinity.Storage.CLocalMemoryStorage::CMTHashCommittedMemorySize"             , &Storage::LocalMemoryStorage::MTHashCommittedMemorySize },
    { "Trinity.Storage.CLocalMemoryStorage::CTotalCommittedMemorySize"              , &Storage::LocalMemoryStorage::TotalCommittedMemorySize },
    { "Trinity.Storage.CLocalMemoryStorage::CTotalCellSize"                         , &Storage::LocalMemoryStorage::TotalCellSize },

    { "Trinity.Storage.CLocalMemoryStorage::CGetLockedCellInfo4CellAccessor"        , &Storage::LocalMemoryStorage::CGetLockedCellInfo4CellAccessor },
    { "Trinity.Storage.CLocalMemoryStorage::CGetLockedCellInfo4SaveCell"            , &Storage::LocalMemoryStorage::CGetLockedCellInfo4SaveCell },
    { "Trinity.Storage.CLocalMemoryStorage::CGetLockedCellInfo4AddCell"             , &Storage::LocalMemoryStorage::CGetLockedCellInfo4AddCell },
    { "Trinity.Storage.CLocalMemoryStorage::CGetLockedCellInfo4UpdateCell"          , &Storage::LocalMemoryStorage::CGetLockedCellInfo4UpdateCell },
    { "Trinity.Storage.CLocalMemoryStorage::CGetLockedCellInfo4LoadCell"            , &Storage::LocalMemoryStorage::CGetLockedCellInfo4LoadCell },
    { "Trinity.Storage.CLocalMemoryStorage::CGetLockedCellInfo4AddOrUseCell"        , &Storage::LocalMemoryStorage::CGetLockedCellInfo4AddOrUseCell },
    { "Trinity.Storage.CLocalMemoryStorage::CLockedGetCellSize"                     , &Storage::LocalMemoryStorage::CLockedGetCellSize },

    { "Trinity.Storage.CLocalMemoryStorage::CStartDebugger"                         , &Trinity::Debugger::TryStartDebugger },
    // the following two entries are not exposed.
    { "Trinity.Storage.CLocalMemoryStorage::CGetTrunkId"                            , &Storage::LocalMemoryStorage::GetTrunkId },
    { "Trinity.Storage.CLocalMemoryStorage::StopDefragAndAwaitCeased"               , &Storage::GCTask::StopDefragAndAwaitCeased },

};


struct MethodDesc
{
    uint64_t m_Flags;
    void*    m_MethodPointer;
};

DLL_EXPORT BOOL __stdcall RegisterInternalCall(void* _MethodDescPtr, char* FunctionName)
{
    MethodDesc *desc = (MethodDesc*)_MethodDescPtr;

    int32_t iCallIndex = -1;

    for (int32_t i = 0; i < sizeof(ICallTable) / sizeof(ICallEntry); ++i)
    {
        if (!strcmp(ICallTable[i].Name, FunctionName))
        {
            iCallIndex = i;
            break;
        }
    }

    if (iCallIndex == -1)//Method not found
        return FALSE;

    uint64_t methodFlags = desc->m_Flags;
    methodFlags |= 0x2000000001000000ULL; // turn on stable point & noinline flags!
    methodFlags &=~0x0001000000000000ULL; // turn off FCall flag!
    if (!Memory::MemoryInject(&(desc->m_Flags), methodFlags)) { return FALSE; }
    if (!Memory::MemoryInject(&(desc->m_MethodPointer), (uint64_t)(ICallTable[iCallIndex].Address))) { return FALSE; }
    return TRUE;
}

#else

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

DLL_EXPORT void              SetDefragmentationPaused(bool value) { Storage::GCTask::SetDefragmentationPaused(value); }
DLL_EXPORT void              RestartDefragmentation() { Storage::GCTask::RestartDefragmentation(); }

DLL_EXPORT uint64_t          CTrunkCommittedMemorySize() { return Storage::LocalMemoryStorage::TrunkCommittedMemorySize(); }
DLL_EXPORT uint64_t          CMTHashCommittedMemorySize() { return Storage::LocalMemoryStorage::MTHashCommittedMemorySize(); }
DLL_EXPORT uint64_t          CTotalCommittedMemorySize() { return Storage::LocalMemoryStorage::TotalCommittedMemorySize(); }
DLL_EXPORT uint64_t          CTotalCellSize() { return Storage::LocalMemoryStorage::TotalCellSize(); }

DLL_EXPORT TrinityErrorCode  CGetLockedCellInfo4CellAccessor(cellid_t cellId, int32_t &cellSize, uint16_t &type, char* &cellPtr, int32_t &entryIndex) { return Storage::LocalMemoryStorage::CGetLockedCellInfo4CellAccessor(cellId, cellSize, type, cellPtr, entryIndex); }
DLL_EXPORT TrinityErrorCode  CGetLockedCellInfo4SaveCell(cellid_t cellId, int32_t cellSize, uint16_t type, char* &cellPtr, int32_t &entryIndex) { return Storage::LocalMemoryStorage::CGetLockedCellInfo4SaveCell(cellId, cellSize, type, cellPtr, entryIndex); }
DLL_EXPORT TrinityErrorCode  CGetLockedCellInfo4AddCell(cellid_t cellId, int32_t cellSize, uint16_t type, char* &cellPtr, int32_t &entryIndex) { return Storage::LocalMemoryStorage::CGetLockedCellInfo4AddCell(cellId, cellSize, type, cellPtr, entryIndex); }
DLL_EXPORT TrinityErrorCode  CGetLockedCellInfo4UpdateCell(cellid_t cellId, int32_t cellSize, char* &cellPtr, int32_t &entryIndex) { return Storage::LocalMemoryStorage::CGetLockedCellInfo4UpdateCell(cellId, cellSize, cellPtr, entryIndex); }
DLL_EXPORT TrinityErrorCode  CGetLockedCellInfo4LoadCell(cellid_t cellId, int32_t &cellSize, char* &cellPtr, int32_t &entryIndex) { return Storage::LocalMemoryStorage::CGetLockedCellInfo4LoadCell(cellId, cellSize, cellPtr, entryIndex); }
DLL_EXPORT TrinityErrorCode  CGetLockedCellInfo4AddOrUseCell(cellid_t cellId, int32_t &cellSize, uint16_t type, char* &cellPtr, int32_t &entryIndex) { return Storage::LocalMemoryStorage::CGetLockedCellInfo4AddOrUseCell(cellId, cellSize, type, cellPtr, entryIndex); }
DLL_EXPORT TrinityErrorCode  CLockedGetCellSize(cellid_t cellId, int32_t entryIndex, int32_t &size) { return Storage::LocalMemoryStorage::CLockedGetCellSize(cellId, entryIndex, size); }

//not exposed
//DLL_EXPORT void CStartDebugger(bool suspendOthers) { Trinity::Debugger::TryStartDebugger(suspendOthers); }
//DLL_EXPORT int32_t CGetTrunkId(cellid_t cellid) { return Storage::LocalMemoryStorage::GetTrunkId(cellid); }
//DLL_EXPORT void StopDefragAndAwaitCeased() { return Storage::GCTask::StopDefragAndAwaitCeased(); }
#endif//CORECLR
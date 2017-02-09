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

//! See InternalCall.cs for explanation of entry names.
ICallEntry ICallTable[] =
{
    //====================== TrinityConfig ==========================//

    { "Trinity.CTrinityConfig::SetStorageRoot"                                      , static_cast<void(*)(const char*, int32_t)>(&TrinityConfig::SetStorageRoot) },
    { "Trinity.CTrinityConfig::CReadOnly"                                           , &TrinityConfig::ReadOnly },
    { "Trinity.CTrinityConfig::CSetReadOnly"                                        , &TrinityConfig::SetReadOnly },
    { "Trinity.CTrinityConfig::CTrunkCount"                                         , &TrinityConfig::TrunkCount },
    { "Trinity.CTrinityConfig::CSetTrunkCount"                                      , &TrinityConfig::SetTrunkCount },
    { "Trinity.CTrinityConfig::GetStorageCapacityProfile"                           , &TrinityConfig::GetStorageCapacityProfile },
    { "Trinity.CTrinityConfig::SetStorageCapacityProfile"                           , &TrinityConfig::SetStorageCapacityProfile },
    { "Trinity.CTrinityConfig::CLargeObjectThreshold"                               , &TrinityConfig::LargeObjectThreshold },
    { "Trinity.CTrinityConfig::CSetLargeObjectThreshold"                            , &TrinityConfig::SetLargeObjectThreshold },
    { "Trinity.CTrinityConfig::CSetGCDefragInterval"                                , &TrinityConfig::SetGCDefragInterval },

    //====================== CStdio ==========================//

#ifdef TRINITY_PLATFORM_WINDOWS
    { "Trinity.CStdio::C_wfopen_s"                                                  , &_wfopen_s },
#else
    { "Trinity.CStdio::C_wfopen_s"                                                  , &_wfopen_cswrapper },
#endif
    { "Trinity.CStdio::fread"                                                       , &fread },
    { "Trinity.CStdio::fwrite"                                                      , &fwrite },
    { "Trinity.CStdio::fflush"                                                      , &fflush },
    { "Trinity.CStdio::fclose"                                                      , &fclose },
    { "Trinity.CStdio::feof"                                                        , &feof },

    //====================== Memory ==========================//

    { "Trinity.Core.Lib.CMemory::Copy(void*,void*,int)"                             , &Memory::Copy },
    { "Trinity.Core.Lib.CMemory::malloc"                                            , &malloc },
    { "Trinity.Core.Lib.CMemory::realloc"                                           , &realloc },
    { "Trinity.Core.Lib.CMemory::memset"                                            , &memset },
    { "Trinity.Core.Lib.CMemory::memcpy"                                            , &memcpy },
    { "Trinity.Core.Lib.CMemory::free"                                              , &free },
    { "Trinity.Core.Lib.CMemory::memmove"                                           , &memmove },
    { "Trinity.Core.Lib.CMemory::memcmp"                                            , &memcmp },
#if defined(TRINITY_PLATFORM_WINDOWS)
    { "Trinity.Core.Lib.CMemory::_aligned_malloc"                                   , &_aligned_malloc },
    { "Trinity.Core.Lib.CMemory::_aligned_free"                                     , &_aligned_free },
#else
    { "Trinity.Core.Lib.CMemory::_aligned_malloc"                                   , &aligned_alloc },
    { "Trinity.Core.Lib.CMemory::_aligned_free"                                     , &free },
#endif
    { "Trinity.Core.Lib.CMemory::AlignedAlloc"                                      , &Memory::AlignedAlloc },
    { "Trinity.Core.Lib.CMemory::SetWorkingSetProfile"                              , &Memory::SetWorkingSetProfile },
    { "Trinity.Core.Lib.CMemory::SetMaxWorkingSet"                                  , &Memory::SetMaxWorkingSet },

    //====================== Math ==========================//

    { "Trinity.Mathematics.CMathUtility::multiply_double_vector"                    , &multiply_double_vector },
    { "Trinity.Mathematics.CMathUtility::multiply_sparse_double_vector"             , &multiply_sparse_double_vector },

#if defined(TRINITY_PLATFORM_WINDOWS)
    { "Trinity.Core.Lib.CTrinityC::GetLastError"                                    , &GetLastError },
#else
    { "Trinity.Core.Lib.CTrinityC::GetLastError"                                    , &errno },
#endif
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
    { "Trinity.Storage.CLocalMemoryStorage::CSaveCell(long,byte*,int,uint16,Trinity.TSL.Lib.CellAccessOptions)", static_cast<TrinityErrorCode(*)(cellid_t, char*, int32_t, uint16_t, CellAccessOptions)>(&Storage::LocalMemoryStorage::SaveCell) },
    { "Trinity.Storage.CLocalMemoryStorage::CAddCell(long,byte*,int,uint16,Trinity.TSL.Lib.CellAccessOptions)", static_cast<TrinityErrorCode(*)(cellid_t, char*, int32_t, uint16_t, CellAccessOptions)>(&Storage::LocalMemoryStorage::AddCell) },
    { "Trinity.Storage.CLocalMemoryStorage::CUpdateCell(long,byte*,int,Trinity.TSL.Lib.CellAccessOptions)", static_cast<TrinityErrorCode(*)(cellid_t, char*, int32_t, CellAccessOptions)>(&Storage::LocalMemoryStorage::UpdateCell) },
    { "Trinity.Storage.CLocalMemoryStorage::CRemoveCell(long,Trinity.TSL.Lib.CellAccessOptions)", static_cast<TrinityErrorCode(*)(cellid_t, CellAccessOptions)>(&Storage::LocalMemoryStorage::RemoveCell) },

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

#ifdef TRINITY_PLATFORM_WINDOWS
    { "Trinity.Storage.CLocalMemoryStorage::CStartDebugger"                         , &Trinity::Debugger::TryStartDebugger },
#endif
    // the following two entries are not exposed.
    { "Trinity.Storage.CLocalMemoryStorage::CGetTrunkId"                            , &Storage::LocalMemoryStorage::GetTrunkId },
    { "Trinity.Storage.CLocalMemoryStorage::StopDefragAndAwaitCeased"               , &Storage::GCTask::StopDefragAndAwaitCeased },

};


#ifdef TRINITY_PLATFORM_WINDOWS

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

/******************************************************************************
Before calling this, make sure that both source and target addresses are JITted!
******************************************************************************/
DLL_EXPORT void __stdcall HotSwapCSharpMethod(void* TargetMethodDescPtr, void* SourceMethodDescPtr)
{
    MethodDesc *src = (MethodDesc*)SourceMethodDescPtr;
    MethodDesc *dst = (MethodDesc*)TargetMethodDescPtr;

    uint64_t dst_addr = (uint64_t)dst->m_MethodPointer;
    Memory::MemoryInject(&dst->m_MethodPointer, (uint64_t)src->m_MethodPointer);
    Memory::MemoryInject(&src->m_MethodPointer, dst_addr);
}

#else
DLL_EXPORT void GetInternalCallEntries(OUT ICallEntry** pEntries, OUT size_t* pCount)
{
    *pCount   = sizeof(ICallTable) / sizeof(ICallEntry);
    *pEntries = ICallTable;
}
#endif
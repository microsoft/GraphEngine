// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "TrinityCommon.h"
#include "Storage/LocalStorage/LocalMemoryStorage.h"
#include "Storage/LocalStorage/GCTask.h"
#include "BackgroundThread/BackgroundThread.h"

//#define RANGE_CHECK

namespace Storage
{
    namespace LocalMemoryStorage
    {
        extern MemoryTrunk* memory_trunks;
        extern MTHash*      hashtables;
        extern MTHash*      hashtables_end;


        namespace Enumeration
        {
            /**
             *  Mounts the enumerator onto mt_hash.
             */
            static TrinityErrorCode _start_enumeration(PLOCAL_MEMORY_STORAGE_ENUMERATOR p_enum)
            {
                TRINITY_INTEROP_ENTER_UNMANAGED();

                if (p_enum->mt_hash == nullptr)
                {
                    TRINITY_INTEROP_LEAVE_UNMANAGED();
                    return TrinityErrorCode::E_FAILURE;
                }

                Trinity::Diagnostics::WriteLine(LogLevel::Debug, "Enumerator {0}: starting enumeration on trunk {1}", p_enum, p_enum->mt_hash->memory_trunk->TrunkId);

                if (!TrinityConfig::ReadOnly())
                    p_enum->mt_hash->memory_trunk->defrag_lock.lock();

                TrinityErrorCode eResult = p_enum->mt_hash->Lock();

                if (TrinityErrorCode::E_SUCCESS == eResult)
                {
                    p_enum->mt_enumerator_active = TRUE;
                    p_enum->mt_enumerator.Initialize(p_enum->mt_hash);
                }
                else
                {
                    p_enum->mt_enumerator_active = FALSE;
                    p_enum->mt_enumerator.Invalidate();
                }

                if (!TrinityConfig::ReadOnly())
                    p_enum->mt_hash->memory_trunk->defrag_lock.unlock();
                p_enum->mt_hash->Unlock();

                TRINITY_INTEROP_LEAVE_UNMANAGED();
                return eResult;
            }

            static void _stop_enumeration(PLOCAL_MEMORY_STORAGE_ENUMERATOR p_enum)
            {
                if (p_enum->mt_enumerator_active)
                {
                    p_enum->mt_enumerator_active = FALSE;
                    p_enum->mt_enumerator.Invalidate();
                    Trinity::Diagnostics::WriteLine(LogLevel::Debug, "Enumerator {0}: finishing enumeration on trunk {1}", p_enum, p_enum->mt_hash->memory_trunk->TrunkId);
                }
            }

            static bool _range_check(PLOCAL_MEMORY_STORAGE_ENUMERATOR p_enum)
            {
#ifdef RANGE_CHECK
                return p_enum->mt_hash >= hashtables && p_enum->mt_hash < hashtables_end;
#else
                return true;
#endif
            }

            TrinityErrorCode Allocate(OUT PLOCAL_MEMORY_STORAGE_ENUMERATOR &pp_enum)
            {
                pp_enum                       = new LOCAL_MEMORY_STORAGE_ENUMERATOR();
                pp_enum->mt_enumerator_active = FALSE;
                pp_enum->mt_hash              = hashtables;

                Trinity::Diagnostics::WriteLine(LogLevel::Debug, "Enumerator {0}: allocation complete.", pp_enum);

                return TrinityErrorCode::E_SUCCESS;
            }

            TrinityErrorCode Deallocate(IN PLOCAL_MEMORY_STORAGE_ENUMERATOR p_enum)
            {
                _stop_enumeration(p_enum);
                delete p_enum;

                Trinity::Diagnostics::WriteLine(LogLevel::Debug, "Enumerator {0}: deallocation complete.", p_enum);

                return TrinityErrorCode::E_SUCCESS;
            }

            REQUIRE_THREAD_CTX TrinityErrorCode MoveNext(IN PLOCAL_MEMORY_STORAGE_ENUMERATOR p_enum)
            {
                if (!_range_check(p_enum))
                {
                    return TrinityErrorCode::E_INVALID_ARGUMENTS;
                }

                PTHREAD_CONTEXT p_ctx = GetCurrentThreadContext();
                while (true)
                {
                    TrinityErrorCode ec = TrinityErrorCode::E_SUCCESS;

                    if (!p_enum->mt_enumerator_active)
                    {
                        ec = _start_enumeration(p_enum);
                    }

                    if (ec == TrinityErrorCode::E_SUCCESS)
                    {
                        // only proceed if _start_enumeration successfully initialize the trunk.
                        ec = p_enum->mt_enumerator.MoveNext();
                    }

                    if (ec == TrinityErrorCode::E_SUCCESS)
                    {
                        /* mt_enumerator reported success. copy data and return now. */
                        p_enum->CellEntryIndex = p_enum->mt_enumerator.CellEntryIndex();
                        p_enum->CellId         = p_enum->mt_enumerator.CellId();
                        p_enum->CellPtr        = p_enum->mt_enumerator.CellPtr();
                        p_enum->CellType       = p_enum->mt_enumerator.CellType();

                        return TrinityErrorCode::E_SUCCESS;
                    }
                    else if (ec == TrinityErrorCode::E_ENUMERATION_END)
                    {
                        /* try to advance to next trunk */
                        _stop_enumeration(p_enum);
                        ++(p_enum->mt_hash);
                        if (p_enum->mt_hash != hashtables_end)
                        {
                            continue;
                        }
                        else
                        {
                            Trinity::Diagnostics::WriteLine(LogLevel::Debug, "Enumerator {0}: Reaching end of storage.", p_enum);
                            p_enum->mt_hash = nullptr;
                            return TrinityErrorCode::E_ENUMERATION_END;
                        }
                    }
                    else
                    {
                        /* mt_enumerator reported failure. stop enumeration and return now. */
                        _stop_enumeration(p_enum);
                        Trinity::Diagnostics::WriteLine(LogLevel::Error, "Enumerator {0}: memory trunk enumerator {1} reported failure, code = {2}", p_enum, p_enum->mt_hash->memory_trunk->TrunkId, ec);
                        p_enum->mt_hash = nullptr;
                        return ec;
                    }

                }
            }

            TrinityErrorCode Reset(IN PLOCAL_MEMORY_STORAGE_ENUMERATOR p_enum)
            {
                _stop_enumeration(p_enum);

                p_enum->mt_hash = hashtables;

                Trinity::Diagnostics::WriteLine(LogLevel::Debug, "Enumerator {0}: cursor reset complete", p_enum);

                return TrinityErrorCode::E_SUCCESS;
            }
        }
    }
}
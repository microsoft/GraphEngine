// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "Storage/MTHash/MT_ENUMERATOR.h"
#include "Storage/MTHash/MTHash.h"
#include "Storage/MemoryTrunk/MemoryTrunk.h"

namespace Storage
{
    void MT_ENUMERATOR::Initialize(MTHash* mth)
    {
        CellEntryPtr   = mth->CellEntries - 1;
        MTEntryPtr     = mth->MTEntries - 1;
        CellEntryIndex = -1;
        TrunkPtr       = mth->memory_trunk->trunkPtr;
        LOPtr          = mth->memory_trunk->LOPtrs;
        EndPtr         = mth->CellEntries + mth->NonEmptyEntryCount;
    }

    TrinityErrorCode MT_ENUMERATOR::MoveNext()
    {
        if (CellEntryPtr < EndPtr)
        {
            do
            {
                ++CellEntryPtr;
                ++MTEntryPtr;
                ++CellEntryIndex;
            } while (currentEntryInvalid());
        }
        if (CellEntryPtr < EndPtr)
        {
            return TrinityErrorCode::E_SUCCESS;
        }
        else
        {
            return TrinityErrorCode::E_ENUMERATION_END;
        }
    }

    char* MT_ENUMERATOR::LOCellPtr()
    {
        if (CellEntryPtr->offset < 0)
            return LOPtr[-(CellEntryPtr->offset)];
        return nullptr;
    }

    char* MT_ENUMERATOR::CellPtr()
    {
        if (CellEntryPtr->offset >= 0)
            return TrunkPtr + (CellEntryPtr->offset);
        else
            return LOPtr[-(CellEntryPtr->offset)];
    }

    cellid_t MT_ENUMERATOR::CellId()
    {
        return MTEntryPtr->Key;
    }

    int32_t MT_ENUMERATOR::CellSize()
    {
        if (CellEntryPtr->offset >= 0)
            return (CellEntryPtr->size) & 0xFFFFFF; // Max size = (16 M - 1)
        else
            return CellEntryPtr->size;
    }

    uint16_t MT_ENUMERATOR::CellType()
    {
        return MTEntryPtr->CellType;
    }

    inline bool MT_ENUMERATOR::currentEntryInvalid()
    {
        return (CellEntryPtr->location == -1) &&
            (CellEntryPtr < EndPtr);
    }
}
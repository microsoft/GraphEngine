// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "Storage/MTHash/MT_ENUMERATOR.h"
#include "Storage/MTHash/MTHash.h"
#include "Storage/MemoryTrunk/MemoryTrunk.h"

namespace Storage
{
    // !Caller guarantees that MTHash is locked.
    void MT_ENUMERATOR::Initialize(MTHash* mth)
    {
        m_CellEntryPtr   = mth->CellEntries - 1;
        m_MTEntryPtr     = mth->MTEntries - 1;
        m_CellEntryIndex = -1;
        m_TrunkPtr       = mth->memory_trunk->trunkPtr;
        m_LOPtr          = mth->memory_trunk->LOPtrs;
        m_EndPtr         = mth->CellEntries + mth->ExtendedInfo->NonEmptyEntryCount;
    }

    void MT_ENUMERATOR::Invalidate()
    {
        m_CellEntryPtr   = nullptr;
        m_MTEntryPtr     = nullptr;
        m_CellEntryIndex = -1;
        m_TrunkPtr       = nullptr;
        m_LOPtr          = nullptr;
        m_EndPtr         = nullptr;
    }

    TrinityErrorCode MT_ENUMERATOR::MoveNext()
    {
        if (m_CellEntryPtr < m_EndPtr)
        {
            do
            {
                ++m_CellEntryPtr;
                ++m_MTEntryPtr;
                ++m_CellEntryIndex;
            } while (currentEntryInvalid());
        }
        if (m_CellEntryPtr < m_EndPtr)
        {
            return TrinityErrorCode::E_SUCCESS;
        }
        else
        {
            Invalidate();
            return TrinityErrorCode::E_ENUMERATION_END;
        }
    }

    char* MT_ENUMERATOR::LOCellPtr() const
    {
        if (m_CellEntryPtr->offset < 0)
            return m_LOPtr[-(m_CellEntryPtr->offset)];
        return nullptr;
    }

    char* MT_ENUMERATOR::CellPtr() const
    {
        if (m_CellEntryPtr->offset >= 0)
            return m_TrunkPtr + (m_CellEntryPtr->offset);
        else
            return m_LOPtr[-(m_CellEntryPtr->offset)];
    }

    cellid_t MT_ENUMERATOR::CellId() const
    {
        return m_MTEntryPtr->Key;
    }

    int32_t MT_ENUMERATOR::CellSize() const
    {
        if (m_CellEntryPtr->offset >= 0)
            return (m_CellEntryPtr->size) & 0xFFFFFF; // Max size = (16 M - 1)
        else
            return m_CellEntryPtr->size;
    }

    uint16_t MT_ENUMERATOR::CellType() const
    {
        return m_MTEntryPtr->CellType;
    }

    CellEntry* MT_ENUMERATOR::CellEntryPtr() const
    {
        return m_CellEntryPtr;
    }

    inline bool MT_ENUMERATOR::currentEntryInvalid() const
    {
        return (m_CellEntryPtr->location == -1) &&
            (m_CellEntryPtr < m_EndPtr);
    }
}

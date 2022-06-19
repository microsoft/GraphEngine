// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include "TrinityCommon.h"

#include "CellEntry.h"

namespace Storage
{
    class MTHash;
    /// Iterates through a locked MTHash. Caller guarantees that the MTHash is locked.
    class MT_ENUMERATOR
    {
    private:
        CellEntry*      m_CellEntryPtr;
        CellEntry*      m_EndPtr;
        MTEntry*        m_MTEntryPtr;
        char*           m_TrunkPtr;
        char**          m_LOPtr;
        int32_t         m_CellEntryIndex;

        bool currentEntryInvalid() const;

    public:
        inline MT_ENUMERATOR(MTHash* mth) { Initialize(mth); }
        void Initialize(MTHash* mth);
        void Invalidate();
        TrinityErrorCode MoveNext();

        /// Returns the cellPtr if the current cell is a large object; otherwise, returns nullptr.
        char*       LOCellPtr() const;
        char*       CellPtr() const;
        cellid_t    CellId() const;
        int32_t     CellSize() const;
        uint16_t    CellType() const;
        CellEntry*  CellEntryPtr() const;
    };
}

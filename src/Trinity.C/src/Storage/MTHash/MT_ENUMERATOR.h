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
    struct MT_ENUMERATOR
    {
        CellEntry*      CellEntryPtr;
        CellEntry*      EndPtr;
        MTEntry*        MTEntryPtr;
        char*           TrunkPtr;
        char**          LOPtr;
        int32_t         CellEntryIndex;

        inline MT_ENUMERATOR(){}
        inline MT_ENUMERATOR(MTHash* mth){ Initialize(mth); }
        void Initialize(MTHash* mth);
        TrinityErrorCode MoveNext();

        /// Returns the cellPtr if the current cell is a large object; otherwise, returns nullptr.
        char*       LOCellPtr();
        char*       CellPtr();
        cellid_t    CellId();
        int32_t     CellSize();
        uint16_t    CellType();

        bool currentEntryInvalid();
    };
}
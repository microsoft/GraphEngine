// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include "TrinityCommon.h"

typedef union
{
    struct
    {
        // offset >= 0 means the object is in MT
        // offset <  0 means the object is LO
        int32_t offset;
        // size should be in the range [0, 2^31 - 1]
        // size must not reach 2^32-1, which could compose
        // a location of -1.
        int32_t size;
    };
    // A location of -1 indicates an invalid entry.
    int64_t location;
}CellEntry;

typedef struct
{
    cellid_t            Key;
    int32_t             NextEntry;
    uint16_t            CellType;
    std::atomic<char>   EntryLock;
    char                Flag;
}MTEntry, *PMTEntry;
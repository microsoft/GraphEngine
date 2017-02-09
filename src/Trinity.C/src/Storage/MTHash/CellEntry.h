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
        int32_t offset;
        int32_t size;
    };
    int64_t location;
}CellEntry;

typedef struct
{
    cellid_t            Key;
    int32_t             NextEntry;
    uint16_t            CellType;
    std::atomic<char>   EntryLock;
    char                Flag;
}MTEntry;
#pragma once
#include <cstdint>
#include "Trinity.h"
#pragma pack(push,1)

struct CellAccessor
{
    int64_t  cellPtr;
    int64_t  cellId;
    int32_t  size;
    int32_t  entryIndex;
    uint16_t type;
    uint8_t  isCell;
    uint8_t  malloced;

    CellAccessor() {}

    inline void Release()
    {
        if (malloced)
        {
            free((void*)cellPtr);
        }
        else if(isCell)
        {
            ::CReleaseCellLock(cellId, entryIndex);
        }
    }
};

#pragma pack(pop)


#pragma once
#include <cstdint>
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
    ~CellAccessor() {}
};

#pragma pack(pop)

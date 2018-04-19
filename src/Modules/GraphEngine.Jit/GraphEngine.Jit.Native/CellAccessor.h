#pragma once
#include <cstdint>
#pragma pack(push,1)

struct CellAccessor
{
    char* cellPtr;
    int64_t cellId;
    int32_t size;
    int32_t entryIndex;
    uint16_t type;
};

#pragma pack(pop)

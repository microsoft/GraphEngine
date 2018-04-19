#pragma once
#include <cstdint>
#pragma pack(push,1)

struct CellAccessor
{
    int64_t cellPtr;
    int64_t cellId;
    int32_t size;
    int32_t entryIndex;
    uint16_t type;

	CellAccessor() {}
	~CellAccessor() {}
};

#pragma pack(pop)

#pragma once
// User swig code shall include this file
#include "CellAccessor.h"

extern "C" __declspec(dllimport) int32_t LockCell(CellAccessor& accessor, const int32_t options);
extern "C" __declspec(dllimport) void UnlockCell(const CellAccessor& accessor);
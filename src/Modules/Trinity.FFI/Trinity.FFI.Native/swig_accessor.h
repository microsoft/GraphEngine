#pragma once
// User swig code shall include this file
#include "CellAccessor.h"

extern "C" __declspec(dllimport) int32_t LockCell(CellAccessor&, const int32_t);
extern "C" __declspec(dllimport) void UnlockCell(const CellAccessor&);
extern "C" __declspec(dllimport) int32_t SaveCell(CellAccessor&);
extern "C" __declspec(dllimport) int32_t LoadCell(CellAccessor&);
#pragma once
// User swig code shall include this file
#include "CellAccessor.h"

enum CellAccessOptions : int32_t
{
	ThrowExceptionOnCellNotFound = 1,
	ReturnNullOnCellNotFound = 2,
	CreateNewOnCellNotFound = 4,
	StrongLogAhead = 8,
	WeakLogAhead = 16
};

extern "C" __declspec(dllimport) int32_t LockCell(CellAccessor&, const int32_t);
extern "C" __declspec(dllimport) void UnlockCell(const CellAccessor&);
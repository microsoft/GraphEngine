#pragma once
// User swig code shall include this file
#include "CellAccessor.h"
#include <type_traits>

extern "C" __declspec(dllimport) int32_t LockCell(CellAccessor&, const int32_t);
extern "C" __declspec(dllimport) void UnlockCell(const CellAccessor&);
extern "C" __declspec(dllimport) int32_t SaveCell(CellAccessor&);
extern "C" __declspec(dllimport) int32_t LoadCell(CellAccessor&);


constexpr char* cast_object(char* object) {
	return object;
}

template<typename T>
constexpr typename std::enable_if<std::is_fundamental<T>::value, T>::type cast_object(T x) {
	return x;
}

constexpr void* cast_object(void* object) {
	return (void*) ((CellAccessor*)object)->cellPtr;
}


void* LockedCell(int64_t cellid, int32_t options)
{
	CellAccessor* accessor = new CellAccessor();
	accessor->cellId = cellid;
	auto errCode = LockCell(*accessor, options);
	if (errCode)
		throw errCode;
	return accessor;
}

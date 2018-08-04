#pragma once
// User swig code shall include this file
#include "CellAccessor.h"
#include <functional>
#include <type_traits>

DLL_IMPORT int32_t LockCell(CellAccessor&, const int32_t, std::function<int32_t(void*)>);
DLL_IMPORT void UnlockCell(const CellAccessor&);
DLL_IMPORT int32_t SaveCell(CellAccessor&);
DLL_IMPORT int32_t LoadCell(CellAccessor&);


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



#include "Trinity.h"
#include "CellAccessor.h"
DLL_EXPORT TrinityErrorCode LockCell(IN OUT CellAccessor& accessor, IN const int32_t options)
{
	//TODO options are dishonored!
	char* ptr;
	auto ret =  ::CGetLockedCellInfo4CellAccessor(accessor.cellId, accessor.size, accessor.type, ptr, accessor.entryIndex);
	accessor.cellPtr = (int64_t)ptr;
    accessor.malloced = 0;
	return ret;
}

DLL_EXPORT void UnlockCell(IN const CellAccessor& accessor)
{
    if (accessor.malloced) { free((void*)accessor.cellPtr); }
    else { ::CReleaseCellLock(accessor.cellId, accessor.entryIndex); }
}

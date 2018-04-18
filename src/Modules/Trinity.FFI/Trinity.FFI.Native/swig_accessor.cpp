#include "../../GraphEngine.Jit/GraphEngine.Jit.Native/Trinity.h"
#include "../../GraphEngine.Jit/GraphEngine.Jit.Native/CellAccessor.h"

DLL_EXPORT TrinityErrorCode LockCell(IN OUT CellAccessor& accessor, IN const int32_t options)
{
	//TODO options are dishonored!
	return ::CGetLockedCellInfo4CellAccessor(accessor.cellId, accessor.size, accessor.type, accessor.cellPtr, accessor.entryIndex);
}

DLL_EXPORT void UnlockCell(IN const CellAccessor& accessor)
{
	::CReleaseCellLock(accessor.cellId, accessor.entryIndex);
}

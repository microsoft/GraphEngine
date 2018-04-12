#include "GraphEngine.Jit.Native.h"
#include "Trinity.h"

TrinityErrorCode LockCell(CellAccessor& accessor)
{

    return ::CGetLockedCellInfo4CellAccessor(accessor.cellId, accessor.size, accessor.type, accessor.cellPtr, accessor.entryIndex);
}

void UnlockCell(CellAccessor& accessor)
{
    ::CReleaseCellLock(accessor.cellId, accessor.entryIndex);
}
#include "GraphEngine.Jit.Native.h"
#include "Storage/LocalStorage/LocalMemoryStorage.h"

using namespace Storage::LocalMemoryStorage;

TrinityErrorCode LockCell(CellAccessor& accessor)
{
    return CGetLockedCellInfo4CellAccessor(accessor.cellId, accessor.size, accessor.type, accessor.cellPtr, accessor.entryIndex);
}

void UnlockCell(CellAccessor& accessor)
{
    ReleaseCellLock(accessor.cellId, accessor.entryIndex);
}
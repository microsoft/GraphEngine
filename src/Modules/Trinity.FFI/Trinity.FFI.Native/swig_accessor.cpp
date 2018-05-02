#include "Trinity.h"
#include "CellAccessor.h"
DLL_EXPORT TrinityErrorCode LockCell(IN OUT CellAccessor& accessor, IN const int32_t options)
{
    //TODO options are dishonored!
    char* ptr;
    auto ret =  ::CGetLockedCellInfo4CellAccessor(accessor.cellId, accessor.size, accessor.type, ptr, accessor.entryIndex);
    accessor.cellPtr = (int64_t)ptr;
    accessor.isCell = 1;
    accessor.malloced = 0;
    return ret;
}

DLL_EXPORT void UnlockCell(IN const CellAccessor& accessor)
{
    if (accessor.malloced) { free((void*)accessor.cellPtr); }
    else { ::CReleaseCellLock(accessor.cellId, accessor.entryIndex); }
}

DLL_EXPORT TrinityErrorCode SaveCell(IN CellAccessor& accessor)
{
    if (!accessor.isCell) return TrinityErrorCode::E_WRONG_CELL_TYPE;
    if (!accessor.malloced) return TrinityErrorCode::E_INVALID_ARGUMENTS;

    return ::CSaveCell(accessor.cellId, (char*)accessor.cellPtr, accessor.size, accessor.type);
}

//  !note, accessor.type is examined against storage cell type
//  error code:
//  
//  - E_SUCCESS         ok
//  - E_DEADLOCK        storage deadlock
//  - E_NOMEM           malloc failure
//  - E_WRONG_CELL_TYPE cell type mismatch
DLL_EXPORT TrinityErrorCode LoadCell(IN OUT CellAccessor& accessor)
{
    char* ptr;
    auto type = accessor.type;
    auto ret = ::CGetLockedCellInfo4CellAccessor(accessor.cellId, accessor.size, accessor.type, ptr, accessor.entryIndex);
    if (ret != TrinityErrorCode::E_SUCCESS) return ret;
    if (type != accessor.type)
    {
        ret = TrinityErrorCode::E_WRONG_CELL_TYPE;
        goto cleanup;
    }

    accessor.cellPtr = (int64_t)malloc(accessor.size);
    if (!accessor.cellPtr)
    {
        ret = TrinityErrorCode::E_NOMEM;
        goto cleanup;
    }

    accessor.malloced = 1;
    accessor.isCell = 1;
    memcpy((void*)accessor.cellPtr, ptr, accessor.size);

cleanup:

    ::CReleaseCellLock(accessor.cellId, accessor.entryIndex);
    return ret;
}
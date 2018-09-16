#include "DiskImageViewer.h"
#include "Storage/LocalStorage/LocalMemoryStorage.h"

DLL_EXPORT TrinityErrorCode LoadIncrementalDiskImage(u16char* lptrunk, u16char* lphash, u16char* lplo)
{
    Storage::DiskImageViewer viewer(lptrunk, lphash, lplo);
    if (!viewer.Good())
    {
        return TrinityErrorCode::E_LOAD_FAIL;
    }

    cellid_t id;
    char* ptr;
    int32_t size;
    uint16_t type;
    while (TrinityErrorCode::E_SUCCESS == viewer.MoveNext(id, ptr, size, type))
    {
        auto error = Storage::LocalMemoryStorage::SaveCell(id, ptr, size, type);
        if (error != TrinityErrorCode::E_SUCCESS) return error;
    }

    return TrinityErrorCode::E_SUCCESS;
}

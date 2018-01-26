#pragma once

#include <vector>
#include "Trinity.FFI.Native.h"

extern TRINITY_INTERFACES* g_TrinityInterfaces;

void Init()
{
    g_TrinityInterfaces = TRINITY_FFI_GET_INTERFACES();
}

TCell LoadCell(int64_t cellId)
{
    TCell ret;
    if (TrinityErrorCode::E_SUCCESS == g_TrinityInterfaces->local_loadcell(cellId, &ret))
        return ret;
    else
        return NULL;
}

bool SaveCell(int64_t cellId, TCell cell)
{
    return (TrinityErrorCode::E_SUCCESS == g_TrinityInterfaces->local_savecell_1(cellId, cell));
}

bool SaveCell2(int64_t cellId, TCell cell, CellAccessOptions options)
{
    return (TrinityErrorCode::E_SUCCESS == g_TrinityInterfaces->local_savecell_2(cellId, options, cell));
}

TCell NewCell(char* cellType)
{
    TCell cell;
    if (TrinityErrorCode::E_SUCCESS == g_TrinityInterfaces->newcell_1(cellType, &cell))
        return cell;
    else
        return NULL;
}

char* CellGetField(TCell cell, char* field)
{
    return g_TrinityInterfaces->cell_getfield(cell, field);
}

//TODO errno
void CellSetField(TCell cell, char* field, char* content)
{
    g_TrinityInterfaces->cell_setfield(cell, field, content);
}

void CellAppendField(TCell cell, char* field, char* content)
{
    g_TrinityInterfaces->cell_appendfield(cell, field, content);
}

void CellRemoveField(TCell cell, char* field)
{
    g_TrinityInterfaces->cell_removefield(cell, field);
}

int32_t CellHasField(TCell cell, char* field)
{
    return g_TrinityInterfaces->cell_hasfield(cell, field);
}

void CellDispose(TCell cell)
{
    g_TrinityInterfaces->cell_dispose(cell);
}

std::vector<char*> CellGetFieldNames(TCell cell)
{
    std::vector<char*> vec;
    do
    {
        TEnumerator etor;
        TFieldInfo fi = NULL;
        char* val = NULL;
        if (TrinityErrorCode::E_SUCCESS != g_TrinityInterfaces->cell_fieldenum_get(cell, &etor)) break;
        while (TrinityErrorCode::E_SUCCESS == g_TrinityInterfaces->cell_fieldenum_movenext(etor, fi)) 
        {
            g_TrinityInterfaces->cell_fieldenum_current(etor, &fi);
            if (TrinityErrorCode::E_SUCCESS == g_TrinityInterfaces->cell_fieldinfo_name(fi, &val)) {
                vec.push_back(val);
            }
        }
        g_TrinityInterfaces->cell_fieldenum_dispose(etor);
    } while (false);

    return vec;
}
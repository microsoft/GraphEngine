#pragma once

#include <vector>
#include "Trinity.FFI.Native.h"
#include "../../GraphEngine.Jit/GraphEngine.Jit.Native/TypeSystem.h"

TRINITY_INTERFACES* g_TrinityInterfaces;

void Init()
{
    g_TrinityInterfaces = TRINITY_FFI_GET_INTERFACES();
}

class Cell
{
private:
    void* m_cell;
public:
    Cell(void* cell) : m_cell(cell) {}

    friend Cell* LoadCell(long long);
    friend bool SaveCell_1(long long, Cell*);
    friend bool SaveCell_2(long long, Cell*, CellAccessOptions);
    friend Cell* NewCell_1(char*);
    friend Cell* NewCell_2(long long, char*);
    friend Cell* NewCell_3(char*, char*);


    char* GetField(char* field)
    {
        return g_TrinityInterfaces->cell_get(m_cell, field);
    }

    //TODO errno
    void SetField(char* field, char* content)
    {
        g_TrinityInterfaces->cell_set(m_cell, field, content);
    }

    void AppendField(char* field, char* content)
    {
        g_TrinityInterfaces->cell_append(m_cell, field, content);
    }

    void RemoveField(char* field)
    {
        g_TrinityInterfaces->cell_delete(m_cell, field);
    }

    int HasField(char* field)
    {
        return g_TrinityInterfaces->cell_has(m_cell, field);
    }

    long long GetID() {
        return g_TrinityInterfaces->cell_getid(m_cell);
    }

    void SetID() {
        g_TrinityInterfaces->cell_setid(m_cell, GetID());
    }

    ~Cell()
    {
        //printf("====================== dtor %llX\n", m_cell);

        g_TrinityInterfaces->gc_free(m_cell);
    }
};

template<typename T, typename F> std::vector<T> __getarray(F f) {
    long size = 0;
    T* pelem = nullptr;
    std::vector<T> vec;
    if (TrinityErrorCode::E_SUCCESS == f((void**)&pelem, &size)) {
        while (size-- > 0) {
            vec.push_back(std::move(*pelem++));
        }
    }
    return vec;
}

std::vector<TypeDescriptor> GetCellDescriptors() {
    return __getarray<TypeDescriptor>(g_TrinityInterfaces->schema_get);
}

Cell* LoadCell(long long cellId) {
    void* pcell = nullptr;
    if (TrinityErrorCode::E_SUCCESS == g_TrinityInterfaces->local_loadcell(cellId, &pcell)) {
        return new Cell(pcell);
    }
    else {
        return nullptr;
    }
}

bool SaveCell_1(long long cellId, Cell* pcell)
{
    return (TrinityErrorCode::E_SUCCESS == g_TrinityInterfaces->local_savecell_1(cellId, pcell->m_cell));
}

bool SaveCell_2(long long cellId, Cell* pcell, CellAccessOptions options)
{
    return (TrinityErrorCode::E_SUCCESS == g_TrinityInterfaces->local_savecell_2(cellId, options, pcell->m_cell));
}

Cell* NewCell_1(char* cellType)
{
    Cell* pcell = new Cell(0);
    if (TrinityErrorCode::E_SUCCESS == g_TrinityInterfaces->cell_new_1(cellType, &pcell->m_cell))
        return pcell;
    else
    {
        delete pcell;
        return nullptr;
    }
}

Cell* NewCell_2(long long cellId, char* cellType)
{
    Cell* pcell = new Cell(0);
    if (TrinityErrorCode::E_SUCCESS == g_TrinityInterfaces->cell_new_2(cellId, cellType, &pcell->m_cell))
        return pcell;
    else
    {
        delete pcell;
        return nullptr;
    }
}

Cell* NewCell_3(char* cellType, char* cellContent)
{
    Cell* pcell = new Cell(0);
    if (TrinityErrorCode::E_SUCCESS == g_TrinityInterfaces->cell_new_3(cellType, cellContent, &pcell->m_cell))
        return pcell;
    else
    {
        delete pcell;
        return nullptr;
    }
}

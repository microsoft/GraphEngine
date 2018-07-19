#pragma once

#include <vector>
#include "Trinity.FFI.Native.h"
#include "TypeSystem.h"

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

TrinityErrorCode LocalStorage_Load()
{
    return g_TrinityInterfaces->local_loadstorage();
}

TrinityErrorCode LocalStorage_Save()
{
    return g_TrinityInterfaces->local_savestorage();
}

TrinityErrorCode LocalStorage_Reset()
{
    return g_TrinityInterfaces->local_resetstorage();
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


/*

IKeyValueStore

C1 c1(1, 2, 3);
Global.LocalStorage.SaveC1(123, c1);

C:

struct C1
{
    int   a;
    char* b;
    int   c;
}

Jit'ed:

C1:
[ 4B a | 8B b | 4B c ]
C1_Accessor
[ 4B a | 4B b.Length xB b.Content | 4B c ]

struct C1
{
    int a;
    std::string b;
    std::vector<std::string> list_of_string;
    int c;
}

class C1Accessor:
    ...

CellAccessor UseC1(cellid_t cellid, CellAccessOptions options)
{
    CellAccessor accessor;
    auto errcode = LockCell(accessor, options);
    if (errcode)
        throw errcode;
    return accessor;
}

SaveC1(C1* c1)
{
    char* ptr = nullptr;
    ptr += 4;
    ptr += b.length();
    ptr += 4;
    ptr += 4;

    char* buf = malloc(ptr);
    ptr = 0;
    *(int*)ptr = c1->a;
    ptr += 4;
    *(int*)ptr = c1->b.length();
    ptr += 4;
    memcpy(ptr, c1->b.c_str());
    ptr += b.length();
    *(int*)ptr = c1->c;

    ::CSaveCell(Types::C1Type, buf, ptr);
}

c1 = C1() # new
storage.SaveC1(c1)

==================================

serialize :: C1 -> nativeptr * int



FFI: generate python interface like new C1()... ---> hooked to Cell::New_Cell, update deferred
Accessor: Jit'ed strong type, generate python interface like UseC1(id) ---> CellAccessor

LoadCell: runtime object ---- C#     runtime obj
                         `--- python runtime obj
                         `--- C++    runtime obj
                            ^
============================|===========
 Trinity Storage:

 [Serialized cell 1]     [Serialized cell 2]

*/
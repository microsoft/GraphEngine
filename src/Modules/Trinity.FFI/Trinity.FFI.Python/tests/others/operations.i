

%module operations
%include <stdint.i>
%{
#include "swig_accessor.h"
#include "CellAccessor.h"
#define SWIG_FILE_WITH_INIT

static void (* _Cell_C2_Set_bar)(void*, char*) = (void (*)(void*, char*))0x208606e00c0ll;
static void Cell_C2_Set_bar(void* subject, char* object)
{
        return _Cell_C2_Set_bar(subject, object);
}
            

static char* (* _Cell_C2_Get_bar)(void*) = (char* (*)(void*))0x208606e0080ll;
static char* Cell_C2_Get_bar(void* subject)
{
        return _Cell_C2_Get_bar(subject);
}
            

static void (* _Cell_C2_Set_lst)(void*, void*) = (void (*)(void*, void*))0x208606e0040ll;
static void Cell_C2_Set_lst(void* subject, void* object)
{
        return _Cell_C2_Set_lst(subject, object);
}
            

static void* (* _Cell_C2_Get_lst)(void*) = (void* (*)(void*))0x208606e0000ll;
static void* Cell_C2_Get_lst(void* subject)
{
        return _Cell_C2_Get_lst(subject);
}
            

CellAccessor* Use_Cell_C2(int64_t cellid, int32_t options)
{
    CellAccessor* accessor = new CellAccessor();
    accessor->cellId = cellid;
    auto errCode = LockCell(*accessor, options);
    if (errCode)
    throw errCode;
    return accessor;
}
                               

static int (* _List_int32_Count)(void*) = (int (*)(void* )) 0x208606e01c0ll;
static int List_int32_Count(void* subject)
{
    return _List_int32_Count(subject);
}
            

static bool (* _List_int32_Contains)(void*, int32_t) = (bool (*)(void*, int32_t)) 0x208606e0180ll;
static bool List_int32_Contains(void* subject, int32_t object)
{
    return _List_int32_Contains(subject, object);
}
            

static void (* _List_int32_Set)(void*, int,  int32_t) = (void (*)(void*, int, int32_t object))0x208606e0140ll;
static void List_int32_Set(void* subject, int idx, int32_t object){
        return _List_int32_Set(subject, idx, object);
}
            

static int32_t (* _List_int32_Get)(void*, int) =  (int32_t (*)(void*, int))0x208606e0100ll;
static int32_t List_int32_Get(void* subject, int idx)
{
        return _List_int32_Get(subject, idx);
}
            

static void (* _Cell_C1_Set_bar)(void*, char*) = (void (*)(void*, char*))0x208606e02c0ll;
static void Cell_C1_Set_bar(void* subject, char* object)
{
        return _Cell_C1_Set_bar(subject, object);
}
            

static char* (* _Cell_C1_Get_bar)(void*) = (char* (*)(void*))0x208606e0280ll;
static char* Cell_C1_Get_bar(void* subject)
{
        return _Cell_C1_Get_bar(subject);
}
            

static void (* _Cell_C1_Set_foo)(void*, int32_t) = (void (*)(void*, int32_t))0x208606e0240ll;
static void Cell_C1_Set_foo(void* subject, int32_t object)
{
        return _Cell_C1_Set_foo(subject, object);
}
            

static int32_t (* _Cell_C1_Get_foo)(void*) = (int32_t (*)(void*))0x208606e0200ll;
static int32_t Cell_C1_Get_foo(void* subject)
{
        return _Cell_C1_Get_foo(subject);
}
            

CellAccessor* Use_Cell_C1(int64_t cellid, int32_t options)
{
    CellAccessor* accessor = new CellAccessor();
    accessor->cellId = cellid;
    auto errCode = LockCell(*accessor, options);
    if (errCode)
    throw errCode;
    return accessor;
}
                               
%}
static void Cell_C2_Set_bar(void* subject, char*);
static char* Cell_C2_Get_bar(void*);
static void Cell_C2_Set_lst(void* subject, void*);
static void* Cell_C2_Get_lst(void*);
CellAccessor* Use_Cell_C2(int64_t cellid, int32_t options);
static int List_int32_Count(void* subject);
static bool List_int32_Contains(void* subject, int32_t);
static void List_int32_Set(void* subject, int idx, int32_t);
static int32_t List_int32_Get(void* subject, int idx);
static void Cell_C1_Set_bar(void* subject, char*);
static char* Cell_C1_Get_bar(void*);
static void Cell_C1_Set_foo(void* subject, int32_t);
static int32_t Cell_C1_Get_foo(void*);
CellAccessor* Use_Cell_C1(int64_t cellid, int32_t options);
%newobject Use_Cell_C2;
%newobject Use_Cell_C1;
            
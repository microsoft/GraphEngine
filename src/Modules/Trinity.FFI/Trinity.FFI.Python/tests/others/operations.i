

%module operations

%{

#include "swig_accessor.h"

#include "CellAccessor.h"

#define SWIG_FILE_WITH_INIT

enum CellAccessOptions{
    a = 1
};


static void (* _Cell_C2_Set_bar)(void*, char*) = (void (*)(void*, char*))2589590618304;

static void Cell_C2_Set_bar(void* subject, char* object)

{

        return _Cell_C2_Set_bar(subject, object);

}

            


static char* (* _Cell_C2_Get_bar)(void*) = (char* (*)(void*))2589590618240;

static char* Cell_C2_Get_bar(void* subject)

{

        return _Cell_C2_Get_bar(subject);

}

            


static void (* _Cell_C2_Set_lst)(void*, void*) = (void (*)(void*, void*))2589590618176;

static void Cell_C2_Set_lst(void* subject, void* object)

{

        return _Cell_C2_Set_lst(subject, object);

}

            


static void* (* _Cell_C2_Get_lst)(void*) = (void* (*)(void*))2589590618112;

static void* Cell_C2_Get_lst(void* subject)

{

        return _Cell_C2_Get_lst(subject);

}

            


CellAccessor Use_Cell_C2(int64_t cellid, CellAccessOptions options);

{

    CellAccessor accessor;

    auto errCode = LockCell(accessor, options);

    if (errCode)

    throw errCode;

    return accessor;

}

                               


static int (* _List_int32_Count)(void*) = (int (*)(void* )) 2589590618560;

static int List_int32_Count(void* subject)

{

    return _List_int32_Count(subject);

}

            


static bool (* _List_int32_Contains)(void*, int32_t) = (bool (*)(void*, int32_t)) 2589590618496;

static bool List_int32_Contains(void* subject, int32_t object)

{

    return _List_int32_Contains(subject, object);

}

            


static void (* _List_int32_Get)(void*, int) = (void (*)(void*, int, int32_t))2589590618432;

static void List_int32_Get(void* subject, int idx, int32_t object){

        return _List_int32_Get(subject, idx, object);

}

            


static int32_t (* _List_int32_Get)(void*, int) =  (int32_t (*)(void*, int))2589590618368;

static int32_t List_int32_Get(void* subject, int idx)

{

        return _List_int32_Get(subject, idx);

}

            


static void (* _Cell_C1_Set_bar)(void*, char*) = (void (*)(void*, char*))2589590618816;

static void Cell_C1_Set_bar(void* subject, char* object)

{

        return _Cell_C1_Set_bar(subject, object);

}

            


static char* (* _Cell_C1_Get_bar)(void*) = (char* (*)(void*))2589590618752;

static char* Cell_C1_Get_bar(void* subject)

{

        return _Cell_C1_Get_bar(subject);

}

            


static void (* _Cell_C1_Set_foo)(void*, int32_t) = (void (*)(void*, int32_t))2589590618688;

static void Cell_C1_Set_foo(void* subject, int32_t object)

{

        return _Cell_C1_Set_foo(subject, object);

}

            


static int32_t (* _Cell_C1_Get_foo)(void*) = (int32_t (*)(void*))2589590618624;

static int32_t Cell_C1_Get_foo(void* subject)

{

        return _Cell_C1_Get_foo(subject);

}

            


CellAccessor Use_Cell_C1(int64_t cellid, CellAccessOptions options);

{

    CellAccessor accessor;

    auto errCode = LockCell(accessor, options);

    if (errCode)

    throw errCode;

    return accessor;

}

                               

%}

static void Cell_C2_Set_bar(void* subject, char* object);
static char* Cell_C2_Get_bar(void* subject);
static void Cell_C2_Set_lst(void* subject, void* object);
static void* Cell_C2_Get_lst(void* subject);
CellAccessor Use_Cell_C2(int64_t cellid, CellAccessOptions options);
static int List_int32_Count(void* subject);
static bool List_int32_Contains(void* subject, int32_t object);
static void List_int32_Get(void* subject, int idx, int32_t object);
static int32_t List_int32_Get(void* subject, int idx);
static void Cell_C1_Set_bar(void* subject, char* object);
static char* Cell_C1_Get_bar(void* subject);
static void Cell_C1_Set_foo(void* subject, int32_t object);
static int32_t Cell_C1_Get_foo(void* subject);
CellAccessor Use_Cell_C1(int64_t cellid, CellAccessOptions options);

            
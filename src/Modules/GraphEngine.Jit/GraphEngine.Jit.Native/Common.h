#pragma once

#include <iostream>
#include <asmjit.h>
#include "TypeSystem.h"
#include "CellAccessor.h"
#include "Trinity.h"
using namespace asmjit;

#define print(x) std::wcout << x << std::endl
#define debug(x) std::wcout << #x << " = " << (x) << std::endl

class ErrHandler : public ErrorHandler
{
public:
    bool handleError(asmjit::Error err, const char* message, CodeEmitter* origin)
    {
        debug(message);
        return false;
    }
};

asmjit::TypeId::Id GetTypeId(IN TypeDescriptor* const type);

extern "C"
{
    char* tsl_getstring(int32_t* trinity_string_ptr);
    char* tsl_getu8string(int32_t* trinity_string_ptr);
    void* tsl_copy(char* ptr, int32_t size);
    void* tsl_copy_dynamic(int32_t* ptr);
    void tsl_assign(CellAccessor* accessor, char* dst, char* src, int32_t size_dst, int32_t size_src);
    void tsl_setstring(CellAccessor* accessor, int32_t* p, u16char* str);
    void tsl_setu8string(CellAccessor* accessor, int32_t* p, char* trinity_string_ptr);
}


#pragma once

#include <iostream>
#include <asmjit.h>
#include "TypeSystem.h"
#include "CellAccessor.h"
#include "Trinity.h"
#include <io>
using namespace asmjit;

#define print(x) // std::wcout << x << std::endl
#define debug(x) // std::wcout << #x << " = " << (x) << std::endl

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

u16char* tsl_getstring(int32_t* trinity_string_ptr);
u16char* tsl_getu8string(int32_t* trinity_string_ptr);
u16char* tsl_utf8_utf16(char* str, int32_t* len);
char* tsl_utf16_utf8(const u16char* str, int32_t* len);
void* tsl_dup(char* ptr, int32_t size);
void* tsl_dup_dynamic(int32_t* ptr);
uint64_t tsl_hash(void* ptr, int32_t len);
int32_t tsl_newaccessor(CellAccessor* ptr, int32_t len, uint16_t type, uint8_t is_cell);

void* tsl_resize(CellAccessor* accessor, void* where, int32_t delta);

#include "Common.h"
#include "Trinity.h"
#include "CellAccessor.h"
#include <map>
#include <codecvt>
#include "Trinity/Hash/NonCryptographicHash.h"

using namespace asmjit;

static std::map<TypeCode, TypeId::Id> s_atom_typemap{
    {TypeCode::TC_U8         , TypeId::kU8 },
    {TypeCode::TC_U16        , TypeId::kU16 },
    {TypeCode::TC_U32        , TypeId::kU32 },
    {TypeCode::TC_U64        , TypeId::kU64 },
    {TypeCode::TC_I8         , TypeId::kI8 },
    {TypeCode::TC_I16        , TypeId::kI16 },
    {TypeCode::TC_I32        , TypeId::kI32 },
    {TypeCode::TC_I64        , TypeId::kI64 },
    {TypeCode::TC_F32        , TypeId::kF32 },
    {TypeCode::TC_F64        , TypeId::kF64 },
    {TypeCode::TC_BOOL       , TypeId::kU8 },
    {TypeCode::TC_CHAR       , TypeId::kU16 },
};

TypeId::Id GetTypeId(IN TypeDescriptor* const type)
{
    auto c = static_cast<TypeCode>(type->get_TypeCode());
    auto i = s_atom_typemap.find(c);
    return i != s_atom_typemap.cend() ? i->second : TypeId::kUIntPtr;
}

u16char* tsl_getstring(int32_t* trinity_string_ptr)
{
    print(__FUNCTION__);
    debug((int64_t)trinity_string_ptr);
    auto len = *trinity_string_ptr;
    u16char* buf = (u16char*)malloc(len + 2);
    memcpy(buf, trinity_string_ptr + 1, len);
    buf[len / 2] = 0;
    debug(len);
    wprintf(L"tsl_getstring = %s\n", buf);
    return buf;
}

u16char* tsl_getu8string(int32_t* trinity_string_ptr)
{
    auto arr = Trinity::String::Utf8ToUtf16((char*)(trinity_string_ptr + 1), *trinity_string_ptr);
    return arr.detach_data();
}

void* tsl_dup(char* ptr, int32_t size)
{
    auto buf = malloc(size);
    return memcpy(buf, ptr, size);
}

void* tsl_dup_dynamic(int32_t* ptr)
{
    auto len = sizeof(int) + *ptr;
    return tsl_dup((char*)ptr, len);
}

u16char* tsl_utf8_utf16(char* str, int32_t* len)
{
    print(__FUNCTION__);
    debug(str);
    debug(strlen(str));
    auto arr = Trinity::String::Utf8ToUtf16(str, strlen(str));
    debug(arr.Length());
    *len = (arr.Length() - 1) * sizeof(u16char); // exclude trailing 0
    debug(*len);
    return arr.detach_data();
}

char* tsl_utf16_utf8(const u16char* str, int32_t* len)
{
    auto s = Trinity::String::FromWcharArray(str, -1);
    *len = s.Length();
    return _strdup(s.c_str());
}

void* tsl_resize(CellAccessor* accessor, void* where, int32_t delta)
{
    print(__FUNCTION__);
    char* cellPtr;
    auto offset = (int64_t)where - accessor->cellPtr;
    debug(offset);
    debug(delta);
    debug((int64_t)where);

    if (accessor->malloced)
    {
        // TODO reservation
        if (delta > 0)
        {
            cellPtr = (char*)realloc((void*)accessor->cellPtr, accessor->size + delta);
            if (cellPtr == nullptr) throw;
            memmove(
                cellPtr + offset + delta,
                cellPtr + offset,
                (uint64_t)(accessor->size - offset));
        }
        else if (delta < 0)
        {
            cellPtr = (char*)accessor->cellPtr;
            memmove(
                cellPtr + offset,
                cellPtr + offset - delta,
                (uint64_t)(accessor->size - offset + delta));
            cellPtr = (char*)realloc((void*)accessor->cellPtr, accessor->size + delta);
            if (cellPtr == nullptr) throw;
        }
        else
        {
            cellPtr = (char*)accessor->cellPtr;
        }
    }
    else { ::CResizeCell(accessor->cellId, accessor->entryIndex, offset, delta, cellPtr); }

    accessor->cellPtr = reinterpret_cast<int64_t>(cellPtr);
    accessor->size += delta;

    auto ret = cellPtr + offset;
    debug((int64_t)ret);
    return ret;
}

int32_t tsl_newaccessor(CellAccessor* paccessor, int32_t len, uint16_t type, uint8_t is_cell)
{
    auto buf = calloc(1, len);

    if (buf == nullptr) return TrinityErrorCode::E_NOMEM;

    paccessor->cellPtr = (int64_t)buf;
    paccessor->size = len;
    paccessor->malloced = 1;
    paccessor->type = type;
    paccessor->isCell = is_cell;

    return TrinityErrorCode::E_SUCCESS;
}

uint64_t tsl_hash(void* ptr, int32_t len)
{
    uint64_t ret;
    Trinity::Hash::hash_64(&ret, (uint8_t*)ptr, len);
    return ret;
}

#include "Common.h"
#include "Trinity.h"
#include "CellAccessor.h"
#include <map>

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

extern "C"
{
    char* tsl_getstring(int32_t* trinity_string_ptr)
    {
        auto len = *trinity_string_ptr;
        auto str = Trinity::String::FromWcharArray((u16char*)(trinity_string_ptr + 1), len / 2);
        return _strdup(str.c_str());
    }

    char* tsl_getu8string(int32_t* trinity_string_ptr)
    {
        int32_t len = 1 + *trinity_string_ptr;
        char* buf = (char*)malloc(len);
        if (strcpy_s(buf, len, (char*)(trinity_string_ptr + 1))) return nullptr;
        else return buf;
    }

    void* tsl_copy(char* ptr, int32_t size)
    {
        auto buf = malloc(size);
        return memcpy(buf, ptr, size);
    }

    void* tsl_copy_dynamic(int32_t* ptr)
    {
        auto len = sizeof(int) + *ptr;
        return tsl_copy((char*)ptr, len);
    }

    void tsl_assign(CellAccessor* accessor, char* dst, char* src, int32_t size_dst, int32_t size_src)
    {
        if (size_dst != size_src)
        {
            auto offset = reinterpret_cast<int64_t>(dst) - accessor->cellPtr;
            char* cellPtr;
            ::CResizeCell(accessor->cellId, accessor->entryIndex, offset, size_src - size_dst, cellPtr);
            accessor->cellPtr = reinterpret_cast<int64_t>(cellPtr);
            dst = cellPtr + offset;
        }

        memcpy(dst, src, size_src);
    }

    void tsl_setstring(CellAccessor* accessor, int32_t* p, u16char* str)
    {
        auto tlen = *p;
        auto slen = wcslen(str) * sizeof(u16char);

        *p = slen;
        tsl_assign(accessor, (char*)(p + 1), (char*)str, tlen, slen);
    }

    void tsl_setu8string(CellAccessor* accessor, int32_t* p, char* trinity_string_ptr)
    {
        Trinity::String str(trinity_string_ptr);
        int32_t tlen = *p;
        int32_t slen = str.Length();

        *p = slen;
        tsl_assign(accessor, (char*)(p + 1), (char*)str.c_str(), tlen, slen);
    }

    int32_t tsl_newaccessor(CellAccessor* paccessor, int32_t len)
    {
        auto buf = calloc(1, len);

        if (buf == nullptr) return TrinityErrorCode::E_NOMEM;

        paccessor->cellPtr = (int64_t)buf;
        paccessor->size = len;
        paccessor->malloced = 1;

        return TrinityErrorCode::E_SUCCESS;
    }
}


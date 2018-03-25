#pragma once
#include <algorithm>

#define pdebug printf("%s, line %d, this=%llx\n", __FUNCTION__, __LINE__, this);
#define pdebug_ printf("%s, line %d\n", __FUNCTION__, __LINE__);
#pragma pack(push, 1)

template<typename T> void __deepcopy(T* &dst, T* const &src)
{
    if (src) { dst = (T*)malloc(sizeof(T)); *dst = *src; }
    else { dst = nullptr; }
}

void __deepcopy(char* &dst, char* const &src)
{
    if (src) { dst = strdup(src); }
    else { dst = nullptr; }
}

template<typename T, typename S> void __deepcopy_arr(T* &dst, S &dst_size, T* const &src, S const &src_size)
{
    if (src && src_size) {
        dst_size = src_size;
        dst = (T*)malloc(sizeof(T) * src_size);
        for (int i = 0; i < src_size; ++i) { dst[i] = src[i]; }
    }
    else { dst = nullptr; dst_size = 0; }
}

template<typename T> std::vector<T*>* __get_ptrs(T* arr, int len)
{
    std::vector<T*>* vec = new std::vector<T*>();
    for(T* p = arr; p < arr + len; ++p) { vec->push_back(p); }
    return vec;
}

struct AttributeDescriptor
{
    char* Key;
    char* Value;

    AttributeDescriptor() { Key = nullptr; Value = nullptr; }

    ~AttributeDescriptor()
    {
        if (Key) free(Key);
        if (Value) free(Value);

        Key = nullptr;
        Value = nullptr;
    }
};

struct MemberDescriptor;

struct TypeDescriptor
{
    TypeDescriptor() { memset(this, 0, sizeof(TypeDescriptor)); }

    TypeDescriptor(TypeDescriptor&& other) {
        memcpy(this, &other, sizeof(TypeDescriptor));
        memset(&other, 0, sizeof(TypeDescriptor));
    }

    TypeDescriptor(const TypeDescriptor& other) { *this = other; }

    TypeDescriptor& operator = (TypeDescriptor &&other) {
        memcpy(this, &other, sizeof(TypeDescriptor));
        memset(&other, 0, sizeof(TypeDescriptor));
        return *this;
    }

    TypeDescriptor& operator = (const TypeDescriptor &other) {
        __deepcopy(TypeName, other.TypeName);
        __deepcopy(QualifiedName, other.QualifiedName);
        TypeCode = other.TypeCode;
        __deepcopy_arr(ElementType, ElementArity, other.ElementType, other.ElementArity);
        __deepcopy_arr(Members, NrMember, other.Members, other.NrMember);
        __deepcopy_arr(TSLAttributes, NrTSLAttribute, other.TSLAttributes, other.NrTSLAttribute);
        return *this;
    }

    ~TypeDescriptor();

    char* get_TypeName()
    {
        return strdup(TypeName);
    }

    char* get_QualifiedName()
    {
        return strdup(QualifiedName);
    }

    std::vector<TypeDescriptor*>* get_ElementType()
    {
        return __get_ptrs(ElementType, ElementArity);
    }

    std::vector<MemberDescriptor*>* get_Members()
    {
        return __get_ptrs(Members, NrMember);
    }

    std::vector<AttributeDescriptor*>* get_TSLAttributes()
    {
        return __get_ptrs(TSLAttributes, NrTSLAttribute);
    }

    int get_TypeCode()
    {
        return TypeCode;
    }

private:

    char*                TypeName;
    char*                QualifiedName; // AssemblyQualifiedName
    TypeDescriptor*      ElementType;   // non-null for container types
    MemberDescriptor*    Members;       // null for non-struct
    AttributeDescriptor* TSLAttributes; // non-null for cell/field with attributes

    int32_t              NrMember;
    int32_t              NrTSLAttribute;
    int32_t              ElementArity;  // 1 for list
    int32_t              TypeCode;

};

struct MemberDescriptor
{
    MemberDescriptor() { memset(this, 0, sizeof(MemberDescriptor)); }

    MemberDescriptor(MemberDescriptor&& other)
    {
        memcpy(this, &other, sizeof(MemberDescriptor));
        memset(&other, 0, sizeof(MemberDescriptor));
    }

    MemberDescriptor(const MemberDescriptor &other)
    {
        __deepcopy(Name, other.Name);
        Type = other.Type;
        Optional = other.Optional;
    }

    MemberDescriptor& operator = (const MemberDescriptor &other)
    {
        __deepcopy(Name, other.Name);
        Type = other.Type;
        Optional = other.Optional;
        return *this;
    }

    MemberDescriptor& operator = (MemberDescriptor &&other)
    {
        memcpy(this, &other, sizeof(MemberDescriptor));
        memset(&other, 0, sizeof(MemberDescriptor));
        return *this;
    }

    char*          Name;
    TypeDescriptor Type;
    uint8_t        Optional; //non-zero for optional fields

    ~MemberDescriptor()
    {
        if (Name) free(Name);
        Name = nullptr;
    }
};

TypeDescriptor::~TypeDescriptor()
{
    for (int i = 0; i < ElementArity; ++i) { ElementType[i].~TypeDescriptor(); }
    for (int i = 0; i < NrMember; ++i) { Members[i].~MemberDescriptor(); }
    for (int i = 0; i < NrTSLAttribute; ++i) { TSLAttributes[i].~AttributeDescriptor(); }

    if (TypeName) free(TypeName);
    if (ElementType) free(ElementType);
    if (Members) free(Members);
    if (TSLAttributes) free(TSLAttributes);

    TypeName = nullptr;
    ElementType = nullptr;
    Members = nullptr;
    TSLAttributes = nullptr;
}

#pragma pack(pop)
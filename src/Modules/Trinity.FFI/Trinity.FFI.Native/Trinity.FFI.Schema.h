#pragma once

// Flags
#define TC_CELL 0x8000
#define TC_LIST 0x4000

#define pdebug printf("%s, line %d, this=%llx\n", __FUNCTION__, __LINE__, this);
#define pdebug_ printf("%s, line %d\n", __FUNCTION__, __LINE__);
#pragma pack(push, 1)

template<typename T> void __deepcopy(T* &dst, T* const &src)
{
    pdebug_;

    if (&dst == &src) {
        printf("same object\n");
        return;
    }

    if (src) {
        dst = (T*)malloc(sizeof(T));
        *dst = *src;
    }
    else {
        dst = nullptr;
    }
}

void __deepcopy(char* &dst, char* const &src)
{
    pdebug_;

    if (&dst == &src) {
        printf("same object\n");
        return;
    }

    if (src) {
        printf("copying string %llx to dst = %llx\n", &src, &dst);
        dst = strdup(src);
        printf("string %s set to dst: %llx\n", src, dst);
    }
    else {
        dst = nullptr;
    }
}

template<typename T, typename S> void __deepcopy_arr(T* &dst, S &dst_size, T* const &src, S const &src_size)
{
    pdebug_;

    if (&dst == &src) {
        printf("same object\n");
        return;
    }

    if (src && src_size) {
        printf("src_size = %d\n", src_size);
        dst_size = src_size;
        dst = (T*)malloc(sizeof(T) * src_size);
        printf("dst = %llx\n", dst);
        for (int i = 0; i < src_size; ++i) { dst[i] = src[i]; }
    }
    else {
        dst = nullptr;
        dst_size = 0;
    }
}

struct AttributeDescriptor
{
    char* Key;
    char* Value;

    AttributeDescriptor() { Key = nullptr; Value = nullptr; }

    ~AttributeDescriptor()
    {
        pdebug;

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

    TypeDescriptor(const TypeDescriptor& other) {
        *this = other;
    }

    TypeDescriptor& operator = (TypeDescriptor &&other) {

        memcpy(this, &other, sizeof(TypeDescriptor));
        memset(&other, 0, sizeof(TypeDescriptor));

        return *this;
    }

    TypeDescriptor& operator = (const TypeDescriptor &other) {
        pdebug;
        printf("TypeName=%s\n", other.TypeName);
        __deepcopy(TypeName, other.TypeName);
        pdebug;
        TypeCode = other.TypeCode;
        pdebug;
        __deepcopy_arr(ElementType, ElementArity, other.ElementType, other.ElementArity);
        pdebug;
        __deepcopy_arr(Members, NrMember, other.Members, other.NrMember);
        pdebug;
        __deepcopy_arr(TSLAttributes, NrTSLAttribute, other.TSLAttributes, other.NrTSLAttribute);
        pdebug;

        return *this;
    }

    char*                TypeName;
    TypeDescriptor*      ElementType;   // non-null for container types
    MemberDescriptor*    Members;       // null for non-struct
    AttributeDescriptor* TSLAttributes; // non-null for cell/field with attributes

    int32_t              NrMember;
    int32_t              NrTSLAttribute;
    int16_t              ElementArity;  // 1 for list
    uint16_t             TypeCode;

    ~TypeDescriptor();
};

struct MemberDescriptor
{
    MemberDescriptor() { memset(this, 0, sizeof(MemberDescriptor)); }
    MemberDescriptor(const MemberDescriptor &other)
    {
        pdebug;
        __deepcopy(Name, other.Name);
        Type = other.Type;
        Optional = other.Optional;
    }

    MemberDescriptor(MemberDescriptor&& other)
    {
        pdebug;
        memcpy(this, &other, sizeof(MemberDescriptor));
        memset(&other, 0, sizeof(MemberDescriptor));
    }

    MemberDescriptor& operator = (const MemberDescriptor &other)
    {
        pdebug;
        __deepcopy(Name, other.Name);
        Type = other.Type;
        Optional = other.Optional;

        return *this;
    }

    MemberDescriptor& operator = (MemberDescriptor &&other)
    {
        pdebug;
        memcpy(this, &other, sizeof(MemberDescriptor));
        memset(&other, 0, sizeof(MemberDescriptor));

        return *this;
    }

    char*          Name;
    TypeDescriptor Type;
    uint8_t        Optional; //non-zero for optional fields

    ~MemberDescriptor()
    {
        pdebug;
        if (Name) free(Name);
        Name = nullptr;
    }
};

TypeDescriptor::~TypeDescriptor()
{
    pdebug;
    if (this->TypeName) printf("%s\n", this->TypeName);
    else printf("!transferred\n");

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
#pragma once

#include <iostream>
#include <vector>
#include <string>
#include <cstdint>
#include <memory>
#include "dependency_resolver.h"
#include "error.h"
#include "flex_bison_common.h"

#pragma region enum definitions
enum FieldType
{
    /* Atoms */
    FT_ATOM,
    /* Containers */
    FT_ARRAY,
    FT_LIST,
    /* Reference, could be a struct or a enum */
    FT_REFERENCE,
    /* A reference will be determined after parsing, into: */
    FT_STRUCT,
    FT_ENUM
};

enum ProtocolPropertyType
{
    PT_UNDEFINED = 0,

    PT_SYN,
    PT_ASYN,
    PT_HTTP,
    PT_VOID_REQUEST,
    PT_STREAM_REQUEST,
    PT_STRUCT_REQUEST,
    PT_VOID_RESPONSE,
    PT_STREAM_RESPONSE,
    PT_STRUCT_RESPONSE,
};

enum LayoutType
{
    LT_FIXED,
    LT_DYNAMIC
};//to be determined after parsing.

enum IndexType
{
    IT_UNDEFINED = 0,
    IT_SUBSTRING,
};
#pragma endregion

#pragma region Forward declarations

struct YYLTYPE;
class NField;
class NEnum;
class NStruct;
class NIndex;
class NCell;
class NFieldType;
class NStructBase;

namespace Trinity
{
    class String;
}
#pragma endregion

#pragma region Module helpers

#define DECLARE_TRAVERSE_MODULE \
    void tb_semantic_check(); \
    void tb_dependency_check(DependencyResolver&); \
    std::vector<Node*> get_children();

#define ADDITIONAL_ERROR_REPORT(additional_string, base_class) \
    virtual void error(std::string str, std::string prefix = "") { \
    prefix = prefix + " " + std::string(additional_string); \
    base_class::error(str, prefix); \
    }\

#pragma endregion

#pragma region Syntax node comparison
bool NFieldType_LessThan(NFieldType* lhs, NFieldType* rhs);
bool NFieldType_LessThan(const std::unique_ptr<NFieldType> &lhs, const std::unique_ptr<NFieldType> &rhs);
int NFieldType_Compare(NFieldType* lhs, NFieldType* rhs);
#pragma endregion

class Node
{
public:
    Node();
    YYLTYPE *sourceLocation;
    virtual ~Node();

    /// Convention: if override, derived classes should print first, add comma, then call the super class' corresponding facility.
    virtual void error(std::string err, std::string prefix = "");

    void semantic_check();

protected:
    /// Each type of non-virtual Node should implement get_children and return a list of its children.
    virtual std::vector<Node*> get_children() = 0;
    virtual void tb_semantic_check() = 0;
    virtual void tb_dependency_check(DependencyResolver&) = 0;

    void dependency_traverse(DependencyResolver &resolver);
    void delete_children();
};

class NNamed : public Node
{
public:
    ADDITIONAL_ERROR_REPORT(*name + ":", Node);
    std::string *name;
    ~NNamed() { delete name; }
};

class NKVPair : public Node
{
public:
    ADDITIONAL_ERROR_REPORT("'" + *key + "':", Node);
    DECLARE_TRAVERSE_MODULE;
    std::string* key;
    std::string* value;
    ~NKVPair() { delete key; delete value; }
};

//Should be synchronized with TypeConversionAction in Traits.cs
enum TypeConversionAction
{
    TC_NONCONVERTIBLE = 0,
    TC_ASSIGN,
    TC_TOSTRING,
    TC_PARSESTRING,
    TC_TOBOOL,
    TC_CONVERTLIST,
    TC_WRAPINLIST,
    TC_ARRAYTOLIST,
    TC_EXTRACTNULLABLE,
};

class NFieldType : public Node
{
public:
    NFieldType() = default;
    NFieldType(NFieldType* const);

    struct ArrayInformation
    {
        NFieldType* arrayElement;
        std::vector<std::string*>* array_dimension_list;
        std::vector<int>* array_dimension_size;
    };
    FieldType fieldType;
    union
    {
        int atom_token;
        std::string* referencedTypeName;
        NFieldType* listElementType;
        ArrayInformation arrayInfo;
    };
    union
    {
        NEnum*   referencedNEnum;
        NStruct* referencedNStruct;
    };

    /**
     * Points to the field that is specified by this field type.
     * When this field type is an element type of a container,
     * the value is set to NULL.
     */
    NField *field;

    friend bool NFieldType_LessThan(NFieldType* lhs, NFieldType* rhs);
    friend int NFieldType_Compare(NFieldType* lhs, NFieldType* rhs);

    ~NFieldType()
    {
        switch (fieldType)
        {
        case FT_REFERENCE:
            delete referencedTypeName;
            break;
        case FT_ARRAY:
            for (auto *pstr : *arrayInfo.array_dimension_list)
                delete pstr;
            delete arrayInfo.array_dimension_list;
            delete arrayInfo.array_dimension_size;
            //TODO FT_LIST delete, will break Trinity.TSL.CodeGen.Neo.cpp: unitialize()
            break;
        }
    }

    ADDITIONAL_ERROR_REPORT("Field type definition:", Node);
    DECLARE_TRAVERSE_MODULE;
    friend class NField;
    LayoutType layoutType;
    bool is_atom();
    bool is_long();
    bool is_enum();
    bool is_bool();
    bool is_integral();
    bool is_floating_point();
    bool is_numeric();
    bool is_nullable();
    bool is_optional();
    bool is_signed();
    bool is_decimal();
    bool is_string();
    bool is_container_of_strings();
    bool is_value_type();
    bool is_list();
    bool is_array();
    bool is_container();
    bool is_struct();
    bool is_datetime();
    bool is_guid();
    bool is_assignable_from(NFieldType*);
    bool is_convertible_from(NFieldType*);
    bool is_array_same_rank_same_element_type(NFieldType*);
    bool can_enumerate(NFieldType*);
    bool is_alias();
    int enumerate_depth(NFieldType*);
    TypeConversionAction get_type_conversion_action(NFieldType*);
    NFieldType* get_container_element_type();
    size_t type_size();
    std::string get_atom_type();
    void fill_with_sub_field_types(std::vector<NFieldType*>* list);
    std::vector<NFieldType*>* resolve_container_chain();

    void parse_array_dimension_size();
};

class NField : public NNamed
{
public:
    NFieldType *fieldType;
    std::vector<int> *modifiers;
    std::vector<NKVPair*> *attributes;
    NStructBase* parent;
    ~NField() { delete modifiers; delete_children(); /* TODO delete attributes */ }
    ADDITIONAL_ERROR_REPORT("Field", NNamed);
    DECLARE_TRAVERSE_MODULE;
    friend class NStructBase;
    LayoutType getLayoutType() { return fieldType->layoutType; }
    bool is_optional();
    void set_optional();
    void unset_optional();
    void aggregate_indices(NCell*);
    std::string* get_attribute(const std::string &key); // get value by key. return nullptr if not found.
};

class NStructBase : public NNamed
{
public:
    ~NStructBase() { /* TODO */ }
    std::vector<NField*> *fieldList;
    std::vector<NKVPair*> *attributes;
    LayoutType layoutType;
    /* true for struct, false for cell. */
    virtual bool is_struct() = 0;

    bool has_optional_fields()
    {
        for (auto *f : *fieldList)
            if (f->is_optional())
                return true;
        return false;
    }

    void fill_with_sub_field_types(std::vector<NFieldType*>* list);
protected:
    friend class NField;
    friend class NFieldType;
};

class NStruct : public NStructBase
{
public:
    ADDITIONAL_ERROR_REPORT("Struct", NNamed);
    DECLARE_TRAVERSE_MODULE;
    virtual bool is_struct() override { return true; }
};

/**
 *  NIndex is not a AST node to be parsed & generated by Bison.
 *  Instead, it will be generated during the semantic check phase.
 *  Thus, we inherit from AST Node only to take advantage of
 *  the infrastructures we've built for error reporting.
 */
class NIndex : public Node
{
public:
    NIndex(NCell*, NField*, NKVPair*);
    NIndex() = delete;

    ADDITIONAL_ERROR_REPORT("Index:", Node);
    /* Bypass traverse module. */
    void tb_semantic_check(){}
    void tb_dependency_check(DependencyResolver&){}
    std::vector<Node*> get_children(){ return std::vector<Node*>(); }

    std::string*                            target;
    IndexType                               type;
    NCell*                                  cell;
    NField*                                 field;//points to the indexed field of the cell.
    NField*                                 target_field;//points to the last on the access path. The real target.
    std::unique_ptr<std::vector<NField*>>   resolve_target();
    bool                                    set_property(Trinity::String&, Trinity::String&);
};

class NCell : public NStructBase
{
public:
    ADDITIONAL_ERROR_REPORT("Cell", NNamed);
    DECLARE_TRAVERSE_MODULE;
    virtual bool is_struct() override { return false; }
    std::vector<NIndex*>* indexList;
};

class NEnumEntry : public NNamed
{
public:
    ADDITIONAL_ERROR_REPORT("Enum entry", NNamed);
    DECLARE_TRAVERSE_MODULE;
    int32_t value;
    bool value_assigned;
};

class NEnum : public NNamed
{
public:
    ADDITIONAL_ERROR_REPORT("Enum", NNamed);
    DECLARE_TRAVERSE_MODULE;
    std::vector<NEnumEntry*> *enumEntryList;
};

class NTrinitySettings : public NNamed
{
public:
    ADDITIONAL_ERROR_REPORT("TrinitySettings", NNamed);
    DECLARE_TRAVERSE_MODULE;
    std::vector<NKVPair*> *settings;
};
class NProtocolProperty : public Node
{
public:
    DECLARE_TRAVERSE_MODULE;
    ProtocolPropertyType propertyType;
    std::string *data;
};
class NProtocol : public NNamed
{
public:
    ADDITIONAL_ERROR_REPORT("Protocol", NNamed);
    DECLARE_TRAVERSE_MODULE;
    std::vector<NProtocolProperty*> *protocolPropertyList;
    //To be calculated in semantic check
    ProtocolPropertyType  pt_request    = PT_UNDEFINED;
    ProtocolPropertyType  pt_response   = PT_UNDEFINED;
    ProtocolPropertyType  pt_type       = PT_UNDEFINED;

    inline bool has_request()             { return (pt_request  != PT_UNDEFINED); }
    inline bool has_response()            { return (pt_response != PT_UNDEFINED); }
    inline bool has_protocol_type()       { return (pt_type     != PT_UNDEFINED); }
    inline bool is_http_protocol()        { return (pt_type     == PT_HTTP); }
    inline bool is_syn_req_protocol()     { return (pt_type     == PT_SYN  && pt_response == PT_VOID_RESPONSE); }
    inline bool is_syn_req_rsp_protocol() { return (pt_type     == PT_SYN  && pt_response != PT_VOID_RESPONSE); }
    inline bool is_asyn_req_protocol()    { return (pt_type     == PT_ASYN && pt_response == PT_VOID_RESPONSE); }
    inline void set_property(NProtocolProperty *p)
    {
        switch (p->propertyType)
        {
        case ProtocolPropertyType::PT_SYN:
            pt_type = PT_SYN;
            break;
        case ProtocolPropertyType::PT_ASYN:
            pt_type = PT_ASYN;
            break;
        case ProtocolPropertyType::PT_HTTP:
            pt_type = PT_HTTP;
            break;
        case PT_STREAM_REQUEST:
            pt_request = PT_STREAM_REQUEST;
            break;
        case PT_VOID_REQUEST:
            pt_request = PT_VOID_REQUEST;
            break;
        case PT_STRUCT_REQUEST:
            pt_request = PT_STRUCT_REQUEST;
            request_message_struct = p->data;
            break;
        case PT_STREAM_RESPONSE:
            pt_response = PT_STREAM_RESPONSE;
            break;
        case PT_VOID_RESPONSE:
            pt_response = PT_VOID_RESPONSE;
            break;
        case PT_STRUCT_RESPONSE:
            pt_response = PT_STRUCT_RESPONSE;
            response_message_struct = p->data;
            break;

        }
    }
    std::string* request_message_struct = NULL;
    std::string* response_message_struct = NULL;
};

class NProtocolReference : public NNamed
{
public:
    ADDITIONAL_ERROR_REPORT("Protocol reference", NNamed);
    DECLARE_TRAVERSE_MODULE;
};

class NProtocolGroup : public NNamed
{
public:
    bool has_http_protocol();
    std::vector<NProtocolReference*> *protocolList;
};
class NServer : public NProtocolGroup
{
public:
    ADDITIONAL_ERROR_REPORT("Server", NNamed);
    DECLARE_TRAVERSE_MODULE;
};
class NProxy : public NProtocolGroup
{
public:
    ADDITIONAL_ERROR_REPORT("Proxy", NNamed);
    DECLARE_TRAVERSE_MODULE;
};
class NModule : public NProtocolGroup
{
public:
    ADDITIONAL_ERROR_REPORT("Module", NNamed);
    DECLARE_TRAVERSE_MODULE;
};

class NTSL : public Node
{
public:
    DECLARE_TRAVERSE_MODULE;
    std::vector<NStruct*>			*structList   = new std::vector<NStruct*>();
    std::vector<NCell*>				*cellList     = new std::vector<NCell*>();
    std::vector<NTrinitySettings*>	*settingsList = new std::vector<NTrinitySettings*>();
    std::vector<NProtocol*>			*protocolList = new std::vector<NProtocol*>();
    std::vector<NServer*>			*serverList   = new std::vector<NServer*>();
    std::vector<NProxy*>			*proxyList    = new std::vector<NProxy*>();
    std::vector<NModule*>			*moduleList   = new std::vector<NModule*>();
    std::vector<NEnum*>				*enumList     = new std::vector<NEnum*>();
    NStruct*    find_struct(std::string* struct_name)
    {
        for (auto _struct : *structList)
            if (*_struct->name == *struct_name)
                return _struct;
        return NULL;
    }
    NStructBase*find_struct_or_cell(std::string* struct_name)
    {
        for (auto _struct : *structList)
            if (*_struct->name == *struct_name)
                return _struct;
        for (auto cell : *cellList)
            if (*cell->name == *struct_name)
                return cell;
        return NULL;
    }
    //was referenced by server inheritence code
    //NProtocolGroup* find_protocol_group(std::string* protocol_group_name)
    //{
    //    for (auto server : *serverList)
    //        if (*server->name == *protocol_group_name)
    //            return server;
    //    for (auto proxy : *proxyList)
    //        if (*proxy->name == *protocol_group_name)
    //            return proxy;
    //    return NULL;
    //}
    NProtocol*  find_protocol(std::string* protocol_name)
    {
        for (auto _proto : *protocolList)
            if (*_proto->name == *protocol_name)
                return _proto;
        return NULL;
    }
    NEnum*      find_enum(std::string* enum_name)
    {
        for (auto _enum : *enumList)
            if (*_enum->name == *enum_name)
                return _enum;
        return NULL;
    }
};

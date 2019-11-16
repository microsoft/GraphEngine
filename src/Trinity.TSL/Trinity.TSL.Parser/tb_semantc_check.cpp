#include "SyntaxNode.h"
#include "error.h"
#include "parser.tab.h"
#include <string>
#include <vector>
#include <unordered_map>
#include <unordered_set>
#include "Trinity/String.h"
#include <array>
using namespace std;

#define DEFINE_TB_SEMANTIC_CHECK(_class, code) \
    void _class::tb_semantic_check() { \
    code; \
    }

inline string get_key(string* str) { return *str; }
inline string get_key(NKVPair* target) { return *target->key; }
inline string get_key(NNamed* target) { return *target->name; }
inline string get_key(NIndex* target)
{
    string ret = *target->target;
    ret.append(":").append(std::to_string(target->type));
    return ret;
}
inline string get_key(NProtocolProperty* target)
{
    switch (target->propertyType)
    {
    case PT_ASYN:
    case PT_SYN:
    case PT_HTTP:
        return "NProtocolProperty::ProtocolType";
        break;
    case PT_STREAM_REQUEST:
    case PT_VOID_REQUEST:
    case PT_STRUCT_REQUEST:
        return "NProtocolProperty::RequestType";
        break;
    case PT_STREAM_RESPONSE:
    case PT_VOID_RESPONSE:
    case PT_STRUCT_RESPONSE:
        return "NProtocolProperty::ResponseType";
        break;
    }
    throw "get_key: should never reach here!";
    return "";
}

template <typename T>
vector<vector<T>> group_entries_by_key(vector<T>* input)
{
    unordered_multimap<string, T> deduper;
    unordered_set<string> keys;
    for (auto *node : *input)
    {
        deduper.insert(std::make_pair(get_key(node), node));
        keys.insert(get_key(node));
    }

    vector<vector<T>> ret;

    for (auto &key : keys)
    {
        vector<T> list;
        auto its = deduper.equal_range(key);
        for (auto it = its.first; it != its.second; ++it)
            list.push_back(it->second);
        ret.push_back(std::move(list));
    }

    return ret;
}

template <typename T>
bool error_on_duplicate_entries(Node* node, const vector<vector<T>> &list, const string& msg)
{
    bool dup = false;
    for (auto &entry_group : list)
        if (entry_group.size() > 1)
        {
            dup = true;
            node->error(msg);
            for (auto entry : entry_group)
                ::error(entry, "\tHere: ");
        }
    return dup;
}

bool error_on_duplicate_entries(Node* node, const vector<vector<string*>> &&list, const string& msg)
{
    bool dup = false;
    for (auto &entry_group : list)
        if (entry_group.size() > 1)
        {
            dup = true;
            node->error(msg);
        }
    return dup;
}

static std::unordered_set<std::string> reserved_typenames
{
    "GenericCell",
    "GenericCellAccessor",
    "GenericField",
    "GenericFieldAccessor",
    "bit",
    "byte",
    "sbyte",
    "bool",
    "char",
    "short",
    "ushort",
    "int",
    "uint",
    "long",
    "ulong",
    "float",
    "double",
    "decimal",
    "DateTime",
    "Guid",
    "string",
    "String",
    "CellIdCache",
    "CellAccessOptions",
    "StringAccessor",
    "BufferAllocator",
    "DateTimeAccessor",
    "EnumAccessor",
    "GuidAccessor",
};

static std::unordered_set<std::string> invalid_identifiers
{
    /* C# keywords */
    "abstract",
    "as",
    "base",
    "bool",
    "break",
    "byte",
    "case",
    "catch",
    "char",
    "checked",
    "class",
    "const",
    "continue",
    "decimal",
    "default",
    "delegate",
    "do",
    "double",
    "else",
    "enum",
    "event",
    "explicit",
    "extern",
    "false",
    "finally",
    "fixed",
    "float",
    "for",
    "foreach",
    "goto",
    "if",
    "implicit",
    "in",
    "int",
    "interface",
    "internal",
    "is",
    "lock",
    "long",
    "namespace",
    "new",
    "null",
    "object",
    "operator",
    "out",
    "override",
    "params",
    "private",
    "protected",
    "public",
    "readonly",
    "ref",
    "return",
    "sbyte",
    "sealed",
    "short",
    "sizeof",
    "stackalloc",
    "static",
    "string",
    "struct",
    "switch",
    "this",
    "throw",
    "true",
    "try",
    "typeof",
    "uint",
    "ulong",
    "unchecked",
    "unsafe",
    "ushort",
    "using",
    "virtual",
    "void",
    "volatile",
    "while",
    //"add",
    //"alias",
    //"ascending",
    //"async",
    //"await",
    //"descending",
    //"dynamic",
    //"from",
    //"get",
    //"global",
    //"group",
    //"into",
    //"join",
    //"let",
    //"orderby",
    //"partial",
    //"remove",
    //"select",
    //"set",
    //"value",
    //"var",
    //"where",
    //"yield",
};

bool error_on_reserved_typename(vector<NNamed*> *list, const string& msg)
{
    bool ret = false;
    for (NNamed* item : *list)
    {
        if (reserved_typenames.count(*item->name))
        {
            item->error(msg);
            ret = true;
        }
    }
    return ret;
}

bool error_on_invalid_identifier(vector<NNamed*> *list, const string& msg)
{
    bool ret = false;
    for (NNamed* item : *list)
    {
        if (invalid_identifiers.count(*item->name))
        {
            item->error(msg);
            ret = true;
        }
    }
    return ret;
}

static std::array<const char*, 4> s_clash_suffices =
{
    "Array",
    "List",
    "_Accessor",
    "_Message_Accessor"
};

bool check_possible_name_clashing_impl(NNamed* lhs, std::string* rhs, const string& msg)
{
    Trinity::String lhs_name  = *lhs->name;
    Trinity::String rhs_name  = *rhs;

    if (!lhs_name.StartsWith(rhs_name))
        return false;

    // lhs starts with rhs.
    for (const char* suffix : s_clash_suffices)
    {
        Trinity::String rhs_with_suffix = rhs_name + suffix;
        if (lhs_name.Contains(rhs_with_suffix))
        {
            lhs->error(msg, "Warning: ");
            return true;
        }
    }

    return false;
}

/**
 *  Checks if it's possible that the given NNamed will clash with a
 *  reserved type.
 */
bool check_possible_name_clashing(NNamed* node, const string& msg)
{
    for (auto reserved : reserved_typenames)
    {
        if (check_possible_name_clashing_impl(node, &reserved, msg))
            return true;
    }

    return false;
}

void warning_on_possible_name_clashing(vector<NNamed*> *list, const string& msg)
{
    for (NNamed* item : *list)
    {
        if (check_possible_name_clashing(item, msg))
            continue;
        for (NNamed* clash_with : *list)
        {
            if (check_possible_name_clashing_impl(item, clash_with->name, msg))
                break;
        }
    }
}

template <typename T>
bool warning_on_empty(Node* node, vector<T> *list, const char* msg)
{
    if (list->size())
        return false;
    node->error(msg, "Warning: ");
    return true;
}

DEFINE_TB_SEMANTIC_CHECK(NKVPair, {/*how could a kvpair go wrong*/ })

DEFINE_TB_SEMANTIC_CHECK(NFieldType, {
    //Apart from error checking, we calculate the LayoutType here

    NStruct* nstruct = NULL;
    NEnum* nenum = NULL;

    switch (fieldType)
    {
    case FT_ATOM://nothing wrong
        layoutType = is_string()  ? LT_DYNAMIC : LT_FIXED;
        break;
    case FT_REFERENCE://check if we can find a struct or a enum
        nstruct = tsl->find_struct(referencedTypeName);
        nenum = tsl->find_enum(referencedTypeName);
        if (nstruct != NULL && nenum != NULL)
        {
            this->error("Ambiguous field type.");
        }
        else if (nstruct == NULL && nenum == NULL)
        {
            this->error("Struct/Enum " + *referencedTypeName + " doesn't exist.");
            //TODO check in other lists and see if a user
            //is trying to reference something else, such as a cell.
        }
        else if (nstruct != NULL)
        {
            this->layoutType = nstruct->getLayoutType();
            this->fieldType = FT_STRUCT;
            this->referencedNStruct = nstruct;
        }
        else /*enum*/
        {
            this->layoutType = LT_FIXED;
            this->fieldType = FT_ENUM;
            this->referencedNEnum = nenum;
        }
        break;
    case FT_ENUM:
    case FT_STRUCT:
        //TODO why it reached here? Semantic check is performed more than once?
        break;
    case FT_LIST:
        this->layoutType = LT_DYNAMIC;
        break;
    case FT_ARRAY:
        this->layoutType = LT_FIXED;
        if (arrayInfo.arrayElement->layoutType == LT_DYNAMIC)
        {
            //dynamic elements in array is not allowed
            error("Could not define an array of dynamic elements. Use 'List' instead.");
            //however, to further detect errors, we set layoutType to dynamic
            //to see what effect will it bring if this particular struct is really dynamic
        }

        if (arrayInfo.arrayElement->fieldType == FT_ARRAY)
        {
            error("Syntax error. Array-of-array is not supported. Instead, to define a multi-dimensional array, use \"Type[Dim1,Dim2,...]\".");
        }

        this->parse_array_dimension_size();
        break;
    }
})

DEFINE_TB_SEMANTIC_CHECK(NField, {
    auto &&kvgroup = group_entries_by_key(attributes);
    error_on_duplicate_entries(this, kvgroup, "Duplicate attributes with same keys: ");

    if (invalid_identifiers.count(*this->name))
    {
        error("Invalid field name.");
    }
})

void SemanticCheckForStructBase(NStructBase* node)
{
    auto &&kvgroup = group_entries_by_key(node->attributes);
    error_on_duplicate_entries(node, kvgroup, "Duplicate attributes with same keys: ");
    auto &&field_group = group_entries_by_key(node->fieldList);
    error_on_duplicate_entries(node, field_group, "Duplicate fields with same names: ");

    //Raise a warning if there's a fixed field after a dynamic one
    bool reached_dynamic = false;
    for (auto *field : *node->fieldList)
        field->parent = node;
    for (auto *field : *node->fieldList)
    {
        if (field->getLayoutType() == LT_DYNAMIC || field->is_optional())
            reached_dynamic = true;
        else if (reached_dynamic /* and layout type is FIXED */)
        {
            node->error("Warning: A fixed field is placed after a dynamic one. Consider moving it to the front for better performance.");
            break;
        }
    }
}

DEFINE_TB_SEMANTIC_CHECK(NStruct, {
    SemanticCheckForStructBase(this);
    warning_on_empty(this, this->fieldList, "Struct empty.");
})
DEFINE_TB_SEMANTIC_CHECK(NCell, {
    SemanticCheckForStructBase(this);
    warning_on_empty(this, this->fieldList, "Cell empty.");

    /* Aggregate index attributes. */
    if (indexList != NULL)
        delete indexList;
    indexList = new std::vector<NIndex*>();
    for (auto *field : *fieldList)
    {
        field->aggregate_indices(this);
    }
    auto &&index_group = group_entries_by_key(indexList);
    error_on_duplicate_entries(this, index_group, "Duplicate indices on the same target.");
})
DEFINE_TB_SEMANTIC_CHECK(NTrinitySettings, {
    auto &&kvgroup = group_entries_by_key(settings);
    error_on_duplicate_entries(this, kvgroup, "Duplicate settings with same keys: ");
    warning_on_empty(this, this->settings, "TrinitySettings empty.");
})
DEFINE_TB_SEMANTIC_CHECK(NProtocol, {
    auto &&property_group = group_entries_by_key(protocolPropertyList);
    error_on_duplicate_entries(
        this,
        property_group,
        "Redefinition of a protocol property: ");
    for (auto *prop : *protocolPropertyList)
        set_property(prop);
    if (!has_protocol_type())
        error("Protocol type (SYN/ASYN/HTTP) not defined.");
    /**
     * @note    We allow users to omit request/response type, indicating a 'void' request/response type.
     */
    if (!has_request())
        pt_request = PT_VOID_REQUEST;
    if (!has_response())
        pt_response = PT_VOID_RESPONSE;

    if (pt_type != PT_HTTP)
    {
        if (pt_request == PT_STREAM_REQUEST)
            error("Invalid request type: 'Stream' is for HTTP message only.");
        if (pt_response == PT_STREAM_RESPONSE)
            error("Invalid request type: 'Stream' is for HTTP message only.");
    }
})
DEFINE_TB_SEMANTIC_CHECK(NProtocolReference, {
    this->referencedNProtocol = tsl->find_protocol(this->name);
    if (!this->referencedNProtocol) { error("Undefined protocol."); }
})

void NProtocolProperty::tb_semantic_check()
{
    if (propertyType == PT_STRUCT_REQUEST && !tsl->find_struct_or_cell(data))
        error("Undefined request message struct '" + *data + "'.");
    if (propertyType == PT_STRUCT_RESPONSE && !tsl->find_struct_or_cell(data))
        error("Undefined response message struct '" + *data + "'.");
}

void protocol_group_semantics_check(NProtocolGroup* node)
{
    auto &&protocol_group = group_entries_by_key(node->protocolList);
    error_on_duplicate_entries(
        node, protocol_group, "Duplicate references to the same protocol:");

}

DEFINE_TB_SEMANTIC_CHECK(NServer, {
    protocol_group_semantics_check(this);
    warning_on_empty(this, this->protocolList, "Server doesn't reference any protocols.");
})

DEFINE_TB_SEMANTIC_CHECK(NProxy, {
    protocol_group_semantics_check(this);
    warning_on_empty(this, this->protocolList, "Proxy doesn't reference any protocols.");
})

DEFINE_TB_SEMANTIC_CHECK(NModule, {
    protocol_group_semantics_check(this);
    warning_on_empty(this, this->protocolList, "Module doesn't reference any protocols.");
})

DEFINE_TB_SEMANTIC_CHECK(NEnum, {

    //Enum reference material:
    //http://msdn.microsoft.com/en-us/library/whbyts4t.aspx
    //And confirmed that C# has the same behaviors

    auto &&enum_group = group_entries_by_key(enumEntryList);
    error_on_duplicate_entries(this, enum_group, "Duplicate enum entry name: ");

    //assign default values to the unassigned entries
    int32_t default_value = 0;
    for (auto entry : *this->enumEntryList)
    {
        if (!entry->value_assigned)
        {
            entry->value = default_value++;
        }
        else
        {
            default_value = entry->value + 1;
        }
    }

    //Should not check for duplicated entry values. It should be allowed.
    warning_on_empty(this, this->enumEntryList, "Enum empty.");
})
DEFINE_TB_SEMANTIC_CHECK(NEnumEntry, {})
DEFINE_TB_SEMANTIC_CHECK(NTSL, {
    error_on_duplicate_entries(this,
    group_entries_by_key(cellList), "Duplicate definition for cell: ");
    error_on_duplicate_entries(this,
                               group_entries_by_key(structList), "Duplicate definition for struct: ");
    error_on_duplicate_entries(this,
                               group_entries_by_key(settingsList), "Duplicate definition for settings: ");
    error_on_duplicate_entries(this,
                               group_entries_by_key(protocolList), "Duplicate definition for protocol: ");
    error_on_duplicate_entries(this,
                               group_entries_by_key(serverList), "Duplicate definition for server: ");
    error_on_duplicate_entries(this,
                               group_entries_by_key(proxyList), "Duplicate definition for proxy: ");
    error_on_duplicate_entries(this,
                               group_entries_by_key(enumList), "Duplicate definition for enum: ");

    vector<NNamed*> *group_together = new vector<NNamed*>();
    group_together->insert(group_together->end(), cellList->begin(), cellList->end());
    group_together->insert(group_together->end(), structList->begin(), structList->end());
    group_together->insert(group_together->end(), settingsList->begin(), settingsList->end());
    group_together->insert(group_together->end(), protocolList->begin(), protocolList->end());
    group_together->insert(group_together->end(), serverList->begin(), serverList->end());
    group_together->insert(group_together->end(), proxyList->begin(), proxyList->end());
    group_together->insert(group_together->end(), enumList->begin(), enumList->end());

    error_on_duplicate_entries(this,
                               group_entries_by_key(group_together), "Multiple definition with same name: ");

    error_on_reserved_typename(group_together, "Type name is reserved.");
    error_on_invalid_identifier(group_together, "Invalid type name.");
    warning_on_possible_name_clashing(group_together, "Type name will possibly clash with a generated or predefined type name.");

    warning_on_empty(this, cellList, "No cell definitions.");
    warning_on_empty(this, protocolList, "No protocol definitions. System will only have basic interfaces.");
    warning_on_empty(this, serverList, "No server definitions. Instantiate servers with the default TrinityServer.");
    warning_on_empty(this, group_together, "TSL empty.");

    delete group_together;
})

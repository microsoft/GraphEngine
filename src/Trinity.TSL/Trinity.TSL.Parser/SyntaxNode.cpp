#include "SyntaxNode.h"
#include "parser.tab.h"
#include "flex_bison_common.h"
#include "debug.h"
#include <unordered_map>
#include <cstdlib>
#include <cstring>
#include "Trinity/String.h"

Node::Node()
{
    //copies lhs_loc, which is extracted in YYLLOC_DEFAULT from the LHS structure 'yyloc'
    sourceLocation = new YYLTYPE();
    sourceLocation->filename = lhs_loc.filename;
    sourceLocation->first_column = lhs_loc.first_column;
    sourceLocation->first_line = lhs_loc.first_line;
    sourceLocation->last_column = lhs_loc.last_column;
    sourceLocation->last_line = lhs_loc.last_line;
#ifdef LOCATION_TEST
    wcout << "Node location:[" << sourceLocation.first_line << ":" << sourceLocation.first_column << "-" << sourceLocation.last_line << ":" << sourceLocation.last_column << "]" << endl;
#endif
}
Node::~Node()
{
    delete sourceLocation;
}

void Node::delete_children()
{
    for (Node* child : this->get_children())
    {
        delete child;
    }
}

void Node::error(string err, string prefix)
{
    std::string error_string = prefix;
    char err_first_char = err.empty() ? ' ' : err.front();

    if (!prefix.empty() && !isspace(prefix.back()) && (!isspace(err_first_char)))
        error_string.push_back(' ');
    error_string.append(err);
    ::error(*sourceLocation, error_string);
}

void Node::dependency_traverse(DependencyResolver &resolver)
{
    resolver.declareNode(this);
    for (Node* &node : this->get_children())
    {
        resolver.declareDependency(this, node);
        node->dependency_traverse(resolver);
    }
    tb_dependency_check(resolver);
}

void Node::semantic_check()
{
    DependencyResolver resolver;
    vector<Node*> traverse_sequence;
    vector<vector<Node*>> recursive_dependencies;

    dependency_traverse(resolver);
    //get_traverse_sequence will remove the vertices with dependency problems.
    //thus, tb_semantic_check() can assume that all the dependencies are met, safely.
    resolver.get_traverse_sequence(traverse_sequence, recursive_dependencies);

    for (auto *it : traverse_sequence)
    {
        it->tb_semantic_check();
    }

    //for now, the legacy codegen does not support recursive dependencies, so
    //we just report such situation as an error. in the future we may want to
    //add support for recursive structures.
    for (auto &rec_ring : recursive_dependencies)
    {
        error("", "Recursive dependency detected.");
        for (auto *node : rec_ring)
        {
            node->error("here:");
        }
    }

    return;
}

/**
 * Copy construct from a pointer of another type(that).
 * Field pointer will be removed, which means that the
 * copy-constructed field type cannot be optional.
 */
NFieldType::NFieldType(NFieldType* const that)
{
    this->fieldType = that->fieldType;
    this->layoutType = that->layoutType;
    this->field = nullptr;
    this->sourceLocation = that->sourceLocation;

    switch (this->fieldType)
    {
    case FT_ARRAY:
        this->arrayInfo = that->arrayInfo;
        break;
    case FT_ATOM:
        this->atom_token = that->atom_token;
        break;
    case FT_ENUM:
        this->referencedNEnum = that->referencedNEnum;
        this->referencedTypeName = that->referencedTypeName;
        break;
    case FT_LIST:
        this->listElementType = that->listElementType;
        break;
    case FT_STRUCT:
        this->referencedNStruct = that->referencedNStruct;
        this->referencedTypeName = that->referencedTypeName;
        break;
    default:
        throw exception("NFieldType::NFieldType(NFieldType* const that)");
    }
}

bool NFieldType::is_atom()
{
    return fieldType == FT_ATOM;
}

bool NFieldType::is_long()
{
    return is_atom() && atom_token == T_LONGTYPE;
}

bool NFieldType::is_enum()
{
    return fieldType == FT_ENUM;
}

bool NFieldType::is_bool()
{
    return fieldType == FT_ATOM && atom_token == T_BOOLTYPE;
}

bool NFieldType::is_integral()
{
    if (fieldType != FT_ATOM)
        return false;
    switch (atom_token)
    {
    case T_BYTETYPE:
    case T_SBYTETYPE:
    case T_SHORTTYPE:
    case T_USHORTTYPE:
    case T_INTTYPE:
    case T_UINTTYPE:
    case T_LONGTYPE:
    case T_ULONGTYPE:
        return true;
    default:
        return false;
    }
}

bool NFieldType::is_signed()
{
    switch (atom_token)
    {
    case T_SBYTETYPE:
    case T_SHORTTYPE:
    case T_INTTYPE:
    case T_LONGTYPE:
    case T_FLOATTYPE:
    case T_DOUBLETYPE:
    case T_DECIMALTYPE:
        return true;
    default:
        /* unsigned or non-integral/non-fp: return false. */
        return false;
    }
}

bool NFieldType::is_decimal()
{
    return fieldType == FT_ATOM && atom_token == T_DECIMALTYPE;
}

bool NFieldType::is_numeric()
{
    return this->is_integral() || this->is_floating_point();
}

bool NFieldType::is_floating_point()
{
    switch (atom_token)
    {
    case T_FLOATTYPE:
    case T_DOUBLETYPE:
    case T_DECIMALTYPE:
        return true;
    default:
        return false;
    }
}

bool NFieldType::is_string()
{
    return fieldType == FT_ATOM && (atom_token == T_STRINGTYPE || atom_token == T_U8STRINGTYPE);
}

bool NFieldType::is_container_of_strings()
{
    switch (fieldType)
    {
    case FT_ARRAY:
        //XXX currently this is not happening as strings don't fit in arrays.
        return (arrayInfo.arrayElement->is_string() || arrayInfo.arrayElement->is_container_of_strings());
    case FT_LIST:
        return (listElementType->is_string() || listElementType->is_container_of_strings());
    default:
        return false;
    }
}

bool NFieldType::is_datetime()
{
    return (fieldType == FT_ATOM && atom_token == T_DATETIMETYPE);
}

bool NFieldType::is_guid()
{
    return (fieldType == FT_ATOM && atom_token == T_GUIDTYPE);
}

bool NFieldType::is_optional()
{
    if (!field)
        return false;
    return field->is_optional();
}

//  A value type is a type 'allocated on stack' instead of 
//  'allocated on heap'.
bool NFieldType::is_value_type()
{
    switch (fieldType)
    {
    case FT_ARRAY:
    case FT_LIST:
        return false;
    case FT_ATOM:
        return !is_string();
    case FT_STRUCT:
    case FT_ENUM:
    default:
        /*Cannot have FT_REFERENCE after semantic calculation*/
        return true;
    }
}

bool NFieldType::is_list()
{
    return fieldType == FT_LIST;
}

bool NFieldType::is_array()
{
    return fieldType == FT_ARRAY;
}

bool NFieldType::is_container()
{
    return is_list() || is_array();
}

bool NFieldType::is_struct()
{
    return fieldType == FT_STRUCT;
}

bool NFieldType::is_nullable()
{
    return is_optional() && is_value_type();
}

/**
 * Tell if both this and that are arrays of the same element type, and
 * of the same rank.
 */
bool NFieldType::is_array_same_rank_same_element_type(NFieldType* that)
{
    return (this != that && this->is_array() && that->is_array() &&
            !NFieldType_Compare(this->arrayInfo.arrayElement, that->arrayInfo.arrayElement)
            &&
            this->arrayInfo.array_dimension_size->size() == that->arrayInfo.array_dimension_size->size());
}

/**
 * Tell if this type is assignable from that type.
 * We say if this type is assignable from that type,
 * if the assignment thisVar = thatVar is valid.
 * By definition, the following rules apply for validity:
 *
 *  - this and that are the same.
 *  - If this is nullable and that is not, but the non-nullable version of this is assignable from that,
 *    then this is still assignable from that.
 *  - If this is non-nullable while that is, then this is not assignable from that.
 *  - this and that are both lists of a same element type.
 *  - this and that are structs with same type.
 *  - this and that are enums with same type.
 *  - this and that are atoms, and:
 *    - both are strings.
 *    - both are bools.
 *    - both are integral, and:
 *      - longer signed type is assignable from shorter signed/unsigned.
 *      - longer unsigned type is assignable from shorter unsigned.
 *    - longer floating-point is assignable from shorter fp.
 *      - does not apply to decimal <- shorter fp.
 *      - however decimal <- shorter integral is valid.
 *    - floating-point is assignable from integral.
 *
 * @param   *this   The target type to examine.
 * @param    that   The other field type to examine.
 */

bool NFieldType::is_assignable_from(NFieldType* that)
{
    if (!NFieldType_Compare(this, that))
        return true;

    if (!this->is_nullable() && that->is_nullable())
        return false;

    if (this->is_nullable() && !that->is_nullable())
    {
        // mark this temporarily as non-optional
        this->field->unset_optional();

        bool non_optional_this_assignable_from_that = this->is_assignable_from(that);

        // mark this back as optional
        this->field->set_optional();

        return non_optional_this_assignable_from_that;
    }

    /* For arrays with same rank & same element type, we say they are compatible. */
    if (this->is_array_same_rank_same_element_type(that))
        return true;

    if (this->is_list() && that->is_list())
        return (0 == NFieldType_Compare(this->listElementType, that->listElementType));

    if (this->is_enum() && that->is_enum())
        return this->referencedNEnum == that->referencedNEnum;

    if (this->is_struct() && that->is_struct())
        return this->referencedNStruct == that->referencedNStruct;

    if (!(this->is_atom() && that->is_atom()))
        return false;

    /* Here we have this->is_atom() and that->is_atom */

    if (this->is_string() && that->is_string())
        return true;

    if (this->is_integral() && that->is_integral())
    {
        return
            (this->type_size() > that->type_size() && (this->is_signed() || !that->is_signed()))
            ||
            (this->type_size() == that->type_size()
             &&
             this->is_signed() == that->is_signed());
    }

    if (this->is_bool() && that->is_bool())
        return true;

    if (this->is_floating_point() && that->is_integral())
        return true;

    if (this->is_floating_point() && that->is_floating_point())
    {
        if (this->is_decimal() /* && !that->is_decimal() */)
            return false;

        return this->type_size() > that->type_size();
    }

    /* anything else, we say this is not assignable from that. */
    return false;
}

/**
 * Determine the conversion action from that type to this type.
 * This is convertible from that if:
 *
 *  - This is assignable from that.
 *  - This is string or that is string.
 *  - This is bool and that is numeric.
 *  - This is list and:
 *    - That is list, and the element of this is convertible from the element of that.
 *    - That can be converted to an element of this.
 *    - That is array and the element types are convertible.
 *  - TODO This is array and that is list
 *  - This is convertible from the non-nullable version of that.
 */

TypeConversionAction NFieldType::get_type_conversion_action(NFieldType* that)
{
    if (this->is_assignable_from(that))
        return TypeConversionAction::TC_ASSIGN;

    if (this->is_string() && !that->is_string())
        return TypeConversionAction::TC_TOSTRING;

    if (!this->is_string() && that->is_string())
        return TypeConversionAction::TC_PARSESTRING;

    if (this->is_bool() && that->is_numeric())
        return TypeConversionAction::TC_TOBOOL;

    if (this->is_list())
    {
        if (that->is_list() && this->listElementType->is_convertible_from(that->listElementType))
            return TypeConversionAction::TC_CONVERTLIST;

        if (this->listElementType->is_convertible_from(that))
            return TypeConversionAction::TC_WRAPINLIST;

        if (that->is_array() && this->listElementType->is_convertible_from(that->arrayInfo.arrayElement))
            return TypeConversionAction::TC_ARRAYTOLIST;
    }

    if (!this->is_nullable() && that->is_nullable())
    {
        that->field->unset_optional();
        TypeConversionAction conv_action_non_nullable = this->get_type_conversion_action(that);
        that->field->set_optional();

        if (conv_action_non_nullable != TypeConversionAction::TC_NONCONVERTIBLE)
            return TypeConversionAction::TC_EXTRACTNULLABLE;
    }

    /* Otherwise, this is not convertible from that. */
    return TypeConversionAction::TC_NONCONVERTIBLE;
}

/**
 * Tell if this type is convertible from that type by the type converter generated by Traits.cs.
 */

bool NFieldType::is_convertible_from(NFieldType* that)
{
    return (TypeConversionAction::TC_NONCONVERTIBLE != this->get_type_conversion_action(that));
}

/**
 * An alias is a field type that we want to hide from being output to the code,
 * and instead be renamed to something else everytime that it appears.
 * The purpose of alias is to support different internal types that map to the
 * same runtime type recognized by the compiler.
 * For example, string and u8string are both mapped to System.String, but we
 * use the u8string as an internal representation to generate accessors.
 */
bool NFieldType::is_alias()
{
    if (this->fieldType == FT_LIST) { return this->listElementType->is_alias(); }
    else { return this->is_string() && this->atom_token == T_U8STRINGTYPE; }
}

int _enumerate_rank(NFieldType* type)
{
    NFieldType* ft_container_element = type->get_container_element_type();
    if (ft_container_element != NULL)
        return 1 + _enumerate_rank(ft_container_element);
    else
        return 0;
}

/**
 * Tells how many orders should this go to yield that.
 * This method will greedily dig into the container elements, until the "order" of the element
 * is no more than that type.
 * For example, calling List<List<int>>->enumerate_depth(string) will not return 0, causing
 * serializer to serialize the whole object as a string. Instead it digs into the container chain
 * and decide that enumerate_depth should be 2 so that it flattens all the integers and converts
 * them to strings.
 * On the other hand, "the order of the element is no more than that type" means that when we
 * want to enumerate containers ("that" is a container), we do not dig too much into "this",
 * and wrap high-order "this" container elements into "that". For example, calling
 * List<int>->enumerate_depth(List<List<int>>) should not wrap each integer element into List<List<int>>,
 * but rather, wrap the whole List<int> into a single element in List<List<int>>. The same applies
 * to List<List<int>>->enumerate_depth(List<int>), we do not enumerate 2nd-order (integer) elements
 * and wrap them each into a list with a single element, but rather enumerate 1st-order List<int>
 * elements.
 *
 * @returns     N if that is convertible from the N-th order container element (this->element->element...),
                (Go down orders) without wrapping the N-th order container element into a container.
 * -otherwise-  0 if that is convertible from this. (Direct conversion, or go up orders)
 * -otherwise- -1 if cannot enumerate.
 */

int NFieldType::enumerate_depth(NFieldType* that)
{
    bool directly_convertible = that->is_convertible_from(this);

    if (_enumerate_rank(this) > _enumerate_rank(that))
    {
        NFieldType* ft_container_element = this->get_container_element_type();
        if (ft_container_element != NULL)
        {
            int element_enum_depth = ft_container_element->enumerate_depth(that);
            if (element_enum_depth >= 0)
                return 1 + element_enum_depth;
        }
    }

    return directly_convertible ? 0 : -1;
}

/**
 * Complementary to enumerate_depth. We say that a field type can enumerate elements of another field type, if:
 *
 *   - That is convertible from this
 *   - This type is a container and its element can enumerate that.
 */
bool NFieldType::can_enumerate(NFieldType* that)
{
    if (that->is_convertible_from(this))
        return true;

    NFieldType* ft_container_element = this->get_container_element_type();
    if (ft_container_element != NULL && ft_container_element->can_enumerate(that))
        return true;

    return false;
}

NFieldType* NFieldType::get_container_element_type()
{
    if (this->is_array())
        return this->arrayInfo.arrayElement;

    if (this->is_list())
        return this->listElementType;

    return NULL;
}

size_t NFieldType::type_size()
{
    if (layoutType != LT_FIXED) { return -1; }

    if (is_atom())
    {
        switch (atom_token)
        {
        case T_BYTETYPE:
        case T_SBYTETYPE:
        case T_BOOLTYPE:
            return 1;
        case T_SHORTTYPE:
        case T_USHORTTYPE:
            return 2;
        case T_INTTYPE:
        case T_UINTTYPE:
        case T_FLOATTYPE:
            return 4;
        case T_LONGTYPE:
        case T_ULONGTYPE:
        case T_DOUBLETYPE:
        case T_DATETIMETYPE:
            return 8;
        case T_DECIMALTYPE:
        case T_GUIDTYPE:
            return 16;
        }
    }
    else if (is_struct())
    {
        size_t size = 0;
        for (auto &field : *referencedNStruct->fieldList)
        {
            size += field->fieldType->type_size();
        }
        return size;
    }
    else if (is_array())
    {
        size_t size = 1;
        for (auto s : *arrayInfo.array_dimension_size) { size *= s; }
        return size * arrayInfo.arrayElement->type_size();
    }
    else if (is_enum())
    {
        return 1;
    }

    error("type_size(): unknown type");
    return -1;
}

std::string NFieldType::get_atom_type()
{
    switch (atom_token)
    {
    case T_BYTETYPE:
        return "byte";
    case T_SBYTETYPE:
        return "sbyte";
    case T_BOOLTYPE:
        return "bool";
    case T_CHARTYPE:
        return "char";
    case T_SHORTTYPE:
        return "short";
    case T_USHORTTYPE:
        return "ushort";
    case T_INTTYPE:
        return  "int";
    case T_UINTTYPE:
        return "uint";
    case T_LONGTYPE:
        return "long";
    case T_ULONGTYPE:
        return "ulong";
    case T_FLOATTYPE:
        return "float";
    case T_DOUBLETYPE:
        return "double";
    case T_DECIMALTYPE:
        return "decimal";
    case T_DATETIMETYPE:
        return "DateTime";
    case T_GUIDTYPE:
        return "Guid";
    case T_STRINGTYPE:
        return "string";
    case T_U8STRINGTYPE:
        return "u8string";
    default:
        return "???";
    }
}

bool string_is_number(string* pstr)
{
    for (auto chr : *pstr)
        if (!isdigit(chr))return false;
    return true;
}

void NFieldType::parse_array_dimension_size()
{
    arrayInfo.array_dimension_size = new vector<int>();
    for (auto *it : *arrayInfo.array_dimension_list)
    {
        if (string_is_number(it))
        {
            arrayInfo.array_dimension_size->push_back(stoi(*it));//TODO exception handling
        }
        else
        {
            //TODO macro support
            error(*it + ": not a number, nor a defined value");
        }
        delete it;
    }
    delete arrayInfo.array_dimension_list;
    arrayInfo.array_dimension_list = NULL;
}

int NFieldType_Compare(NFieldType* lhs, NFieldType* rhs)
{
    int ret;

    ret = lhs->is_optional() - rhs->is_optional();

    if (ret != 0)
        return ret;

    ret = lhs->fieldType - rhs->fieldType;
    if (ret != 0)
        return ret;
    switch (lhs->fieldType)
    {
    case FT_ARRAY:
        ret = NFieldType_Compare(lhs->arrayInfo.arrayElement, rhs->arrayInfo.arrayElement);
        if (ret != 0)
            return ret;
        ret =
            lhs->arrayInfo.array_dimension_size->size()
            -
            rhs->arrayInfo.array_dimension_size->size();
        if (ret != 0)
            return ret;
        for (size_t dim = 0; dim < lhs->arrayInfo.array_dimension_size->size(); ++dim)
        {
            ret = lhs->arrayInfo.array_dimension_size->at(dim) - rhs->arrayInfo.array_dimension_size->at(dim);
            if (ret != 0)
                return ret;
        }
        return 0;
    case FT_ATOM:
        return (lhs->atom_token - rhs->atom_token);
    case FT_ENUM:
    case FT_REFERENCE:
    case FT_STRUCT:
        return (lhs->referencedTypeName->compare(*rhs->referencedTypeName));
    case FT_LIST:
        return NFieldType_Compare(lhs->listElementType, rhs->listElementType);
    }
    /* Should not reach here. */
    error("Internal error T5002.");
    return -1;
}
/* Return true if lhs < rhs */
bool NFieldType_LessThan(NFieldType *lhs, NFieldType *rhs)
{
    return NFieldType_Compare(lhs, rhs) < 0;
}
bool NFieldType_LessThan(const std::unique_ptr<NFieldType> &lhs, const std::unique_ptr<NFieldType> &rhs)
{
    return NFieldType_Compare(lhs.get(), rhs.get()) < 0;
}

bool NField::is_optional()
{
    for (auto m : *modifiers)
        if (m == T_OPTIONALMODIFIER)
            return true;
    return false;
}

void NField::set_optional()
{
    modifiers->push_back(T_OPTIONALMODIFIER);
}

void NField::unset_optional()
{
    auto it = std::find(modifiers->begin(), modifiers->end(), T_OPTIONALMODIFIER);
    if (it != modifiers->end())
        modifiers->erase(it);
}

/**
 *  @return     True if a property is successfully set.
 *              False if there's an error.
 */

bool NIndex::set_property(Trinity::String& key, Trinity::String& value)
{
    key.Trim().ToLower();
    if (key == "target")
    {
        if (this->target != NULL)
        {
            error("Duplicated target designation.");
            return false;
        }
        this->target = new std::string(*field->name);
        this->target->append(".").append(value.Data());
        return true;
    }
    else if (key == "type")
    {
        value.ToLower();
        if (value == "substring")
        {
            this->type = IndexType::IT_SUBSTRING;
        }
        else
        {
            error("Unrecognized index type.");
            return false;
        }
        return true;
    }
    else
    {
        error("Unrecognized index property.");
        return false;
    }
}

std::unique_ptr<std::vector<NField*>> NIndex::resolve_target()
{
    std::unique_ptr<std::vector<NField*>>   path(new std::vector<NField*>());
    NStructBase*                            current_struct = cell;
    size_t                                  current_offset = 0;
    size_t                                  length = target->length();

    do
    {
        auto next_offset = target->find_first_of('.', current_offset);
        std::string field_name = target->substr(current_offset,
                                                next_offset == std::string::npos ?
                                                std::string::npos :
                                                next_offset - current_offset);
        NField* current_field = NULL;
        for (auto* f : *current_struct->fieldList)
        {
            if (*f->name == field_name)
            {
                current_field = f;
                break;
            }
        }
        if (current_field == NULL)
        {
            error("Unrecognized field'" + field_name + "'.");
            goto resolve_target_failed;
        }
        path->push_back(current_field);
        if (next_offset == std::string::npos)
        {
            break;
        }
        else
        {
            current_offset = next_offset + 1;
            if (current_offset >= length)
            {
                error("Syntax error. Expecting an identifier.");
                goto resolve_target_failed;
            }
            if (current_field->fieldType->is_struct())
            {
                current_struct = current_field->fieldType->referencedNStruct;
            }
            else if (current_field->fieldType->is_container())
            {
                auto *container_chain = current_field->fieldType->resolve_container_chain();
                //TODO
                error("Field '" + field_name + "' is not a struct.");
                goto resolve_target_failed;
            }
            else
            {
                error("Field '" + field_name + "' is not a struct.");
                goto resolve_target_failed;
            }
        }
    } while (true);

    return path;

resolve_target_failed:
    return std::unique_ptr<std::vector<NField*>>(nullptr);
}

NIndex::NIndex(NCell* cell, NField* field, NKVPair* indexDescriptor)
{
    this->cell           = cell;
    this->sourceLocation = indexDescriptor->sourceLocation;
    this->type           = IndexType::IT_UNDEFINED;
    this->target         = NULL;
    this->field          = field;

    Trinity::String index_str = *indexDescriptor->value;
    if (index_str.StartsWith("\"") && index_str.EndsWith("\""))
        index_str = index_str.Substring(1, index_str.Length() - 2);
    auto properties = index_str.Split(";,");
    for (auto property : properties)
    {
        auto kvp = property.Trim().Split("=");
        if (kvp.Length() != 2)
        {
            error("Syntax error. Index properties should be specified in 'property = value;' form.");
            return;
        }
        if (!set_property(kvp[0], kvp[1]))
            return;
    }

    if (this->target == NULL)
    {
        //XXX memory leak
        this->target = field->name;
    }

    if (this->type == IndexType::IT_UNDEFINED)
    {
        this->type = IndexType::IT_SUBSTRING;
    }

    auto target_field_accessing_path = resolve_target();
    if (target_field_accessing_path.get() == NULL)
    {
        error("Unrecognized target.");
        return;
    }

    NField* target_field = target_field_accessing_path->back();
    if (this->type == IndexType::IT_SUBSTRING &&
        !target_field->fieldType->is_string() &&
        !target_field->fieldType->is_container_of_strings())
    {
        error("Substring index is only valid for strings or containers of strings.");
        return;
    }
    this->target_field = target_field;
}

void NField::aggregate_indices(NCell* cell)
{
    std::vector<NIndex*>    *indexList = cell->indexList;
    for (auto* kvpair : *attributes)
    {
        Trinity::String key = *kvpair->key;
        key.Trim().ToLower();
        if (key == "index")
        {
            auto idx = new NIndex(cell, this, kvpair);
            if (idx->target != NULL)
                indexList->push_back(idx);
            else
                delete idx;
        }
    }
}

std::string* NField::get_attribute(const std::string& key)
{
    for (auto &kvp : *attributes)
    {
        if (*kvp->key == key)
            return kvp->value;
    }

    return nullptr;
}

void NFieldType::fill_with_sub_field_types(std::vector<NFieldType*>* list)
{
    switch (fieldType)
    {
    case FT_ARRAY:
        list->push_back(arrayInfo.arrayElement);
        arrayInfo.arrayElement->fill_with_sub_field_types(list);
        break;
    case FT_LIST:
        list->push_back(listElementType);
        listElementType->fill_with_sub_field_types(list);
        break;
    case FT_STRUCT:
        referencedNStruct->fill_with_sub_field_types(list);
        break;
    case FT_ATOM:
    case FT_ENUM:
    default:
        break;
    }
}

std::vector<NFieldType*>* NFieldType::resolve_container_chain()
{
    NFieldType* current_ft        = const_cast<NFieldType*>(this);
    std::vector<NFieldType*>* ret = new std::vector<NFieldType*>();
    while (true)
    {
        ret->push_back(current_ft);
        switch (current_ft->fieldType)
        {
        case FT_ARRAY:
            current_ft = current_ft->arrayInfo.arrayElement;
            break;
        case FT_LIST:
            current_ft = current_ft->listElementType;
            break;
        default:
            return ret;
        }
    }
}

void NStructBase::fill_with_sub_field_types(std::vector<NFieldType*>* list)
{
    for (auto *field : *fieldList)
    {
        list->push_back(field->fieldType);
        field->fieldType->fill_with_sub_field_types(list);
    }
}

bool NProtocolGroup::has_http_protocol()
{
    for (auto *protocol_reference : *protocolList)
    {
        if (tsl->find_protocol(protocol_reference->name)->is_http_protocol())
            return true;
    }
    return false;
}


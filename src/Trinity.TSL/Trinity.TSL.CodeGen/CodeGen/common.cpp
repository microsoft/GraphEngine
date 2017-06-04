#include <corelib>
#include "Trinity.TSL.CodeGen.h"
#include "common.h"
#include "parser.tab.h"
namespace Trinity
{
    namespace Codegen
    {
        std::string GetDataTypeString_impl(NFieldType* n_field_type)
        {
            std::string ret;
            Trinity::String atom_str;
            switch (n_field_type->fieldType)
            {
            case FT_ARRAY:
                ret.append(GetString(n_field_type->arrayInfo.arrayElement));
                ret.append("[");
                for (size_t dim = 1; dim < n_field_type->arrayInfo.array_dimension_size->size(); ++dim)
                    ret.append(",");
                ret.append("]");
                return ret;
            case FT_ATOM:
                atom_str = n_field_type->get_atom_type();
                atom_str.Replace("u8string", "string");//alias u8string to string. TODO generalize alias
                return atom_str;
            case FT_ENUM:
            case FT_STRUCT:
                return *n_field_type->referencedTypeName;
            case FT_LIST:
                ret = "List<";
                ret.append(GetString(n_field_type->listElementType));
                ret.append(">");
                return ret;
            }
            /* Should not reach here. */
            error("Internal error T5001.");
            return "";
        }

        /**
        * Returns the typestring of the given field.
        * The string can be directly embedded into generated code,
        * and will reference the correct type by CSC.
        * Note that, for managed types(non-value types), we surpress
        * '?' at the end to prevent Nullable<ManagedType>.
        *
        * @param    n_field_type  Target type.
        * @returns                Type string.
        */
        std::string GetDataTypeString(NFieldType* n_field_type)
        {
            std::string field_typename = GetDataTypeString_impl(n_field_type);
            if (n_field_type->is_optional() && n_field_type->is_value_type())
                field_typename.append("?");
            return field_typename;
        }

        std::string GetNonNullableValueTypeString(NFieldType* n_field_type)
        {
            return GetDataTypeString_impl(n_field_type);
        }

        /**
         * The "DisplayString" is not a type string, but serves as a part
         * of a variable name/function name. All symbols invalid in an identifier
         * are removed, for example, List<int> becomes List_int.
         */
        std::string GetDataTypeDisplayString(NFieldType* n_field_type)
        {
            std::string ret;
            switch (n_field_type->fieldType)
            {
            case FT_ARRAY:
                ret.append(GetDataTypeDisplayString(n_field_type->arrayInfo.arrayElement));
                ret.append("_Array");
                for (int dim_size : *n_field_type->arrayInfo.array_dimension_size)
                {
                    ret.append("_");
                    ret.append(std::to_string(dim_size));
                }
                break;
            case FT_LIST:
                ret = "List_";
                ret.append(GetDataTypeDisplayString(n_field_type->listElementType));
                break;
            default:
                ret = GetDataTypeString_impl(n_field_type);
                break;
            }
            if (n_field_type->is_nullable())
                ret.append("_nullable");
            return ret;
        }

        //TODO
        std::string GetGenericDataTypeString(NFieldType* n_field_type)
        {
            if (n_field_type->is_struct())
            {
                if (n_field_type->is_optional())
                    return "GenericStruct?";
                else
                    return "GenericStruct";
            }
            else return GetString(n_field_type);
        }

        /**
         * Tell if the data type should be accessed with an
         * accessor, or simply via pointer.
         * All dynamic-layout types need accessor.
         * For the fixed-layout types, the primitive types
         * (int, bool, enum, etc) can be accessed with pointers,
         * but the struct types, including built-in structs
         * such as DateTime, Guid, should be accessed with
         * accessors.
         */
        bool data_type_need_accessor(NFieldType* type)
        {
            // TODO POD structs like DateTime and Guid does not require accessors.
            // Investigate whether we can simply modify the code here and make
            // the codegen generate more efficient code.
            // NOTE POD structs have no arrays, so LT_FIXED is not a sufficient condition for POD.
            return type->layoutType == LT_DYNAMIC || type->is_array() || type->is_struct() || type->is_datetime() || type->is_guid();
        }

        /**
         * Tell if the data type is compatible with the cell.
         * We say that the data type is compatible with the cell,
         * if any of the cell fields are assignable from the given type,
         * that is, at least one of the field can be assigned from the given type
         * (cell.field = type_value).
         * @param    type         The target type to examine.
         * @param    cell         The cell to examine.
         */
        bool data_type_compatible_with_cell(NFieldType* type, NCell* cell)
        {
            for (auto *cell_field : *cell->fieldList)
                if (cell_field->fieldType->is_assignable_from(type))
                    return true;
            return false;
        }

        bool data_type_need_bool_convert(NFieldType *type, NFieldType* cell_field)
        {
            return cell_field->is_bool() && (type->is_integral() || type->is_floating_point());
        }

        bool data_type_need_string_parse(NFieldType *type, NFieldType* cell_field)
        {
            return type->is_string() && !cell_field->is_string();
        }

        bool data_type_need_tostring(NFieldType *type, NFieldType* cell_field)
        {
            return !type->is_string() && cell_field->is_string();
        }

        /**
         * @return  True if the type cannot be parsed with
         *          built-in Type.TryParse() routine, or
         *          a TSL-generated Struct.TryParse() routine.
         *
         *          The rules are simple:
         *          1. For arrays and lists, there are no TryParse(),
         *             so we use ExternalParser to call the parser
         *             of the element type.
         *          2. For nullable, there is no TryParse(),
         *             so we use ExternalParser to call the parser
         *             of the non-nullable version of the type.
         *          3. For bool, we want to extend the capability
         *             of TryParse() so that it accepts numerical
         *             strings. So we handle that in ExternalParser.
         *          4. For DateTime, we want to handle some cases
         *             where DateTime.Parse(string) would fail.
         */
        bool data_type_need_external_parser(NFieldType* type)
        {
            if (type->is_nullable())
                return true;

            if (type->is_bool())
                return true;

            if (type->is_datetime())
                return true;

            switch (type->fieldType)
            {
            case FT_ARRAY:
            case FT_LIST:
                return true;
            default:
                return false;
            }
        }

        /**
         * Tells if it is necessary to generate SetField/TryParse
         * method for the given field type.
         *
         * @param   type        The target type.
         * @param   type_list   A std::vector<NFieldType*>* containing all
         *                      types involed in the current context.
         * @return  True if the type is:
         *         [
         *          1. non-optional or value type,
         *              - or -
         *          2. if optional & non-value-type, and there're no
         *             non-optional equivalent in the given vector.
         *         ]
         *
         * The logic behind this is that, if a type is non-optional/value-
         * type, it must be present in one of the SetField methods.
         * An optional non-value-type is not valid in C# (such as string?),
         * as Nullable<T> only accepts T being value types.
         * However, if there are no equivalent non-optional type of a
         * non-value-type (managed type), there will be no way to parse
         * the field.
         */
        bool data_type_need_set_field(NFieldType* type, std::vector<NFieldType*>* type_list)
        {
            if (!type->is_optional() || type->is_value_type())
                return true;

            /**
             * We have optional & non-value-type here. Disabling optional
             * modifier temporarily.
             */
            type->field->unset_optional();

            for (NFieldType* p_type : *type_list)
            {
                if (p_type != type && !NFieldType_Compare(p_type, type))
                {
                    type->field->set_optional();
                    return false;
                }
            }
            type->field->set_optional();
            return true;
        }

        /**
         * @return      True if type needs set field, and is not duplicated array.
         */

        bool data_type_need_type_id(NFieldType* type, std::vector<NFieldType*>* type_list)
        {
            if (!data_type_need_set_field(type, type_list))
                return false;

            /* We nailed out the optional Nullable<T> issue */

            if (type->is_list())
                return data_type_is_not_duplicate_nested_list_of_array(type, type_list);

            if (!type->is_array())
                return true;

            return data_type_is_not_duplicate_array(type, type_list);
        }

        int get_list_nest_depth_and_element_type(NFieldType* type, NFieldType*& element_type)
        {
            int acc = 1;
            while (type->listElementType->is_list())
            {
                type = type->listElementType;
                acc += 1;
            }
            element_type = type->listElementType;
            return acc;
        }

        bool data_type_is_not_duplicate_nested_list_of_array(NFieldType* type, std::vector<NFieldType*>* type_list)
        {
            NFieldType* this_element_type;
            int this_depth = get_list_nest_depth_and_element_type(type, this_element_type);
            if (!this_element_type->is_array())
                return true;

            for (NFieldType* p_type : *type_list)
            {
                if (!p_type->is_list())
                    continue;

                NFieldType* that_element_type;
                int that_depth = get_list_nest_depth_and_element_type(p_type, that_element_type);
                if (this_depth != that_depth)
                    continue;
                if (!this_element_type->is_array_same_rank_same_element_type(that_element_type))
                    continue;

                int cmp = this_element_type->compare_array_dimension_size_with(that_element_type);
                if (cmp > 0)
                    return false;
                if (cmp < 0)
                    continue;

                /* If we reach here, it means that our deduplication routine has failed. */
                error("Internal error T5004.");
                return false;
            }

            return true;
        }

        /**
         * Tells if the generation of the TryParse(...) in GenericCell of
         * the given type would cause a duplicated definition of method error.
         *
         * When we generate TryParse(...) for array types in GenericCell,
         * we have to be very careful that, all the arrays of different lengths
         * of the same type are fed into the method in the form of T[dim_list],
         * where sizes of the dimensions are omitted. This causes aliasing in
         * the types, and if we generate a method for each of these types,
         * there will be duplicated definitions.
         *
         * Thus, given any array of a specific type in a given type list, we return
         * true in this function iff it has the least capacity in every dimension.
         * Note: arrays of different ranks are not considered for duplication.
         */
        bool data_type_is_not_duplicate_array(NFieldType* type, std::vector<NFieldType*>* type_list)
        {
            if (!type->is_array())
                return true;
            for (NFieldType* p_type : *type_list)
            {
                /* p_type and type points to arrays of the same element type, and rank is the same */
                if (type->is_array_same_rank_same_element_type(p_type))
                {
                    /* We hit a very similar one. Now compare the dimension sizes. */
                    int cmp = type->compare_array_dimension_size_with(p_type);
                    if (cmp > 0)
                        return false;
                    if (cmp < 0)
                        continue;
                    /* If we reach here, it means that our deduplication routine has failed. */
                    error("Internal error T5004.");
                    return false;
                }
            }
            /* We're safe now. */
            return true;
        }

        /**
         * Returns a typestring of an accessor for a type.
         */
        std::string data_type_get_accessor_name(NFieldType* type)
        {
            String ret;
            switch (type->fieldType)
            {
            case FT_ARRAY:
                ret = data_type_get_accessor_name(type->arrayInfo.arrayElement) + "Array";
                for (int &len : *type->arrayInfo.array_dimension_size)
                    ret.Append('_').Append(GetString(len));
                return ret;
            case FT_LIST:
                return data_type_get_accessor_name(type->listElementType) + "ListAccessor";
            case FT_ATOM:
                if (data_type_need_accessor(type))
                {
                    switch (type->atom_token)
                    {
                    case T_DATETIMETYPE:
                        return "DateTimeAccessor";
                    case T_GUIDTYPE:
                        return "GuidAccessor";
                    case T_STRINGTYPE:
                        return "StringAccessor";
                    case T_U8STRINGTYPE:
                        return "U8StringAccessor";
                    }
                }
                else
                {
                    // these FT_ATOM need no accessor, the accessor type is the non-nullable value type.
                    return GetNonNullableValueTypeString(type);
                }
                break;
            case FT_ENUM:
                return *type->referencedNEnum->name;
            case FT_STRUCT:
                return *type->referencedNStruct->name + "_Accessor";
            }

            error(type, "data_type_get_accessor_name: Unknown type");
            return "";
        }

        /**
         * Tells if the given data type is prefixed by a (currently) 4-byte
         * length field in the cell memory. Currently, only variable-length
         * containers have such layouts (strings and lists).
         */
        bool data_type_is_length_prefixed(NFieldType* type)
        {
            return (type->is_string() || type->is_list());
        }

        std::string data_type_get_array_type_with_size_string(NFieldType* type)
        {
            //TODO assert type->is_array()
            std::string ret;
            ret.append(GetDataTypeDisplayString(type->arrayInfo.arrayElement));
            ret.append("[");
            bool first = true;
            for (int dim_size : *type->arrayInfo.array_dimension_size)
            {
                if (!first)
                    ret.append(",");
                else
                    first = false;
                ret.append(std::to_string(dim_size));
            }
            ret.append("]");
            return ret;
        }

        std::string get_comm_class_basename(NProtocolGroup* protocol_group)
        {
            switch (protocol_group->type())
            {
            case PGT_SERVER:
                return "TrinityServer";
            case PGT_PROXY:
                return "TrinityProxy";
            case PGT_MODULE:
                return "CommunicationModule";
            default:
                error(protocol_group, "get_comm_class_basename");
                return "";
            }
        }

        std::string get_comm_protocol_type_string(NProtocol* protocol)
        {
            if (protocol->is_syn_req_protocol())
                return "SynReq";
            if (protocol->is_syn_req_rsp_protocol())
                return "SynReqRsp";
            if (protocol->is_asyn_req_protocol())
                return "AsynReq";
            error(protocol, "get_comm_protocol_type_string");
            return "";
        }

        // Must be synchronized with TrinityMessageType
        std::string get_comm_protocol_trinitymessagetype(NProtocol* protocol)
        {
            if (protocol->is_syn_req_protocol())
                return "TrinityMessageType.SYNC";
            if (protocol->is_syn_req_rsp_protocol())
                return "TrinityMessageType.SYNC_WITH_RSP";
            if (protocol->is_asyn_req_protocol())
                return "TrinityMessageType.ASYNC";
            error(protocol, "get_comm_protocol_type_string");
            return "";
        }

        std::string get_http_handler_parameters(NProtocol* protocol)
        {
            std::string ret;
            switch (protocol->pt_request)
            {
            case PT_STREAM_REQUEST:
                ret.append("HttpListenerRequest request");
                break;
            case PT_STRUCT_REQUEST:
                ret.append(*tsl->find_struct_or_cell(protocol->request_message_struct)->name).append(" request");
                break;
            case PT_VOID_REQUEST:
                /* do nothing */
                break;
            }
            if (protocol->pt_request != PT_VOID_REQUEST && protocol->pt_response != PT_VOID_RESPONSE)
                ret.append(", ");
            switch (protocol->pt_response)
            {
            case PT_STREAM_RESPONSE:
                ret.append("HttpListenerResponse response");
                break;
            case PT_STRUCT_RESPONSE:
                ret.append("out ").append(*tsl->find_struct_or_cell(protocol->response_message_struct)->name).append(" response");
                break;
            case PT_VOID_RESPONSE:
                /* do nothing */
                break;
            }

            return ret;
        }

        std::string get_http_handler_calling_parameters(NProtocol* protocol)
        {
            std::string ret;
            switch (protocol->pt_request)
            {
            case PT_STREAM_REQUEST:
                ret.append("context.Request");
                break;
            case PT_STRUCT_REQUEST:
                ret.append("request_struct");
                break;
            case PT_VOID_REQUEST:
                /* do nothing */
                break;
            }
            if (protocol->pt_request != PT_VOID_REQUEST && protocol->pt_response != PT_VOID_RESPONSE)
                ret.append(", ");
            switch (protocol->pt_response)
            {
            case PT_STREAM_RESPONSE:
                ret.append("context.Response");
                break;
            case PT_STRUCT_RESPONSE:
                ret.append("out response_struct");
                break;
            case PT_VOID_RESPONSE:
                /* do nothing */
                break;
            }

            return ret;
        }

        std::string get__index_member_name(NIndex* idx)
        {
            Trinity::String ret = *idx->target;
            ret.Replace("_", "__");
            ret.Replace('.', '_');
            return ret;
        }

        std::string get__index_access_path_for_cell(NIndex* idx)
        {
            Trinity::String ret = *idx->cell->name;
            auto access_path = idx->resolve_target();
            for (auto *field : *access_path)
            {
                ret.Append(".").Append(*field->name);
                if (field->fieldType->is_nullable())
                    ret.Append(".Value");
            }
            return ret;
        }

        std::string get_signature_string(NFieldType* n_field_type)
        {
            std::string ret;
            switch (n_field_type->fieldType)
            {
            case FT_ARRAY:
                ret.append(get_signature_string(n_field_type->arrayInfo.arrayElement));
                ret.append("[");
                for (size_t dim = 0; dim < n_field_type->arrayInfo.array_dimension_size->size(); ++dim)
                {
                    if (dim != 0)
                        ret.append(",");
                    ret.append(GetString(n_field_type->arrayInfo.array_dimension_size->at(dim)));
                }
                ret.append("]");
                return ret;
            case FT_ATOM:
                return n_field_type->get_atom_type();
            case FT_ENUM:
                return *n_field_type->referencedTypeName;
            case FT_STRUCT:
                return get_signature_string(n_field_type->referencedNStruct);
            case FT_LIST:
                ret = "List<";
                ret.append(get_signature_string(n_field_type->listElementType));
                ret.append(">");
                return ret;
            }
            /* Should not reach here. */
            error("Internal error T5014.");
            return "";
        }

        std::string get_signature_string(NStructBase* s)
        {
            std::string ret = "{";
            bool first = true;

            for (auto* field : *s->fieldList)
            {
                if (first)
                    first = false;
                else
                    ret.append("|");

                if (field->is_optional())
                    ret.append("optional ");

                ret.append(get_signature_string(field->fieldType));
            }

            ret.append("}");
            return ret;
        }
    }
}

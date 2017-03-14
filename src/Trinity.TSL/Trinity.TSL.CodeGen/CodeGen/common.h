#pragma once
#include "Trinity.TSL.CodeGen.h"
#include "OptionalFieldCalculator.h"
#include "AccessorType.h"
#include <string>
#include <SyntaxNode.h>
using std::string;

namespace Trinity
{
    namespace Codegen
    {
        extern std::string target_namespace;
        extern bool        contains_substring_index;

        extern std::vector<NFieldType*>                                    *TSLDataTypeVector;
        extern std::vector<NFieldType*>									   *TSLExternalParserDataTypeVector;
        extern std::vector<NFieldType*>									   *TSLSerializerDataTypeVector;
        extern std::vector<std::unique_ptr<NFieldType>>                    *TSLSubstringIndexedListTypes;

        extern std::vector<NCell*>*                                         TSLIndexIdentifierCellVector;
        extern std::vector<NStruct*>*    									TSLIndexIdentifierStructVector;
        extern std::unordered_map<NStructBase*, std::vector<NField*>*>*     TSLIndexIdentifierSubstructureMap;
        extern std::unordered_map<NStructBase*, std::vector<NField*>*>*     TSLIndexIdentifierTargetMap;
        extern std::unordered_map<NField*, int>*                            TSLIndexFieldIdMap;
        extern std::unordered_map<NIndex*, int>*                            TSLIndexIdMap;
        extern std::vector<NIndex*>*                                        TSLIndexTargetVector;

        std::string GetDataTypeString(NFieldType* n_field_type);
        std::string GetDataTypeDisplayString(NFieldType* n_field_type);
        std::string GetNonNullableValueTypeString(NFieldType* n_field_type);

        inline std::string GetNamespace()
        {
            return target_namespace;
        }

        inline std::string GetString(std::string* str)
        {
            return *str;
        }

        inline std::string GetString(NFieldType* n_field_type)
        {
            return GetDataTypeString(n_field_type);
        }

        template<typename T>
        inline std::string GetString(std::unique_ptr<T> &ptr)
        {
            return GetString(ptr.get());
        }

        inline std::string GetString(int val)
        {
            return std::to_string(val);
        }

        inline std::string GetString(const std::string& val)
        {
            return val;
        }

        inline std::string Discard(const std::string& str)
        {
            return "";
        }
        inline const std::string Discard(int x)
        {
            return "";
        }
        inline const std::string Discard(const std::string* str)
        {
            return "";
        }
        
        bool data_type_need_accessor(NFieldType* type);

        bool data_type_compatible_with_cell(NFieldType* type, NCell* cell);

        bool data_type_need_string_parse(NFieldType *type, NFieldType* cell_field);

        bool data_type_need_bool_convert(NFieldType *type, NFieldType* cell_field);

        bool data_type_need_tostring(NFieldType *type, NFieldType* cell_field);

        bool data_type_need_external_parser(NFieldType* type);

        bool data_type_need_set_field(NFieldType* type, std::vector<NFieldType*>* type_list);

        bool data_type_need_type_id(NFieldType* type, std::vector<NFieldType*>* type_list);

        bool data_type_is_not_duplicate_array(NFieldType* type, std::vector<NFieldType*>* type_list);

        std::string data_type_get_array_type_with_size_string(NFieldType* type);

        std::string data_type_get_accessor_name(NFieldType* type);

        std::string get_http_handler_parameters(NProtocol* protocol);

        std::string get_http_handler_calling_parameters(NProtocol* protocol);

        std::string get__index_member_name(NIndex* idx);

        std::string get__index_access_path_for_cell(NIndex* idx);

        std::string get_signature_string(NStructBase*);
    }
}
#include <unordered_set>
#include <corelib>
#include <utilities>
#include <io>
#include "common.h"
#include "parser.tab.h"

#define NF(x) #x, x

template<typename Func, typename N>
static void fileop_impl(const String& target_path, const String& name, Func gen, N* node, const char* fmode, std::vector<std::string*>* files)
{
    if (!strcmp(fmode, "a"))
    {
        Console::WriteLine("Updating {0}.cs...", name);
    }
    else
    {
        Console::WriteLine("Generating {0}.cs...", name);
    }
    std::string* content         = gen(node);
    Trinity::String content_path = Path::GetFullPath(Path::Combine(target_path, name + ".cs"));

    /* Case-insensitive filesystem/filename duplication fix, must be synchronized with TSLCompiler.GenCode.Interface.cs */
    String lower_content_path = content_path;
    lower_content_path.ToLower();
    while (files && files->end() != std::find_if(files->begin(), files->end(), 
        [&](const std::string* pfname){ return String(pfname->c_str()).ToLower() == lower_content_path; }))
    {
        content_path = lower_content_path = Path::Combine(
            Path::GetDirectoryName(content_path),
            Path::GetFileNameWithoutExtension(content_path) + "_.cs");
        lower_content_path.ToLower();
    }

    Trinity::String content_dir  = Path::GetDirectoryName(content_path);
    if (!Directory::Exists(content_dir))
        Directory::EnsureDirectory(content_dir);
    FILE *fp;
    _wfopen_s(&fp, Trinity::String(content_path).ToWcharArray(), Trinity::String(fmode).ToWcharArray());

    if (fp == NULL)
    {
        error("Cannot open file " + content_path);
        exit(-1);
    }

    // supress warnings
    fprintf(fp, "#pragma warning disable 162,168,649,660,661,1522\n");
    fprintf(fp, "%s\n", content->c_str());
    fprintf(fp, "#pragma warning restore 162,168,649,660,661,1522\n");
    fclose(fp);

    delete content;

    if (files)
    {
        files->push_back(new std::string(content_path.c_str()));
    }
}

template<typename Func, typename N>
static void write_file(const String& target_path, const String& name, Func gen, N* node, std::vector<std::string*>* files)
{
    fileop_impl(target_path, name, gen, node, "w", files);
}

template<typename Func, typename N>
static void append_file(const String& target_path, const String& name, Func gen, N* node, std::vector<std::string*>* files)
{
    fileop_impl(target_path, name, gen, node, "a", files);
}

namespace Trinity
{
    namespace Codegen
    {
        std::string                                                  target_namespace;
        bool                     									 contains_substring_index;

        /**
         * TSLDataTypeVector:               data types referenced as cell fields
         * TSLExternalParserDataTypeVector: TSLDataTypeVector + data types referenced as struct fields
         * TSLSerializerDataTypeVector:     TSLExternalParserDataTypeVector + all structs(no matter referenced or not).
         */

        std::vector<NFieldType*>                                    *TSLDataTypeVector = NULL;
        std::vector<NFieldType*> 									*TSLExternalParserDataTypeVector = NULL;
        std::vector<NFieldType*> 								    *TSLSerializerDataTypeVector = NULL;
        std::vector<std::unique_ptr<NFieldType>>                    *TSLSubstringIndexedListTypes = NULL;
        NFieldType               								    *force_add_string_field_type = NULL;
        NFieldType               								    *force_add_list_string_field_type = NULL;
        std::vector<std::unique_ptr<NFieldType>> 					*force_add_struct_type_list = NULL;

        std::vector<NCell*>*                                         TSLIndexIdentifierCellVector = NULL;
        std::vector<NStruct*>*    								     TSLIndexIdentifierStructVector = NULL;
        std::unordered_map<NStructBase*, std::vector<NField*>*>*     TSLIndexIdentifierSubstructureMap = NULL;
        std::unordered_map<NStructBase*, std::vector<NField*>*>*     TSLIndexIdentifierTargetMap = NULL;
        std::unordered_map<NField*, int>*                            TSLIndexFieldIdMap = NULL;
        std::unordered_map<NIndex*, int>*                            TSLIndexIdMap = NULL;
        std::vector<NIndex*>*                                        TSLIndexTargetVector = NULL;

#pragma region Helper functions
        NFieldType* _get_raw_ptr(NFieldType* ptr)
        {
            return ptr;
        }

        NFieldType* _get_raw_ptr(const std::unique_ptr<NFieldType> &ptr)
        {
            return ptr.get();
        }

        void _sort_ft_vec(std::vector<NFieldType*>* vec)
        {
            std::sort(vec->begin(), vec->end(), (bool(*)(NFieldType*, NFieldType*))NFieldType_LessThan);
        }

        void _sort_ft_vec(std::vector<std::unique_ptr<NFieldType>>* vec)
        {
            std::sort(vec->begin(), vec->end(), (bool(*)(const std::unique_ptr<NFieldType>&, const std::unique_ptr<NFieldType>&))NFieldType_LessThan);
        }
#pragma endregion

        template<typename NFieldTypePtr>
        void DeduplicateNFieldTypeVector(std::vector<NFieldTypePtr>* vec)
        {
            _sort_ft_vec(vec);

            NFieldType* previous = NULL;
            NFieldType* current = NULL;

            for (auto i = vec->begin(); i != vec->end();)
            {
                /* forcibly remove "string?" */
                if ((*i)->is_string() && (*i)->is_optional())
                {
                    i = vec->erase(i);
                    continue;
                }

                /* forcibly remove aliases */
                if ((*i)->is_alias())
                {
                    i = vec->erase(i);
                    continue;
                }

                current = _get_raw_ptr(*i);

                if (previous == NULL || NFieldType_Compare(current, previous))
                {
                    previous = _get_raw_ptr(*i++);
                }
                else
                {
                    i = vec->erase(i);
                }
            }
        }

        void CalculateCellFieldDataTypes(NTSL* node)
        {
            TSLDataTypeVector->clear();
            for (auto *cell : *node->cellList)
                cell->fill_with_sub_field_types(TSLDataTypeVector);

            /* Forcibly enable string type. */

            force_add_string_field_type             = new NFieldType();
            force_add_string_field_type->fieldType  = FT_ATOM;
            force_add_string_field_type->atom_token = T_STRINGTYPE;
            force_add_string_field_type->field      = NULL;
            force_add_string_field_type->layoutType = LT_DYNAMIC;

            TSLDataTypeVector->push_back(force_add_string_field_type);

            /* Forcibly enable List<string> type. */

            force_add_list_string_field_type                  = new NFieldType();
            force_add_list_string_field_type->fieldType       = FT_LIST;
            force_add_list_string_field_type->field           = NULL;
            force_add_list_string_field_type->layoutType      = LT_DYNAMIC;
            force_add_list_string_field_type->listElementType = force_add_string_field_type;

            TSLDataTypeVector->push_back(force_add_list_string_field_type);

            DeduplicateNFieldTypeVector(TSLDataTypeVector);
        }

        void CalculateExternalParserDataTypes(NTSL* node)
        {
            TSLExternalParserDataTypeVector->clear();
            TSLExternalParserDataTypeVector->insert(TSLExternalParserDataTypeVector->begin(),
                                                    TSLDataTypeVector->begin(),
                                                    TSLDataTypeVector->end());
            for (auto *struct_ : *node->structList)
                struct_->fill_with_sub_field_types(TSLExternalParserDataTypeVector);

            std::vector<NFieldType*> forcibly_add_non_nullable_types;

            for (auto* field_type : *TSLExternalParserDataTypeVector)
            {
                if (field_type->is_nullable())
                {
                    //forcibly enable the non-nullable version

                    forcibly_add_non_nullable_types.push_back(new NFieldType(field_type));
                }
            }

            TSLExternalParserDataTypeVector->insert(TSLExternalParserDataTypeVector->end(),
                                                    forcibly_add_non_nullable_types.begin(),
                                                    forcibly_add_non_nullable_types.end());

            DeduplicateNFieldTypeVector(TSLExternalParserDataTypeVector);

        }

        void CalculateSerializerDataTypes(NTSL* node)
        {
            TSLSerializerDataTypeVector->clear();
            TSLSerializerDataTypeVector->insert(TSLSerializerDataTypeVector->begin(),
                                                TSLExternalParserDataTypeVector->begin(),
                                                TSLExternalParserDataTypeVector->end());

            /* Forcibly enable struct types. */

            force_add_struct_type_list = new std::vector<std::unique_ptr<NFieldType>>();
            for (auto *struct_ : *node->structList)
            {
                NFieldType* force_add_struct_type         = new NFieldType();
                force_add_struct_type->fieldType          = FT_STRUCT;
                force_add_struct_type->field              = NULL;
                force_add_struct_type->referencedTypeName = struct_->name;
                force_add_struct_type->layoutType         = LT_DYNAMIC;
                force_add_struct_type->referencedNStruct  = struct_;

                if (std::binary_search(
                    TSLSerializerDataTypeVector->begin(),
                    TSLSerializerDataTypeVector->end(),
                    force_add_struct_type,
                    (bool(*)(NFieldType*, NFieldType*))NFieldType_LessThan))
                {
                    delete force_add_struct_type;
                    continue;
                }
                force_add_struct_type_list->push_back(std::unique_ptr<NFieldType>(force_add_struct_type));
                TSLSerializerDataTypeVector->push_back(force_add_struct_type);
            }

            DeduplicateNFieldTypeVector(TSLSerializerDataTypeVector);
        }

        inline void IndexAddSubstructure(NStructBase* from, NField* to)
        {
            if (!TSLIndexIdentifierSubstructureMap->count(from))
                (*TSLIndexIdentifierSubstructureMap)[from] = new std::vector<NField*>();
            (*TSLIndexIdentifierSubstructureMap)[from]->push_back(to);
        }

        inline void IndexAddTarget(NStructBase* from, NField* to)
        {
            if (!TSLIndexIdentifierTargetMap->count(from))
                (*TSLIndexIdentifierTargetMap)[from] = new std::vector<NField*>();
            (*TSLIndexIdentifierTargetMap)[from]->push_back(to);
        }

        void CalculateIndexVariables(NTSL* tsl)
        {
            std::unordered_set<NCell*>   cellSet;
            std::unordered_set<NStruct*> structSet;
            int                          field_id = 0;
            for (auto* cell : *tsl->cellList)
            {
                if (cell->indexList->size())
                    cellSet.insert(cell);
                for (auto *index : *cell->indexList)
                {
                    auto path = index->resolve_target();
                    NStructBase* structBase = cell;
                    for (auto* field : *path)
                    {
                        if (field->fieldType->is_struct())
                        {
                            auto referencedStruct = field->fieldType->referencedNStruct;
                            IndexAddSubstructure(structBase, field);
                            structBase = referencedStruct;
                            structSet.insert(referencedStruct);
                        }
                        else
                        {
                            IndexAddTarget(structBase, field);
                            TSLIndexTargetVector->push_back(index);
                            (*TSLIndexFieldIdMap)[field] = field_id++;
                        }
                    }
                }
            }

            for (auto* index : *TSLIndexTargetVector)
            {
                (*TSLIndexIdMap)[index] = (*TSLIndexFieldIdMap)[index->target_field];
            }

            TSLIndexIdentifierCellVector->insert(
                TSLIndexIdentifierCellVector->begin(), cellSet.begin(), cellSet.end());
            TSLIndexIdentifierStructVector->insert(
                TSLIndexIdentifierStructVector->begin(), structSet.begin(), structSet.end());

        }

        void CalculateSubstringIndexedDataTypes(NTSL* tsl)
        {
            contains_substring_index = false;
            for (auto* index : *TSLIndexTargetVector)
            {
                if (index->type != IT_SUBSTRING)
                {
                    continue;
                }

                contains_substring_index = true;

                if (index->target_field->fieldType->is_string())
                {
                    /* string is always included in code generation */
                }
                else
                {
                    auto* p_ft  = new NFieldType(*index->target_field->fieldType);
                    p_ft->field = NULL;//remove field reference so that p_ft doesn't appear optional.
                    TSLSubstringIndexedListTypes->push_back(std::unique_ptr<NFieldType>(p_ft));
                }
            }

            DeduplicateNFieldTypeVector(TSLSubstringIndexedListTypes);
        }

        void CalculateRootNamespace(NTSL* tsl)
        {
            for (auto* setting : *tsl->settingsList)
            {
                for (auto* kvpair : *setting->settings)
                {
                    if (*kvpair->key == "RootNamespace")
                        target_namespace = *kvpair->value;
                }
            }
        }

#define NEW(x) x = new std::remove_pointer<decltype(x)>::type()
        void initialize(NTSL* tsl)
        {
            NEW(TSLDataTypeVector);
            NEW(TSLExternalParserDataTypeVector);
            NEW(TSLSerializerDataTypeVector);
            NEW(TSLSubstringIndexedListTypes);

            NEW(TSLIndexIdentifierCellVector);
            NEW(TSLIndexIdentifierStructVector);
            NEW(TSLIndexIdentifierSubstructureMap);
            NEW(TSLIndexIdentifierTargetMap);
            NEW(TSLIndexFieldIdMap);
            NEW(TSLIndexIdMap);
            NEW(TSLIndexTargetVector);

            CalculateCellFieldDataTypes(tsl);
            CalculateExternalParserDataTypes(tsl);
            CalculateSerializerDataTypes(tsl);
            CalculateIndexVariables(tsl);
            CalculateSubstringIndexedDataTypes(tsl);
            CalculateRootNamespace(tsl);
        }

        void uninitialize()
        {
            delete TSLDataTypeVector;
            delete TSLExternalParserDataTypeVector;
            delete TSLSerializerDataTypeVector;
            delete TSLSubstringIndexedListTypes;

            delete force_add_string_field_type;
            delete force_add_list_string_field_type;
            delete force_add_struct_type_list;

            delete TSLIndexIdentifierCellVector;
            delete TSLIndexIdentifierStructVector;
            delete TSLIndexIdentifierSubstructureMap;
            delete TSLIndexIdentifierTargetMap;
            delete TSLIndexFieldIdMap;
            delete TSLIndexIdMap;
            delete TSLIndexTargetVector;
            //TODO delete vectors in maps?
        }

        std::vector<std::string*>* codegen_entry(NTSL* tsl, const std::string& target_path, const std::string& target_namespace)
        {
            initialize(tsl);
            auto *files = new std::vector<std::string*>();
            if (Trinity::Codegen::target_namespace.empty())
                Trinity::Codegen::target_namespace = target_namespace;

            String lib_path             = Path::Combine(target_path, "Lib");
            String linq_path            = Path::Combine(lib_path, "LINQ");
            String substring_index_path = Path::Combine(lib_path, "SubstringIndex");

            if (tsl->cellList->size() != 0)
            {
                write_file(lib_path, NF(GenericCell), tsl, files);
                write_file(lib_path, NF(StorageSchema), tsl, files);
            }

            write_file(lib_path, NF(Enum), tsl, files);
            write_file(lib_path, NF(ExternalParser), tsl, files);
            write_file(lib_path, NF(Traits), tsl, files);
            write_file(lib_path, NF(GenericFieldAccessor), tsl, files);
            write_file(lib_path, NF(HTTP), tsl, files);
            write_file(lib_path, NF(CommunicationSchema), tsl, files);
            write_file(lib_path, NF(Serializer), tsl, files);
            write_file(lib_path, NF(CellSelectors), tsl, files);
            write_file(lib_path, NF(Throw), tsl, files);
            write_file(lib_path, NF(Index), tsl, files);
            write_file(lib_path, NF(ExtensionAttribute), tsl, files);

            /* basic data structure accessors */
            write_file(lib_path, NF(BufferAllocator), tsl, files);
            write_file(lib_path, NF(byteListAccessor), tsl, files);
            write_file(lib_path, NF(DateTimeAccessor), tsl, files);
            write_file(lib_path, NF(doubleListAccessor), tsl, files);
            write_file(lib_path, NF(EnumAccessor), tsl, files);
            write_file(lib_path, NF(GuidAccessor), tsl, files);
            write_file(lib_path, NF(intListAccessor), tsl, files);
            write_file(lib_path, NF(longListAccessor), tsl, files);
            write_file(lib_path, NF(StringAccessor), tsl, files);
            write_file(lib_path, NF(U8StringAccessor), tsl, files);

            /* Index and LINQ */
            if (contains_substring_index)
            {
                write_file(substring_index_path, NF(Indexer), tsl, files);
                write_file(substring_index_path, NF(IndexItem), tsl, files);
                write_file(substring_index_path, NF(Searcher), tsl, files);
            }
            write_file(linq_path, NF(ExpressionTreeRewritter), tsl, files);
            write_file(linq_path, NF(IndexQueryTreeExecutor), tsl, files);
            write_file(linq_path, NF(IndexQueryTreeNode), tsl, files);
            write_file(linq_path, NF(PLINQWrapper), tsl, files);

            /* Cell and Struct */
            for (auto* cell : *tsl->cellList)
            {
                append_file(target_path, *cell->name, Cell, cell, NULL);
            }

            for (auto* _struct : *tsl->structList)
            {
                append_file(target_path, "Structs", Struct, _struct, NULL);
            }

            uninitialize();
            return files;
        }

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

            if (!type->is_array())
                return true;

            return data_type_is_not_duplicate_array(type, type_list);
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
                    for (int i = 0, size = p_type->arrayInfo.array_dimension_size->size(); i != size; ++i)
                    {
                        //TODO invent an iterator over two/more containers
                        int cmp =
                            type->arrayInfo.array_dimension_size->at(i)
                            -
                            p_type->arrayInfo.array_dimension_size->at(i);

                        if (cmp > 0)
                            return false;
                        if (cmp < 0)
                            goto array_is_less;
                    }
                    /* If we reach here, it means that our deduplication routine has failed. */
                    error("Internal error T5004.");
                    return false;
                array_is_less:
                    continue;/*for p_type*/
                }
            }
            /* We're safe now. */
            return true;
        }

        std::string data_type_get_string_container_accessor_name(NFieldType* type)
        {
            //TODO assert type->is_list()
            if (type->listElementType->is_string())
                return "StringAccessorListAccessor";
            std::string ret = data_type_get_string_container_accessor_name(type->listElementType);
            ret.append("ListAccessor");
            return ret;
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


        ///**
        // * Returns a typestring of an accessor of a type,
        // * compatible with legacy TSL codegen.
        // */
        //TODO
        //std::string data_type_get_accessor_type_string(NFieldType* type)
        //{
        //    switch (type->fieldType)
        //    {
        //    case FT_ARRAY:
        //        break;
        //    case FT_LIST:
        //            return data_type_get_accessor_type_string(type->listElementType) + "List";
        //    case FT_ATOM:
        //        break;
        //    case FT_ENUM:
        //        break;
        //    case FT_STRUCT:
        //        break;
        //    case FT_REFERENCE:
        //        break;
        //    }
        //}
    }
}

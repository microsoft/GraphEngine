#include <unordered_set>
#include <corelib>
#include <utilities>
#include <io>
#include "common.h"
#include "parser.tab.h"

#define NF(x) #x, x
#define NAME(x) #x

template<typename Func, typename N>
static void write_file(const String& target_path, const String& name, Func gen, N* node, std::vector<std::string*>* files)
{
    Console::WriteLine("Generating {0}.cs...", name);
    std::string* content         = gen(node);
    Trinity::String content_path = Path::GetFullPath(Path::Combine(target_path, name + ".cs"));

    String lower_content_path = content_path;
    lower_content_path.ToLower();
    while (files && files->end() != std::find_if(files->begin(), files->end(),
                                                 [&](const std::string* pfname) { return String(pfname->c_str()).ToLower() == lower_content_path; }))
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
    _wfopen_s(&fp, Trinity::String(content_path).ToWcharArray(), _u("w"));

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

namespace Trinity
{
    namespace Codegen
    {
        std::string                                                  target_namespace;
        bool                     									 contains_substring_index;
        int                                                          celltype_offset;

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

        std::unordered_set<NStructBase*>                             message_structs;

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

        void CalculateMessageAccessors(NTSL* tsl)
        {
            std::unordered_set<NStructBase*> message_cells;
            auto try_add = [&](std::string* name)
            {
                if (name == nullptr) return;
                auto s = tsl->find_struct_or_cell(name);
                if (s == nullptr) return;
                if (s->is_struct()) message_structs.insert(s);
                else message_cells.insert(s);
            };

            for (auto proto : *tsl->protocolList)
            {
                if (proto->is_http_protocol()) continue;
                if (proto->has_request()) { try_add(proto->request_message_struct); }
                if (proto->has_response()) { try_add(proto->response_message_struct); }
            }

            /* add cell structs for messages */
            for (auto msg_cell : message_cells)
            {
                /* deep copy from msg_cell */
                NStruct* s = new NStruct(msg_cell);
                /* append _Message to the name of the cell */
                *s->name += "_Message";

                /* add optional long CellId field */
                NField *field = new NField();
                field->attributes = new std::vector<NKVPair*>();
                field->parent = msg_cell;
                field->modifiers = new std::vector<int>();
                field->modifiers->push_back(T_OPTIONALMODIFIER);
                field->name = new std::string("CellId");

                field->fieldType = new NFieldType();
                field->fieldType->field = field;
                field->fieldType->fieldType = FT_ATOM;
                field->fieldType->atom_token = T_LONGTYPE;
                field->fieldType->layoutType = LT_FIXED;

                s->fieldList->push_back(field);

                /* add the cell struct to the struct list */
                tsl->structList->push_back(s);
                /* replicate it back to a cell, add to message accessor list. 
                 * note, this NCell will only be used for generating a message accessor.
                 */
                NCell* c = new NCell(s);
                *c->name = *msg_cell->name;
                message_structs.insert(c);
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
                            NStruct* referencedStruct = dynamic_cast<NStruct*>(field->fieldType->referencedNStruct);
                            if (!referencedStruct) { error(cell, "CalculateIndexVariables: referencing a cell"); continue; }
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

            CalculateMessageAccessors(tsl);
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

        std::vector<std::string*>* codegen_entry(NTSL* tsl, const std::string& target_path, const std::string& target_namespace, const int cell_type_offset)
        {
            initialize(tsl);
            auto *files = new std::vector<std::string*>();
            Trinity::Codegen::celltype_offset = cell_type_offset;
            if (Trinity::Codegen::target_namespace.empty())
                Trinity::Codegen::target_namespace = target_namespace;

            String lib_path             = Path::Combine(target_path, "Lib");
            String linq_path            = Path::Combine(lib_path, "LINQ");
            String cell_path            = Path::Combine(target_path, "Cells");
            String struct_path          = Path::Combine(target_path, "Structs");
            String substring_index_path = Path::Combine(lib_path, "SubstringIndex");

            if (tsl->cellList->size() != 0)
            {
                write_file(lib_path, NF(GenericCell), tsl, files);
                write_file(lib_path, NF(StorageSchema), tsl, files);
				write_file(lib_path, NF(CellTypeEnum), tsl, files);
                write_file(lib_path, NF(CellTypeExtension), tsl, files);
            }

            write_file(lib_path, NF(Enum), tsl, files);
            write_file(lib_path, NF(ExternalParser), tsl, files);
            write_file(lib_path, NF(Traits), tsl, files);
            write_file(lib_path, NF(GenericFieldAccessor), tsl, files);
            write_file(lib_path, NF(HTTP), tsl, files);
            write_file(lib_path, NF(Protocols), tsl, files);
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

            /* containers */
            write_file(lib_path, NF(Containers), tsl, files);

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
                write_file(cell_path, *cell->name, Cell, cell, files);
            }

            for (auto* _struct : *tsl->structList)
            {
                write_file(struct_path, *_struct->name, Struct, _struct, files);
            }

            /* Message accessors */
            std::vector<NStructBase*> message_accessors;
            message_accessors.insert(message_accessors.end(), message_structs.begin(), message_structs.end());
            write_file(lib_path, NF(MessageAccessors), &message_accessors, files);

            uninitialize();
            return files;
        }

    }
}

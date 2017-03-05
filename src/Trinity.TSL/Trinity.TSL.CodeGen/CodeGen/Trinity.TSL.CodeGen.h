#pragma once
#include <string>
#include <vector>
#include "SyntaxNode.h"

namespace Trinity
{
    namespace Codegen
    {
        struct ModuleContext
        {
            int                      m_stack_depth;
            std::vector<std::string> m_arguments;
        };

        /**
         * Main entry point of CodeGen.
         * @param   tsl              Root of the syntax tree.
         * @param   target_path      Target path to store generated source files
         * @returns                  A std::vector of the filenames of the generated source files.
         */
        std::vector<std::string*>* codegen_entry(NTSL* tsl, const std::string& target_path, const std::string& target_namespace);

#pragma region Codegen routines

        std::string* GenericCell(NTSL* node);
        std::string* Enum(NTSL* node);
        std::string* Cell(NCell* node);
        std::string* Struct(NStruct* node);
        std::string* ExternalParser(NTSL* node);
        std::string* Traits(NTSL* node);
        std::string* GenericCellAccessor(NTSL* node);
        std::string* GenericFieldAccessor(NTSL* node);
        std::string* HTTP(NTSL* node);
        std::string* Serializer(NTSL* node);
        std::string* CellSelectors(NTSL* node);
        std::string* Index(NTSL* node);
        std::string* Throw(NTSL* node);
        std::string* StorageSchema(NTSL* node);
        std::string* CommunicationSchema(NTSL* node);
        std::string* ExtensionAttribute(NTSL* node);

        std::string* BufferAllocator(NTSL* node);
        std::string* byteListAccessor(NTSL* node);
        std::string* DateTimeAccessor(NTSL* node);
        std::string* doubleListAccessor(NTSL* node);
        std::string* EnumAccessor(NTSL* node);
        std::string* GuidAccessor(NTSL* node);
        std::string* intListAccessor(NTSL* node);
        std::string* longListAccessor(NTSL* node);
        std::string* StringAccessor(NTSL* node);
        std::string* U8StringAccessor(NTSL* node);

#pragma region Indexer and LINQ
        std::string* Indexer(NTSL* node);
        std::string* IndexItem(NTSL* node);
        std::string* Searcher(NTSL* node);

        std::string* ExpressionTreeRewritter(NTSL* node);
        std::string* IndexQueryTreeExecutor(NTSL* node);
        std::string* IndexQueryTreeNode(NTSL* node);
        std::string* PLINQWrapper(NTSL* node);
#pragma endregion

        namespace Modules
        {
            std::string* HTTPModule(NProtocolGroup* node, ModuleContext* context);
            std::string* EnumerateFromFieldModule(NField* node, ModuleContext* context);
            std::string* CommunicationSchemaModule(NProtocolGroup* node, ModuleContext* context);
            std::string* AccessorFieldAssignment(NField* node, ModuleContext* context);
        }

#pragma endregion
    }
}
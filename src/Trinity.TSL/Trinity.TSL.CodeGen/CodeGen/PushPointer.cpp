#include "common.h"
#include "AccessorType.h"
#include <string>
#include <SyntaxNode.h>

using std::string;
using namespace Trinity::Codegen;

static void push_impl(std::string* source, int stack_depth, NStructBase* node, NField* push_until);

static void push_impl(std::string* source, int stack_depth, NFieldType* type)
{
    if (type->layoutType == LT_FIXED)
    {
        *source += "targetPtr += " + GetString(type->type_size()) + ";";
        return;
    }

    /* type is LT_DYNAMIC */
    if (data_type_is_length_prefixed(type))
    {
        *source += "targetPtr += *(int*)targetPtr + sizeof(int);";
        return;
    }

    /* type is not length-prefixed. */
    if (type->is_struct())
    {
        push_impl(source, stack_depth + 1, type->referencedNStruct, nullptr);
        return;
    }

    /* type is not struct */
    error(type, "push_impl: unknown type");
}

static void push_impl(std::string* source, int stack_depth, NStructBase* node, NField* push_until)
{
    source->append("{");

    std::string indent = "            ";
    int pushnum = 0;
    std::string opt_ptr_name = "optheader_" + GetString(stack_depth);
    OptionalFieldCalculator opt(node, opt_ptr_name);
    if (opt.fieldCount != 0)//Contains optional fields.
    {
        *source += indent + "byte* " + opt_ptr_name + " = targetPtr;\n";
        *source += indent + "targetPtr += " + opt.headerLength + ";\n";
    }
    pushnum = 0;

    auto flush_batchpush = [&]()
    {
        if (pushnum != 0)
            *source += indent + "targetPtr += " + GetString(pushnum) + ";\r\n";
        pushnum = 0;
    };

    for (NField* field : *node->fieldList)
    {
        if (field == push_until) break;
        bool field_optional = field->is_optional();
        bool field_fixed = (field->fieldType->layoutType == LT_FIXED);

        if (!field_fixed)
        {
            flush_batchpush();
        }

        if (field_optional)
        {
            *source += R"::(
                if ()::" + opt.GenerateReadBitExpression(field) + R"::()
                {
)::";
        }

        if (field_fixed)
        {
            if (field_optional)//Do not include optional field in a batch fixed push
                *source += indent + "targetPtr += " + GetString(field->fieldType->type_size()) + ";\r\n";
            else
                pushnum += field->fieldType->type_size();
        }
        else
        {
            push_impl(source, stack_depth + 1, field->fieldType);
        }

        if (field_optional)
        {
            *source += R"::(
                }
)::";
        }
    }

    flush_batchpush();
    source->append("}");
}

namespace Trinity
{
    namespace Codegen
    {
        namespace Modules
        {
            /**
             * From the beginning of a struct, push targetPtr to the current field.
             * Assumes that targetPtr has already been placed at the beginning of the struct.
             */
            std::string* PushPointerToCurrentField(NField* node, ModuleContext* context)
            {
                std::string* source = new std::string();
                push_impl(source, context->m_stack_depth, node->parent, node);
                return source;
            }

            /**
             * Push through a field type. The target pointer is "targetPtr"
             */
            std::string* PushPointerThroughFieldType(NFieldType* node, ModuleContext* context)
            {
                std::string* source = new std::string();
                push_impl(source, context->m_stack_depth, node);
                return source;
            }

            /**
             * Push through the whole struct. The target pointer is "targetPtr"
             */
            std::string* PushPointerThroughStruct(NStructBase* node, ModuleContext* context)
            {
                std::string* source = new std::string();
                push_impl(source, context->m_stack_depth, node, nullptr);
                return source;
            }

        }
    }
}
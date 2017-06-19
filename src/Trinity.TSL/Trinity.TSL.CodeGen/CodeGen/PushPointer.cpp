#include "common.h"
#include "AccessorType.h"
#include <string>
#include "SyntaxNode.h"
#include "parser.tab.h"

using std::string;
using namespace Trinity::Codegen;

static std::string indent = "            ";
static void push_impl(std::string* source, int stack_depth, NStructBase* node, NField* push_until);
static void push_and_assign_impl(std::string* source, int stack_depth, NFieldType* node, std::string varName, bool OnlyPushPointer);

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

    int pushnum = 0;
    std::string opt_ptr_name = "optheader_" + GetString(stack_depth);
    OptionalFieldCalculator opt(node, opt_ptr_name);
    if (opt.fieldCount != 0)//Contains optional fields.
    {
        *source += indent + "byte* " + opt_ptr_name + " = targetPtr;\n";
        *source += indent + "targetPtr += " + GetString(opt.headerLength) + ";\n";
    }

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

static void push_and_assign_direct(std::string* source, int stack_depth, NFieldType* node, std::string varName, bool OnlyPushPointer)
{
    if (!OnlyPushPointer)
    {
        *source += indent + "*(" + GetNonNullableValueTypeString(node) + "*)targetPtr = " + varName + ";\n";
    }
    *source += indent + "targetPtr += " + GetString(node->type_size()) + ";\n";
}

static void push_and_assign_guid(std::string* source, int stack_depth, NFieldType* node, std::string varName, bool OnlyPushPointer)
{
    if (OnlyPushPointer)
        *source += "targetPtr += 16;";
    else *source += R"::(
        {
            byte[] tmpGuid = )::" + varName + R"::(.ToByteArray();
            fixed(byte* tmpGuidPtr = tmpGuid)
            {
                Memory.Copy(tmpGuidPtr, targetPtr, 16);
            }
            targetPtr += 16;
        }
)::";
}

static void push_and_assign_datetime(std::string* source, int stack_depth, NFieldType* node, std::string varName, bool OnlyPushPointer)
{
    if (OnlyPushPointer)
        *source += "targetPtr += 8;";
    else *source += R"::(
        {
            *(long*)targetPtr = )::" + varName + R"::(.ToBinary();
            targetPtr += 8;
        }
)::";
}

static void push_and_assign_struct(std::string* source, int stack_depth, NFieldType* node, std::string varName, bool OnlyPushPointer)
{
    if (OnlyPushPointer && node->layoutType == LT_FIXED)
    {
        *source += "targetPtr += " + GetString(node->type_size()) + ";\n";
        return;
    }

    *source += "\n" + indent + "{\n";
    std::string pointer_name = "optheader_" + GetString(stack_depth);
    OptionalFieldCalculator opt(node->referencedNStruct, pointer_name);

    if (opt.fieldCount != 0)
    {
        if (!OnlyPushPointer)
        {
            *source += indent + "byte* " + pointer_name + " = targetPtr;\n";
            *source += indent + opt.GenerateClearAllBitsCode();
        }
        *source += indent + "targetPtr += " + GetString(opt.headerLength) + ";\n";
    }

    for (auto field : *node->referencedNStruct->fieldList)
    {
        bool currentFieldIsOptional = field->is_optional();
        string field_name;

        if (varName == "")
        {
            /* empty varName means the field is directly accessible as a parameter */
            field_name = *field->name;
        }
        else
        {
            field_name = varName + "." + *field->name;
        }

        if (currentFieldIsOptional)
        {
            *source += indent + "if( " + field_name + "!= null)\n";
            *source += indent + "{\n";
        }
        if (currentFieldIsOptional && field->fieldType->is_value_type())
        {
            field_name += ".Value";
        }

        push_and_assign_impl(source, stack_depth + 1, field->fieldType, field_name, OnlyPushPointer);

        if (currentFieldIsOptional)
        {
            if (!OnlyPushPointer)
                *source += opt.GenerateMaskOnCode(field);
            *source += "\n" + indent + "}\n";
        }
    }

    *source += "\n" + indent + "}";
}

static void push_and_assign_array(std::string* source, int stack_depth, NFieldType* node, std::string varName, bool OnlyPushPointer)
{
    /* Note, element-wise push-assign code in the legacy codegen was dropped. */
    string total_length_string = GetString(node->type_size());
    string element_type = GetNonNullableValueTypeString(node->arrayInfo.arrayElement);
    string pointer = "storedPtr_" + GetString(stack_depth);
    if (OnlyPushPointer)
    {
        *source += "targetPtr += " + total_length_string + ";\n";
        return;
    }
    int arity = node->arrayInfo.array_dimension_size->size();

    *source += indent + "if(" + varName + "!= null){\n";
    *source += indent + "   if(" + varName + ".Rank != " + GetString(arity) + ") throw new IndexOutOfRangeException(\"The assigned array'storage Rank mismatch.\");\n";
    *source += indent + "   if(";
    for (int i = 0; i < arity; i++)
        *source += varName + ".GetLength(" + GetString(i) + ") != " + GetString(node->arrayInfo.array_dimension_size->at(i)) + " || ";
    // Trim " || "
    source->pop_back(); source->pop_back(); source->pop_back(); source->pop_back();
    *source += ") throw new IndexOutOfRangeException(\"The assigned array'storage dimension mismatch.\");\n";

    *source += indent + "   fixed(" + element_type + "* " + pointer + " = " + varName + ")\n";
    *source += indent + "       Memory.memcpy(targetPtr, " + pointer + ", (ulong)(" + total_length_string + "));\n";
    *source += indent + "}else{\n";
    *source += indent + "   Memory.memset(targetPtr, 0, (ulong)(" + total_length_string + "));\n";
    *source += indent + "}";
    *source += indent + "targetPtr += " + total_length_string + ";\n";
}

static void push_and_assign_string(std::string* source, int stack_depth, NFieldType* node, std::string varName, bool OnlyPushPointer)
{
    string strlen_name   = "strlen_" + GetString(stack_depth);
    string pstr_name     = "pstr_" + GetString(stack_depth);

    if (!OnlyPushPointer)
    {
        *source += R"::(
        if()::" + varName + R"::(!= null)
        {
            int )::" + strlen_name + " = " + varName + R"::(.Length * 2;
            *(int*)targetPtr = )::" + strlen_name + R"::(;
            targetPtr += sizeof(int);
            fixed(char* )::" + pstr_name + " = " + varName + R"::()
            {
                Memory.Copy()::" + pstr_name + ", targetPtr, " + strlen_name + R"::();
                targetPtr += )::" + strlen_name + R"::(;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }
)::";
    }
    else//Only push pointer
    {
        *source += R"::(
        if()::" + varName + R"::(!= null)
        {
            int )::" + strlen_name + " = " + varName + R"::(.Length * 2;
            targetPtr += )::" + strlen_name + R"::(+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }
)::";
    }
}

static void push_and_assign_u8string(std::string* source, int stack_depth, NFieldType* node, std::string varName, bool OnlyPushPointer)
{
    string u8buffer_name = "u8buffer_" + GetString(stack_depth);
    string u8len_name    = "u8len_" + GetString(stack_depth);

    if (!OnlyPushPointer)
    {
        *source += R"::(
        if()::"+ varName + R"::(!= null)
        {
            byte[] )::" + u8buffer_name + " = Encoding.UTF8.GetBytes(" + varName + R"::();
            int )::" + u8len_name + " = " + u8buffer_name + R"::(.Length;
            *(int*)targetPtr = )::" + u8len_name + R"::(;
            targetPtr += sizeof(int);
            Memory.Copy()::" + u8buffer_name + ", targetPtr, " + u8len_name + R"::();
            targetPtr += )::"+ u8len_name + R"::(;
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }
)::";
    }
    else//Only push pointer
    {
        *source += R"::(
        if()::" + varName + R"::(!= null)
        {
            int )::" + u8len_name + " = Encoding.UTF8.GetByteCount(" + varName + R"::();
            targetPtr += )::" + u8len_name + R"::( + sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }
)::";
    }
}

static void push_and_assign_list(std::string* source, int stack_depth, NFieldType* node, std::string varName, bool OnlyPushPointer)
{
    string iterator_name = "iterator_" + GetString(stack_depth);
    string pointer_name = "storedPtr_" + GetString(stack_depth);

    if (node->listElementType->layoutType == LT_FIXED)
    {
        string len_str = GetString(node->listElementType->type_size());
        string precalculated_length = varName + ".Count*" + len_str;
        if (!OnlyPushPointer)
        {
            *source += R"::(
if()::" + varName + R"::(!= null)
{
    *(int*)targetPtr = )::" + precalculated_length + ";";
            *source += R"::(
    targetPtr += sizeof(int);
    for(int )::" + iterator_name + " = 0;" + iterator_name + "<" + varName + ".Count;++" + iterator_name + R"::()
    {
)::";
            push_and_assign_impl(source, stack_depth + 1, node->listElementType, varName + "[" + iterator_name + "]", OnlyPushPointer);
            *source += R"::(
    }
)::";
            *source += R"::(
}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}
)::";
        }
        else//Only push pointer
        {
            *source += R"::(
if()::" + varName + R"::(!= null)
{
    targetPtr += )::" + precalculated_length + R"::(+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}
)::";
        }
    }
    else
    {
        *source += R"::(
{
)::";
        if (!OnlyPushPointer)
        {
            *source += R"::(byte *)::" + pointer_name + R"::( = targetPtr;
)::";
        }

        *source += R"::(
    targetPtr += sizeof(int);
    if()::" + varName + R"::(!= null)
    {
        for(int )::" + iterator_name + " = 0;" + iterator_name + "<" + varName + ".Count;++" + iterator_name + R"::()
        {
)::";
        push_and_assign_impl(source, stack_depth + 1, node->listElementType, varName + "[" + iterator_name + "]", OnlyPushPointer);
        *source += R"::(
        }
    }
)::";

        if (!OnlyPushPointer)
        {
            *source += "*(int*)" + pointer_name + " = (int)(targetPtr - " + pointer_name + R"::( - 4);
)::";
        }
        *source += R"::(
}
)::";
    }
}

static void push_and_assign_impl(std::string* source, int stack_depth, NFieldType* node, std::string varName, bool OnlyPushPointer)
{
    if (!data_type_need_accessor(node))
    {
        push_and_assign_direct(source, stack_depth, node, varName, OnlyPushPointer);
    }
    // TODO datetime, fixed struct, guid can be merged to !data_type_need_accessor
    else if (node->is_datetime())
    {
        push_and_assign_datetime(source, stack_depth, node, varName, OnlyPushPointer);
    }
    else if (node->is_guid())
    {
        push_and_assign_guid(source, stack_depth, node, varName, OnlyPushPointer);
    }
    else if (node->is_struct())
    {
        push_and_assign_struct(source, stack_depth, node, varName, OnlyPushPointer);
    }
    else if (node->is_array())
    {
        push_and_assign_array(source, stack_depth, node, varName, OnlyPushPointer);
    }
    else if (node->is_list())
    {
        push_and_assign_list(source, stack_depth, node, varName, OnlyPushPointer);
    }
    else if (node->is_string() && node->atom_token == T_STRINGTYPE)
    {
        push_and_assign_string(source, stack_depth, node, varName, OnlyPushPointer);
    }
    else if (node->is_string() && node->atom_token == T_U8STRINGTYPE)
    {
        push_and_assign_u8string(source, stack_depth, node, varName, OnlyPushPointer);
    }
    else
    {
        error(node, "push_and_assign_impl: unknown field type");
    }
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

            /**
             * Checks a variable, and parallely push the "targetPtr" pointer, optionally
             * also assign the value from the variable into targetPtr.
             *
             * Arguments:
             *  0. varname: the variable to check against. If the given NFieldType
             *     is a struct, an empty varname means that the field values are
             *     given as parameters, accessible directly from "fieldname", instead
             *     of being accessible from "varname.fieldname". SerializeParametersToBuffer
             *     module uses this form.
             *  1. action: "push" for push-only; "assign" for assign-and-push
             *
             * Note:
             *  For value-type optional fields, a nullable (type?) variable cannot be
             *  directly assigned, and we should append ".Value" to varname. Since
             *  we don't know whether varname is Nullable<T> or not, it is the caller's
             *  responsibility to maintain a proper 'varname'.
             */
            std::string* PushPointerFromVariable(NFieldType* node, ModuleContext* context)
            {
                bool push_only;
                if (context->m_arguments[1] == "push") { push_only = true; }
                else if (context->m_arguments[1] == "assign") { push_only = false; }
                else { error(node, "PushPointerFromVariable: unrecognized action " + context->m_arguments[1]); }

                std::string varname = context->m_arguments[0];
                std::string* source = new std::string();
                push_and_assign_impl(source, context->m_stack_depth, node, varname, push_only);
                return source;
            }

            /**
             * Like PushPointerFromVariable, but does not look inside a struct to access the fields.
             * Instead it expcets the fields to be given as parameters.
             *
             * Arguments:
             *  0. action: "push" for push-only; "assign" for assign-and-push
             */
            std::string* PushPointerFromParameters(NStructBase* node, ModuleContext* context)
            {
                bool push_only;
                if (context->m_arguments[0] == "push") { push_only = true; }
                else if (context->m_arguments[0] == "assign") { push_only = false; }
                else { error(node, "PushPointerFromParameters: unrecognized action " + context->m_arguments[0]); }

                NFieldType *field = new NFieldType();
                field->fieldType = FT_STRUCT;
                field->referencedNStruct = node;
                field->referencedTypeName = node->name;
                field->layoutType = node->getLayoutType();

                std::string* source = new std::string();
                push_and_assign_impl(source, context->m_stack_depth, field, "", push_only);

                delete field;
                return source;
            }
        }
    }
}
#include "common.h"
#include "AccessorType.h"
#include <string>
#include "SyntaxNode.h"

using std::string;
using namespace Trinity::Codegen;

static void _ValueTypeToAccessorFieldAssignment(NFieldType* type, bool create_optional, string* source)
{
    if (create_optional)
    {
        source->append("\
                targetPtr = this.ResizeFunction(targetPtr, 0, ").append(Codegen::GetString(type->type_size())).append(");");
    }
    source->append("\
                *(").append(GetString(type)).append("*)targetPtr = value;");
}

static void _FixedLengthAccessorFieldAssignment(NFieldType* type, string accessor_field_name, bool create_optional, string* source)
{
    string fieldLength = Codegen::GetString(type->type_size());
    if (!create_optional)
    {
        source->append("\
                Memory.Copy(value.CellPtr, targetPtr, " + fieldLength + "); ");
    }
    else
    {
        source->append(R":(
                int offset = (int)(targetPtr - CellPtr);
                int length = ):" + fieldLength + R":(;
                if (value.CellID != this.CellID)
                {
                    this.CellPtr = this.ResizeFunction(this.CellPtr, offset, length);
                    Memory.Copy(value.CellPtr, this.CellPtr + offset, length);
                }
                else
                {
                    byte[] tmpcell = new byte[length];
                    fixed (byte* tmpcellptr = tmpcell)
                    {
                        Memory.Copy(value.CellPtr, tmpcellptr, length);
                        this.CellPtr = this.ResizeFunction(this.CellPtr, offset, length);
                        Memory.Copy(tmpcellptr, this.CellPtr + offset, length);
                    }
                }
):");
    }
}

static void _StructAccessorFieldAssignment(NFieldType* type, string accessor_field_name, bool create_optional, string* source, ModuleContext* context)
{
    bool isfixed = type->layoutType == LT_FIXED;
    string ret = R"::(
                int offset = (int)(targetPtr - CellPtr);
                byte* oldtargetPtr = targetPtr;)::";

    if (!create_optional)
    {
        Modules::PushPointerThroughFieldType(type, context);
        ret += R"::(
                int oldlength = (int)(targetPtr - oldtargetPtr);)::";
    }
    else
    {
        ret += R"::(
                int oldlength = 0;)::";
    }

    ret += R"::(
                targetPtr = value.CellPtr;)::";
    Modules::PushPointerThroughFieldType(type, context);
    ret += R"::(
                int newlength = (int)(targetPtr - value.CellPtr);)::";
    if (isfixed)
    {
        ret += R"::(
                Memory.Copy(value.CellPtr, oldtargetPtr, oldlength);)::";
    }
    else
    {
        ret += R"::(
                if (newlength != oldlength)
                {
                    if (value.CellID != this.CellID)
                    {
                        this.CellPtr = this.ResizeFunction(this.CellPtr, offset, newlength - oldlength);
                        Memory.Copy(value.CellPtr, this.CellPtr + offset, newlength);
                    }
                    else
                    {
                        byte[] tmpcell = new byte[newlength];
                        fixed(byte* tmpcellptr = tmpcell)
                        {
                            Memory.Copy(value.CellPtr, tmpcellptr, newlength);
                            this.CellPtr = this.ResizeFunction(this.CellPtr, offset, newlength - oldlength);
                            Memory.Copy(tmpcellptr, this.CellPtr + offset, newlength);
                        }
                    }
                }
                else
                {
                    Memory.Copy(value.CellPtr, oldtargetPtr, oldlength);
                })::";
    }
    source->append(ret);
}

static void _LengthPrefixedAccessorFieldAssignment(NFieldType* type, string accessor_field_name, bool create_optional, string* source)
{
    string ret;
    ret += R"::(
                int length = *(int*)(value.CellPtr - 4);)::";
    string resize_len;
                

    if (!create_optional)
    {
        ret += R"::(
                int oldlength = *(int*)targetPtr;)::";
        resize_len = "length - oldlength";
    }
    else
    {
        resize_len = "length + sizeof(int)";
    }

    ret += R"::(
                if (value.CellID != " + accessorName + @".CellID)
                {
                    //if not in the same Cell
                    )::" + accessor_field_name + ".CellPtr = " + accessor_field_name + ".ResizeFunction(targetPtr, 0, " + resize_len + R"::();
                    Memory.Copy(value.CellPtr - 4, )::" + accessor_field_name + R"::(.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        )::" + accessor_field_name + ".CellPtr = " + accessor_field_name + ".ResizeFunction(targetPtr, 0, " + resize_len + R"::();
                        Memory.Copy(tmpcellptr, " + accessorName + @".CellPtr, length + 4);
                    }
                }
)::";

    source->append(ret);
}

namespace Trinity
{
    namespace Codegen
    {
        namespace Modules
        {
            /**
             * Generates accessor to accessor-field assignment code.
             * When this module is called, the caller should guarantee that 'targetPtr'
             * points to the location of the field to be assigned. Also, if an accessor
             * is needed for the field, the CellPtr field of the accessor should point
             * to targetPtr, and CellID should be properly set.
             * Arguments:
             * 0. accessor field name
             * 1. "FieldDoesNotExist" or "FieldExists" -> determines whether
             *    we create an optional field.
             */
            string* AccessorToAccessorFieldAssignment(NFieldType* type, ModuleContext* context)
            {
                string* source          = new string();
                string  accessor_name   = context->m_arguments[0];
                bool    create_optional = (context->m_arguments[1] == "FieldDoesNotExist");

                if (!data_type_need_accessor(type))
                {
                    _ValueTypeToAccessorFieldAssignment(type, create_optional, source);
                }
                else if (type->layoutType == LT_FIXED)
                {
                    _FixedLengthAccessorFieldAssignment(type, accessor_name, create_optional, source);
                }
                else if (data_type_is_length_prefixed(type))
                {
                    _LengthPrefixedAccessorFieldAssignment(type, accessor_name, create_optional, source);
                }
                else if (type->is_struct())
                {
                    _StructAccessorFieldAssignment(type, accessor_name, create_optional, source, context);
                }
                else
                {
                    error(type, "AccessorToAccessorFieldAssignment: could not process type" + Codegen::GetString(type));
                }

                return source;
            }
        }
    }
}
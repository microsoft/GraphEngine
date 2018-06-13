using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL;

namespace t_Namespace
{
    class AccessorFields : __meta
    {
        [MODULE_BEGIN]
        [TARGET("NStructBase")]
        [MAP_LIST("t_field", "node->fieldList")]
        [MAP_VAR("t_field", "")]
        [MAP_VAR("t_field_name", "name", MemberOf = "t_field")]
        [MAP_VAR("t_field_type", "fieldType")]
        [MAP_VAR("t_data_type", "GetNonNullableValueTypeString($$->fieldType)" , MemberOf = "t_field")]
        [MAP_VAR("t_accessor_type", "data_type_get_accessor_name($$->fieldType)", MemberOf = "t_field")]

        [FOREACH]
        [META_VAR("bool", "field_fixed", "($t_field_type->layoutType == LT_FIXED)")]
        [META_VAR("bool", "field_optional", "($t_field->is_optional())")]
        [META_VAR("bool", "field_need_accessor", "(data_type_need_accessor($t_field_type))")]
        [META_VAR("bool", "field_lenprefix", "(data_type_is_length_prefixed($t_field_type))")]
        [META_VAR("OptionalFieldCalculator", "optcalc", "OptionalFieldCalculator(node, \"this.m_ptr\")")]
        [META_VAR("std::string", "accessor_field_name", "(*$t_field_name) + \"_Accessor_Field\"")]
        [IF("%field_need_accessor")]
        t_accessor_type t_field_name_Accessor_Field;
        [END]

        [IF("%field_optional")]
        ///<summary>
        ///Represents the presence of the optional field t_field_name.
        ///</summary>
        public bool Contains_t_field_name
        {
            get
            {
                unchecked
                {
                    return META_OUTPUT("%optcalc.GenerateReadBitExpression($t_field)"); ;//meta command swallows one ';'
                }
            }
            internal set
            {
                unchecked
                {
                    if (value)
                    {
                        META_OUTPUT("%optcalc.GenerateMaskOnCode($t_field)");
                    }
                    else
                    {
                        META_OUTPUT("%optcalc.GenerateMaskOffCode($t_field)");
                    }
                }
            }
        }

        ///<summary>
        ///Removes the optional field t_field_name from the object being operated.
        ///</summary>
        public unsafe void Remove_t_field_name()
        {
            if (!this.Contains_t_field_name)
            {
                throw new Exception("Optional field t_field_name doesn't exist for current cell.");
            }
            this.Contains_t_field_name = false;
            byte* targetPtr = m_ptr;
            MODULE_CALL("PushPointerToCurrentField", "$t_field");
            byte* startPtr = targetPtr;
            MODULE_CALL("PushPointerThroughFieldType", "$t_field_type");
            this.ResizeFunction(startPtr, 0, (int)(startPtr - targetPtr));
        }
        [END]

        ///<summary>
        ///Provides in-place access to the object field t_field_name.
        ///</summary>
        public unsafe t_accessor_type t_field_name
        {
            get
            {
                IF("%field_optional");
                if (!this.Contains_t_field_name)
                {
                    throw new Exception("Optional field t_field_name doesn't exist for current cell.");
                }
                END();
                byte* targetPtr = m_ptr;
                MODULE_CALL("PushPointerToCurrentField", "$t_field");

                IF("!%field_need_accessor");

                return *(t_data_type*)(targetPtr);

                ELIF("%field_lenprefix");

                t_field_name_Accessor_Field.m_ptr = targetPtr + 4;
                t_field_name_Accessor_Field.m_cellId = this.m_cellId;
                return t_field_name_Accessor_Field;

                ELSE();//accessor, no length prefix

                t_field_name_Accessor_Field.m_ptr = targetPtr;
                t_field_name_Accessor_Field.m_cellId = this.m_cellId;
                return t_field_name_Accessor_Field;

                END();
            }
            set
            {
                IF("%field_need_accessor");
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                t_field_name_Accessor_Field.m_cellId = this.m_cellId;
                END();

                byte* targetPtr = m_ptr;
                MODULE_CALL("PushPointerToCurrentField", "$t_field");

                IF("%field_optional");

                bool creatingOptionalField = (!this.Contains_t_field_name);
                if (creatingOptionalField)
                {
                    this.Contains_t_field_name = true;
                    MODULE_CALL("AccessorToAccessorFieldAssignment", "$t_field_type", "%accessor_field_name", "\"FieldDoesNotExist\"");
                }
                else
                {
                    MODULE_CALL("AccessorToAccessorFieldAssignment", "$t_field_type", "%accessor_field_name", "\"FieldExists\"");
                }

                ELSE();

                MODULE_CALL("AccessorToAccessorFieldAssignment", "$t_field_type", "%accessor_field_name", "\"FieldExists\"");

                END();
            }
        }


        [END]//FOREACH


        [MODULE_END]

        public unsafe byte* m_ptr { get; private set; }
        private long m_cellId;
        private unsafe byte* ResizeFunction(byte* targetPtr, int v1, int v2)
        {
            throw new NotImplementedException();
        }

    }
}

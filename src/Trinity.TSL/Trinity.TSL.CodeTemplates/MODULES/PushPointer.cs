using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Trinity.TSL
{
    partial class PushPointerModule : __meta
    {
        public static unsafe void PushPointer(byte* t_ptr)
        {
            //prepare context here; content before MODULE_BEGIN are invisible to the final source.
#pragma warning disable
            t_field_type t_field_names   = null;
#pragma warning enable
            //MAP_VAR can also map to a c++ routine
            int t_fixed_var_size          = 0;
            int t_fixed_list_element_size = 0;
            //Module begins here
            MODULE_BEGIN();
            MAP_VAR("t_ptr", "context->m_arguments[0]");
            MAP_LIST("t_field", "node->fieldList");
            MAP_VAR("t_field_name", "name");
            MAP_VAR("t_field_type", "fieldType");
            MAP_VAR("t_fixed_var_size", "0");//TODO map to fixed size var calculation
            MAP_VAR("t_fixed_list_element_size", "0");
            MAP_VAR("t_push_iter", "std::string(\"push_iterator_\") + GetString(context->m_stack_depth)");
            MAP_VAR("t_push_size", "std::string(\"push_size_\") + GetString(context->m_stack_depth)");
            TARGET("NStructBase");

            FOREACH();
            USE_LIST("t_field");
            {
                IF("$t_field_type->layoutType == LT_FIXED");
                {
                    t_ptr += t_fixed_var_size;
                }
                ELIF("$t_field_type->fieldType == FT_LIST");
                {

                    IF("$t_field_type->listElementType->layoutType == LT_FIXED");
                    {
                        t_ptr += sizeof(int) + t_fixed_list_element_size * (*(int*)t_ptr);
                    }
                    ELIF("$t_field_type->listElementType->fieldType == FT_REFERENCE");
                    {
                        for (int t_push_iter = 0, t_push_size = *(int*)t_ptr; t_push_iter < t_push_size; ++t_push_iter)
                        {
                            MODULE_CALL("PushPointer", "tsl->find_struct($t_field_type->referencedTypeName)", "$t_ptr");
                        }
                    }
                    END();
                }
                ELIF("$t_field_type->fieldType == FT_REFERENCE");
                {
                    MODULE_CALL("PushPointer", "tsl->find_struct($t_field_type->referencedTypeName)", "$t_ptr");
                }
                END();
            }
            END();
            MODULE_END();
            //Module ends here

            //Note that, a module does not necessarily
            //reside in the function body!
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL;

namespace t_Namespace
{
    class EnumerateFromFieldModule : __meta
    {
        private IEnumerable<T> _enumerate_from_t_field_name<T>()
        {
            MODULE_BEGIN();
            TARGET("NField");
            META_VAR("bool", "for_accessor", "context->m_arguments[0] == \"true\"");
            MAP_VAR("t_field", "node");
            MAP_VAR("t_field_type", "node->fieldType");
            MAP_VAR("t_field_name", "node->name");
            MAP_VAR("t_field_type_display", "Trinity::Codegen::GetDataTypeDisplayString(node->fieldType)");
            MAP_LIST("t_data_type", "Trinity::Codegen::TSLExternalParserDataTypeVector");
            MAP_VAR("t_data_type", "");
            MAP_VAR("t_uint", "GET_ITERATOR_VALUE()");
            IF("$t_field->is_optional()");
            IF("%for_accessor");
            if (this.Contains_t_field_name)
                ELSE();
            if (this.t_field_name != null)
                END();//for_accessor
            END();//if optional
            switch (TypeConverter<T>.type_id)
            {
                /*FOREACH*/
                /*USE_LIST("t_data_type")*/
                /*META_VAR("int", "enum_depth", "$t_field_type->enumerate_depth($t_data_type)")*/
                /*IF("%enum_depth >= 0")*/
                case t_uint:
                    {
                        IF("%enum_depth == 0");
                        yield return TypeConverter<T>.ConvertFrom_t_field_type_display(this.t_field_name);
                        ELSE();//otherwise, we flatten t_field_type into t_data_type(s)
                        {
                            META_VAR("int", "enum_iter", "0");
                            META_VAR("NFieldType*", "enum_type", "$t_field_type");
                            var element0 = this.t_field_name;
                            META("for(%enum_iter = 1; %enum_iter <= %enum_depth; ++%enum_iter){");
                            META("%enum_type = %enum_type->get_container_element_type();");
                            foreach (var element/*META_OUTPUT("%enum_iter")*/ in /*MUTE*/this.t_field_name/*MUTE_END*/ /*LITERAL_OUTPUT("element")*/ /*META_OUTPUT("%enum_iter - 1")*/)
                            /*META("}")*/
                            //for
                            {
                                LITERAL_OUTPUT("yield return TypeConverter<T>.ConvertFrom_");
                                META_OUTPUT("Trinity::Codegen::GetDataTypeDisplayString(%enum_type)");
                                LITERAL_OUTPUT("(element");
                                META_OUTPUT("%enum_depth");
                                LITERAL_OUTPUT(");");
                            }
                        }
                        END();
                    }
                    break;
                /*END*/
                /*END*/
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            MODULE_END();
        }

        public string t_field_name { get; set; }

        public bool Contains_t_field_name { get; set; }
    }
}

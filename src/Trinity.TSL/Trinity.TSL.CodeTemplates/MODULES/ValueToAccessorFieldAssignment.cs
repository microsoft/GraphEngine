using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.TSL
{
    class ValueToAccessorFieldAssignment : __meta
    {
        internal class __Accessor
        {

            public unsafe t_field_type t_field_name { get; set; }

            internal void Remove_t_field_name()
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// ValueToAccessorFieldAssignment:
        /// Assign from t_source_name into t_accessor_name.
        /// t_source_name is a value/reference, not an accessor, so it will
        /// be implicitly converted to accessor first. This module is used
        /// internally to support the icell interfaces.
        /// Arguments:
        /// 0. Accessor name
        /// 1. Assignment source name.
        /// </summary>
        internal static void Assign(__Accessor t_accessor_name, t_field_type t_source_name)
        {

            MODULE_BEGIN();
            TARGET("NField");
            MAP_VAR("t_field_type", "node->fieldType");
            MAP_VAR("t_field_name", "node->name");
            MAP_VAR("t_accessor_name", "context->m_arguments[0]");
            MAP_VAR("t_source_name", "context->m_arguments[1]");

            IF("$t_field_type->is_nullable()");
            {
                if (t_source_name.HasValue)
                    t_accessor_name.t_field_name = t_source_name.Value;
                else
                    t_accessor_name.Remove_t_field_name();
            }
            ELIF("$t_field_type->is_optional()");
            {
                if (t_source_name != default(t_field_type))
                    t_accessor_name.t_field_name = t_source_name;
                else
                    t_accessor_name.Remove_t_field_name();
            }
            ELSE();
            {
                t_accessor_name.t_field_name = t_source_name;
            }
            END();


            MODULE_END();
        }
    }
}

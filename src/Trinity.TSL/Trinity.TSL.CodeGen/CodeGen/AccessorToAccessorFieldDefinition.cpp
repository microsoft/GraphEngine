#include "common.h"
#include "AccessorType.h"
#include <string>
#include <SyntaxNode.h>

using std::string;
using namespace Trinity::Codegen;

namespace Trinity
{
    namespace Codegen
    {
        namespace Modules
        {
            /**
             * Generates accessor to accessor-field assignment code.
             * Arguments:
             * 0. accessor field name
             * 1. "FieldDoesNotExist" or "FieldExists" -> determines whether
             *    we create an optional field.
             */
            string* AccessorToAccessorFieldAssignment(NFieldType* type, ModuleContext* context)
            {
                string* source = new string();
                string  accessor_name = context->m_arguments[0];
                bool    create_optional = (context->m_arguments[1] == "FieldDoesNotExist");
                bool    need_accessor = type->is_optional();

                return source;
            }
        }
    }
}
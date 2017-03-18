#include "common.h"
#include "Trinity.TSL.CodeGen.h"
#include "AccessorType.h"
#include <string>
#include "SyntaxNode.h"

namespace Trinity
{
    namespace Codegen
    {
        namespace Modules
        {
            std::string* StructAccessorReverseImplicitOperator(NStructBase* node, ModuleContext* ctx)
            {
                NFieldType* type = new NFieldType();
                type->fieldType = FT_STRUCT;
                type->referencedNStruct = node;
                type->referencedTypeName = node->name;
                type->layoutType = node->layoutType;

                std::string* ret = AccessorReverseImplicitOperator(type, ctx);

                delete type;

                return ret;
            }
        }
    }
}
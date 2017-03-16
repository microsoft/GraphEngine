#include "common.h"
#include <string>
#include <SyntaxNode.h>

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        namespace Modules
        {
            string* 
StructAccessorEqualOperator(
NStructBase* node, ModuleContext* context)
            {
                string* source = new string();
                
source->append(R"::(
        public static bool operator ==()::");
source->append(Codegen::GetString(node->name));
source->append(R"::(_Accessor a, )::");
source->append(Codegen::GetString(node->name));
source->append(R"::(_Accessor b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;
            if (a.CellPtr == b.CellPtr) return true;
            byte* targetPtr = a.CellPtr;
            int lengthA = (int)(targetPtr - a.CellPtr);
            targetPtr = b.CellPtr;
            int lengthB = (int)(targetPtr - b.CellPtr);
            if(lengthA != lengthB) return false;
            return Memory.Compare(a.CellPtr,b.CellPtr,lengthA);
        }
        public static bool operator != ()::");
source->append(Codegen::GetString(node->name));
source->append(R"::(_Accessor a, )::");
source->append(Codegen::GetString(node->name));
source->append(R"::(_Accessor b)
        {
            return !(a == b);
        }
        )::");

                return source;
            }
        }
    }
}

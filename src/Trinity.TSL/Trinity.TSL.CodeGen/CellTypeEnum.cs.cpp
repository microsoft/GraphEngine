#include "common.h"
#include <string>
#include "SyntaxNode.h"

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
CellTypeEnum(
NTSL* node)
        {
            string* source = new string();
            
source->append(R"::(using Trinity.TSL;
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(
{
    public enum CellTypeEnum : ushort
    {
        Undefined = 0,
        )::");
for (size_t iterator_1 = 0; iterator_1 < (node->cellList)->size();++iterator_1)
{
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::( = )::");
source->append(Codegen::GetString(iterator_1 + 1));
source->append(R"::(,
        )::");
}
source->append(R"::(
    }
}
)::");

            return source;
        }
    }
}

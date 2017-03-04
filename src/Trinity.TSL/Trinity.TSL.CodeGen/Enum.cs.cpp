#include "common.h"
#include <string>
#include <SyntaxNode.h>

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
Enum(
NTSL* node)
        {
            string* source = new string();
            
source->append(R"::(using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.TSL;
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(
{
    )::");
for (size_t iterator_1 = 0; iterator_1 < (node->enumList)->size();++iterator_1)
{
source->append(R"::(
    /// <summary>
    /// Represents the enum type )::");
source->append(Codegen::GetString((*(node->enumList))[iterator_1]->name));
source->append(R"::( defined in the TSL.
    /// </summary>
    public enum )::");
source->append(Codegen::GetString((*(node->enumList))[iterator_1]->name));
source->append(R"::( : byte
    {
        )::");
for (size_t iterator_2 = 0; iterator_2 < ((*(node->enumList))[iterator_1]->enumEntryList)->size();++iterator_2)
{
source->append(Codegen::GetString((*((*(node->enumList))[iterator_1]->enumEntryList))[iterator_2]->name));
source->append(R"::( = )::");
source->append(Codegen::GetString((*((*(node->enumList))[iterator_1]->enumEntryList))[iterator_2]->value));
if (iterator_2 < ((*(node->enumList))[iterator_1]->enumEntryList)->size() - 1)
source->append(",");
}
source->append(R"::(
    }
    )::");
}
source->append(R"::(
}
)::");

            return source;
        }
    }
}

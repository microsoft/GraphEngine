#include "common.h"
#include <string>
#include "SyntaxNode.h"

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
CommunicationModule(
NTSL* node)
        {
            string* source = new string();
            
source->append(R"::(using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL;
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(
{
    )::");
for (size_t iterator_1 = 0; iterator_1 < (node->moduleList)->size();++iterator_1)
{
source->append(R"::(
    public abstract partial class t_module_name : Trinity.Network.CommunicationModule
    {
        protected override void RegisterMessageHandler()
        {
        }
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

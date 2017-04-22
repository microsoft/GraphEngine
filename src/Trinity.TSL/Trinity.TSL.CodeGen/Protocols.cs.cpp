#include "common.h"
#include <string>
#include "SyntaxNode.h"

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
Protocols(
NTSL* node)
        {
            string* source = new string();
            
source->append(R"::(using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL;
using Trinity.Core.Lib;
using Trinity.Network;
using Trinity.Network.Messaging;
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(
{
    )::");
for (size_t iterator_1 = 0; iterator_1 < (node->serverList)->size();++iterator_1)
{

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = 0;
std::string* module_content = Modules::CommunicationClass((*(node->serverList))[iterator_1], &module_ctx);
    source->append(*module_content);
    delete module_content;
}
}
for (size_t iterator_1 = 0; iterator_1 < (node->proxyList)->size();++iterator_1)
{

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = 0;
std::string* module_content = Modules::CommunicationClass((*(node->proxyList))[iterator_1], &module_ctx);
    source->append(*module_content);
    delete module_content;
}
}
for (size_t iterator_1 = 0; iterator_1 < (node->moduleList)->size();++iterator_1)
{

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = 0;
std::string* module_content = Modules::CommunicationClass((*(node->moduleList))[iterator_1], &module_ctx);
    source->append(*module_content);
    delete module_content;
}
}
source->append(R"::(
    public static class MessagePassingExtension
    {
        #region Server
        )::");
for (size_t iterator_1 = 0; iterator_1 < (node->serverList)->size();++iterator_1)
{

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = 0;
std::string* module_content = Modules::MessagePassingMethods((*(node->serverList))[iterator_1], &module_ctx);
    source->append(*module_content);
    delete module_content;
}
}
source->append(R"::(
        #endregion
        #region Proxy
        )::");
for (size_t iterator_1 = 0; iterator_1 < (node->proxyList)->size();++iterator_1)
{

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = 0;
std::string* module_content = Modules::MessagePassingMethods((*(node->proxyList))[iterator_1], &module_ctx);
    source->append(*module_content);
    delete module_content;
}
}
source->append(R"::(
        #endregion
        
    }
    #region Module
    )::");
for (size_t iterator_1 = 0; iterator_1 < (node->moduleList)->size();++iterator_1)
{
source->append(R"::(
    public abstract partial class )::");
source->append(Codegen::GetString((*(node->moduleList))[iterator_1]->name));
source->append(R"::(Base : CommunicationModule
    {
        )::");

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = 0;
std::string* module_content = Modules::MessagePassingMethods((*(node->moduleList))[iterator_1], &module_ctx);
    source->append(*module_content);
    delete module_content;
}
source->append(R"::(
    }
    )::");
}
source->append(R"::(
    #endregion
    
}
)::");

            return source;
        }
    }
}

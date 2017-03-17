#include "common.h"
#include <string>
#include "SyntaxNode.h"

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
MessagePassingExtension(
NTSL* node)
        {
            string* source = new string();
            
source->append(R"::(
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity;
using Trinity.Network;
using Trinity.Network.Http;
using Trinity.TSL;
using Trinity.TSL.Lib;
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(
{
    public static class MessagePassingExtension
    {
        #region Server
        )::");
for (size_t iterator_1 = 0; iterator_1 < (node->serverList)->size();++iterator_1)
{

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = 0;
module_ctx.m_arguments.push_back(Codegen::GetString("TrinityServer"));
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
module_ctx.m_arguments.push_back(Codegen::GetString("TrinityProxy"));
std::string* module_content = Modules::MessagePassingMethods((*(node->proxyList))[iterator_1], &module_ctx);
    source->append(*module_content);
    delete module_content;
}
}
source->append(R"::(
        #endregion
        #region mute
        
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
module_ctx.m_arguments.push_back(Codegen::GetString("CommunicationModule"));
std::string* module_content = Modules::MessagePassingMethods((*(node->moduleList))[iterator_1], &module_ctx);
    source->append(*module_content);
    delete module_content;
}
source->append(R"::(
        #region mute
        
        #endregion
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

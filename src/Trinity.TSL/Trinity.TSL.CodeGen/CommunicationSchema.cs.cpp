#include "common.h"
#include <string>
#include "SyntaxNode.h"

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
CommunicationSchema(
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
using Trinity.Network.Messaging;
using Trinity.TSL;
using Trinity.TSL.Lib;
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(
{
    
    internal class ProtocolDescriptor : IProtocolDescriptor
    {
        public string Name
        {
            get;
            set;
        }
        public string RequestSignature
        {
            get;
            set;
        }
        public string ResponseSignature
        {
            get;
            set;
        }
        public TrinityMessageType Type
        {
            get;
            set;
        }
    }
    #region Server
    )::");
for (size_t iterator_1 = 0; iterator_1 < (node->serverList)->size();++iterator_1)
{

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = 0;
module_ctx.m_arguments.push_back(Codegen::GetString("TrinityServer"));
std::string* module_content = Modules::CommunicationSchemaModule((*(node->serverList))[iterator_1], &module_ctx);
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
std::string* module_content = Modules::CommunicationSchemaModule((*(node->proxyList))[iterator_1], &module_ctx);
    source->append(*module_content);
    delete module_content;
}
}
source->append(R"::(
    #endregion
    #region Module
    )::");
for (size_t iterator_1 = 0; iterator_1 < (node->moduleList)->size();++iterator_1)
{

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = 0;
module_ctx.m_arguments.push_back(Codegen::GetString("CommunicationModule"));
std::string* module_content = Modules::CommunicationSchemaModule((*(node->moduleList))[iterator_1], &module_ctx);
    source->append(*module_content);
    delete module_content;
}
}
source->append(R"::(
    #endregion
    
}
)::");

            return source;
        }
    }
}

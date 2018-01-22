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
using System.Threading;
using System.Threading.Tasks;
using Trinity;
using Trinity.TSL;
using Trinity.Core.Lib;
using Trinity.Network;
using Trinity.Network.Messaging;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using Trinity.Storage;
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
for (size_t iterator_1 = 0; iterator_1 < (node->serverList)->size();++iterator_1)
{
source->append(R"::(
    namespace )::");
source->append(Codegen::GetString((*(node->serverList))[iterator_1]->name));
source->append(R"::(
    {
        public static class MessagePassingExtension
        {
            )::");

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = 0;
std::string* module_content = Modules::MessagePassingMethods((*(node->serverList))[iterator_1], &module_ctx);
    source->append(*module_content);
    delete module_content;
}
source->append(R"::(
        }
    }
    )::");
}
for (size_t iterator_1 = 0; iterator_1 < (node->proxyList)->size();++iterator_1)
{
source->append(R"::(
    namespace )::");
source->append(Codegen::GetString((*(node->proxyList))[iterator_1]->name));
source->append(R"::(
    {
        public static class MessagePassingExtension
        {
            )::");

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = 0;
std::string* module_content = Modules::MessagePassingMethods((*(node->proxyList))[iterator_1], &module_ctx);
    source->append(*module_content);
    delete module_content;
}
source->append(R"::(
        }
    }
    )::");
}
for (size_t iterator_1 = 0; iterator_1 < (node->moduleList)->size();++iterator_1)
{
source->append(R"::(
    namespace )::");
source->append(Codegen::GetString((*(node->moduleList))[iterator_1]->name));
source->append(R"::(
    {
        public static class MessagePassingExtension
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
    }
    )::");
}
source->append(R"::(
    #region Legacy
    public static class LegacyMessagePassingExtension
    {
        )::");
for (size_t iterator_1 = 0; iterator_1 < (node->serverList)->size();++iterator_1)
{

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = 0;
std::string* module_content = Modules::LegacyMessagePassingMethods((*(node->serverList))[iterator_1], &module_ctx);
    source->append(*module_content);
    delete module_content;
}
}
for (size_t iterator_1 = 0; iterator_1 < (node->proxyList)->size();++iterator_1)
{

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = 0;
std::string* module_content = Modules::LegacyMessagePassingMethods((*(node->proxyList))[iterator_1], &module_ctx);
    source->append(*module_content);
    delete module_content;
}
}
source->append(R"::(
    }
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
std::string* module_content = Modules::LegacyMessagePassingMethods((*(node->moduleList))[iterator_1], &module_ctx);
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

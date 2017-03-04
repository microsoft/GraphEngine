using System.Text;
using System.Collections.Generic;

namespace Trinity.TSL.CodeTemplates
{
    internal partial class SourceFiles
    {
        internal static string 
HTTP(
NTSL node)
        {
            StringBuilder source = new StringBuilder();
            
source.Append(@"
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
namespace ");
source.Append(Codegen.GetString(Trinity::Codegen::GetNamespace()));
source.Append(@"
{
    #region Server
    ");
for (int iterator_1 = 0; iterator_1 < (node->serverList).Count;++iterator_1)
{

{
    ModuleContext module_ctx = new ModuleContext();
    module_ctx.m_stack_depth = 0;
module_ctx.m_arguments.Add(Codegen.GetString("TrinityServer"));
string module_content = Modules.HTTPModule((node->serverList)[iterator_1], module_ctx);
    source.Append(module_content);
}
}
source.Append(@"
    #endregion
    #region Proxy
    ");
for (int iterator_1 = 0; iterator_1 < (node->proxyList).Count;++iterator_1)
{

{
    ModuleContext module_ctx = new ModuleContext();
    module_ctx.m_stack_depth = 0;
module_ctx.m_arguments.Add(Codegen.GetString("TrinityProxy"));
string module_content = Modules.HTTPModule((node->proxyList)[iterator_1], module_ctx);
    source.Append(module_content);
}
}
source.Append(@"
    #endregion
    #region Module
    ");
for (int iterator_1 = 0; iterator_1 < (node->moduleList).Count;++iterator_1)
{

{
    ModuleContext module_ctx = new ModuleContext();
    module_ctx.m_stack_depth = 0;
module_ctx.m_arguments.Add(Codegen.GetString("CommunicationModule"));
string module_content = Modules.HTTPModule((node->moduleList)[iterator_1], module_ctx);
    source.Append(module_content);
}
}
source.Append(@"
    #endregion
    
}
");

            return source.ToString();
        }
    }
}

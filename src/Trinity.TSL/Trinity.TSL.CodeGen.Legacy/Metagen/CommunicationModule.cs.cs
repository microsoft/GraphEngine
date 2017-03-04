using System.Text;
using System.Collections.Generic;

namespace Trinity.TSL.CodeTemplates
{
    internal partial class SourceFiles
    {
        internal static string 
CommunicationModule(
NTSL node)
        {
            StringBuilder source = new StringBuilder();
            
source.Append(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL;
namespace ");
source.Append(Codegen.GetString(Trinity::Codegen::GetNamespace()));
source.Append(@"
{
    ");
for (int iterator_1 = 0; iterator_1 < (node->moduleList).Count;++iterator_1)
{
source.Append(@"
    public abstract partial class t_module_name : Trinity.Network.CommunicationModule
    {
        protected override void RegisterMessageHandler()
        {
        }
    }
    ");
}
source.Append(@"
}
");

            return source.ToString();
        }
    }
}

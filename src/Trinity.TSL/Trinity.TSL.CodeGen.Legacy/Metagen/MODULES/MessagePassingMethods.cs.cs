using System.Text;
using System.Collections.Generic;

namespace Trinity.TSL.CodeTemplates
{
    internal partial class Modules
    {
        internal static string 
MessagePassingMethods(
NProtocolGroup node, ModuleContext context)
        {
            StringBuilder source = new StringBuilder();
            
NProtocol* protocol_1;
std::string method_name_1;
for (int iterator_1 = 0; iterator_1 < (node.protocolList).Count;++iterator_1)
{
protocol_1 = tsl->find_protocol((node.protocolList)[iterator_1].name);
method_name_1 = (node.protocolList)[iterator_1].name + "To" + node->name
source.Append(@"
        public unsafe static t_return_type");
source.Append(Codegen.GetString(method_name_1));
source.Append(@"(this Trinity.Storage.MemoryCloud storage, int ");
source.Append(Codegen.GetString(node->name));
source.Append(@"Id)
        {
        }
        ");
}

            return source.ToString();
        }
    }
}

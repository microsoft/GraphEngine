using System.Text;
using System.Collections.Generic;

namespace Trinity.TSL.CodeTemplates
{
    internal partial class SourceFiles
    {
        internal static string 
Enum(
NTSL node)
        {
            StringBuilder source = new StringBuilder();
            
source.Append(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.TSL;
namespace ");
source.Append(Codegen.GetString(Trinity::Codegen::GetNamespace()));
source.Append(@"
{
    ");
for (int iterator_1 = 0; iterator_1 < (node->enumList).Count;++iterator_1)
{
source.Append(@"
    /// <summary>
    /// Represents the enum type ");
source.Append(Codegen.GetString((node->enumList)[iterator_1].name));
source.Append(@" defined in the TSL.
    /// </summary>
    public enum ");
source.Append(Codegen.GetString((node->enumList)[iterator_1].name));
source.Append(@" : byte
    {
        ");
for (int iterator_2 = 0; iterator_2 < ((node->enumList)[iterator_1].enumEntryList).Count;++iterator_2)
{
source.Append(Codegen.GetString(((node->enumList)[iterator_1].enumEntryList)[iterator_2].name));
source.Append(@" = ");
source.Append(Codegen.GetString(((node->enumList)[iterator_1].enumEntryList)[iterator_2].value));
if (iterator_2 < ((node->enumList)[iterator_1].enumEntryList).Count - 1)
source.Append(",");
}
source.Append(@"
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

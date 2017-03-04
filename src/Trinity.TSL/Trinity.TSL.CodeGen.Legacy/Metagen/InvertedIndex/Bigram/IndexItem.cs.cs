using System.Text;
using System.Collections.Generic;

namespace Trinity.TSL.CodeTemplates
{
    internal partial class SourceFiles
    {
        internal static string 
IndexItem(
NTSL node)
        {
            StringBuilder source = new StringBuilder();
            
source.Append(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL;
namespace ");
source.Append(Codegen.GetString(Trinity::Codegen::GetNamespace()));
source.Append(@".InvertedIndex
{
    [StructLayout(LayoutKind.Explicit, Size = 10)]
    struct IndexItem
    {
        [FieldOffset(0)]
        internal long CellId;
        [FieldOffset(8)]
        internal ushort Offset;
    }
}
");

            return source.ToString();
        }
    }
}

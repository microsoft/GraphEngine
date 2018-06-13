using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Trinity.TSL;
/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
namespace t_Namespace.InvertedIndex
{
    [TARGET("NTSL")]
    [StructLayout(LayoutKind.Explicit, Size = 10)]
    struct IndexItem
    {
        [FieldOffset(0)]
        internal long m_cellId;
        [FieldOffset(8)]
        internal ushort Offset;
    }
}

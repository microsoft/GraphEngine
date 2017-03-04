#include "common.h"
#include <string>
#include <SyntaxNode.h>

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
IndexItem(
NTSL* node)
        {
            string* source = new string();
            
source->append(R"::(using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL;
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.InvertedIndex
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
)::");

            return source;
        }
    }
}

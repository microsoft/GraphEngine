using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.TSL
{
    internal class InterfaceDescriptor : AbstractStruct
    {
        internal InterfaceDescriptor(Lexer l)
        {
        }

        internal static InterfaceDescriptor ReadInterface(Lexer l)
        {
            return new InterfaceDescriptor(l);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.TSL
{
    internal class EnumDescriptor : AbstractStruct
    {
        internal List<string> Items;
        internal EnumDescriptor(WrappedSyntaxNode node)
        {
            Items = new List<string>();
            Name = node.name;
            foreach(var entry in node.children)
            {
                if (entry.name == null)
                    CompilerError.Throw("Internal error: 301");
                if(!entry.data.ContainsKey("value"))
                    CompilerError.Throw("Internal error: 302");
                Items.Add(entry.name + " = " + entry.data["value"]);
            }
        }
    }
}

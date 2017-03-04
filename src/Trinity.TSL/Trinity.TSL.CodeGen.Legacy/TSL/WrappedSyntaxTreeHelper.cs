using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.TSL
{
    class WrappedSyntaxTreeHelper
    {
        //TODO check if possible null desc
        public static void ReadAttributes(AttributeDescriptor desc, WrappedSyntaxNode node)
        {
            //kvpair children are attributes.
            foreach (var child in node.children)
                if (child.type == "NKVPair")
                {
                    desc.AttributeValuePairs[child.data["key"]] = child.data["value"];
                }
        }
    }
}

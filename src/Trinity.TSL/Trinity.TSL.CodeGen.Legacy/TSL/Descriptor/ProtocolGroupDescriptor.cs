using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.TSL
{
    class ProtocolGroupDescriptor : AbstractStruct
    {
        public List<ProtocolDescriptor> ProtocolDescriptors = new List<ProtocolDescriptor>();
        public ProtocolGroupType Type;

        public ProtocolGroupDescriptor(WrappedSyntaxNode node, SpecificationScript script, ProtocolGroupType type)
        {
            Name = node.name;
            Type = type;
            foreach (var protocol in node.children)
            {

                var desc = script.FindProtocolDescriptor(protocol.name);
                if (desc != null)
                    ProtocolDescriptors.Add(desc);
            }
        }
    }
}

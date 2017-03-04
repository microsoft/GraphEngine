using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.TSL;
using Trinity.Network.Messaging;

namespace Trinity.TSL
{

    internal class MessageAccessorCollection
    {
        internal HashSet<StructDescriptor> MessageAccessorTuples     = new HashSet<StructDescriptor>();
        internal HashSet<StructDescriptor> CellMessageAccessorTuples = new HashSet<StructDescriptor>();

        internal void TryAddAccessor(StructDescriptor desc, SpecificationScript script)
        {
            if (desc == StructDescriptor.VOID)
                return;

            if (script.FindStructDescriptor(desc.Name) != null)
                MessageAccessorTuples.Add(desc);
            else
                CellMessageAccessorTuples.Add(desc);
        }

        public MessageAccessorCollection(SpecificationScript script)
        {
            foreach (var proto_desc in script.ProtocolDescriptors)
            {
                TryAddAccessor(proto_desc.Request, script);
                TryAddAccessor(proto_desc.Response, script);
            }
        }
    }
}

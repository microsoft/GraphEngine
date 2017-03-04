using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.TSL
{
    public class AttributeDescriptor
    {
        internal Dictionary<string, string> AttributeValuePairs;

        internal AttributeDescriptor()
        {
            AttributeValuePairs = new Dictionary<string, string>();
        }

        internal bool Contains(string attribute)
        {
            return AttributeValuePairs.ContainsKey(attribute.Trim());
        }

        internal bool TryGetValue(string attribute, out string value)
        {
            return AttributeValuePairs.TryGetValue(attribute, out value);
        }

        internal static bool TryGetValue(AttributeDescriptor desc, string attribute, out string value)
        {
            if (desc != null)
            {
                return desc.TryGetValue(attribute, out value);
            }
            value = null;
            return false;
        }

    }
}

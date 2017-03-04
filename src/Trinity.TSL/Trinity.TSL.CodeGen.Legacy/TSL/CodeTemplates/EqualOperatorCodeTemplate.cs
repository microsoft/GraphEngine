using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.TSL.CodeTemplates
{
    class EqualOperatorCodeTemplate
    {
        internal static string GenerateMessageEqualOperatorCode(StructDescriptor formattype, bool isReader)
        {
            string accessorName = formattype.Name + ((isReader) ? ("Reader") : ("Writer"));
            string ret = @"
        public static bool operator == (" + accessorName + @" a, " + accessorName + @" b)
        {
            if (ReferenceEquals(a, b))
              return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
              return false;
            // If both are same instance, return true.
            if (a.CellPtr == b.CellPtr) return true;
            byte* targetPtr = a.CellPtr;
";
            foreach (var field in formattype.Fields) ret += field.Type.GeneratePushPointerCode();
            ret += @"
            int lengthA = (int)(targetPtr - a.CellPtr);
            targetPtr = b.CellPtr;
";
            foreach (var field in formattype.Fields) ret += field.Type.GeneratePushPointerCode();
            ret += @"
            int lengthB = (int)(targetPtr - b.CellPtr);
            if(lengthA != lengthB) return false;
            return Memory.Compare(a.CellPtr,b.CellPtr,lengthA);
        }

        public static bool operator != (" + accessorName + @" a, " + accessorName + @" b)
        {
            return !(a == b);
        }
";
            return ret;
        }
        internal static string GenerateFormatEqualOperatorCode(StructDescriptor formattype, bool isReadOnly)
        {
            string accessorName = formattype.Name + ((isReadOnly) ? ("_Accessor_ReadOnly") : ("_Accessor"));
            string ret = @"

        public static bool operator == (" + accessorName + @" a, " + accessorName + @" b)
        {
            if (ReferenceEquals(a, b))
              return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
              return false;
            // If both are same instance, return true.
            if (a.CellPtr == b.CellPtr) return true;
            byte* targetPtr = a.CellPtr;
";
            ret += AccessorCodeTemplate.GenerateFieldPushPointerCode(formattype, formattype.Fields.Count, "a");
            ret += @"
            int lengthA = (int)(targetPtr - a.CellPtr);
            targetPtr = b.CellPtr;
";
            ret += AccessorCodeTemplate.GenerateFieldPushPointerCode(formattype, formattype.Fields.Count, "b");
            ret += @"
            int lengthB = (int)(targetPtr - b.CellPtr);
            if(lengthA != lengthB) return false;
            return Memory.Compare(a.CellPtr,b.CellPtr,lengthA);
        }

        public static bool operator != (" + accessorName + @" a, " + accessorName + @" b)
        {
            return !(a == b);
        }
";
            return ret;
        }
    }
}

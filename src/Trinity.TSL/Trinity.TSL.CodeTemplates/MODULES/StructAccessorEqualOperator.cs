using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.TSL;

namespace t_Namespace
{
    public unsafe partial class t_struct_name_Accessor : __meta
    {
        [MODULE_BEGIN]
        [TARGET("NStructBase")]
        [MAP_VAR("t_struct_name", "node->name")]

        public static bool operator ==(t_struct_name_Accessor a, t_struct_name_Accessor b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;
            // If both are same instance, return true.
            if (a.CellPtr == b.CellPtr) return true;
            byte* targetPtr = a.CellPtr;
            MODULE_CALL("PushPointerThroughStruct", "node");
            int lengthA = (int)(targetPtr - a.CellPtr);
            targetPtr = b.CellPtr;
            MODULE_CALL("PushPointerThroughStruct", "node");
            int lengthB = (int)(targetPtr - b.CellPtr);
            if(lengthA != lengthB) return false;
            return Memory.Compare(a.CellPtr,b.CellPtr,lengthA);
        }

        public static bool operator != (t_struct_name_Accessor a, t_struct_name_Accessor b)
        {
            return !(a == b);
        }

        [MODULE_END]

        void foo() { }
    }
}

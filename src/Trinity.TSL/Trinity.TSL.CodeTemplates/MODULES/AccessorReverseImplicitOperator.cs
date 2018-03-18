using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using t_Namespace;
using Trinity.Core.Lib;
using Trinity.TSL;

namespace Trinity.TSL
{
    public partial struct t_accessor_type
    {
        [MODULE_BEGIN]
        [TARGET("NFieldType")]
        [MAP_VAR("t_accessor_type", "data_type_get_accessor_name(node)")]
        [MAP_VAR("t_data_type", "GetNonNullableValueTypeString(node)")]
        [META_VAR("bool", "for_cell", "(node->is_struct() && !node->referencedNStruct->is_struct())")]
        [META_VAR("bool", "has_resize", "(!%for_cell && node->layoutType != LT_FIXED)")]
        public unsafe static implicit operator t_accessor_type(t_data_type field)
        {
            byte* targetPtr = null;
            __meta.MODULE_CALL("PushPointerFromVariable", "node", "\"field\"", "\"push\"");
            byte* tmpcellptr = BufferAllocator.AllocBuffer((int)targetPtr);
            Memory.memset(tmpcellptr, 0, (ulong)targetPtr);
            targetPtr = tmpcellptr;
            __meta.MODULE_CALL("PushPointerFromVariable", "node", "\"field\"", "\"assign\"");


            t_accessor_type ret;

            __meta.IF("%has_resize");
            ret = new t_accessor_type(tmpcellptr, null);
            __meta.ELSE();
            ret = new t_accessor_type(tmpcellptr);
            __meta.END();

            __meta.IF("%for_cell");
            ret.CellId = field.CellId;
            __meta.END();

            return ret;
        }
        [MODULE_END]

        public unsafe t_accessor_type(byte* cellPtr, object p) : this(cellPtr)
        {
        }
    }
}

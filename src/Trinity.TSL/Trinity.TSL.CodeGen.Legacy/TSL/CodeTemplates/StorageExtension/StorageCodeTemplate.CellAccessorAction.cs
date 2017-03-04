using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.Utilities;

namespace Trinity.TSL
{
    partial class StorageCodeTemplate
    {
        private static string GenerateCellAction(SpecificationScript script)
        {
            CodeWriter ret = new CodeWriter();
            ret += @"
    /// <summary>
    /// Represents an action performed in-place on a cell.
    /// </summary>
    public class CellAccessorAction
    {";
            foreach (var desc in script.CellDescriptors)
            {
                ret += @"
        /// <summary>
        /// Initializes a CellAccessorAction with the given action.
        /// </summary>
        /// <param name=""action"">The action to be performed on the cells of type " + desc.Name + @".
        public unsafe CellAccessorAction(Action<" + desc.Name + @"_Accessor> action)
        {
            CellType = CellType." + desc.Name + @";
            _action = (cellID, long_ptr, CellEntryIndex) =>
                {
                    " + desc.Name + @"_Accessor accessor = new " + desc.Name + @"_Accessor((byte*)long_ptr);
                    accessor.CellID = cellID;
                    accessor.CellEntryIndex = CellEntryIndex;
                    action(accessor);
                };
        }";
            }
            ret += @"
        internal Action<long, long, int> _action;
        internal CellType CellType;
    }";
            return ret;
        }
    }
}
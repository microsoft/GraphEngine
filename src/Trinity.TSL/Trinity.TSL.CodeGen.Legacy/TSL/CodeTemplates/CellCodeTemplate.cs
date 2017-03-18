using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.Utilities;

namespace Trinity.TSL
{
    internal class CellCodeTemplate
    {
        /// <summary>
        /// Generate core code that serialize the parameters into byte[] tmpcell
        /// If generatePreserveHeaderCode is set to true, the prototype must include "int preservedHeaderLength"
        /// If forCell is set to true, additional code will be generated to satisfy special format for cells ( e.g. first byte as cell type )
        /// </summary>
        public static string GenerateParametersToByteArrayCode(StructDescriptor descriptor, bool generatePreserveHeaderCode = false, bool forCell = true, bool unmanagedBuf = false)
        {
            return "";
        }
    }
}

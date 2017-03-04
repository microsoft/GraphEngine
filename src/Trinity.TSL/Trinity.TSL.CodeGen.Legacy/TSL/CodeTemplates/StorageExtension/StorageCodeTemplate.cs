using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.Utilities;

namespace Trinity.TSL
{
    partial class StorageCodeTemplate
    {
        public static string GenerateStorageIntegrationCode(SpecificationScript script)
        {
            StringBuilder ret = new StringBuilder();

            //Cell load/save/use extensions
            foreach (var cell_desc in script.CellDescriptors)
            {
                ret.Append(GenerateStorageExtensionForCell(cell_desc, script));
            }

            ret.Append(GenerateCellTypeExtension(script));
            //ret.Append(GenerateForEachExtension());
            //ret.Append(GenerateCellAction(script));

            return ret.ToString();
        }
    }
}

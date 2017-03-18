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

            ret.Append(GenerateCellTypeExtension(script));
            //ret.Append(GenerateForEachExtension());
            //ret.Append(GenerateCellAction(script));

            return ret.ToString();
        }
    }
}

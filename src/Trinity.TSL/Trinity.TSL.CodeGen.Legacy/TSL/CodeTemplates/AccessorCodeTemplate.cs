using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Trinity.TSL.CodeTemplates;
using Trinity.Utilities;

namespace Trinity.TSL
{
    class AccessorCodeTemplate
    {
        public static string GenerateFieldPropertiesCode(StructDescriptor structDesc, bool isReadOnly)
        {
            return "";
        }

        internal static string GenerateOptionalFieldMap(StructDescriptor structDesc)
        {
            return "";
        }

        internal static string GenerateFieldPushPointerCode(StructDescriptor structDesc, int cnt, string cellName)
        {
            string indent = "            ";
            int pushnum = 0;
            CodeWriter ret = new CodeWriter();
            OptionalFieldCalculator opt = new OptionalFieldCalculator(structDesc.OptionalFieldSequenceMap.Count);
            #region Push pointer
            if (structDesc.OptionalFieldSequenceMap.Count != 0)//Contains optional fields.
            {
                ret += indent + "targetPtr += " + opt.headerLength + ";\r\n";
            }
            pushnum = 0;
            for (int i = 0; i < cnt; i++)//Push through fields before current one
            {
                bool pushFieldIsOptional = structDesc.OptionalFieldSequenceMap.ContainsKey(structDesc.Fields[i]);
                #region OptionalFieldHeader
                if (pushFieldIsOptional)
                {
                    if (structDesc.Fields[i].Type is DynamicFieldType)
                    {
                        if (pushnum != 0)
                            ret += indent + "targetPtr += " + pushnum.ToString(CultureInfo.InvariantCulture) + ";\r\n";
                        pushnum = 0;
                    }
                    ret += @"
                if(" + cellName + @".Contains_" + structDesc.Fields[i].Name + @")
                {
";
                }
                #endregion
                if (structDesc.Fields[i].Type is FixedFieldType)
                {
                    if (pushFieldIsOptional)//Do not include optional field in a batch fixed push
                        ret += indent + "targetPtr += " + (structDesc.Fields[i].Type as FixedFieldType).Length.ToString(CultureInfo.InvariantCulture) + ";\r\n";
                    else
                        pushnum += (structDesc.Fields[i].Type as FixedFieldType).Length;
                }
                else
                {
                    if (!pushFieldIsOptional)
                    {
                        if (pushnum != 0)
                            ret += indent + "targetPtr += " + pushnum.ToString(CultureInfo.InvariantCulture) + ";\r\n";
                        pushnum = 0;
                    }
                    ret += indent + (structDesc.Fields[i].Type as DynamicFieldType).GeneratePushPointerCode();
                }
                #region OptionalFieldFooter
                if (pushFieldIsOptional)
                {
                    ret += @"
                }
";
                }
                #endregion
            }
            if (pushnum != 0)
                ret += indent + "targetPtr += " + pushnum.ToString(CultureInfo.InvariantCulture) + ";\r\n";
            return ret;
            #endregion

        }
    }
}
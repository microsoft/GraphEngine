using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.TSL.CodeTemplates;
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
            CodeWriter ret = new CodeWriter();
            if (!@generatePreserveHeaderCode)
                ret += @"
        byte* targetPtr = null;
";
            else
                ret += @"
        byte* targetPtr = (byte*) preservedHeaderLength;";

            OptionalFieldCalculator opt = new OptionalFieldCalculator(descriptor.OptionalFieldSequenceMap.Count);
            string optHeaderVarname = TSLCompiler.GenerateRandomVariableName();
            bool containsOptionalFields = (descriptor.OptionalFieldSequenceMap.Count != 0);
            if (containsOptionalFields)
            {
                ret += @"
        targetPtr += " + opt.headerLength + @";
";
            }

            foreach (var field in descriptor.Fields)
            {
                bool currentFieldIsOptional = (descriptor.OptionalFieldSequenceMap.ContainsKey(field));
                string varName = field.Name;
                if (currentFieldIsOptional)
                {
                    ret += @"
        if( " + field.Name + @"!= null)
        {";
                    if (TSLCompiler.FieldNeedNullableWrapper(field.Type))
                        varName += ".Value";
                }
                ret += field.Type.GenerateAssignCodeForConstructor(varName, 0, true);
                if (currentFieldIsOptional)
                {
                    ret += @"
        }";
                }
            }

            if (unmanagedBuf)
            {
                ret += @"
        BufferLength     = (int)targetPtr;
        byte* tmpcellptr = (byte*)Memory.malloc((ulong)targetPtr);
        Memory.memset(tmpcellptr, 0, (ulong)targetPtr);
        {
            targetPtr = tmpcellptr;
";
            }
            else
            {
                ret += @"
        byte[] tmpcell = new byte[(int)(targetPtr)];
        fixed(byte* tmpcellptr = tmpcell)
        {
            targetPtr = tmpcellptr;
";
            }
            if (generatePreserveHeaderCode)
                ret += @"
            tmpcellptr += preservedHeaderLength;
            targetPtr  += preservedHeaderLength;
";

            if (containsOptionalFields)
            {
                ret += @"
        targetPtr += " + opt.headerLength + @";
";
            }

            foreach (var field in descriptor.Fields)
            {
                bool currentFieldIsOptional = (descriptor.OptionalFieldSequenceMap.ContainsKey(field));
                string varName = field.Name;
                if (currentFieldIsOptional)
                {
                    ret += @"
            if( " + field.Name + @"!= null)
            {";
                    if (TSLCompiler.FieldNeedNullableWrapper(field.Type))
                        varName += ".Value";
                }
                ret += field.Type.GenerateAssignCodeForConstructor(varName, 0, false);
                if (currentFieldIsOptional)
                {
                    ret += @"
                " + opt.GenerateMaskOnCode(descriptor.OptionalFieldSequenceMap[field], "tmpcellptr") + @"
            }";
                }
            }
            ret += @"
        }
";
            return ret;
        }
    }
}

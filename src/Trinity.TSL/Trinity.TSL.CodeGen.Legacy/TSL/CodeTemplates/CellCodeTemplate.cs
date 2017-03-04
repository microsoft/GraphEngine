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
        static string GenerateCellHeader(StructDescriptor cellDesc)
        {
            CodeWriter cw = new CodeWriter();
            cw += @"
    public unsafe partial class " + cellDesc.Name + @"_Accessor: IDisposable
    {
        internal " + cellDesc.Name + @"_Accessor(long cellId, byte[] buffer)
        {
            this.CellID  = cellId;
            handle       = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            this.CellPtr = (byte*)handle.AddrOfPinnedObject().ToPointer();";
            foreach (var field in cellDesc.Fields)
            {
                if (cellDesc.IsFixed())
                    cw += TSLCompiler.GenerateAccessorFieldAssignmentCodeForFixedField(field.Type, false, field.Name + "_Accessor_Field");
                else
                    cw += TSLCompiler.GenerateAccessorFieldAssignmentCode(new DynamicStructFieldType(cellDesc), field.Type, false, field.Name + "_Accessor_Field", true);
            }
            cw += @"
            this.CellEntryIndex = -1;
        }

"
                + AccessorCodeTemplate.GenerateOptionalFieldMap(cellDesc) +
                @"
        ///<summary>
        ///Copies the cell content into a byte array.
        ///</summary>
        public byte[] ToByteArray()
        {
            byte* targetPtr = CellPtr;
";
            cw += AccessorCodeTemplate.GenerateFieldPushPointerCode(cellDesc, cellDesc.Fields.Count, "this");
            cw += @"
            int size   = (int)(targetPtr - CellPtr);
            byte[] ret = new byte[size];
            Memory.Copy(CellPtr,0,ret,0,size);
            return ret;";
            cw += @"
        }

        internal unsafe " + cellDesc.Name + @"_Accessor(long CellID, CellAccessOptions options)
        {
            this.Initialize(CellID, options);
";
            foreach (var field in cellDesc.Fields)
            {
                if (cellDesc.IsFixed())
                    cw += TSLCompiler.GenerateAccessorFieldAssignmentCodeForFixedField(field.Type, false, field.Name + "_Accessor_Field");
                else
                    cw += TSLCompiler.GenerateAccessorFieldAssignmentCode(new DynamicStructFieldType(cellDesc), field.Type, false, field.Name + "_Accessor_Field", true);
            }
            cw += @"
            this.CellID = CellID;
        }

        internal unsafe " + cellDesc.Name + @"_Accessor(byte* _CellPtr)
        {
            CellPtr = _CellPtr;";
            foreach (var field in cellDesc.Fields)
            {
                if (cellDesc.IsFixed())
                    cw += TSLCompiler.GenerateAccessorFieldAssignmentCodeForFixedField(field.Type, false, field.Name + "_Accessor_Field");
                else
                    cw += TSLCompiler.GenerateAccessorFieldAssignmentCode(new DynamicStructFieldType(cellDesc), field.Type, false, field.Name + "_Accessor_Field", true);
            }
            cw += @"
            this.CellEntryIndex = -1;
        }";
            return cw;
        }

        static string GenerateCellConstructor(StructDescriptor cellDesc)
        {
            CodeWriter ret = new CodeWriter();
            ret += TSLCompiler.GenerateAssignmentPrototypeParameterList("internal static unsafe byte[] construct(long CellID", cellDesc);
            ret += GenerateParametersToByteArrayCode(
                cellDesc,
                generatePreserveHeaderCode: false,
                forCell: true);
            ret += @"
            return tmpcell;
        }
";
            return ret;
        }
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

        static public string GenerateCellCode(StructDescriptor cellDesc)
        {
            CodeWriter src = new CodeWriter();
            src += GenerateCellHeader(cellDesc);
            src += GenerateCellConstructor(cellDesc);
            src += AccessorCodeTemplate.GenerateFieldPropertiesCode(cellDesc, false);
            //Cell class has hard coded resize function, so storage no ResizeFunctionDelegate parameter passed in in the constructor
            src += ImplicitOperatorCodeTemplate.GenerateCellImplicitOperatorCode(cellDesc);
            src += EqualOperatorCodeTemplate.GenerateFormatEqualOperatorCode(cellDesc, false);
            src += "\t}\r\n";
            return src;
        }
    }
}

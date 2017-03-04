using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.Utilities;

namespace Trinity.TSL
{
    internal abstract class StructFieldType : FieldType
    {
        public StructDescriptor descriptor; //The referenced descriptor
        public string Name
        {
            get { return descriptor.Name; }
        }

        public string CSharpName
        {
            get { return descriptor.Name; }
        }

        public string GenerateAssignCodeForConstructor(string VarName, int currentLevel, bool OnlyPushPointer)
        {
            if (OnlyPushPointer && this is FixedStructFieldType)
            {
                return "targetPtr += " + (this as FixedStructFieldType).Length.ToString() + @";
";
            }

            CodeWriter ret = @"
        {";
            string pointer_name = "storedPtr_" + currentLevel.ToString();
            int optionalHeaderLength = -1;
            OptionalFieldCalculator opt = new OptionalFieldCalculator(descriptor.OptionalFieldSequenceMap.Count);

            if (descriptor.OptionalFieldSequenceMap.Count != 0)
            {
                optionalHeaderLength = opt.headerLength;
                if (!OnlyPushPointer)
                {
                    ret += "byte* " + pointer_name + " = targetPtr;\r\n";
                    ret += opt.GenerateClearAllBitsCode(pointer_name);
                }
                ret += "targetPtr += " + optionalHeaderLength + ";\r\n";
            }

            foreach (var field in descriptor.Fields)
            {
                bool currentFieldIsOptional = (descriptor.OptionalFieldSequenceMap.ContainsKey(field));
                if (currentFieldIsOptional)
                {
                    ret += @"
                if( " + VarName + "." + field.Name + @"!= null)
                {";
                }
                string new_varname = VarName + "." + field.Name;
                if (currentFieldIsOptional && TSLCompiler.FieldNeedNullableWrapper(field.Type))
                {
                    new_varname += ".Value";
                }
                ret += field.Type.GenerateAssignCodeForConstructor(new_varname, currentLevel + 1, OnlyPushPointer);
                if (currentFieldIsOptional)
                {
                    if (!OnlyPushPointer)
                        ret += opt.GenerateMaskOnCode(descriptor.OptionalFieldSequenceMap[field], pointer_name);
                    ret += @"
                }";
                }
            }

            ret += @"
        }";
            return ret;
        }
        public abstract string GeneratePushPointerCode();
    }
}

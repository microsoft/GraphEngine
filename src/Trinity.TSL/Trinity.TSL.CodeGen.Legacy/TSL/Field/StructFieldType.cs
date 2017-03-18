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
            return "";
        }
        public abstract string GeneratePushPointerCode();
    }
}

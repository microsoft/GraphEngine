using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.TSL
{
    internal class UnresolvedFieldType : FixedFieldType, DynamicFieldType
    {
        public UnresolvedFieldType(string Name)
        {
            UnresolvedFormatName = Name;
        }
        public string UnresolvedFormatName;
        public int Length
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public string GenerateAssignCodeForConstructor(string VarName, int currentLevel, bool OnlyPushPointer)
        {
            throw new NotImplementedException();
        }

        public string GeneratePushPointerCode()
        {
            throw new NotImplementedException();
        }

        public string CSharpName
        {
            get { throw new NotImplementedException(); }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.TSL
{
    internal interface FieldType
    {
        string Name { get; }

        /// <summary>
        /// In the constructor, we have all parameters passed in, so for each field, we can do an assignment like:
        /// *(int*)targetPtr = [VarName].SomeProperty;
        /// targetPtr += sizeof(int);
        /// 
        /// When OnlyPushPointer is true, do not write to the targetPtr, since its just a simulation to calculate cell length.
        /// currentLevel: How many "dots" are there in current VarName. This is actually the "depth" of the current field
        /// Some FieldTypes may require targetPtr to be stored inside a temporary pointer, or iterate through a container.
        /// We make following convention:
        ///     targetPtr stored on currentLevel becomes "storedPtr_currentLevel", e.g. "storedPtr_1"
        ///     when iterating on currentLevel, the iterator is "iterator_currentLevel", thus the inner variable name
        ///     won't conflict with outer variable names.
        /// </summary>
        string GenerateAssignCodeForConstructor(string VarName, int currentLevel, bool OnlyPushPointer);

        string GeneratePushPointerCode();

        string CSharpName { get; }
    }
}

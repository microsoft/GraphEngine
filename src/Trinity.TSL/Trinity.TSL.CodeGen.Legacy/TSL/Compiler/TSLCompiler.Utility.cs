using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.TSL
{
    public partial class TSLCompiler
    {
        internal static string GetAccessorTypeName(bool isReadOnly, FieldType fieldType)
        {
            string accessor_type_name = (fieldType is StructFieldType) ? (fieldType.Name + ((isReadOnly) ? ("_Accessor_ReadOnly") : ("_Accessor"))) : (fieldType.Name);
            return accessor_type_name;
        }

        internal static string GetDefaultString(FieldType type, bool nullable)
        {
            if (FieldNeedNullableWrapper(type) && !nullable)
                return "default(" + type.CSharpName + ")";
            else return "null";
        }

        internal static string GetNullableTypeName(Field field)
        {
            if (FieldNeedNullableWrapper(field.Type))
                return field.Type.CSharpName + "?";
            else
                return field.Type.CSharpName;
        }

        internal static bool FieldNeedNullableWrapper(FieldType type)
        {
            return (type is AtomType || type is StructFieldType || type is GuidType || type is DateTimeType || type is EnumType);
        }

        internal static bool IsValueType(FieldType type)
        {
            return type is AtomType || (type is StructFieldType && (type as StructFieldType).descriptor.ContainsOnlyValueTypeFields());
        }

        internal static string GenerateRandomVariableName()
        {
            string ret = "tmp_";
            for (int i = 0; i < 32; ++i)
            {
                ret += charset[tmpVarNameRandom.Next(charset.Length)];
            }
            return ret;
        }

    }
}

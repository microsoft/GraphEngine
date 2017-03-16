using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Trinity.TSL.CodeTemplates
{
    class ImplicitOperatorCodeTemplate
    {
        internal static string GenerateReverseImplicitOperaterCode(FieldType fieldtype, bool isReadOnly, bool hasResizeFunction, bool forCell = false)
        {
            return "";
        }
        internal static string GenerateListImplicitOperaterCode(DynamicFieldType listtype)
        {
            return "";
        }
        public static string GenerateCellImplicitOperatorCode(StructDescriptor cellDesc)
        {
            return "";
        }
        internal static string GenerateArrayImplicitOperatorCode(ArrayType arraytype)
        {
            return "";
        }
        internal static string ArraySetPropertiesCode(Field field, string accessor_field_name, bool createOptionalField)
        {
            return "";
        }
        internal static string GuidSetPropertiesCode(Field field, bool createOptionalField)
        {
            return "";
        }
        internal static string DateTimeSetPropertiesCode(Field field, bool createOptionalField)
        {
            return "";
        }
        internal static string EnumSetPropertiesCode(Field field, bool createOptionalField)
        {
            return "";
        }
        public static string ContainerSetPropertiesCode(FieldType type, string accessorName, bool CreateOptionalField)
        {
            return "";
        }
        internal static string FormatSetPropertiesCode(Field field, bool createOptionalField, string whoseReizeFunc = "this")
        {
            return "";
        }

    }
}

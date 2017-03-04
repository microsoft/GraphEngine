using System.Text;
using System.Collections.Generic;

namespace Trinity.TSL.CodeTemplates
{
    internal partial class SourceFiles
    {
        internal static string 
StorageSchema(
NTSL node)
        {
            StringBuilder source = new StringBuilder();
            
source.Append(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Storage;
using Trinity.TSL;
namespace ");
source.Append(Codegen.GetString(Trinity::Codegen::GetNamespace()));
source.Append(@"
{
    /// <summary>
    /// Exposes the data modeling schema defined in the TSL.
    /// </summary>
    public class StorageSchema : IStorageSchema
    {
        #region CellType lookup table
        internal static Dictionary<string, CellType> cellTypeLookupTable = new Dictionary<string, CellType>()
        {
            ");
for (int iterator_1 = 0; iterator_1 < (node->cellList).Count;++iterator_1)
{
source.Append(@"
            {""");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@""", global::");
source.Append(Codegen.GetString(Trinity::Codegen::GetNamespace()));
source.Append(@".CellType.");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"}
            ");
if (iterator_1 < (node->cellList).Count - 1)
source.Append(",");
}
source.Append(@"
        };
        #endregion
        ");
for (int iterator_1 = 0; iterator_1 < (node->cellList).Count;++iterator_1)
{
source.Append(@"
        internal static readonly Type   s_cellType_");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"       = typeof(global::");
source.Append(Codegen.GetString(Trinity::Codegen::GetNamespace()));
source.Append(@".");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@");
        internal static readonly string s_cellTypeName_");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"   = """);
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@""";
        internal class ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_descriptor : ICellDescriptor
        {
            private static IReadOnlyDictionary<string, string> s_attributes = new Dictionary<string, string>
            {
                ");
for (int iterator_2 = 0; iterator_2 < ((node->cellList)[iterator_1].attributes).Count;++iterator_2)
{
source.Append(@"
                { """);
source.Append(Codegen.GetString(((node->cellList)[iterator_1].attributes)[iterator_2].key));
source.Append(@""", """);
source.Append(Codegen.GetString(((node->cellList)[iterator_1].attributes)[iterator_2].value));
source.Append(@""" }
                ");
if (iterator_2 < ((node->cellList)[iterator_1].attributes).Count - 1)
source.Append(",");
}
source.Append(@"
            };
            internal static bool check_attribute(IAttributeCollection attributes, string attributeKey, string attributeValue)
            {
                if (attributeKey == null)
                    return true;
                if (attributeValue == null)
                    return attributes.Attributes.ContainsKey(attributeKey);
                else
                    return attributes.Attributes.ContainsKey(attributeKey) && attributes.Attributes[attributeKey] == attributeValue;
            }
            ");
for (int iterator_2 = 0; iterator_2 < ((node->cellList)[iterator_1].fieldList).Count;++iterator_2)
{
source.Append(@"
            internal class ");
source.Append(Codegen.GetString(((node->cellList)[iterator_1].fieldList)[iterator_2].name));
source.Append(@"_descriptor : IFieldDescriptor
            {
                private static IReadOnlyDictionary<string, string> s_attributes = new Dictionary<string, string>
                {
                    ");
for (int iterator_3 = 0; iterator_3 < (((node->cellList)[iterator_1].fieldList)[iterator_2].attributes).Count;++iterator_3)
{
source.Append(@"
                    { """);
source.Append(Codegen.GetString((((node->cellList)[iterator_1].fieldList)[iterator_2].attributes)[iterator_3].key));
source.Append(@""", """);
source.Append(Codegen.GetString((((node->cellList)[iterator_1].fieldList)[iterator_2].attributes)[iterator_3].value));
source.Append(@""" }
                    ");
if (iterator_3 < (((node->cellList)[iterator_1].fieldList)[iterator_2].attributes).Count - 1)
source.Append(",");
}
source.Append(@"
                };
                private static string s_typename = """);
source.Append(Codegen.GetString(((node->cellList)[iterator_1].fieldList)[iterator_2].fieldType));
source.Append(@""";
                private static Type   s_type     = typeof(");
source.Append(Codegen.GetString(((node->cellList)[iterator_1].fieldList)[iterator_2].fieldType));
source.Append(@");
                public string Name
                {
                    get { return """);
source.Append(Codegen.GetString(((node->cellList)[iterator_1].fieldList)[iterator_2].name));
source.Append(@"""; }
                }
                public bool Optional
                {
                    get
                    {
                        ");
if (((node->cellList)[iterator_1].fieldList)[iterator_2]->is_optional())
{
source.Append(@"
                        return true;
                        ");
}
else
{
source.Append(@"
                        return false;
                        ");
}
source.Append(@"
                    }
                }
                public bool IsOfType<T>()
                {
                    return typeof(T) == Type;
                }
                public bool IsList()
                {
                    ");
if (((node->cellList)[iterator_1].fieldList)[iterator_2].fieldType->is_list())
{
source.Append(@"
                    return true;
                    ");
}
else
{
source.Append(@"
                    return false;
                    ");
}
source.Append(@"
                }
                public string TypeName
                {
                    get { return s_typename; }
                }
                public Type Type
                {
                    get { return s_type; }
                }
                public IReadOnlyDictionary<string, string> Attributes
                {
                    get { return s_attributes; }
                }
                public string GetAttributeValue(string attributeKey)
                {
                    string ret = null;
                    s_attributes.TryGetValue(attributeKey, out ret);
                    return ret;
                }
            }
            internal static ");
source.Append(Codegen.GetString(((node->cellList)[iterator_1].fieldList)[iterator_2].name));
source.Append(@"_descriptor/*_*/");
source.Append(Codegen.GetString(((node->cellList)[iterator_1].fieldList)[iterator_2].name));
source.Append(@" = new ");
source.Append(Codegen.GetString(((node->cellList)[iterator_1].fieldList)[iterator_2].name));
source.Append(@"_descriptor();
            ");
}
source.Append(@"
            #region ICellDescriptor
            public IEnumerable<string> GetFieldNames()
            {
                ");
int field_cnt_1 = 0;
for (int iterator_2 = 0; iterator_2 < ((node->cellList)[iterator_1].fieldList).Count;++iterator_2)
{
source.Append(@"
                yield return """);
source.Append(Codegen.GetString(((node->cellList)[iterator_1].fieldList)[iterator_2].name));
source.Append(@""";
                ");
++field_cnt_1;
}
if (field_cnt_1 == 0)
{
source.Append(@"
                {
                    yield break;
                }
                ");
}
source.Append(@"
            }
            public IAttributeCollection GetFieldAttributes(string fieldName)
            {
                int field_id = global::");
source.Append(Codegen.GetString(Trinity::Codegen::GetNamespace()));
source.Append(@".");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@".FieldLookupTable.Lookup(fieldName);
                if (field_id == -1)
                    Throw.undefined_field();
                switch (field_id)
                {
                    ");
for (int iterator_2 = 0; iterator_2 < ((node->cellList)[iterator_1].fieldList).Count;++iterator_2)
{
source.Append(@"
                    case ");
source.Append(Codegen.GetString(iterator_2));
source.Append(@":
                        return ");
source.Append(Codegen.GetString(((node->cellList)[iterator_1].fieldList)[iterator_2].name));
source.Append(@";
                        break;
                    ");
}
source.Append(@"
                }
                /* Should not reach here */
                throw new Exception(""Internal error T6001"");
            }
            public IEnumerable<IFieldDescriptor> GetFieldDescriptors()
            {
                ");
int field_cnt_2 = 0;
for (int iterator_2 = 0; iterator_2 < ((node->cellList)[iterator_1].fieldList).Count;++iterator_2)
{
source.Append(@"
                yield return ");
source.Append(Codegen.GetString(((node->cellList)[iterator_1].fieldList)[iterator_2].name));
source.Append(@";
                ");
++field_cnt_2;
}
if (field_cnt_2 == 0)
{
source.Append(@"
                {
                    yield break;
                }
                ");
}
source.Append(@"
            }
            ushort ICellDescriptor.CellType
            {
                get { return (ushort)CellType.");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"; }
            }
            #endregion
            #region ITypeDescriptor
            public string TypeName
            {
                get { return s_cellTypeName_");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"; }
            }
            public Type Type
            {
                get { return s_cellType_");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"; }
            }
            public bool IsOfType<T>()
            {
                return typeof(T) == s_cellType_");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@";
            }
            public bool IsList()
            {
                return false;
            }
            #endregion
            #region IAttributeCollection
            public IReadOnlyDictionary<string, string> Attributes
            {
                get { return s_attributes; }
            }
            public string GetAttributeValue(string attributeKey)
            {
                string ret = null;
                s_attributes.TryGetValue(attributeKey, out ret);
                return ret;
            }
            #endregion
        }
        internal static readonly ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_descriptor s_cellDescriptor_");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@" = new ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_descriptor();
        /// <summary>
        /// Get the cell descriptor for ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@".
        /// </summary>
        public static ICellDescriptor ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@" { get { return s_cellDescriptor_");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"; } }
        ");
}
source.Append(@"
        /// <summary>
        /// Enumerates descriptors for all cells defined in the TSL.
        /// </summary>
        public static IEnumerable<ICellDescriptor> CellDescriptors
        {
            get
            {
                ");
for (int iterator_1 = 0; iterator_1 < (node->cellList).Count;++iterator_1)
{
source.Append(@"
                yield return ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@";
                ");
}
source.Append(@"
                yield break;
            }
        }
        /// <summary>
        /// Converts a type string to <see cref=""");
source.Append(Codegen.GetString(Trinity::Codegen::GetNamespace()));
source.Append(@".CellType""/>.
        /// </summary>
        /// <param name=""cellTypeString"">The type string to be converted.</param>
        /// <returns>The converted cell type.</returns>
        public static CellType GetCellType(string cellTypeString)
        {
            CellType ret;
            if (!cellTypeLookupTable.TryGetValue(cellTypeString, out ret))
                throw new Exception(""Unrecognized cell type string."");
            return ret;
        }
        #region IStorageSchema implementation
        IEnumerable<ICellDescriptor> IStorageSchema.CellDescriptors
        {
            get { return StorageSchema.CellDescriptors; }
        }
        ushort IStorageSchema.GetCellType(string cellTypeString)
        {
            return (ushort)StorageSchema.GetCellType(cellTypeString);
        }
        IEnumerable<string> IStorageSchema.CellTypeSignatures
        {
            get
            {
                ");
for (int iterator_1 = 0; iterator_1 < (node->cellList).Count;++iterator_1)
{
source.Append(@"
                yield return """);
source.Append(Codegen.GetString(Trinity::Codegen::get_signature_string((node->cellList)[iterator_1])));
source.Append(@""";
                ");
}
source.Append(@"
                yield break;
            }
        }
        #endregion
    }
}
");

            return source.ToString();
        }
    }
}

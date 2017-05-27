#include "common.h"
#include <string>
#include "SyntaxNode.h"

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
StorageSchema(
NTSL* node)
        {
            string* source = new string();
            
source->append(R"::(using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Storage;
using Trinity.TSL;
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(
{
    /// <summary>
    /// Exposes the data modeling schema defined in the TSL.
    /// </summary>
    
    public class StorageSchema : IStorageSchema
    {
        #region CellType lookup table
        internal static Dictionary<string, CellType> cellTypeLookupTable = new Dictionary<string, CellType>()
        {
            )::");
for (size_t iterator_1 = 0; iterator_1 < (node->cellList)->size();++iterator_1)
{
source->append(R"::(
            {")::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(", global::)::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.CellType.)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(}
            )::");
if (iterator_1 < (node->cellList)->size() - 1)
source->append(",");
}
source->append(R"::(
        };
        #endregion
        )::");
for (size_t iterator_1 = 0; iterator_1 < (node->cellList)->size();++iterator_1)
{
source->append(R"::(
        internal static readonly Type   s_cellType_)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(       = typeof(global::)::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::();
        internal static readonly string s_cellTypeName_)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(   = ")::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(";
        internal class )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_descriptor : ICellDescriptor
        {
            private static IReadOnlyDictionary<string, string> s_attributes = new Dictionary<string, string>
            {
                )::");
for (size_t iterator_2 = 0; iterator_2 < ((*(node->cellList))[iterator_1]->attributes)->size();++iterator_2)
{
source->append(R"::(
                { ")::");
source->append(Codegen::GetString((*((*(node->cellList))[iterator_1]->attributes))[iterator_2]->key));
source->append(R"::(", ")::");
source->append(Codegen::GetString((*((*(node->cellList))[iterator_1]->attributes))[iterator_2]->value));
source->append(R"::(" }
                )::");
if (iterator_2 < ((*(node->cellList))[iterator_1]->attributes)->size() - 1)
source->append(",");
}
source->append(R"::(
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
            )::");
for (size_t iterator_2 = 0; iterator_2 < ((*(node->cellList))[iterator_1]->fieldList)->size();++iterator_2)
{
source->append(R"::(
            internal class )::");
source->append(Codegen::GetString((*((*(node->cellList))[iterator_1]->fieldList))[iterator_2]->name));
source->append(R"::(_descriptor : IFieldDescriptor
            {
                private static IReadOnlyDictionary<string, string> s_attributes = new Dictionary<string, string>
                {
                    )::");
for (size_t iterator_3 = 0; iterator_3 < ((*((*(node->cellList))[iterator_1]->fieldList))[iterator_2]->attributes)->size();++iterator_3)
{
source->append(R"::(
                    { ")::");
source->append(Codegen::GetString((*((*((*(node->cellList))[iterator_1]->fieldList))[iterator_2]->attributes))[iterator_3]->key));
source->append(R"::(", ")::");
source->append(Codegen::GetString((*((*((*(node->cellList))[iterator_1]->fieldList))[iterator_2]->attributes))[iterator_3]->value));
source->append(R"::(" }
                    )::");
if (iterator_3 < ((*((*(node->cellList))[iterator_1]->fieldList))[iterator_2]->attributes)->size() - 1)
source->append(",");
}
source->append(R"::(
                };
                private static string s_typename = ")::");
source->append(Codegen::GetString((*((*(node->cellList))[iterator_1]->fieldList))[iterator_2]->fieldType));
source->append(R"::(";
                private static Type   s_type     = typeof()::");
source->append(Codegen::GetString((*((*(node->cellList))[iterator_1]->fieldList))[iterator_2]->fieldType));
source->append(R"::();
                public string Name
                {
                    get { return ")::");
source->append(Codegen::GetString((*((*(node->cellList))[iterator_1]->fieldList))[iterator_2]->name));
source->append(R"::("; }
                }
                public bool Optional
                {
                    get
                    {
                        )::");
if ((*((*(node->cellList))[iterator_1]->fieldList))[iterator_2]->is_optional())
{
source->append(R"::(
                        return true;
                        )::");
}
else
{
source->append(R"::(
                        return false;
                        )::");
}
source->append(R"::(
                    }
                }
                public bool IsOfType<T>()
                {
                    return typeof(T) == Type;
                }
                public bool IsList()
                {
                    )::");
if ((*((*(node->cellList))[iterator_1]->fieldList))[iterator_2]->fieldType->is_list())
{
source->append(R"::(
                    return true;
                    )::");
}
else
{
source->append(R"::(
                    return false;
                    )::");
}
source->append(R"::(
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
            internal static )::");
source->append(Codegen::GetString((*((*(node->cellList))[iterator_1]->fieldList))[iterator_2]->name));
source->append(R"::(_descriptor )::");
source->append(Codegen::GetString((*((*(node->cellList))[iterator_1]->fieldList))[iterator_2]->name));
source->append(R"::( = new )::");
source->append(Codegen::GetString((*((*(node->cellList))[iterator_1]->fieldList))[iterator_2]->name));
source->append(R"::(_descriptor();
            )::");
}
source->append(R"::(
            #region ICellDescriptor
            public IEnumerable<string> GetFieldNames()
            {
                )::");
int field_cnt_1 = 0;
for (size_t iterator_2 = 0; iterator_2 < ((*(node->cellList))[iterator_1]->fieldList)->size();++iterator_2)
{
source->append(R"::(
                yield return ")::");
source->append(Codegen::GetString((*((*(node->cellList))[iterator_1]->fieldList))[iterator_2]->name));
source->append(R"::(";
                )::");
++field_cnt_1;
}
if (field_cnt_1 == 0)
{
source->append(R"::(
                {
                    yield break;
                }
                )::");
}
source->append(R"::(
            }
            public IAttributeCollection GetFieldAttributes(string fieldName)
            {
                int field_id = global::)::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(.FieldLookupTable.Lookup(fieldName);
                if (field_id == -1)
                    Throw.undefined_field();
                switch (field_id)
                {
                    )::");
for (size_t iterator_2 = 0; iterator_2 < ((*(node->cellList))[iterator_1]->fieldList)->size();++iterator_2)
{
source->append(R"::(
                    case )::");
source->append(Codegen::GetString(iterator_2));
source->append(R"::(:
                        return )::");
source->append(Codegen::GetString((*((*(node->cellList))[iterator_1]->fieldList))[iterator_2]->name));
source->append(R"::(;
                    )::");
}
source->append(R"::(
                }
                /* Should not reach here */
                throw new Exception("Internal error T6001");
            }
            public IEnumerable<IFieldDescriptor> GetFieldDescriptors()
            {
                )::");
int field_cnt_2 = 0;
for (size_t iterator_2 = 0; iterator_2 < ((*(node->cellList))[iterator_1]->fieldList)->size();++iterator_2)
{
source->append(R"::(
                yield return )::");
source->append(Codegen::GetString((*((*(node->cellList))[iterator_1]->fieldList))[iterator_2]->name));
source->append(R"::(;
                )::");
++field_cnt_2;
}
if (field_cnt_2 == 0)
{
source->append(R"::(
                {
                    yield break;
                }
                )::");
}
source->append(R"::(
            }
            ushort ICellDescriptor.CellType
            {
                get { return (ushort)CellType.)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(; }
            }
            #endregion
            #region ITypeDescriptor
            public string TypeName
            {
                get { return s_cellTypeName_)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(; }
            }
            public Type Type
            {
                get { return s_cellType_)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(; }
            }
            public bool IsOfType<T>()
            {
                return typeof(T) == s_cellType_)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(;
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
        internal static readonly )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_descriptor s_cellDescriptor_)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::( = new )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_descriptor();
        /// <summary>
        /// Get the cell descriptor for )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(.
        /// </summary>
        public static ICellDescriptor )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::( { get { return s_cellDescriptor_)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(; } }
        )::");
}
source->append(R"::(
        /// <summary>
        /// Enumerates descriptors for all cells defined in the TSL.
        /// </summary>
        public static IEnumerable<ICellDescriptor> CellDescriptors
        {
            get
            {
                )::");
for (size_t iterator_1 = 0; iterator_1 < (node->cellList)->size();++iterator_1)
{
source->append(R"::(
                yield return )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(;
                )::");
}
source->append(R"::(
                yield break;
            }
        }
        /// <summary>
        /// Converts a type string to <see cref=")::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.CellType"/>.
        /// </summary>
        /// <param name="cellTypeString">The type string to be converted.</param>
        /// <returns>The converted cell type.</returns>
        public static CellType GetCellType(string cellTypeString)
        {
            CellType ret;
            if (!cellTypeLookupTable.TryGetValue(cellTypeString, out ret))
                throw new Exception("Unrecognized cell type string.");
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
                )::");
for (size_t iterator_1 = 0; iterator_1 < (node->cellList)->size();++iterator_1)
{
source->append(R"::(
                yield return ")::");
source->append(Codegen::GetString(Trinity::Codegen::get_signature_string((*(node->cellList))[iterator_1])));
source->append(R"::(";
                )::");
}
source->append(R"::(
                yield break;
            }
        }
        #endregion
    }
}
)::");

            return source;
        }
    }
}

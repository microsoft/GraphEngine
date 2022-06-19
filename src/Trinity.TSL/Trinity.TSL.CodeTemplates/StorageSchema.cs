using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Storage;
using Trinity.TSL;

/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
namespace t_Namespace
{
    /// <summary>
    /// Exposes the data modeling schema defined in the TSL.
    /// </summary>
    [TARGET("NTSL")]
    [MAP_LIST("t_cell", "node->cellList")]
    [MAP_VAR("t_cell_name", "name")]
    [MAP_VAR("t_cell_signature", "Trinity::Codegen::get_signature_string($$)")]
    [MAP_LIST("t_cell_attribute", "attributes", MemberOf = "t_cell")]
    [MAP_VAR("t_cell_attribute_key", "key")]
    [MAP_VAR("t_cell_attribute_value", "value")]
    [MAP_LIST("t_field", "fieldList", MemberOf = "t_cell")]
    [MAP_VAR("t_field", "")]
    [MAP_VAR("t_field_name", "name")]
    [MAP_VAR("t_field_type", "fieldType")]
    [MAP_LIST("t_field_attribute", "attributes", MemberOf = "t_field")]
    [MAP_VAR("t_field_attribute_key", "key")]
    [MAP_VAR("t_field_attribute_value", "value")]
    [MAP_VAR("t_int", "GET_ITERATOR_VALUE()")]
    public class StorageSchema : __meta, IStorageSchema
    {
        #region CellType lookup table
        internal static Dictionary<string, CellType> cellTypeLookupTable = new Dictionary<string, CellType>()
        {
            /*FOREACH(",")*/
            {"t_cell_name", global::t_Namespace.CellType.t_cell_name}
            /*END()*/
        };
        #endregion
        [FOREACH]
        internal static readonly Type   s_cellType_t_cell_name       = typeof(global::t_Namespace.t_cell_name);
        internal static readonly string s_cellTypeName_t_cell_name   = "t_cell_name";
        internal class t_cell_name_descriptor : ICellDescriptor
        {
            private static IReadOnlyDictionary<string, string> s_attributes = new Dictionary<string, string>
            {
                /*FOREACH(",")*/
                { "t_cell_attribute_key", "t_cell_attribute_value" }
                /*END*/
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


            [FOREACH]
            internal class t_field_name_descriptor : IFieldDescriptor
            {
                private static IReadOnlyDictionary<string, string> s_attributes = new Dictionary<string, string>
                {
                    /*FOREACH(",")*/
                    { "t_field_attribute_key", "t_field_attribute_value" }
                    /*END*/
                };
                private static string s_typename = "t_field_type";
                private static Type   s_type     = typeof(t_field_type);
                public string Name
                {
                    get { return "t_field_name"; }
                }

                public bool Optional
                {
                    get
                    {
                        IF("$t_field->is_optional()");
                        return true;
                        ELSE();
                        return false;
                        END();
                    }
                }

                public bool IsOfType<T>()
                {
                    return typeof(T) == Type;
                }

                public bool IsList()
                {
                    IF("$t_field_type->is_list()");
                    return true;
                    ELSE();
                    return false;
                    END();
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
            internal static t_field_name_descriptor t_field_name = new t_field_name_descriptor();
            [END]

            #region ICellDescriptor
            public IEnumerable<string> GetFieldNames()
            {
                META_VAR("int", "field_cnt", "0");
                FOREACH();
                yield return "t_field_name";
                META("++%field_cnt;");
                END();//FOREACH
                IF("%field_cnt == 0");
                {
                    yield break;
                }
                END();//IF 
            }

            public IAttributeCollection GetFieldAttributes(string fieldName)
            {
                int field_id = global::t_Namespace.t_cell_name.FieldLookupTable.Lookup(fieldName);
                if (field_id == -1)
                    Throw.undefined_field();
                switch (field_id)
                {
                    /*FOREACH*/
                    /*USE_LIST("t_field")*/
                    case t_int:
                        return t_field_name;
                    /*END*/
                }

                /* Should not reach here */
                throw new Exception("Internal error T6001");
            }

            public IEnumerable<IFieldDescriptor> GetFieldDescriptors()
            {
                META_VAR("int", "field_cnt", "0");
                FOREACH();
                yield return t_field_name;
                META("++%field_cnt;");
                END();//FOREACH
                IF("%field_cnt == 0");
                {
                    yield break;
                }
                END();//IF 
            }

            ushort ICellDescriptor.CellType
            {
                get { return (ushort)CellType.t_cell_name; }
            }
            #endregion

            #region ITypeDescriptor
            public string TypeName
            {
                get { return s_cellTypeName_t_cell_name; }
            }

            public Type Type
            {
                get { return s_cellType_t_cell_name; }
            }

            public bool IsOfType<T>()
            {
                return typeof(T) == s_cellType_t_cell_name;
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
        internal static readonly t_cell_name_descriptor s_cellDescriptor_t_cell_name = new t_cell_name_descriptor();
        /// <summary>
        /// Get the cell descriptor for t_cell_name.
        /// </summary>
        public static ICellDescriptor t_cell_name { get { return s_cellDescriptor_t_cell_name; } }
        /**/

        /// <summary>
        /// Enumerates descriptors for all cells defined in the TSL.
        /// </summary>
        public static IEnumerable<ICellDescriptor> CellDescriptors
        {
            get
            {
                FOREACH();
                yield return t_cell_name;
                END();
                yield break;
            }
        }

        /// <summary>
        /// Converts a type string to <see cref="t_Namespace.CellType"/>.
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
                FOREACH();
                yield return "t_cell_signature";
                END();
                yield break;
            }
        }
        #endregion

    }
}

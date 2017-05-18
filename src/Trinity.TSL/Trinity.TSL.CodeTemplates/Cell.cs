using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;
using System.Security;

using Trinity;
using Trinity.Core.Lib;
using Trinity.Storage;
using Trinity.Utilities;
using Trinity.TSL.Lib;
using Trinity.Network;
using Trinity.Network.Sockets;
using Trinity.Network.Messaging;
using Trinity.TSL;
using System.Runtime.CompilerServices;
#pragma warning disable

/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
namespace t_Namespace
{
    //Generates a class [TemplateName]_Generator
    //Setting target. Will expand to "NStruct* node;" in C++
    [TARGET("NCell")]
    //Setting [STRUCT] will instruct metagen to translate that unit to a struct
    [STRUCT]
    //All units are written as class, as structs are non-inheritable, and we
    //want to inherit from __meta.
    [MAP_VAR("t_cell_name", "node->name")]
    [MAP_LIST("t_field", "node->fieldList")]
    [MAP_VAR("t_field", "")]
    //When matching a template, all satisfied maps
    //will be expanded one after another.
    [MAP_VAR("t_field_name", "name")]
    [MAP_VAR("t_field_type", "fieldType")]
    [MAP_VAR("t_field_type_display", "Trinity::Codegen::GetDataTypeDisplayString($$->fieldType)")]
    [MAP_VAR("t_field_type_remove_nullable", "Trinity::Codegen::GetNonNullableValueTypeString($$->fieldType)")]
    [MAP_VAR("t_field_type_list_element_type", "fieldType->listElementType")]
    [MAP_VAR("t_field_type_list_element_type_display", "Trinity::Codegen::GetDataTypeDisplayString($$->fieldType->listElementType)")]
    [MAP_LIST("t_field_attribute", "attributes", MemberOf = "t_field")]
    [MAP_VAR("t_field_attribute_key", "key")]
    [MAP_VAR("t_field_attribute_value", "value")]
    [MAP_LIST("t_data_type", "Trinity::Codegen::TSLExternalParserDataTypeVector")]
    [MAP_VAR("t_data_type", "")]
    [MAP_VAR("t_data_type_display", "Trinity::Codegen::GetDataTypeDisplayString($$)")]
    [MAP_VAR("t_int", "GET_ITERATOR_VALUE()")]
    [MAP_VAR("t_uint", "GET_ITERATOR_VALUE()")]
    [MAP_VAR("t_int_2", "%iter_val++")]
    [META_VAR("bool", "struct_nonempty", "node->fieldList->size() > 0")]
    [MAP_VAR("t_accessor_type", "data_type_get_accessor_name($$->fieldType)", MemberOf = "t_field")]

    /*META_VAR("std::unordered_set<std::string>", "field_attributes")*/
    /*FOREACH()*/
    /*  USE_LIST("t_field")*/
    /*  FOREACH()*/
    /*    USE_LIST("t_field_attribute")*/
    /*    META("%field_attributes.insert(*$t_field_attribute_key);")*/
    /*  END()*/
    /*END()*/
    /// <summary>
    /// A .NET runtime object representation of t_cell_name defined in TSL.
    /// </summary>
    public partial class t_cell_name : __meta, ICell
    {
        ///<summary>
        ///The id of the cell.
        ///</summary>
        public long CellID;
        ///<summary>
        ///Initializes a new instance of t_cell_name with the specified parameters.
        ///</summary>
        public t_cell_name(long cell_id, [FOREACH(",")] t_field_type t_field_name = default(t_field_type)/*END*/)
        {
            FOREACH();
            this.t_field_name = t_field_name;
            END();
            CellID = cell_id;
        }

        //Loop via pre-defined variables
        //Will expand to C++ for(...)
        [IF("%struct_nonempty")]
        ///<summary>
        ///Initializes a new instance of t_cell_name with the specified parameters.
        ///</summary>
        public t_cell_name([FOREACH(",")] t_field_type t_field_name = default(t_field_type)/*END*/)
        {
            FOREACH();
            this.t_field_name = t_field_name;
            END();
            CellID = CellIDFactory.NewCellID();
        }

        [MUTE]
        public t_cell_name(long cell_id, t_field_type t_field_name, t_field_type t_field_name1) : this(cell_id, t_field_name)
        {
            throw new NotImplementedException();
        }
        [MUTE_END]

        [END]

        [FOREACH]
        public t_field_type t_field_name;
        [END]

        public static bool operator ==(t_cell_name a, t_cell_name b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }
            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }
            IF("%struct_nonempty");
            // Return true if the fields match:
            return
                /*FOREACH("&&")*/
                (a.t_field_name == b.t_field_name)
                /*END*/
                ;
            ELSE();
            return true;
            END();
        }

        public static bool operator !=(t_cell_name a, t_cell_name b)
        {
            return !(a == b);
        }

        #region Text processing
        /// <summary>
        /// Converts the string representation of a t_cell_name to its
        /// struct equivalent. A return value indicates whether the 
        /// operation succeeded.
        /// </summary>
        /// <param name="input>A string to convert.</param>
        /// <param name="value">
        /// When this method returns, contains the struct equivalent of the value contained 
        /// in input, if the conversion succeeded, or default(t_cell_name) if the conversion
        /// failed. The conversion fails if the input parameter is null or String.Empty, or is 
        /// not of the correct format. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// True if input was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryParse(string input, out t_cell_name value)
        {
            try
            {
                value = Newtonsoft.Json.JsonConvert.DeserializeObject<t_cell_name>(input);
                return true;
            }
            catch { value = default(t_cell_name); return false; }
        }

        public static t_cell_name Parse(string input)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<t_cell_name>(input);
        }

        ///<summary>Converts a t_cell_name to its string representation, in JSON format.</summary>
        ///<returns>A string representation of the t_cell_name.</returns>
        public override string ToString()
        {
            return Serializer.ToString(this);
        }
        #endregion

        #region Lookup tables
        internal static StringLookupTable FieldLookupTable = new StringLookupTable(
            /*FOREACH(",")*/
            "t_field_name"
            /*END*/
            );

        internal static HashSet<string> AppendToFieldRerouteSet = new HashSet<string>()
        {
            /*FOREACH()*/
            /*IF("$t_field_type->is_value_type()")*/
            "t_field_name"
            /*LITERAL_OUTPUT(",")*/
            /*END*/
            /*END*/
        };
        #endregion

        #region ICell implementation

        /// <summary>
        /// Get the field of the specified name in the cell.
        /// </summary>
        /// <typeparam name="T">
        /// The desired type that the field is supposed 
        /// to be intepreted as. Automatic type casting 
        /// will be attempted if the desired type is not 
        /// implicitly convertible from the type of the field.
        /// </typeparam>
        /// <param name="fieldName">The name of the target field.</param>
        /// <returns>The value of the field.</returns>
        public T GetField<T>(string fieldName)
        {
            switch (FieldLookupTable.Lookup(fieldName))
            {
                case -1:
                    Throw.undefined_field();
                    break;
                /*FOREACH*/
                /*USE_LIST("t_field")*/
                case t_int:
                    return TypeConverter<T>.ConvertFrom_t_field_type_display(this.t_field_name);
                    /*END*/
            }

            /* Should not reach here */
            throw new Exception("Internal error T5005");
        }

        /// <summary>
        /// Set the value of the target field.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the value.
        /// </typeparam>
        /// <param name="fieldName">The name of the target field.</param>
        /// <param name="value">
        /// The value of the field. Automatic type casting 
        /// will be attempted if the desired type is not 
        /// implicitly convertible from the type of the field.
        /// </param>
        public void SetField<T>(string fieldName, T value)
        {
            switch (FieldLookupTable.Lookup(fieldName))
            {
                case -1:
                    Throw.undefined_field();
                    break;
                /*FOREACH*/
                /*USE_LIST("t_field")*/
                case t_int:
                    this.t_field_name = TypeConverter<T>.ConvertTo_t_field_type_display(value);
                    break;
                /*END*/
                default:
                    Throw.data_type_incompatible_with_field(typeof(T).ToString());
                    break;
            }
        }

        /// <summary>
        /// Tells if a field with the given name exists in the current cell.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        /// <returns>The existence of the field.</returns>
        public bool ContainsField(string fieldName)
        {
            switch (FieldLookupTable.Lookup(fieldName))
            {
                /*FOREACH*/
                /*USE_LIST("t_field")*/
                case t_int:
                    IF("$t_field_type->is_optional()");
                    return this.t_field_name != null;
                    ELSE();
                    return true;
                    END();
                /*END*/
                default:
                    return false;
            }
        }

        /// <summary>
        /// Append <paramref name="value"/> to the target field. Note that if the target field
        /// is not appendable(string or list), calling this method is equivalent to <see cref="t_Namespace.GenericCellAccessor.SetField(string, T)"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the value.
        /// </typeparam>
        /// <param name="fieldName">The name of the target field.</param>
        /// <param name="value">The value to be appended. 
        /// If the value is incompatible with the element 
        /// type of the field, automatic type casting will be attempted.
        /// </param>
        public void AppendToField<T>(string fieldName, T value)
        {
            if (AppendToFieldRerouteSet.Contains(fieldName))
            {
                SetField(fieldName, value);
                return;
            }

            switch (FieldLookupTable.Lookup(fieldName))
            {
                case -1:
                    Throw.undefined_field();
                    break;
                /*FOREACH*/
                /*USE_LIST("t_field")*/
                /*IF("$t_field_type->is_string() || $t_field_type->is_list()")*/
                case t_int:
                    IF("$t_field_type->is_string()");
                    {
                        if (this.t_field_name == null)
                            this.t_field_name = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.t_field_name += TypeConverter<T>.ConvertTo_string(value);
                    }
                    ELIF("$t_field_type->is_list()");
                    {
                        if (this.t_field_name == null)
                            this.t_field_name = new t_field_type();

                        switch (TypeConverter<T>.GetConversionActionTo_t_field_type_display())
                        {
                            case TypeConversionAction.TC_ASSIGN:
                                foreach (var element in value as t_field_type)
                                    this.t_field_name.Add(element);
                                break;
                            case TypeConversionAction.TC_CONVERTLIST:
                            case TypeConversionAction.TC_ARRAYTOLIST:
                                foreach (var element in TypeConverter<T>.Enumerate_t_field_type_list_element_type_display(value))
                                    this.t_field_name.Add(element);
                                break;
                            case TypeConversionAction.TC_WRAPINLIST:
                            case TypeConversionAction.TC_PARSESTRING:
                                // TC_PARSESTRING is a special case. It means that T is string. It is ambiguous in the sense that
                                // a string can be converted into both element and the container.
                                // So we make a rule that in an AppendToField call, if T is string, it always means to parse the
                                // string as an element and then insert into the field.
                                this.t_field_name.Add(TypeConverter<T>.ConvertTo_t_field_type_list_element_type_display(value));
                                break;
                            default:
                                Throw.data_type_incompatible_with_list(typeof(T).ToString());
                                break;
                        }
                    }
                    END();
                    break;
                /*END*/
                /*END*/
                default:
                    Throw.target__field_not_list();
                    break;
            }
        }

        long ICell.CellID { get { return CellID; } set { CellID = value; } }

        public IEnumerable<KeyValuePair<string, T>> SelectFields<T>(string attributeKey, string attributeValue)
        {
            switch (TypeConverter<T>.type_id)
            {
                // Only enable the case if there are any compatible fields
                /*FOREACH*/
                /*  USE_LIST("t_data_type")*/
                /*  META_VAR("bool", "compatible", "false"*/
                /*  FOREACH()*/
                /*    USE_LIST("t_field")*/
                /*    IF("$t_data_type->is_convertible_from($t_field_type)")*/
                /*      META("%compatible = true;")*/
                /*    END()*/
                /*  END()*/
                /*  IF("%compatible")*/
                case t_uint:
                    FOREACH();
                    USE_LIST("t_field");
                    IF("$t_data_type->is_convertible_from($t_field_type)");
                    if (StorageSchema.t_cell_name_descriptor.check_attribute(StorageSchema.t_cell_name_descriptor.t_field_name, attributeKey, attributeValue))
                        /*IF("$t_field->is_optional()")*/
                        if (this.t_field_name != null)
                            /*END()*/
                            yield return new KeyValuePair<string, T>("t_field_name", TypeConverter<T>.ConvertFrom_t_field_type_display(this.t_field_name));
                    END();
                    END();
                    break;
                /*END()*/
                /**/
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
        }

        #region enumerate value constructs
        [FOREACH]
        [USE_LIST("t_field")]
        private IEnumerable<T> _enumerate_from_t_field_name<T>()
        {
            MODULE_CALL("EnumerateFromFieldModule", "$t_field", /*for accessor:*/"false");
            MUTE();
            yield break;
            MUTE_END();
        }
        [END]
        private static StringLookupTable s_field_attribute_id_table = new StringLookupTable(
            /*META_VAR("int", "iter_val", "0")*/
            /*META("for(const std::string& attr : %field_attributes){")*/
            "/*META_OUTPUT(attr)*/"
            /*META("++%iter_val;")*/
            /*IF("%iter_val < %field_attributes.size()")*/
            /*LITERAL_OUTPUT(",")*/
            /*END*/
            /*META("}")*/
            );
        #endregion

        public IEnumerable<T> EnumerateField<T>(string fieldName)
        {
            switch (FieldLookupTable.Lookup(fieldName))
            {
                /*FOREACH*/
                /*USE_LIST("t_field")*/
                case t_int:
                    return _enumerate_from_t_field_name<T>();
                /*END*/
                default:
                    Throw.undefined_field();
                    return null;
            }
        }

        public IEnumerable<T> EnumerateValues<T>(string attributeKey, string attributeValue)
        {
            int attr_id;
            if (attributeKey == null)
            {
                FOREACH();
                foreach (var val in _enumerate_from_t_field_name<T>())
                    yield return val;
                END();
            }
            else if (-1 != (attr_id = s_field_attribute_id_table.Lookup(attributeKey)))
            {
                switch (attr_id)
                {
                    /*META_VAR("int", "iter_val", "0")*/
                    /*META("for(const std::string& attr : %field_attributes){")*/
                    case t_int_2:
                        FOREACH();
                        {
                            USE_LIST("t_field");
                            META_VAR("std::string*", "p_field_attr_value", "$t_field->get_attribute(attr)");
                            IF("%p_field_attr_value != nullptr");
                            {
                                if (attributeValue == null || attributeValue == "/*META_OUTPUT(%p_field_attr_value)*/")
                                {
                                    foreach (var val in _enumerate_from_t_field_name<T>())
                                        yield return val;
                                }
                            }
                            END();
                        }
                        END();
                        break;
                        /*META("}")*/
                }
            }
            yield break;
        }

        #endregion

        #region Other interfaces
        string ITypeDescriptor.TypeName
        {
            get { return StorageSchema.s_cellTypeName_t_cell_name; }
        }

        Type ITypeDescriptor.Type
        {
            get { return StorageSchema.s_cellType_t_cell_name; }
        }

        bool ITypeDescriptor.IsOfType<T>()
        {
            return typeof(T) == StorageSchema.s_cellType_t_cell_name;
        }

        bool ITypeDescriptor.IsList()
        {
            return false;
        }


        IEnumerable<IFieldDescriptor> ICellDescriptor.GetFieldDescriptors()
        {
            return StorageSchema.t_cell_name.GetFieldDescriptors();
        }

        IAttributeCollection ICellDescriptor.GetFieldAttributes(string fieldName)
        {
            return StorageSchema.t_cell_name.GetFieldAttributes(fieldName);
        }

        string IAttributeCollection.GetAttributeValue(string attributeKey)
        {
            return StorageSchema.t_cell_name.GetAttributeValue(attributeKey);
        }

        IReadOnlyDictionary<string, string> IAttributeCollection.Attributes
        {
            get { return StorageSchema.t_cell_name.Attributes; }
        }

        IEnumerable<string> ICellDescriptor.GetFieldNames()
        {
            META_VAR("int", "field_cnt", "0");
            FOREACH();
            IF("$t_field->is_optional()");
            {
                // optional reference types
                if (this.t_field_name != null)
                    yield return "t_field_name";
            }
            ELSE();
            {
                // mandatory types
                yield return "t_field_name";
            }
            META("++%field_cnt;");
            END();//IF
            END();//FOREACH
            IF("%field_cnt == 0");
            {
                yield break;
            }
            END();//IF
        }

        ushort ICellDescriptor.CellType
        {
            get
            {
                return (ushort)CellType.t_cell_name;
            }
        }
        #endregion

    }

    /// <summary>
    /// Provides in-place operations of t_cell_name defined in TSL.
    /// </summary>
    public unsafe class t_cell_name_Accessor : __meta, ICellAccessor
    {
        internal t_cell_name_Accessor(long cellId, byte[] buffer)
        {
            this.CellID       = cellId;
            this.handle       = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            this.CellPtr      = (byte*)handle.AddrOfPinnedObject().ToPointer();

            FOREACH();
            USE_LIST("t_field");
            MODULE_CALL("CellFieldAccessorInitialization", "$t_field");
            END();//FOREACH

            this.CellEntryIndex = -1;
        }

        [MODULE_CALL("OptionalFields", "node")]

        public byte[] ToByteArray()
        {
            byte* targetPtr = CellPtr;

            MODULE_CALL("PushPointerThroughStruct", "node");

            int size = (int)(targetPtr - CellPtr);
            byte[] ret = new byte[size];
            Memory.Copy(CellPtr, 0, ret, 0, size);
            return ret;
        }

        internal unsafe t_cell_name_Accessor(long cellId, CellAccessOptions options)
        {
            Initialize(cellId, options);

            FOREACH();
            USE_LIST("t_field");
            MODULE_CALL("CellFieldAccessorInitialization", "$t_field");
            END();//FOREACH

            this.CellID = cellId;
        }

        public unsafe t_cell_name_Accessor(byte* _CellPtr)
        {
            CellPtr = _CellPtr;

            FOREACH();
            USE_LIST("t_field");
            MODULE_CALL("CellFieldAccessorInitialization", "$t_field");
            END();//FOREACH

            this.CellEntryIndex = -1;
        }

        internal static unsafe byte[] construct(long CellID /*FOREACH*/, t_field_type t_field_name = default(t_field_type) /*END*/)
        {
            MUTE();
            byte[] tmpcell = null;
            MUTE_END();
            MODULE_CALL("SerializeParametersToBuffer", "node", "\"cell\"");
            return tmpcell;
        }

        [MODULE_CALL("AccessorFieldsDefinition", "node")]
        [MUTE]
        #region MUTE
        t_accessor_type t_field_name_Accessor_Field;

        internal void Remove_t_field_name()
        {
            throw new NotImplementedException();
        }

        internal bool Contains_t_field_name
        {
            get;
            set;
        }
        public unsafe t_field_type t_field_name
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public unsafe static implicit operator t_cell_name_Accessor(t_cell_name cell)
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(t_cell_name_Accessor a, t_cell_name_Accessor b)
        {
            throw new NotImplementedException();
        }

        public static bool operator !=(t_cell_name_Accessor a, t_cell_name_Accessor b)
        {
            return !(a == b);
        }

        #endregion
        [MUTE_END]

        public static unsafe implicit operator t_cell_name(t_cell_name_Accessor accessor)
        {
            FOREACH();
            IF("$t_field->is_optional()");
            t_field_type _t_field_name = default(t_field_type);
            if (accessor.Contains_t_field_name)
            {
                IF("$t_field_type->is_value_type()");
                _t_field_name = (t_field_type_remove_nullable)accessor.t_field_name;
                ELSE();
                _t_field_name = accessor.t_field_name;
                END();
            }
            END();
            END();

            if (accessor.CellID != null)
            {
                return new t_cell_name(accessor.CellID.Value,
                /*FOREACH(",")*/
                /*IF("$t_field->is_optional()")*/
                _t_field_name /*MUTE*/ , /*MUTE_END*/
                                         /*ELSE*/
                        accessor.t_field_name
                /*END*/
                /*END*/
                );
            }
            else
            {
                return new t_cell_name(
                /*FOREACH(",")*/
                /*IF("$t_field->is_optional()")*/
                _t_field_name /*MUTE*/ , /*MUTE_END*/
                                         /*ELSE*/
                        accessor.t_field_name
                /*END*/
                /*END*/
                );
            }
        }
        [MODULE_CALL("StructAccessorReverseImplicitOperator", "node")]

        [MODULE_CALL("StructAccessorEqualOperator", "node")]


        public static bool operator ==(t_cell_name_Accessor a, t_cell_name b)
        {
            t_cell_name_Accessor bb = b;
            return (a == bb);
        }

        public static bool operator !=(t_cell_name_Accessor a, t_cell_name b)
        {
            return !(a == b);
        }


        #region Fields
        /// <summary>
        /// Get a pointer to the underlying raw binary blob. Take caution when accessing data with
        /// the raw pointer, as no boundary checks are employed, and improper operations will cause data corruption and/or system crash.
        /// </summary>
        internal byte* CellPtr { get; set; }
        /// <summary>
        /// Get the size of the cell content, in bytes.
        /// </summary>
        public int CellSize { get { int size; Global.LocalStorage.LockedGetCellSize(this.CellID.Value, this.CellEntryIndex, out size); return size; } }
        /// <summary>
        /// Get the cell id. The value can be null when the id is undefined.
        /// </summary>
        public long? CellID { get; internal set; }
        internal    int                     CellEntryIndex;
        internal    bool                    m_IsIterator   = false;
        internal    CellAccessOptions       m_options      = 0;
        private     GCHandle                handle;
        private     const CellAccessOptions c_WALFlags     = CellAccessOptions.StrongLogAhead | CellAccessOptions.WeakLogAhead;
        #endregion

        #region Internal
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void Initialize(long cellId, CellAccessOptions options)
        {
            int     cellSize;
            ushort  cellType;
            byte*   cellPtr;
            int     cellEntryIndex;
            var eResult = Global.LocalStorage.GetLockedCellInfo(cellId, out cellSize, out cellType, out cellPtr, out cellEntryIndex);

            switch (eResult)
            {
                case TrinityErrorCode.E_CELL_NOT_FOUND:
                    {
                        if ((options & CellAccessOptions.ThrowExceptionOnCellNotFound) != 0)
                        {
                            Throw.cell_not_found(cellId);
                        }
                        else if ((options & CellAccessOptions.CreateNewOnCellNotFound) != 0)
                        {
                            byte[]  defaultContent    = construct(cellId);
                            int     size              = defaultContent.Length;
                            eResult                   = Global.LocalStorage.AddOrUse(cellId, defaultContent, ref size, (ushort)CellType.t_cell_name, out cellPtr, out cellEntryIndex);

                            if (eResult == TrinityErrorCode.E_WRONG_CELL_TYPE)
                            {
                                Throw.wrong_cell_type();
                            }
                        }
                        else if ((options & CellAccessOptions.ReturnNullOnCellNotFound) != 0)
                        {
                            cellPtr        = null; /** Which indicates initialization failure. */
                            cellEntryIndex = -1;
                        }
                        else
                        {
                            Throw.cell_not_found(cellId);
                        }
                        break;
                    }
                case TrinityErrorCode.E_SUCCESS:
                    {
                        if (cellType != (ushort)CellType.t_cell_name)
                        {
                            Global.LocalStorage.ReleaseCellLock(cellId, cellEntryIndex);
                            Throw.wrong_cell_type();
                        }
                        break;
                    }
                default:
                    throw new NotImplementedException();
            }

            // If it made this far without throwing exceptions, we can accept the result.

            this.CellID         = cellId;
            this.CellPtr        = cellPtr;
            this.CellEntryIndex = cellEntryIndex;
            this.m_options      = options;

        }

        [ThreadStatic]
        internal static t_cell_name_Accessor s_accessor = null;

        internal static t_cell_name_Accessor New(long CellID, CellAccessOptions options)
        {
            t_cell_name_Accessor ret = null;

            if (s_accessor != (t_cell_name_Accessor)null)
            {
                ret = s_accessor;
                ret.Initialize(CellID, options);
                s_accessor = null;
            }
            else
            {
                ret = new t_cell_name_Accessor(CellID, options);
            }

            if (ret.CellPtr == null)
            {
                s_accessor = ret;
                ret        = null;
            }

            return ret;
        }

        // Does not call initialize. Caller guarantees that entry lock is obtained.
        // Does not handle CellAccessOptions. Only copy to the accessor.
        // This method is currently only used by GenericCell.cs
        internal static t_cell_name_Accessor New(long CellId, byte* cellPtr, int entryIndex, CellAccessOptions options)
        {
            t_cell_name_Accessor ret = null;

            if (s_accessor != (t_cell_name_Accessor)null)
            {
                ret = s_accessor;
                s_accessor = null;
                ret.CellPtr = cellPtr;
            }
            else
            {
                ret = new t_cell_name_Accessor(cellPtr);
            }

            ret.CellID         = CellId;
            ret.CellEntryIndex = entryIndex;
            ret.m_options      = options;

            return ret;
        }

        internal unsafe byte* ResizeFunction(byte* ptr, int ptr_offset, int delta)
        {
            int offset = (int)(ptr - CellPtr) + ptr_offset;
            CellPtr    = Global.LocalStorage.ResizeCell((long)CellID, CellEntryIndex, offset, delta);
            return CellPtr + (offset - ptr_offset);
        }

        internal static t_cell_name_Accessor AllocIterativeAccessor(CellInfo info)
        {

            t_cell_name_Accessor ret = null;
            if (s_accessor != (t_cell_name_Accessor)null)
            {
                ret                = s_accessor;
                ret.CellPtr        = info.CellPtr;
                s_accessor         = null;
            }
            else
            {
                ret                = new t_cell_name_Accessor(info.CellPtr);
            }

            ret.CellEntryIndex = info.CellEntryIndex;
            ret.CellID         = info.CellId;
            ret.m_IsIterator   = true;

            return ret;
        }
        #endregion//Internal

        #region Public
        /// <summary>
        /// Dispose the accessor.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// the cell lock will be released.
        /// If write-ahead-log behavior is specified on <see cref="t_Namespace.StorageExtension_t_cell_name.Uset_cell_name"/>,
        /// the changes will be committed to the write-ahead log.
        /// </summary>
        public void Dispose()
        {
            if (CellEntryIndex >= 0)
            {

                if ((m_options & c_WALFlags) != 0)
                {
                    LocalMemoryStorage.CWriteAheadLog(this.CellID.Value, this.CellPtr, this.CellSize, (ushort)CellType.t_cell_name, m_options);
                }

                if (!m_IsIterator)
                {
                    Global.LocalStorage.ReleaseCellLock(CellID.Value, CellEntryIndex);
                }

                if (s_accessor == (t_cell_name_Accessor)null)
                {
                    CellPtr        = null;
                    m_IsIterator   = false;
                    s_accessor     = this;
                }
            }

            if (handle != null && handle.IsAllocated)
                handle.Free();
        }

        /// <summary>
        /// Get the cell type id.
        /// </summary>
        /// <returns>A 16-bit unsigned integer representing the cell type id.</returns>
        public ushort GetCellType()
        {
            if (!CellID.HasValue)
            {
                Throw.cell_id_is_null();
            }
            ushort cellType;
            if (Global.LocalStorage.GetCellType(CellID.Value, out cellType) == TrinityErrorCode.E_CELL_NOT_FOUND)
            {
                Throw.cell_not_found();
            }
            return cellType;
        }

        /// <summary>Converts a t_cell_name_Accessor to its string representation, in JSON format.</summary>
        /// <returns>A string representation of the t_cell_name.</returns>
        public override string ToString()
        {
            return Serializer.ToString(this);
        }
        #endregion//Public

        #region Lookup tables
        internal static StringLookupTable FieldLookupTable = new StringLookupTable(
            /*FOREACH(",")*/
            "t_field_name"
            /*END*/
            );

        static HashSet<string> AppendToFieldRerouteSet = new HashSet<string>()
        {
            /*FOREACH()*/
            /*IF("$t_field_type->is_value_type()")*/
            "t_field_name"
            /*LITERAL_OUTPUT(",")*/
            /*END*/
            /*END*/
        };
        #endregion

        #region ICell implementation
        public T GetField<T>(string fieldName)
        {
            int field_divider_idx = fieldName.IndexOf('.');
            if (-1 != field_divider_idx)
            {
                string field_name_string = fieldName.Substring(0, field_divider_idx);
                switch (FieldLookupTable.Lookup(field_name_string))
                {
                    case -1:
                        Throw.undefined_field();
                        break;
                    /*FOREACH*/
                    /*USE_LIST("t_field")*/
                    /*  IF("$t_field_type->is_struct()")*/
                    case t_int:
                        return GenericFieldAccessor.GetField<T>(this.t_field_name, fieldName, field_divider_idx + 1);
                    /*  END*/
                    /*END*/
                    default:
                        // @note   We cannot go further unless it's a struct.
                        //         Throw exception now.
                        Throw.member_access_on_non_struct__field(field_name_string);
                        break;
                }
            }

            switch (FieldLookupTable.Lookup(fieldName))
            {
                case -1:
                    Throw.undefined_field();
                    break;
                /*FOREACH*/
                /*USE_LIST("t_field")*/
                case t_int:
                    return TypeConverter<T>.ConvertFrom_t_field_type_display(this.t_field_name);
                    /*END*/
            }

            /* Should not reach here */
            throw new Exception("Internal error T5005");
        }

        public void SetField<T>(string fieldName, T value)
        {
            int field_divider_idx = fieldName.IndexOf('.');

            if (-1 != field_divider_idx)
            {
                string field_name_string = fieldName.Substring(0, field_divider_idx);
                switch (FieldLookupTable.Lookup(field_name_string))
                {
                    case -1:
                        Throw.undefined_field();
                        break;
                    /*FOREACH*/
                    /*USE_LIST("t_field")*/
                    /*  IF("$t_field_type->is_struct()")*/
                    case t_int:
                        GenericFieldAccessor.SetField(this.t_field_name, fieldName, field_divider_idx + 1, value);
                        break;
                    /*  END*/
                    /*END*/
                    default:
                        // @note   We cannot go further unless it's a struct.
                        //         Throw exception now.
                        Throw.member_access_on_non_struct__field(field_name_string);
                        break;
                }
                return;
            }

            switch (FieldLookupTable.Lookup(fieldName))
            {
                case -1:
                    Throw.undefined_field();
                    break;
                /*FOREACH*/
                /*USE_LIST("t_field")*/
                case t_int:
                    {
                        t_field_type conversion_result = TypeConverter<T>.ConvertTo_t_field_type_display(value);
                        MODULE_CALL("ValueToAccessorFieldAssignment", "$t_field", "\"this\"", "\"conversion_result\"");
                    }
                    break;
                    /*END*/
            }
        }


        /// <summary>
        /// Tells if a field with the given name exists in the current cell.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        /// <returns>The existence of the field.</returns>
        public bool ContainsField(string fieldName)
        {
            switch (FieldLookupTable.Lookup(fieldName))
            {
                /*FOREACH*/
                /*USE_LIST("t_field")*/
                case t_int:
                    IF("$t_field_type->is_optional()");
                    return this.Contains_t_field_name;
                    ELSE();
                    return true;
                    END();
                /*END*/
                default:
                    return false;
            }
        }


        //TODO incomplete support: cannot append to a field of a field. GenericFieldAccessor does not have AppendToField yet.
        public void AppendToField<T>(string fieldName, T value)
        {
            if (AppendToFieldRerouteSet.Contains(fieldName))
            {
                SetField(fieldName, value);
                return;
            }

            switch (FieldLookupTable.Lookup(fieldName))
            {
                case -1:
                    Throw.undefined_field();
                    break;
                /*FOREACH*/
                /*USE_LIST("t_field")*/
                /*IF("$t_field_type->is_string() || $t_field_type->is_list()")*/
                case t_int:
                    IF("$t_field_type->is_string()");
                    {
                        IF("$t_field_type->is_optional()");
                        if (!this.Contains_t_field_name)
                            this.t_field_name = "";
                        END();

                        this.t_field_name += TypeConverter<T>.ConvertTo_string(value);
                    }
                    ELIF("$t_field_type->is_list()");
                    {
                        IF("$t_field_type->is_optional()");
                        if (!this.Contains_t_field_name)
                            this.t_field_name = new t_field_type();
                        END();

                        switch (TypeConverter<T>.GetConversionActionTo_t_field_type_display())
                        {
                            case TypeConversionAction.TC_ASSIGN:
                                foreach (var element in value as t_field_type)
                                    this.t_field_name.Add(element);
                                break;
                            case TypeConversionAction.TC_CONVERTLIST:
                            case TypeConversionAction.TC_ARRAYTOLIST:
                                foreach (var element in TypeConverter<T>.Enumerate_t_field_type_list_element_type_display(value))
                                    this.t_field_name.Add(element);
                                break;
                            case TypeConversionAction.TC_WRAPINLIST:
                            case TypeConversionAction.TC_PARSESTRING:
                                this.t_field_name.Add(TypeConverter<T>.ConvertTo_t_field_type_list_element_type_display(value));
                                break;
                            default:
                                Throw.data_type_incompatible_with_list(typeof(T).ToString());
                                break;
                        }
                    }
                    END();
                    break;
                /*END*/
                /*END*/
                default:
                    Throw.target__field_not_list();
                    break;
            }
        }

        long ICell.CellID { get { return CellID.Value; } set { CellID = value; } }


        IEnumerable<KeyValuePair<string, T>> ICell.SelectFields<T>(string attributeKey, string attributeValue)
        {
            switch (TypeConverter<T>.type_id)
            {
                // Only enable the case if there are any compatible fields
                /*FOREACH*/
                /*  USE_LIST("t_data_type")*/
                /*  META_VAR("bool", "compatible", "false"*/
                /*  FOREACH()*/
                /*    USE_LIST("t_field")*/
                /*    IF("$t_data_type->is_convertible_from($t_field_type)")*/
                /*      META("%compatible = true;")*/
                /*    END()*/
                /*  END()*/
                /*  IF("%compatible")*/
                case t_uint:
                    FOREACH();
                    USE_LIST("t_field");
                    IF("$t_data_type->is_convertible_from($t_field_type)");
                    if (StorageSchema.t_cell_name_descriptor.check_attribute(StorageSchema.t_cell_name_descriptor.t_field_name, attributeKey, attributeValue))
                        /*   IF("$t_field->is_optional()")*/
                        if (Contains_t_field_name)
                            /*END()*/
                            yield return new KeyValuePair<string, T>("t_field_name", TypeConverter<T>.ConvertFrom_t_field_type_display(this.t_field_name));
                    END();
                    END();
                    break;
                /*  END()*/
                /*END()*/
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
        }

        #region enumerate value methods
        [FOREACH]
        [USE_LIST("t_field")]
        private IEnumerable<T> _enumerate_from_t_field_name<T>()
        {
            MODULE_CALL("EnumerateFromFieldModule", "$t_field", /*for accessor:*/"true");
            MUTE();
            yield break;
            MUTE_END();
        }
        [END]

        private static StringLookupTable s_field_attribute_id_table = new StringLookupTable(
            /*META_VAR("int", "iter_val", "0")*/
            /*META("for(const std::string& attr : %field_attributes){")*/
            "/*META_OUTPUT(attr)*/"
            /*META("++%iter_val;")*/
            /*IF("%iter_val < %field_attributes.size()")*/
            /*LITERAL_OUTPUT(",")*/
            /*END*/
            /*META("}")*/
            );
        #endregion

        public IEnumerable<T> EnumerateField<T>(string fieldName)
        {
            switch (FieldLookupTable.Lookup(fieldName))
            {
                /*FOREACH*/
                /*USE_LIST("t_field")*/
                case t_int:
                    return _enumerate_from_t_field_name<T>();
                /*END*/
                default:
                    Throw.undefined_field();
                    return null;
            }
        }

        IEnumerable<T> ICell.EnumerateValues<T>(string attributeKey, string attributeValue)
        {
            int attr_id;
            if (attributeKey == null)
            {
                FOREACH();
                foreach (var val in _enumerate_from_t_field_name<T>())
                    yield return val;
                END();
            }
            else if (-1 != (attr_id = s_field_attribute_id_table.Lookup(attributeKey)))
            {
                switch (attr_id)
                {
                    /*META_VAR("int", "iter_val", "0")*/
                    /*META("for(const std::string& attr : %field_attributes){")*/
                    case t_int_2:
                        FOREACH();
                        {
                            USE_LIST("t_field");
                            META_VAR("std::string*", "p_field_attr_value", "$t_field->get_attribute(attr)");
                            IF("%p_field_attr_value != nullptr");
                            {
                                if (attributeValue == null || attributeValue == "/*META_OUTPUT(%p_field_attr_value)*/")
                                {
                                    foreach (var val in _enumerate_from_t_field_name<T>())
                                        yield return val;
                                }
                            }
                            END();
                        }
                        END();
                        break;
                        /*META("}")*/
                }
            }
            yield break;
        }

        IEnumerable<string> ICellDescriptor.GetFieldNames()
        {
            META_VAR("int", "field_cnt", "0");
            FOREACH();
            IF("$t_field->is_optional()");
            {
                // optional reference types
                if (Contains_t_field_name)
                    yield return "t_field_name";
            }
            ELSE();
            {
                // mandatory types
                yield return "t_field_name";
            }
            META("++%field_cnt;");
            END();//IF
            END();//FOREACH
            IF("%field_cnt == 0");
            {
                yield break;
            }
            END();//IF
        }

        IAttributeCollection ICellDescriptor.GetFieldAttributes(string fieldName)
        {
            return StorageSchema.t_cell_name.GetFieldAttributes(fieldName);
        }

        IEnumerable<IFieldDescriptor> ICellDescriptor.GetFieldDescriptors()
        {
            return StorageSchema.t_cell_name.GetFieldDescriptors();
        }

        string ITypeDescriptor.TypeName
        {
            get { return StorageSchema.s_cellTypeName_t_cell_name; }
        }

        Type ITypeDescriptor.Type
        {
            get { return StorageSchema.s_cellType_t_cell_name; }
        }

        bool ITypeDescriptor.IsOfType<T>()
        {
            return typeof(T) == StorageSchema.s_cellType_t_cell_name;
        }

        bool ITypeDescriptor.IsList()
        {
            return false;
        }

        IReadOnlyDictionary<string, string> IAttributeCollection.Attributes
        {
            get { return StorageSchema.t_cell_name.Attributes; }
        }

        string IAttributeCollection.GetAttributeValue(string attributeKey)
        {
            return StorageSchema.t_cell_name.GetAttributeValue(attributeKey);
        }

        ushort ICellDescriptor.CellType
        {
            get
            {
                return (ushort)CellType.t_cell_name;
            }
        }
        #endregion


    }

    ///<summary>
    ///Provides interfaces for accessing t_cell_name cells
    ///on <see cref="Trinity.Storage.LocalMemorySotrage"/>.
    static public class StorageExtension_t_cell_name
    {
        #region LocalStorage non logging
        /// <summary>
        /// Adds a new cell of type t_cell_name to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The value of the cell is specified in the method parameters.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellID">A 64-bit cell Id.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool Savet_cell_name(this Trinity.Storage.LocalMemoryStorage storage, long CellID/*FOREACH*/, t_field_type t_field_name = default(t_field_type)/*END*/)
        {
            __meta.MUTE();
            byte[] tmpcell = null;
            __meta.MUTE_END();
            __meta.MODULE_CALL("SerializeParametersToBuffer", "node", "\"cell\"");
            return storage.SaveCell(CellID, tmpcell, (ushort)CellType.t_cell_name) == TrinityErrorCode.E_SUCCESS;
        }

        /// <summary>
        /// Adds a new cell of type t_cell_name to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The parameter <paramref name="CellID"/> overrides the cell id in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellID">A 64-bit cell Id.</param>
        /// <param name="CellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool Savet_cell_name(this Trinity.Storage.LocalMemoryStorage storage, long CellID, t_cell_name CellContent)
        {
            return Savet_cell_name(storage, CellID /*FOREACH*/ , CellContent.t_field_name /*END*/);
        }

        /// <summary>
        /// Adds a new cell of type t_cell_name to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. Cell Id is specified by the CellID field in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool Savet_cell_name(this Trinity.Storage.LocalMemoryStorage storage, t_cell_name CellContent)
        {
            return Savet_cell_name(storage, CellContent.CellID /*FOREACH*/ , CellContent.t_field_name /*END*/);
        }

        /// <summary>
        /// Allocate a cell accessor on the specified cell, which inteprets
        /// the cell as a t_cell_name. Any changes done to the accessor
        /// are written to the storage immediately.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock. Otherwise this method is wait-free.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellId">The id of the specified cell.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>A <see cref="t_Namespace.t_cell_name"/> instance.</returns>
        public unsafe static t_cell_name_Accessor Uset_cell_name(this Trinity.Storage.LocalMemoryStorage storage, long CellID, CellAccessOptions options)
        {
            //TODO
            //if (TSLCompiler.CompileWithDebugFeatures)
            //{
            //    cw += @"
            //if (storage.GetCellType(CellID) != (ushort)CellType.t_cell_name)
            //    throw new CellTypeNotMatchException(" + "\"Cell Type doesn't match, cell id is \" + " + @"CellID);";
            //}
            return t_cell_name_Accessor.New(CellID, options);
        }

        /// <summary>
        /// Allocate a cell accessor on the specified cell, which inteprets
        /// the cell as a t_cell_name. Any changes done to the accessor
        /// are written to the storage immediately.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellId">The id of the specified cell.</param>
        /// <returns>A <see cref="" + script.RootNamespace + ".t_cell_name"/> instance.</returns>
        public unsafe static t_cell_name_Accessor Uset_cell_name(this Trinity.Storage.LocalMemoryStorage storage, long CellID)
        {
            //if (TSLCompiler.CompileWithDebugFeatures)
            //{
            //    cw += @"
            //if (storage.GetCellType(CellID) != (ushort)CellType.t_cell_name)
            //    throw new CellTypeNotMatchException(" + "\"Cell Type doesn't match, cell id is \" + " + @"CellID);";
            //}
            return t_cell_name_Accessor.New(CellID, CellAccessOptions.ThrowExceptionOnCellNotFound);
        }

        /// <summary>
        /// Loads the content of the specified cell. Any changes done to this object are not written to the store, unless
        /// the content object is saved back into the storage.
        /// </summary>
        public unsafe static t_cell_name Loadt_cell_name(this Trinity.Storage.LocalMemoryStorage storage, long CellID)
        {
            using (var cell = new t_cell_name_Accessor(CellID, CellAccessOptions.ThrowExceptionOnCellNotFound))
            {
                t_cell_name ret = cell;
                ret.CellID = CellID;
                return ret;
            }
        }

        #endregion

        #region LocalStorage logging
        /// <summary>
        /// Adds a new cell of type t_cell_name to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The value of the cell is specified in the method parameters.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellID">A 64-bit cell Id.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool Savet_cell_name(this Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions options, long CellID/*FOREACH*/, t_field_type t_field_name = default(t_field_type)/*END*/)
        {
            __meta.MUTE();
            byte[] tmpcell = null;
            __meta.MUTE_END();
            __meta.MODULE_CALL("SerializeParametersToBuffer", "node", "\"cell\"");
            return storage.SaveCell(options, CellID, tmpcell, (ushort)CellType.t_cell_name) == TrinityErrorCode.E_SUCCESS;
        }
        /// <summary>
        /// Adds a new cell of type t_cell_name to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The parameter <paramref name="CellID"/> overrides the cell id in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellID">A 64-bit cell Id.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="CellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool Savet_cell_name(this Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions options, long CellID, t_cell_name CellContent)
        {
            return Savet_cell_name(storage, options, CellID /*FOREACH*/ , CellContent.t_field_name /*END*/);
        }

        /// <summary>
        /// Adds a new cell of type t_cell_name to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. Cell Id is specified by the CellID field in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="CellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool Savet_cell_name(this Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions options, t_cell_name CellContent)
        {
            return Savet_cell_name(storage, options, CellContent.CellID /*FOREACH*/ , CellContent.t_field_name /*END*/);
        }
        #endregion

        #region CloudStorage
        /// <summary>
        /// Adds a new cell of type t_cell_name to the cloud storage if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The value of the cell is specified in the method parameters. 
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.MemoryCloud"/> instance.</param>
        /// <param name="CellID">A 64-bit cell Id.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool Savet_cell_name(this Trinity.Storage.MemoryCloud storage, long CellID/*FOREACH*/, t_field_type t_field_name = default(t_field_type)/*END*/)
        {
            __meta.MUTE();
            byte[] tmpcell = null;
            __meta.MUTE_END();
            __meta.MODULE_CALL("SerializeParametersToBuffer", "node", "\"cell\"");
            return storage.SaveCell(CellID, tmpcell, (ushort)CellType.t_cell_name) == TrinityErrorCode.E_SUCCESS;
        }
        /// <summary>
        /// Adds a new cell of type t_cell_name to the cloud storage if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The parameter <paramref name="CellID"/> overrides the cell id in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.MemoryCloud"/> instance.</param>
        /// <param name="CellID">A 64-bit cell Id.</param>
        /// <param name="CellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool Savet_cell_name(this Trinity.Storage.MemoryCloud storage, long CellID, t_cell_name CellContent)
        {
            return Savet_cell_name(storage, CellID /*FOREACH*/ , CellContent.t_field_name /*END*/);
        }
        /// <summary>
        /// Adds a new cell of type t_cell_name to the cloud storage if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. Cell Id is specified by the CellID field in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.MemoryCloud"/> instance.</param>
        /// <param name="CellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool Savet_cell_name(this Trinity.Storage.MemoryCloud storage, t_cell_name CellContent)
        {
            return Savet_cell_name(storage, CellContent.CellID /*FOREACH*/ , CellContent.t_field_name /*END*/);
        }
        /// <summary>
        /// Loads the content of the specified cell. Any changes done to this object are not written to the store, unless
        /// the content object is saved back into the storage.
        /// </summary>
        public unsafe static t_cell_name Loadt_cell_name(this Trinity.Storage.MemoryCloud storage, long CellID)
        {
            byte[] cellBuff;
            var eResult = storage.LoadCell(CellID, out cellBuff);
            if (eResult == TrinityErrorCode.E_CELL_NOT_FOUND)
            { throw new CellNotFoundException("Cell with ID=" + CellID + " not found!"); }
            else if (eResult == TrinityErrorCode.E_NETWORK_SEND_FAILURE)
            { throw new IOException("Network error when loading cell with ID=" + CellID); }
            fixed (byte* ptr = cellBuff)
            {
                using (var cell = new t_cell_name_Accessor(ptr))
                {
                    t_cell_name ret = cell;
                    ret.CellID = CellID;
                    return ret;
                }
            }
        }
    }
    #endregion
}

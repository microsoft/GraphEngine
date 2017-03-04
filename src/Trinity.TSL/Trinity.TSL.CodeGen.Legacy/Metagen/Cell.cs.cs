using System.Text;
using System.Collections.Generic;

namespace Trinity.TSL.CodeTemplates
{
    internal partial class SourceFiles
    {
        internal static string 
Cell(
NCell node)
        {
            StringBuilder source = new StringBuilder();
            
source.Append(@"
namespace ");
source.Append(Codegen.GetString(Trinity::Codegen::GetNamespace()));
source.Append(@"
{
    ");
std::unordered_set<std::string> field_attributes_1;
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
for (int iterator_2 = 0; iterator_2 < ((node->fieldList)[iterator_1].attributes).Count;++iterator_2)
{
source.Append(@"
    /*    ");
field_attributes_1.insert(*((node->fieldList)[iterator_1].attributes)[iterator_2].key);
source.Append(@"*/
    ");
}
}
source.Append(@"
    /// <summary>
    /// A .NET runtime object representation of ");
source.Append(Codegen.GetString(node->name));
source.Append(@" defined in TSL.
    /// </summary>
    public partial struct ");
source.Append(Codegen.GetString(node->name));
source.Append(@" : ICell
    {
        #region MUTE
        
        #endregion
        #region Text processing
        /// <summary>
        /// Converts the string representation of a ");
source.Append(Codegen.GetString(node->name));
source.Append(@" to its
        /// struct equivalent. A return value indicates whether the 
        /// operation succeeded.
        /// </summary>
        /// <param name=""input>A string to convert.</param>
        /// <param name=""value"">
        /// When this method returns, contains the struct equivalent of the value contained 
        /// in input, if the conversion succeeded, or default(");
source.Append(Codegen.GetString(node->name));
source.Append(@") if the conversion
        /// failed. The conversion fails if the input parameter is null or String.Empty, or is 
        /// not of the correct format. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// True if input was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryParse(string input, out ");
source.Append(Codegen.GetString(node->name));
source.Append(@" value)
        {
            try
            {
                value = Newtonsoft.Json.JsonConvert.DeserializeObject<");
source.Append(Codegen.GetString(node->name));
source.Append(@">(input);
                return true;
            }
            catch { value = default(");
source.Append(Codegen.GetString(node->name));
source.Append(@"); return false; }
        }
        public static ");
source.Append(Codegen.GetString(node->name));
source.Append(@" Parse(string input)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<");
source.Append(Codegen.GetString(node->name));
source.Append(@">(input);
        }
        ///<summary>Converts a ");
source.Append(Codegen.GetString(node->name));
source.Append(@" to its string representation, in JSON format.</summary>
        ///<returns>A string representation of the ");
source.Append(Codegen.GetString(node->name));
source.Append(@".</returns>
        public override string ToString()
        {
            return Serializer.ToString(this);
        }
        #endregion
        internal static StringLookupTable FieldLookupTable = new StringLookupTable(
            ");
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
source.Append(@"
            """);
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@"""
            ");
if (iterator_1 < (node->fieldList).Count - 1)
source.Append(",");
}
source.Append(@"
            );
        internal static HashSet<string> AppendToFieldRerouteSet = new HashSet<string>()
        {
            ");
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
if ((node->fieldList)[iterator_1].fieldType->is_value_type())
{
source.Append(@"
            """);
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@"""
            ,");
}
}
source.Append(@"
        };
        #region ICell implementation
        /// <summary>
        /// Get the field of the specified name in the cell.
        /// </summary>
        /// <typeparam name=""T"">
        /// The desired type that the field is supposed 
        /// to be intepreted as. Automatic type casting 
        /// will be attempted if the desired type is not 
        /// implicitly convertible from the type of the field.
        /// </typeparam>
        /// <param name=""fieldName"">The name of the target field.</param>
        /// <returns>The value of the field.</returns>
        public T GetField<T>(string fieldName)
        {
            switch (FieldLookupTable.Lookup(fieldName))
            {
                case -1:
                    Throw.undefined_field();
                    break;
                ");
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
source.Append(@"
                case ");
source.Append(Codegen.GetString(iterator_1));
source.Append(@":
                    return TypeConverter<T>.ConvertFrom_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((node->fieldList)[iterator_1]->fieldType)));
source.Append(@"(this.");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@");
                ");
}
source.Append(@"
            }
            /* Should not reach here */
            throw new Exception(""Internal error T5005"");
        }
        /// <summary>
        /// Set the value of the target field.
        /// </summary>
        /// <typeparam name=""T"">
        /// The type of the value.
        /// </typeparam>
        /// <param name=""fieldName"">The name of the target field.</param>
        /// <param name=""value"">
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
                ");
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
source.Append(@"
                case ");
source.Append(Codegen.GetString(iterator_1));
source.Append(@":
                    this.");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@" = TypeConverter<T>.ConvertTo_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((node->fieldList)[iterator_1]->fieldType)));
source.Append(@"(value);
                    break;
                ");
}
source.Append(@"
                default:
                    Throw.data_type_incompatible_with_field(typeof(T).ToString());
                    break;
            }
        }
        /// <summary>
        /// Tells if a field with the given name exists in the current cell.
        /// </summary>
        /// <param name=""fieldName"">The name of the field.</param>
        /// <returns>The existence of the field.</returns>
        public bool ContainsField(string fieldName)
        {
            switch (FieldLookupTable.Lookup(fieldName))
            {
                ");
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
source.Append(@"
                case ");
source.Append(Codegen.GetString(iterator_1));
source.Append(@":
                    ");
if ((node->fieldList)[iterator_1].fieldType->is_optional())
{
source.Append(@"
                    return this.");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@" != null;
                    ");
}
else
{
source.Append(@"
                    return true;
                    ");
}
}
source.Append(@"
                default:
                    return false;
            }
        }
        /// <summary>
        /// Append <paramref name=""value""/> to the target field. Note that if the target field
        /// is not appendable(string or list), calling this method is equivalent to <see cref=""");
source.Append(Codegen.GetString(Trinity::Codegen::GetNamespace()));
source.Append(@".GenericCellAccessor.SetField(string, T)""/>.
        /// </summary>
        /// <typeparam name=""T"">
        /// The type of the value.
        /// </typeparam>
        /// <param name=""fieldName"">The name of the target field.</param>
        /// <param name=""value"">The value to be appended. 
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
                ");
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
if ((node->fieldList)[iterator_1].fieldType->is_string() || (node->fieldList)[iterator_1].fieldType->is_list())
{
source.Append(@"
                case ");
source.Append(Codegen.GetString(iterator_1));
source.Append(@":
                    ");
if ((node->fieldList)[iterator_1].fieldType->is_string())
{
source.Append(@"
                    {
                        if (this.");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@" == null)
                            this.");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@" = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@" += TypeConverter<T>.ConvertTo_string(value);
                    }
                    ");
}
else if ((node->fieldList)[iterator_1].fieldType->is_list())
{
source.Append(@"
                    {
                        if (this.");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@" == null)
                            this.");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@" = new ");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].fieldType));
source.Append(@"();
                        switch (TypeConverter<T>.GetConversionActionTo_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((node->fieldList)[iterator_1]->fieldType)));
source.Append(@"())
                        {
                            case TypeConversionAction.TC_ASSIGN:
                                foreach (var element in value as ");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].fieldType));
source.Append(@")
                                    this.");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@".Add(element);
                                break;
                            case TypeConversionAction.TC_CONVERTLIST:
                            case TypeConversionAction.TC_ARRAYTOLIST:
                                foreach (var element in TypeConverter<T>.Enumerate_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((node->fieldList)[iterator_1]->fieldType->listElementType)));
source.Append(@"(value))
                                    this.");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@".Add(element);
                                break;
                            case TypeConversionAction.TC_WRAPINLIST:
                            case TypeConversionAction.TC_PARSESTRING:
                                this.");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@".Add(TypeConverter<T>.ConvertTo_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((node->fieldList)[iterator_1]->fieldType->listElementType)));
source.Append(@"(value));
                                break;
                            default:
                                Throw.data_type_incompatible_with_list(typeof(T).ToString());
                                break;
                        }
                    }
                    ");
}
source.Append(@"
                    break;
                ");
}
}
source.Append(@"
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
                ");
for (int iterator_1 = 0; iterator_1 < (Trinity::Codegen::TSLExternalParserDataTypeVector).Count;++iterator_1)
{
bool compatible_1 = false;
for (int iterator_2 = 0; iterator_2 < (node->fieldList).Count;++iterator_2)
{
if ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->is_convertible_from((node->fieldList)[iterator_2].fieldType))
{
compatible_1 = true;
}
}
if (compatible_1)
{
source.Append(@"
                case  ");
source.Append(Codegen.GetString(iterator_1));
source.Append(@":
                    ");
for (int iterator_2 = 0; iterator_2 < (node->fieldList).Count;++iterator_2)
{
if ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->is_convertible_from((node->fieldList)[iterator_2].fieldType))
{
source.Append(@"
                    if (StorageSchema.");
source.Append(Codegen.GetString(node->name));
source.Append(@"_descriptor.check_attribute(StorageSchema.");
source.Append(Codegen.GetString(node->name));
source.Append(@"_descriptor.");
source.Append(Codegen.GetString((node->fieldList)[iterator_2].name));
source.Append(@", attributeKey, attributeValue))
                        ");
if ((node->fieldList)[iterator_2]->is_optional())
{
source.Append(@"
                        if (this.");
source.Append(Codegen.GetString((node->fieldList)[iterator_2].name));
source.Append(@" != null)
                            ");
}
source.Append(@"
                            yield return new KeyValuePair<string, T>(""");
source.Append(Codegen.GetString((node->fieldList)[iterator_2].name));
source.Append(@""", TypeConverter<T>.ConvertFrom_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((node->fieldList)[iterator_2]->fieldType)));
source.Append(@"(this.");
source.Append(Codegen.GetString((node->fieldList)[iterator_2].name));
source.Append(@"));
                    ");
}
}
source.Append(@"
                    break;
                ");
}
}
source.Append(@"
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
        }
        #region enumerate value constructs
        ");
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
source.Append(@"
        private IEnumerable<T> _enumerate_from_");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@"<T>()
        {
            ");

{
    ModuleContext module_ctx = new ModuleContext();
    module_ctx.m_stack_depth = 0;
module_ctx.m_arguments.Add(Codegen.GetString(/*for accessor:*/"false"));
string module_content = Modules.EnumerateFromFieldModule((node->fieldList)[iterator_1], module_ctx);
    source.Append(module_content);
}
source.Append(@"
        }
        ");
}
source.Append(@"
        private static StringLookupTable s_field_attribute_id_table = new StringLookupTable(
            ");
int iter_val_1 = 0;
for(const std::string& attr : field_attributes_1){
source.Append(@"
            """);
source.Append(Codegen.GetString(attr));
source.Append(@"""
            ");
++iter_val_1;
if (iter_val_1 < field_attributes_1.size())
{
source.Append(@",");
}
}
source.Append(@"
            );
        #endregion
        public IEnumerable<T> EnumerateField<T>(string fieldName)
        {
            switch (FieldLookupTable.Lookup(fieldName))
            {
                ");
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
source.Append(@"
                case ");
source.Append(Codegen.GetString(iterator_1));
source.Append(@":
                    return _enumerate_from_");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@"<T>();
                ");
}
source.Append(@"
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
                ");
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
source.Append(@"
                foreach (var val in _enumerate_from_");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@"<T>())
                    yield return val;
                ");
}
source.Append(@"
            }
            else if (-1 != (attr_id = s_field_attribute_id_table.Lookup(attributeKey)))
            {
                switch (attr_id)
                {
                    ");
int iter_val_2 = 0;
for(const std::string& attr : field_attributes_1){
source.Append(@"
                    case  ");
source.Append(Codegen.GetString(iter_val_2++));
source.Append(@":
                        ");
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
source.Append(@"
                        {
                            ");
std::string* p_field_attr_value_1 = (node->fieldList)[iterator_1]->get_attribute(attr);
if (p_field_attr_value_1 != nullptr)
{
source.Append(@"
                            {
                                if (attributeValue == null || attributeValue == """);
source.Append(Codegen.GetString(p_field_attr_value_1));
source.Append(@""")
                                {
                                    foreach (var val in _enumerate_from_");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@"<T>())
                                        yield return val;
                                }
                            }
                            ");
}
source.Append(@"
                        }
                        ");
}
source.Append(@"
                        break;
                    ");
}
source.Append(@"
                }
            }
            yield break;
        }
        #endregion
        #region Other interfaces
        string ITypeDescriptor.TypeName
        {
            get { return StorageSchema.s_cellTypeName_");
source.Append(Codegen.GetString(node->name));
source.Append(@"; }
        }
        Type ITypeDescriptor.Type
        {
            get { return StorageSchema.s_cellType_");
source.Append(Codegen.GetString(node->name));
source.Append(@"; }
        }
        bool ITypeDescriptor.IsOfType<T>()
        {
            return typeof(T) == StorageSchema.s_cellType_");
source.Append(Codegen.GetString(node->name));
source.Append(@";
        }
        bool ITypeDescriptor.IsList()
        {
            return false;
        }
        IEnumerable<IFieldDescriptor> ICellDescriptor.GetFieldDescriptors()
        {
            return StorageSchema.");
source.Append(Codegen.GetString(node->name));
source.Append(@".GetFieldDescriptors();
        }
        IAttributeCollection ICellDescriptor.GetFieldAttributes(string fieldName)
        {
            return StorageSchema.");
source.Append(Codegen.GetString(node->name));
source.Append(@".GetFieldAttributes(fieldName);
        }
        string IAttributeCollection.GetAttributeValue(string attributeKey)
        {
            return StorageSchema.");
source.Append(Codegen.GetString(node->name));
source.Append(@".GetAttributeValue(attributeKey);
        }
        IReadOnlyDictionary<string, string> IAttributeCollection.Attributes
        {
            get { return StorageSchema.");
source.Append(Codegen.GetString(node->name));
source.Append(@".Attributes; }
        }
        IEnumerable<string> ICellDescriptor.GetFieldNames()
        {
            ");
int field_cnt_1 = 0;
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
if ((node->fieldList)[iterator_1]->is_optional())
{
source.Append(@"
            {
                if (this.");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@" != null)
                    yield return """);
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@""";
            }
            ");
}
else
{
source.Append(@"
            {
                yield return """);
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@""";
            }
            ");
++field_cnt_1;
}
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
        ushort ICellDescriptor.CellType
        {
            get
            {
                return (ushort)CellType.");
source.Append(Codegen.GetString(node->name));
source.Append(@";
            }
        }
        #endregion
    }
    /// <summary>
    /// Provides in-place operations of ");
source.Append(Codegen.GetString(node->name));
source.Append(@" defined in TSL.
    /// </summary>
    public unsafe partial class ");
source.Append(Codegen.GetString(node->name));
source.Append(@"_Accessor : ICellAccessor
    {
        #region MUTE
        
        #endregion
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
        internal    int         	  		CellEntryIndex;
        internal    bool        	  		m_IsIterator   = false;
        internal    CellAccessOpt");
source.Append(@"ions 		m_options      = 0;
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
                     ");
source.Append(@"   else if ((options & CellAccessOptions.CreateNewOnCellNotFound) != 0)
                        {
                            byte[]  defaultContent    = construct(cellId);
                            int     size              = defaultContent.Length;
                            eResult                   = Global.LocalStorage.AddOrUse(cellId, defaultContent, ref size, (ushort)CellType.");
source.Append(Codegen.GetString(node->name));
source.Append(@", out cellPtr, out cellEntryIndex);
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
                        if (cellType != (ushort)CellType.");
source.Append(Codegen.GetString(node->name));
source.Append(@")
                        {
                            Global.LocalStorage.ReleaseCellLock(cellId, cellEntryIndex);
                            Throw.wrong_cell_type();
                        }
                        break;
                    }
                default:
                    throw new NotImplementedException();
            }
            this.CellID         = cellId;
            this.CellPtr        = cellPtr;
            this.CellEntryIndex = cellEntryIndex;
            this.m_options      = options;
        }
        [ThreadStatic]
        internal static ");
source.Append(Codegen.GetString(node->name));
source.Append(@"_Accessor s_accessor = null;
        internal static ");
source.Append(Codegen.GetString(node->name));
source.Append(@"_Accessor New(long CellID, CellAccessOptions options)
        {
            ");
source.Append(Codegen.GetString(node->name));
source.Append(@"_Accessor ret = null;
            if (s_accessor != (");
source.Append(Codegen.GetString(node->name));
source.Append(@"_Accessor)null)
            {
                ret = s_accessor;
                ret.Initialize(CellID, options);
                s_accessor = null;
            }
            else
            {
                ret = new ");
source.Append(Codegen.GetString(node->name));
source.Append(@"_Accessor(CellID, options);
            }
            if (ret.CellPtr == null)
            {
                s_accessor = ret;
                ret        = null;
            }
            return ret;
        }
        internal static ");
source.Append(Codegen.GetString(node->name));
source.Append(@"_Accessor New(long CellId, byte* cellPtr, int entryIndex, CellAccessOptions options)
        {
            ");
source.Append(Codegen.GetString(node->name));
source.Append(@"_Accessor ret = null;
            if (s_accessor != (");
source.Append(Codegen.GetString(node->name));
source.Append(@"_Accessor)null)
            {
                ret = s_accessor;
                s_accessor = null;
                ret.CellPtr = cellPtr;
            }
            else
            {
                ret = new ");
source.Append(Codegen.GetString(node->name));
source.Append(@"_Accessor(cellPtr);
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
        internal static ");
source.Append(Codegen.GetString(node->name));
source.Append(@"_Accessor AllocIterativeAccessor(CellInfo info)
        {
            ");
source.Append(Codegen.GetString(node->name));
source.Append(@"_Accessor ret = null;
            if (s_accessor != (");
source.Append(Codegen.GetString(node->name));
source.Append(@"_Accessor)null)
            {
                ret                = s_accessor;
                ret.CellPtr        = info.CellPtr;
                s_accessor         = null;
            }
            else
            {
                ret                = new ");
source.Append(Codegen.GetString(node->name));
source.Append(@"_Accessor(info.CellPtr);
            }
            ret.CellEntryIndex = info.CellEntryIndex;
            ret.CellID         = info.CellId;
            ret.m_IsIterator   = true;
            return ret;
        }
        #endregion
        #region Public
        /// <summary>
        /// Dispose the accessor.
        /// If <c><see cref=""Trinity.TrinityConfig.ReadOnly""/> == false</c>,
        /// the cell lock will be released.
        /// If write-ahead-log behavior is specified on <see cref=""");
source.Append(Codegen.GetString(Trinity::Codegen::GetNamespace()));
source.Append(@".StorageExtension_");
source.Append(Codegen.GetString(node->name));
source.Append(@".Use");
source.Append(Codegen.GetString(node->name));
source.Append(@"""/>,
        /// the changes will be committed to the write-ahead log.
        /// </summary>
        public void Dispose()
        {
            if (CellEntryIndex >= 0)
            {
                if ((m_options & c_WALFlags) != 0)
                {
                    LocalMemoryStorage.CWriteAheadLog(this.CellID.Value, this.CellPtr, this.CellSize, (ushort)CellType.");
source.Append(Codegen.GetString(node->name));
source.Append(@", m_options);
                }
                if (!m_IsIterator)
                {
                    Global.LocalStorage.ReleaseCellLock(CellID.Value, CellEntryIndex);
                }
                if (s_accessor == (");
source.Append(Codegen.GetString(node->name));
source.Append(@"_Accessor)null)
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
        /// <summary>Converts a ");
source.Append(Codegen.GetString(node->name));
source.Append(@"_Accessor to its string representation, in JSON format.</summary>
        /// <returns>A string representation of the ");
source.Append(Codegen.GetString(node->name));
source.Append(@".</returns>
        public override string ToString()
        {
            return Serializer.ToString(this);
        }
        #endregion
        internal static StringLookupTable FieldLookupTable = new StringLookupTable(
            ");
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
source.Append(@"
            """);
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@"""
            ");
if (iterator_1 < (node->fieldList).Count - 1)
source.Append(",");
}
source.Append(@"
            );
        static HashSet<string> AppendToFieldRerouteSet = new HashSet<string>()
        {
            ");
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
if ((node->fieldList)[iterator_1].fieldType->is_value_type())
{
source.Append(@"
            """);
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@"""
            ,");
}
}
source.Append(@"
        };
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
                    ");
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
if ((node->fieldList)[iterator_1].fieldType->is_struct())
{
source.Append(@"
                    case ");
source.Append(Codegen.GetString(iterator_1));
source.Append(@":
                        return GenericFieldAccessor.GetField<T>(this.");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@", fieldName, field_divider_idx + 1);
                    ");
}
}
source.Append(@"
                    default:
                        Throw.member_access_on_non_struct__field(field_name_string);
                        break;
                }
            }
            switch (FieldLookupTable.Lookup(fieldName))
            {
                case -1:
                    Throw.undefined_field();
                    break;
                ");
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
source.Append(@"
                case ");
source.Append(Codegen.GetString(iterator_1));
source.Append(@":
                    return TypeConverter<T>.ConvertFrom_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((node->fieldList)[iterator_1]->fieldType)));
source.Append(@"(this.");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@");
                ");
}
source.Append(@"
            }
            /* Should not reach here */
            throw new Exception(""Internal error T5005"");
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
                    ");
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
if ((node->fieldList)[iterator_1].fieldType->is_struct())
{
source.Append(@"
                    case ");
source.Append(Codegen.GetString(iterator_1));
source.Append(@":
                        GenericFieldAccessor.SetField(this.");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@", fieldName, field_divider_idx + 1, value);
                        break;
                    ");
}
}
source.Append(@"
                    default:
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
                ");
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
source.Append(@"
                case ");
source.Append(Codegen.GetString(iterator_1));
source.Append(@":
                    {
                        ");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].fieldType));
source.Append(@" conversion_result = TypeConverter<T>.ConvertTo_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((node->fieldList)[iterator_1]->fieldType)));
source.Append(@"(value);
                        ");

{
    ModuleContext module_ctx = new ModuleContext();
    module_ctx.m_stack_depth = 0;
module_ctx.m_arguments.Add(Codegen.GetString("this"));
module_ctx.m_arguments.Add(Codegen.GetString("conversion_result"));
string module_content = Modules.AccessorFieldAssignment((node->fieldList)[iterator_1], module_ctx);
    source.Append(module_content);
}
source.Append(@"
                    }
                    break;
                ");
}
source.Append(@"
            }
        }
        /// <summary>
        /// Tells if a field with the given name exists in the current cell.
        /// </summary>
        /// <param name=""fieldName"">The name of the field.</param>
        /// <returns>The existence of the field.</returns>
        public bool ContainsField(string fieldName)
        {
            switch (FieldLookupTable.Lookup(fieldName))
            {
                ");
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
source.Append(@"
                case ");
source.Append(Codegen.GetString(iterator_1));
source.Append(@":
                    ");
if ((node->fieldList)[iterator_1].fieldType->is_optional())
{
source.Append(@"
                    return this.Contains_");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@";
                    ");
}
else
{
source.Append(@"
                    return true;
                    ");
}
}
source.Append(@"
                default:
                    return false;
            }
        }
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
                ");
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
if ((node->fieldList)[iterator_1].fieldType->is_string() || (node->fieldList)[iterator_1].fieldType->is_list())
{
source.Append(@"
                case ");
source.Append(Codegen.GetString(iterator_1));
source.Append(@":
                    ");
if ((node->fieldList)[iterator_1].fieldType->is_string())
{
source.Append(@"
                    {
                        ");
if ((node->fieldList)[iterator_1].fieldType->is_optional())
{
source.Append(@"
                        if (!this.Contains_");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@")
                            this.");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@" = """";
                        ");
}
source.Append(@"
                        this.");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@" += TypeConverter<T>.ConvertTo_string(value);
                    }
                    ");
}
else if ((node->fieldList)[iterator_1].fieldType->is_list())
{
source.Append(@"
                    {
                        ");
if ((node->fieldList)[iterator_1].fieldType->is_optional())
{
source.Append(@"
                        if (!this.Contains_");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@")
                            this.");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@" = new ");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].fieldType));
source.Append(@"();
                        ");
}
source.Append(@"
                        switch (TypeConverter<T>.GetConversionActionTo_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((node->fieldList)[iterator_1]->fieldType)));
source.Append(@"())
                        {
                            case TypeConversionAction.TC_ASSIGN:
                                foreach (var element in value as ");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].fieldType));
source.Append(@")
                                    this.");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@".Add(element);
                                break;
                            case TypeConversionAction.TC_CONVERTLIST:
                            case TypeConversionAction.TC_ARRAYTOLIST:
                                foreach (var element in TypeConverter<T>.Enumerate_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((node->fieldList)[iterator_1]->fieldType->listElementType)));
source.Append(@"(value))
                                    this.");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@".Add(element);
                                break;
                            case TypeConversionAction.TC_WRAPINLIST:
                            case TypeConversionAction.TC_PARSESTRING:
                                this.");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@".Add(TypeConverter<T>.ConvertTo_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((node->fieldList)[iterator_1]->fieldType->listElementType)));
source.Append(@"(value));
                                break;
                            default:
                                Throw.data_type_incompatible_with_list(typeof(T).ToString());
                                break;
                        }
                    }
                    ");
}
source.Append(@"
                    break;
                ");
}
}
source.Append(@"
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
                ");
for (int iterator_1 = 0; iterator_1 < (Trinity::Codegen::TSLExternalParserDataTypeVector).Count;++iterator_1)
{
bool compatible_2 = false;
for (int iterator_2 = 0; iterator_2 < (node->fieldList).Count;++iterator_2)
{
if ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->is_convertible_from((node->fieldList)[iterator_2].fieldType))
{
compatible_2 = true;
}
}
if (compatible_2)
{
source.Append(@"
                case  ");
source.Append(Codegen.GetString(iterator_1));
source.Append(@":
                    ");
for (int iterator_2 = 0; iterator_2 < (node->fieldList).Count;++iterator_2)
{
if ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->is_convertible_from((node->fieldList)[iterator_2].fieldType))
{
source.Append(@"
                    if (StorageSchema.");
source.Append(Codegen.GetString(node->name));
source.Append(@"_descriptor.check_attribute(StorageSchema.");
source.Append(Codegen.GetString(node->name));
source.Append(@"_descriptor.");
source.Append(Codegen.GetString((node->fieldList)[iterator_2].name));
source.Append(@", attributeKey, attributeValue))
                        ");
if ((node->fieldList)[iterator_2]->is_optional())
{
source.Append(@"
                        if (Contains_");
source.Append(Codegen.GetString((node->fieldList)[iterator_2].name));
source.Append(@")
                            ");
}
source.Append(@"
                            yield return new KeyValuePair<string, T>(""");
source.Append(Codegen.GetString((node->fieldList)[iterator_2].name));
source.Append(@""", TypeConverter<T>.ConvertFrom_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((node->fieldList)[iterator_2]->fieldType)));
source.Append(@"(this.");
source.Append(Codegen.GetString((node->fieldList)[iterator_2].name));
source.Append(@"));
                    ");
}
}
source.Append(@"
                    break;
                ");
}
}
source.Append(@"
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
        }
        #region enumerate value methods
        ");
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
source.Append(@"
        private IEnumerable<T> _enumerate_from_");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@"<T>()
        {
            ");

{
    ModuleContext module_ctx = new ModuleContext();
    module_ctx.m_stack_depth = 0;
module_ctx.m_arguments.Add(Codegen.GetString(/*for accessor:*/"true"));
string module_content = Modules.EnumerateFromFieldModule((node->fieldList)[iterator_1], module_ctx);
    source.Append(module_content);
}
source.Append(@"
        }
        ");
}
source.Append(@"
        private static StringLookupTable s_field_attribute_id_table = new StringLookupTable(
            ");
int iter_val_3 = 0;
for(const std::string& attr : field_attributes_1){
source.Append(@"
            """);
source.Append(Codegen.GetString(attr));
source.Append(@"""
            ");
++iter_val_3;
if (iter_val_3 < field_attributes_1.size())
{
source.Append(@",");
}
}
source.Append(@"
            );
        #endregion
        public IEnumerable<T> EnumerateField<T>(string fieldName)
        {
            switch (FieldLookupTable.Lookup(fieldName))
            {
                ");
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
source.Append(@"
                case ");
source.Append(Codegen.GetString(iterator_1));
source.Append(@":
                    return _enumerate_from_");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@"<T>();
                ");
}
source.Append(@"
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
                ");
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
source.Append(@"
                foreach (var val in _enumerate_from_");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@"<T>())
                    yield return val;
                ");
}
source.Append(@"
            }
            else if (-1 != (attr_id = s_field_attribute_id_table.Lookup(attributeKey)))
            {
                switch (attr_id)
                {
                    ");
int iter_val_4 = 0;
for(const std::string& attr : field_attributes_1){
source.Append(@"
                    case  ");
source.Append(Codegen.GetString(iter_val_4++));
source.Append(@":
                        ");
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
source.Append(@"
                        {
                            ");
std::string* p_field_attr_value_2 = (node->fieldList)[iterator_1]->get_attribute(attr);
if (p_field_attr_value_2 != nullptr)
{
source.Append(@"
                            {
                                if (attributeValue == null || attributeValue == """);
source.Append(Codegen.GetString(p_field_attr_value_2));
source.Append(@""")
                                {
                                    foreach (var val in _enumerate_from_");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@"<T>())
                                        yield return val;
                                }
                            }
                            ");
}
source.Append(@"
                        }
                        ");
}
source.Append(@"
                        break;
                    ");
}
source.Append(@"
                }
            }
            yield break;
        }
        IEnumerable<string> ICellDescriptor.GetFieldNames()
        {
            ");
int field_cnt_2 = 0;
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
if ((node->fieldList)[iterator_1]->is_optional())
{
source.Append(@"
            {
                if (Contains_");
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@")
                    yield return """);
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@""";
            }
            ");
}
else
{
source.Append(@"
            {
                yield return """);
source.Append(Codegen.GetString((node->fieldList)[iterator_1].name));
source.Append(@""";
            }
            ");
++field_cnt_2;
}
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
        IAttributeCollection ICellDescriptor.GetFieldAttributes(string fieldName)
        {
            return StorageSchema.");
source.Append(Codegen.GetString(node->name));
source.Append(@".GetFieldAttributes(fieldName);
        }
        IEnumerable<IFieldDescriptor> ICellDescriptor.GetFieldDescriptors()
        {
            return StorageSchema.");
source.Append(Codegen.GetString(node->name));
source.Append(@".GetFieldDescriptors();
        }
        string ITypeDescriptor.TypeName
        {
            get { return StorageSchema.s_cellTypeName_");
source.Append(Codegen.GetString(node->name));
source.Append(@"; }
        }
        Type ITypeDescriptor.Type
        {
            get { return StorageSchema.s_cellType_");
source.Append(Codegen.GetString(node->name));
source.Append(@"; }
        }
        bool ITypeDescriptor.IsOfType<T>()
        {
            return typeof(T) == StorageSchema.s_cellType_");
source.Append(Codegen.GetString(node->name));
source.Append(@";
        }
        bool ITypeDescriptor.IsList()
        {
            return false;
        }
        IReadOnlyDictionary<string, string> IAttributeCollection.Attributes
        {
            get { return StorageSchema.");
source.Append(Codegen.GetString(node->name));
source.Append(@".Attributes; }
        }
        string IAttributeCollection.GetAttributeValue(string attributeKey)
        {
            return StorageSchema.");
source.Append(Codegen.GetString(node->name));
source.Append(@".GetAttributeValue(attributeKey);
        }
        ushort ICellDescriptor.CellType
        {
            get
            {
                return (ushort)CellType.");
source.Append(Codegen.GetString(node->name));
source.Append(@";
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

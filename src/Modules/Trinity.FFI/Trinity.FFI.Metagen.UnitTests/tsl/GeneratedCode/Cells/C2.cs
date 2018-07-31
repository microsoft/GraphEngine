#pragma warning disable 162,168,649,660,661,1522
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
using Trinity.Storage.Transaction;
using Microsoft.Extensions.ObjectPool;
namespace Trinity.Extension
{
    
    /// <summary>
    /// A .NET runtime object representation of C2 defined in TSL.
    /// </summary>
    public partial struct C2 : ICell
    {
        ///<summary>
        ///The id of the cell.
        ///</summary>
        public long CellId;
        ///<summary>
        ///Initializes a new instance of C2 with the specified parameters.
        ///</summary>
        public C2(long cell_id , int s = default(int), List<int> ls = default(List<int>))
        {
            
            this.s = s;
            
            this.ls = ls;
            
            CellId = cell_id;
        }
        
        ///<summary>
        ///Initializes a new instance of C2 with the specified parameters.
        ///</summary>
        public C2( int s = default(int), List<int> ls = default(List<int>))
        {
            
            this.s = s;
            
            this.ls = ls;
            
            CellId = CellIdFactory.NewCellId();
        }
        
        public int s;
        
        public List<int> ls;
        
        public static bool operator ==(C2 a, C2 b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }
            
            return
                
                (a.s == b.s)
                &&
                (a.ls == b.ls)
                
                ;
            
        }
        public static bool operator !=(C2 a, C2 b)
        {
            return !(a == b);
        }
        #region Text processing
        /// <summary>
        /// Converts the string representation of a C2 to its
        /// struct equivalent. A return value indicates whether the 
        /// operation succeeded.
        /// </summary>
        /// <param name="input">A string to convert.</param>
        /// <param name="value">
        /// When this method returns, contains the struct equivalent of the value contained 
        /// in input, if the conversion succeeded, or default(C2) if the conversion
        /// failed. The conversion fails if the input parameter is null or String.Empty, or is 
        /// not of the correct format. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// True if input was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryParse(string input, out C2 value)
        {
            try
            {
                value = Newtonsoft.Json.JsonConvert.DeserializeObject<C2>(input);
                return true;
            }
            catch { value = default(C2); return false; }
        }
        public static C2 Parse(string input)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<C2>(input);
        }
        ///<summary>Converts a C2 to its string representation, in JSON format.</summary>
        ///<returns>A string representation of the C2.</returns>
        public override string ToString()
        {
            return Serializer.ToString(this);
        }
        #endregion
        #region Lookup tables
        internal static StringLookupTable FieldLookupTable = new StringLookupTable(
            
            "s"
            ,
            "ls"
            
            );
        internal static HashSet<string> AppendToFieldRerouteSet = new HashSet<string>()
        {
            
            "s"
            ,
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
                
                case 0:
                return TypeConverter<T>.ConvertFrom_int(this.s);
                
                case 1:
                return TypeConverter<T>.ConvertFrom_List_int(this.ls);
                
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
                
                case 0:
                this.s = TypeConverter<T>.ConvertTo_int(value);
                break;
                
                case 1:
                this.ls = TypeConverter<T>.ConvertTo_List_int(value);
                break;
                
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
                
                case 0:
                
                return true;
                
                case 1:
                
                return true;
                
                default:
                return false;
            }
        }
        /// <summary>
        /// Append <paramref name="value"/> to the target field. Note that if the target field
        /// is not appendable(string or list), calling this method is equivalent to <see cref="Trinity.Extension.GenericCellAccessor.SetField(string, T)"/>.
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
                
                case 1:
                
                {
                    if (this.ls == null)
                        this.ls = new List<int>();
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.ls.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.ls.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.ls.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                default:
                Throw.target__field_not_list();
                break;
            }
        }
        long ICell.CellId { get { return CellId; } set { CellId = value; } }
        public IEnumerable<KeyValuePair<string, T>> SelectFields<T>(string attributeKey, string attributeValue)
        {
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                
                if (StorageSchema.C2_descriptor.check_attribute(StorageSchema.C2_descriptor.s, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("s", TypeConverter<T>.ConvertFrom_int(this.s));
                
                break;
                
                case 1:
                
                if (StorageSchema.C2_descriptor.check_attribute(StorageSchema.C2_descriptor.s, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("s", TypeConverter<T>.ConvertFrom_int(this.s));
                
                if (StorageSchema.C2_descriptor.check_attribute(StorageSchema.C2_descriptor.ls, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("ls", TypeConverter<T>.ConvertFrom_List_int(this.ls));
                
                break;
                
                case 2:
                
                if (StorageSchema.C2_descriptor.check_attribute(StorageSchema.C2_descriptor.s, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("s", TypeConverter<T>.ConvertFrom_int(this.s));
                
                if (StorageSchema.C2_descriptor.check_attribute(StorageSchema.C2_descriptor.ls, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("ls", TypeConverter<T>.ConvertFrom_List_int(this.ls));
                
                break;
                
                case 3:
                
                if (StorageSchema.C2_descriptor.check_attribute(StorageSchema.C2_descriptor.s, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("s", TypeConverter<T>.ConvertFrom_int(this.s));
                
                if (StorageSchema.C2_descriptor.check_attribute(StorageSchema.C2_descriptor.ls, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("ls", TypeConverter<T>.ConvertFrom_List_int(this.ls));
                
                break;
                
                default:
                Throw.incompatible_with_cell();
                break;
            }
            yield break;
        }
        #region enumerate value constructs
        
        private IEnumerable<T> _enumerate_from_s<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_int(this.s);
                        
                    }
                    break;
                
                case 1:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_int(this.s);
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_int(this.s);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_int(this.s);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_ls<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.ls;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 1:
                    {
                        
                        {
                            
                            var element0 = this.ls;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.ls);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.ls);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private static StringLookupTable s_field_attribute_id_table = new StringLookupTable(
            
            );
        #endregion
        public IEnumerable<T> EnumerateField<T>(string fieldName)
        {
            switch (FieldLookupTable.Lookup(fieldName))
            {
                
                case 0:
                return _enumerate_from_s<T>();
                
                case 1:
                return _enumerate_from_ls<T>();
                
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
                
                foreach (var val in _enumerate_from_s<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_ls<T>())
                    yield return val;
                
            }
            else if (-1 != (attr_id = s_field_attribute_id_table.Lookup(attributeKey)))
            {
                switch (attr_id)
                {
                    
                }
            }
            yield break;
        }
        #endregion
        #region Other interfaces
        string ITypeDescriptor.TypeName
        {
            get { return StorageSchema.s_cellTypeName_C2; }
        }
        Type ITypeDescriptor.Type
        {
            get { return StorageSchema.s_cellType_C2; }
        }
        bool ITypeDescriptor.IsOfType<T>()
        {
            return typeof(T) == StorageSchema.s_cellType_C2;
        }
        bool ITypeDescriptor.IsList()
        {
            return false;
        }
        IEnumerable<IFieldDescriptor> ICellDescriptor.GetFieldDescriptors()
        {
            return StorageSchema.C2.GetFieldDescriptors();
        }
        IAttributeCollection ICellDescriptor.GetFieldAttributes(string fieldName)
        {
            return StorageSchema.C2.GetFieldAttributes(fieldName);
        }
        string IAttributeCollection.GetAttributeValue(string attributeKey)
        {
            return StorageSchema.C2.GetAttributeValue(attributeKey);
        }
        IReadOnlyDictionary<string, string> IAttributeCollection.Attributes
        {
            get { return StorageSchema.C2.Attributes; }
        }
        IEnumerable<string> ICellDescriptor.GetFieldNames()
        {
            
            {
                yield return "s";
            }
            
            {
                yield return "ls";
            }
            
        }
        ushort ICellDescriptor.CellType
        {
            get
            {
                return (ushort)CellType.C2;
            }
        }
        #endregion
    }
    /// <summary>
    /// Provides in-place operations of C2 defined in TSL.
    /// </summary>
    public unsafe class C2_Accessor : ICellAccessor
    {
        #region Fields
        internal   long                    m_cellId;
        /// <summary>
        /// A pointer to the underlying raw binary blob. Take caution when accessing data with
        /// the raw pointer, as no boundary checks are employed, and improper operations will cause data corruption and/or system crash.
        /// </summary>
        internal byte*                   m_ptr;
        internal LocalTransactionContext m_tx;
        internal int                     m_cellEntryIndex;
        internal CellAccessOptions       m_options;
        internal bool                    m_IsIterator;
        private  const CellAccessOptions c_WALFlags = CellAccessOptions.StrongLogAhead | CellAccessOptions.WeakLogAhead;
        #endregion
        #region Constructors
        private unsafe C2_Accessor()
        {
                    ls_Accessor_Field = new intListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.m_ptr);
                    this.ResizeFunction(this.m_ptr, ptr_offset + substructure_offset, delta);
                    return this.m_ptr + substructure_offset;
                });
        }
        #endregion
        
        internal static string[] optional_field_names = null;
        ///<summary>
        ///Get an array of the names of all optional fields for object type t_struct_name.
        ///</summary>
        public static string[] GetOptionalFieldNames()
        {
            if (optional_field_names == null)
                optional_field_names = new string[]
                {
                    
                };
            return optional_field_names;
        }
        ///<summary>
        ///Get a list of the names of available optional fields in the object being operated by this accessor.
        ///</summary>
        internal List<string> GetNotNullOptionalFields()
        {
            List<string> list = new List<string>();
            BitArray ba = new BitArray(GetOptionalFieldMap());
            string[] optional_fields = GetOptionalFieldNames();
            for (int i = 0; i < ba.Count; i++)
            {
                if (ba[i])
                    list.Add(optional_fields[i]);
            }
            return list;
        }
        internal unsafe byte[] GetOptionalFieldMap()
        {
            
            return new byte[0];
            
        }
        
        #region IAccessor Implementation
        public byte[] ToByteArray()
        {
            byte* targetPtr = m_ptr;
            {            targetPtr += 4;
targetPtr += *(int*)targetPtr + sizeof(int);}
            int size = (int)(targetPtr - m_ptr);
            byte[] ret = new byte[size];
            Memory.Copy(m_ptr, 0, ret, 0, size);
            return ret;
        }
        public unsafe byte* GetUnderlyingBufferPointer()
        {
            return m_ptr;
        }
        public unsafe int GetBufferLength()
        {
            byte* targetPtr = m_ptr;
            {            targetPtr += 4;
targetPtr += *(int*)targetPtr + sizeof(int);}
            int size = (int)(targetPtr - m_ptr);
            return size;
        }
        public ResizeFunctionDelegate ResizeFunction { get; set; }
        #endregion
        private static byte[] s_default_content = null;
        private static unsafe byte[] construct( int s = default(int) , List<int> ls = default(List<int>) )
        {
            if (s_default_content != null) return s_default_content;
            
            byte* targetPtr;
            
            targetPtr = null;
            
            {
            targetPtr += 4;

if(ls!= null)
{
    targetPtr += ls.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

            }
            byte[] tmpcell = new byte[(int)(targetPtr)];
            fixed (byte* _tmpcellptr = tmpcell)
            {
                targetPtr = _tmpcellptr;
                
            {
            *(int*)targetPtr = s;
            targetPtr += 4;

if(ls!= null)
{
    *(int*)targetPtr = ls.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<ls.Count;++iterator_2)
    {
            *(int*)targetPtr = ls[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

            }
            }
            
            s_default_content = tmpcell;
            return tmpcell;
        }
        
        ///<summary>
        ///Provides in-place access to the object field s.
        ///</summary>
        public unsafe int s
        {
            get
            {
                
                byte* targetPtr = m_ptr;
                {}
                return *(int*)(targetPtr);
                
            }
            set
            {
                
                byte* targetPtr = m_ptr;
                {}                *(int*)targetPtr = value;
            }
        }
        intListAccessor ls_Accessor_Field;
        
        ///<summary>
        ///Provides in-place access to the object field ls.
        ///</summary>
        public unsafe intListAccessor ls
        {
            get
            {
                
                byte* targetPtr = m_ptr;
                {            targetPtr += 4;
}ls_Accessor_Field.m_ptr = targetPtr + 4;
                ls_Accessor_Field.m_cellId = this.m_cellId;
                return ls_Accessor_Field;
                
            }
            set
            {
                
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                ls_Accessor_Field.m_cellId = this.m_cellId;
                
                byte* targetPtr = m_ptr;
                {            targetPtr += 4;
}
                int length = *(int*)(value.m_ptr - 4);
                int oldlength = *(int*)targetPtr;
                if (value.m_cellId != ls_Accessor_Field.m_cellId)
                {
                    //if not in the same Cell
                    ls_Accessor_Field.m_ptr = ls_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.m_ptr - 4, ls_Accessor_Field.m_ptr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.m_ptr - 4, tmpcellptr, length + 4);
                        ls_Accessor_Field.m_ptr = ls_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, ls_Accessor_Field.m_ptr, length + 4);
                    }
                }

            }
        }
        
        public static unsafe implicit operator C2(C2_Accessor accessor)
        {
            
            return new C2(accessor.CellId
            
            ,
            
                    accessor.s
            ,
            
                    accessor.ls
            );
        }
        
        public unsafe static implicit operator C2_Accessor(C2 field)
        {
            byte* targetPtr = null;
            
            {
            targetPtr += 4;

if(field.ls!= null)
{
    targetPtr += field.ls.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

            }
            byte* tmpcellptr = BufferAllocator.AllocBuffer((int)targetPtr);
            Memory.memset(tmpcellptr, 0, (ulong)targetPtr);
            targetPtr = tmpcellptr;
            
            {
            *(int*)targetPtr = field.s;
            targetPtr += 4;

if(field.ls!= null)
{
    *(int*)targetPtr = field.ls.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<field.ls.Count;++iterator_2)
    {
            *(int*)targetPtr = field.ls[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

            }C2_Accessor ret;
            
            ret = C2_Accessor._get()._Setup(field.CellId, tmpcellptr, -1, 0, null);
            ret.m_cellId = field.CellId;
            
            return ret;
        }
        
        public static bool operator ==(C2_Accessor a, C2_Accessor b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;
            if (a.m_ptr == b.m_ptr) return true;
            byte* targetPtr = a.m_ptr;
            {            targetPtr += 4;
targetPtr += *(int*)targetPtr + sizeof(int);}
            int lengthA = (int)(targetPtr - a.m_ptr);
            targetPtr = b.m_ptr;
            {            targetPtr += 4;
targetPtr += *(int*)targetPtr + sizeof(int);}
            int lengthB = (int)(targetPtr - b.m_ptr);
            if(lengthA != lengthB) return false;
            return Memory.Compare(a.m_ptr,b.m_ptr,lengthA);
        }
        public static bool operator != (C2_Accessor a, C2_Accessor b)
        {
            return !(a == b);
        }
        
        public static bool operator ==(C2_Accessor a, C2 b)
        {
            C2_Accessor bb = b;
            return (a == bb);
        }
        public static bool operator !=(C2_Accessor a, C2 b)
        {
            return !(a == b);
        }
        /// <summary>
        /// Get the size of the cell content, in bytes.
        /// </summary>
        public int CellSize { get { int size; Global.LocalStorage.LockedGetCellSize(this.CellId, this.m_cellEntryIndex, out size); return size; } }
        #region Internal
        private unsafe byte* _Resize_NonTx(byte* ptr, int ptr_offset, int delta)
        {
            int offset = (int)(ptr - m_ptr) + ptr_offset;
            m_ptr = Global.LocalStorage.ResizeCell((long)CellId, m_cellEntryIndex, offset, delta);
            return m_ptr + (offset - ptr_offset);
        }
        private unsafe byte* _Resize_Tx(byte* ptr, int ptr_offset, int delta)
        {
            int offset = (int)(ptr - m_ptr) + ptr_offset;
            m_ptr = Global.LocalStorage.ResizeCell(m_tx, (long)CellId, m_cellEntryIndex, offset, delta);
            return m_ptr + (offset - ptr_offset);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe C2_Accessor _Lock(long cellId, CellAccessOptions options)
        {
            ushort cellType;
            this.CellId = cellId;
            this.m_options = options;
            this.ResizeFunction = _Resize_NonTx;
            TrinityErrorCode eResult = Global.LocalStorage.GetLockedCellInfo(cellId, out _, out cellType, out this.m_ptr, out this.m_cellEntryIndex);
            switch (eResult)
            {
                case TrinityErrorCode.E_SUCCESS:
                {
                    if (cellType != (ushort)CellType.C2)
                    {
                        Global.LocalStorage.ReleaseCellLock(cellId, this.m_cellEntryIndex);
                        _put(this);
                        Throw.wrong_cell_type();
                    }
                    break;
                }
                case TrinityErrorCode.E_CELL_NOT_FOUND:
                {
                    if ((options & CellAccessOptions.ThrowExceptionOnCellNotFound) != 0)
                    {
                        _put(this);
                        Throw.cell_not_found(cellId);
                    }
                    else if ((options & CellAccessOptions.CreateNewOnCellNotFound) != 0)
                    {
                        byte[]  defaultContent = construct();
                        int     size           = defaultContent.Length;
                        eResult                = Global.LocalStorage.AddOrUse(cellId, defaultContent, ref size, (ushort)CellType.C2, out this.m_ptr, out this.m_cellEntryIndex);
                        if (eResult == TrinityErrorCode.E_WRONG_CELL_TYPE)
                        {
                            _put(this);
                            Throw.wrong_cell_type();
                        }
                    }
                    else if ((options & CellAccessOptions.ReturnNullOnCellNotFound) != 0)
                    {
                        _put(this);
                        return null;
                    }
                    else
                    {
                        _put(this);
                        Throw.cell_not_found(cellId);
                    }
                    break;
                }
                default:
                _put(this);
                throw new NotImplementedException();
            }
            return this;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe C2_Accessor _Lock(long cellId, CellAccessOptions options, LocalTransactionContext tx)
        {
            ushort cellType;
            this.CellId = cellId;
            this.m_options = options;
            this.m_tx = tx;
            this.ResizeFunction = _Resize_Tx;
            TrinityErrorCode eResult = Global.LocalStorage.GetLockedCellInfo(tx, cellId, out _, out cellType, out this.m_ptr, out this.m_cellEntryIndex);
            switch (eResult)
            {
                case TrinityErrorCode.E_SUCCESS:
                {
                    if (cellType != (ushort)CellType.C2)
                    {
                        Global.LocalStorage.ReleaseCellLock(tx, cellId, this.m_cellEntryIndex);
                        _put(this);
                        Throw.wrong_cell_type();
                    }
                    break;
                }
                case TrinityErrorCode.E_CELL_NOT_FOUND:
                {
                    if ((options & CellAccessOptions.ThrowExceptionOnCellNotFound) != 0)
                    {
                        _put(this);
                        Throw.cell_not_found(cellId);
                    }
                    else if ((options & CellAccessOptions.CreateNewOnCellNotFound) != 0)
                    {
                        byte[]  defaultContent = construct();
                        int     size           = defaultContent.Length;
                        eResult                = Global.LocalStorage.AddOrUse(tx, cellId, defaultContent, ref size, (ushort)CellType.C2, out this.m_ptr, out this.m_cellEntryIndex);
                        if (eResult == TrinityErrorCode.E_WRONG_CELL_TYPE)
                        {
                            _put(this);
                            Throw.wrong_cell_type();
                        }
                    }
                    else if ((options & CellAccessOptions.ReturnNullOnCellNotFound) != 0)
                    {
                        _put(this);
                        return null;
                    }
                    else
                    {
                        _put(this);
                        Throw.cell_not_found(cellId);
                    }
                    break;
                }
                default:
                _put(this);
                throw new NotImplementedException();
            }
            return this;
        }
        private class PoolPolicy : IPooledObjectPolicy<C2_Accessor>
        {
            public C2_Accessor Create()
            {
                return new C2_Accessor();
            }
            public bool Return(C2_Accessor obj)
            {
                return !obj.m_IsIterator;
            }
        }
        private static DefaultObjectPool<C2_Accessor> s_accessor_pool = new DefaultObjectPool<C2_Accessor>(new PoolPolicy());
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static C2_Accessor _get() => s_accessor_pool.Get();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void _put(C2_Accessor item) => s_accessor_pool.Return(item);
        /// <summary>
        /// For internal use only.
        /// Caller guarantees that entry lock is obtained.
        /// Does not handle CellAccessOptions. Only copy to the accessor.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal C2_Accessor _Setup(long CellId, byte* cellPtr, int entryIndex, CellAccessOptions options)
        {
            this.CellId      = CellId;
            m_cellEntryIndex = entryIndex;
            m_options        = options;
            m_ptr            = cellPtr;
            m_tx             = null;
            this.ResizeFunction = _Resize_NonTx;
            return this;
        }
        /// <summary>
        /// For internal use only.
        /// Caller guarantees that entry lock is obtained.
        /// Does not handle CellAccessOptions. Only copy to the accessor.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal C2_Accessor _Setup(long CellId, byte* cellPtr, int entryIndex, CellAccessOptions options, LocalTransactionContext tx)
        {
            this.CellId      = CellId;
            m_cellEntryIndex = entryIndex;
            m_options        = options;
            m_ptr            = cellPtr;
            m_tx             = tx;
            this.ResizeFunction = _Resize_Tx;
            return this;
        }
        /// <summary>
        /// For internal use only.
        /// </summary>
        internal static C2_Accessor AllocIterativeAccessor(CellInfo info, LocalTransactionContext tx)
        {
            C2_Accessor accessor = new C2_Accessor();
            accessor.m_IsIterator = true;
            if (tx != null) accessor._Setup(info.CellId, info.CellPtr, info.CellEntryIndex, 0, tx);
            else accessor._Setup(info.CellId, info.CellPtr, info.CellEntryIndex, 0);
            return accessor;
        }
        #endregion
        #region Public
        /// <summary>
        /// Dispose the accessor.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// the cell lock will be released.
        /// If write-ahead-log behavior is specified on <see cref="Trinity.Extension.StorageExtension_C2.UseC2"/>,
        /// the changes will be committed to the write-ahead log.
        /// </summary>
        public void Dispose()
        {
            if (m_cellEntryIndex >= 0)
            {
                if ((m_options & c_WALFlags) != 0)
                {
                    LocalMemoryStorage.CWriteAheadLog(this.CellId, this.m_ptr, this.CellSize, (ushort)CellType.C2, m_options);
                }
                if (!m_IsIterator)
                {
                    if (m_tx == null) Global.LocalStorage.ReleaseCellLock(CellId, m_cellEntryIndex);
                    else Global.LocalStorage.ReleaseCellLock(m_tx, CellId, m_cellEntryIndex);
                }
            }
            _put(this);
        }
        /// <summary>
        /// Get the cell type id.
        /// </summary>
        /// <returns>A 16-bit unsigned integer representing the cell type id.</returns>
        public ushort GetCellType()
        {
            ushort cellType;
            if (Global.LocalStorage.GetCellType(CellId, out cellType) == TrinityErrorCode.E_CELL_NOT_FOUND)
            {
                Throw.cell_not_found();
            }
            return cellType;
        }
        /// <summary>Converts a C2_Accessor to its string representation, in JSON format.</summary>
        /// <returns>A string representation of the C2.</returns>
        public override string ToString()
        {
            return Serializer.ToString(this);
        }
        #endregion
        #region Lookup tables
        internal static StringLookupTable FieldLookupTable = new StringLookupTable(
            
            "s"
            ,
            "ls"
            
            );
        static HashSet<string> AppendToFieldRerouteSet = new HashSet<string>()
        {
            
            "s"
            ,
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
                
                case 0:
                return TypeConverter<T>.ConvertFrom_int(this.s);
                
                case 1:
                return TypeConverter<T>.ConvertFrom_List_int(this.ls);
                
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
                
                case 0:
                {
                    int conversion_result = TypeConverter<T>.ConvertTo_int(value);
                    
            {
                this.s = conversion_result;
            }
            
                }
                break;
                
                case 1:
                {
                    List<int> conversion_result = TypeConverter<T>.ConvertTo_List_int(value);
                    
            {
                this.ls = conversion_result;
            }
            
                }
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
                
                case 0:
                
                return true;
                
                case 1:
                
                return true;
                
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
                
                case 1:
                
                {
                    
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.ls.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.ls.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.ls.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                default:
                Throw.target__field_not_list();
                break;
            }
        }
        public long CellId { get { return m_cellId; } set { m_cellId = value; } }
        IEnumerable<KeyValuePair<string, T>> ICell.SelectFields<T>(string attributeKey, string attributeValue)
        {
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                
                if (StorageSchema.C2_descriptor.check_attribute(StorageSchema.C2_descriptor.s, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("s", TypeConverter<T>.ConvertFrom_int(this.s));
                
                break;
                
                case 1:
                
                if (StorageSchema.C2_descriptor.check_attribute(StorageSchema.C2_descriptor.s, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("s", TypeConverter<T>.ConvertFrom_int(this.s));
                
                if (StorageSchema.C2_descriptor.check_attribute(StorageSchema.C2_descriptor.ls, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("ls", TypeConverter<T>.ConvertFrom_List_int(this.ls));
                
                break;
                
                case 2:
                
                if (StorageSchema.C2_descriptor.check_attribute(StorageSchema.C2_descriptor.s, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("s", TypeConverter<T>.ConvertFrom_int(this.s));
                
                if (StorageSchema.C2_descriptor.check_attribute(StorageSchema.C2_descriptor.ls, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("ls", TypeConverter<T>.ConvertFrom_List_int(this.ls));
                
                break;
                
                case 3:
                
                if (StorageSchema.C2_descriptor.check_attribute(StorageSchema.C2_descriptor.s, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("s", TypeConverter<T>.ConvertFrom_int(this.s));
                
                if (StorageSchema.C2_descriptor.check_attribute(StorageSchema.C2_descriptor.ls, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("ls", TypeConverter<T>.ConvertFrom_List_int(this.ls));
                
                break;
                
                default:
                Throw.incompatible_with_cell();
                break;
            }
            yield break;
        }
        #region enumerate value methods
        
        private IEnumerable<T> _enumerate_from_s<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_int(this.s);
                        
                    }
                    break;
                
                case 1:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_int(this.s);
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_int(this.s);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_int(this.s);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_ls<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.ls;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 1:
                    {
                        
                        {
                            
                            var element0 = this.ls;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.ls);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.ls);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private static StringLookupTable s_field_attribute_id_table = new StringLookupTable(
            
            );
        #endregion
        public IEnumerable<T> EnumerateField<T>(string fieldName)
        {
            switch (FieldLookupTable.Lookup(fieldName))
            {
                
                case 0:
                return _enumerate_from_s<T>();
                
                case 1:
                return _enumerate_from_ls<T>();
                
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
                
                foreach (var val in _enumerate_from_s<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_ls<T>())
                    yield return val;
                
            }
            else if (-1 != (attr_id = s_field_attribute_id_table.Lookup(attributeKey)))
            {
                switch (attr_id)
                {
                    
                }
            }
            yield break;
        }
        IEnumerable<string> ICellDescriptor.GetFieldNames()
        {
            
            {
                yield return "s";
            }
            
            {
                yield return "ls";
            }
            
        }
        IAttributeCollection ICellDescriptor.GetFieldAttributes(string fieldName)
        {
            return StorageSchema.C2.GetFieldAttributes(fieldName);
        }
        IEnumerable<IFieldDescriptor> ICellDescriptor.GetFieldDescriptors()
        {
            return StorageSchema.C2.GetFieldDescriptors();
        }
        string ITypeDescriptor.TypeName
        {
            get { return StorageSchema.s_cellTypeName_C2; }
        }
        Type ITypeDescriptor.Type
        {
            get { return StorageSchema.s_cellType_C2; }
        }
        bool ITypeDescriptor.IsOfType<T>()
        {
            return typeof(T) == StorageSchema.s_cellType_C2;
        }
        bool ITypeDescriptor.IsList()
        {
            return false;
        }
        IReadOnlyDictionary<string, string> IAttributeCollection.Attributes
        {
            get { return StorageSchema.C2.Attributes; }
        }
        string IAttributeCollection.GetAttributeValue(string attributeKey)
        {
            return StorageSchema.C2.GetAttributeValue(attributeKey);
        }
        ushort ICellDescriptor.CellType
        {
            get
            {
                return (ushort)CellType.C2;
            }
        }
        #endregion
        public ICell Deserialize()
        {
            return (C2)this;
        }
    }
    ///<summary>
    ///Provides interfaces for accessing C2 cells
    ///on <see cref="Trinity.Storage.LocalMemorySotrage"/>.
    static public class StorageExtension_C2
    {
        #region IKeyValueStore non logging
        /// <summary>
        /// Adds a new cell of type C2 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The value of the cell is specified in the method parameters.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.IKeyValueStore"/> instance.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC2(this IKeyValueStore storage, long cellId, int s = default(int), List<int> ls = default(List<int>))
        {
            
            byte* targetPtr;
            
            targetPtr = null;
            
            {
            targetPtr += 4;

if(ls!= null)
{
    targetPtr += ls.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

            }
            byte[] tmpcell = new byte[(int)(targetPtr)];
            fixed (byte* _tmpcellptr = tmpcell)
            {
                targetPtr = _tmpcellptr;
                
            {
            *(int*)targetPtr = s;
            targetPtr += 4;

if(ls!= null)
{
    *(int*)targetPtr = ls.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<ls.Count;++iterator_2)
    {
            *(int*)targetPtr = ls[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

            }
            }
            
            return storage.SaveCell(cellId, tmpcell, (ushort)CellType.C2) == TrinityErrorCode.E_SUCCESS;
        }
        /// <summary>
        /// Adds a new cell of type C2 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The parameter <paramref name="cellId"/> overrides the cell id in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.IKeyValueStore"/> instance.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="cellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC2(this IKeyValueStore storage, long cellId, C2 cellContent)
        {
            return SaveC2(storage, cellId  , cellContent.s  , cellContent.ls );
        }
        /// <summary>
        /// Adds a new cell of type C2 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. Cell Id is specified by the CellId field in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.IKeyValueStore"/> instance.</param>
        /// <param name="cellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC2(this IKeyValueStore storage, C2 cellContent)
        {
            return SaveC2(storage, cellContent.CellId  , cellContent.s  , cellContent.ls );
        }
        /// <summary>
        /// Loads the content of the specified cell. Any changes done to this object are not written to the store, unless
        /// the content object is saved back into the storage.
        /// <param name="storage"/>A <see cref="Trinity.Storage.IKeyValueStore"/> instance.</param>
        /// </summary>
        public unsafe static C2 LoadC2(this IKeyValueStore storage, long cellId)
        {
            if (TrinityErrorCode.E_SUCCESS == storage.LoadCell(cellId, out var buff))
            {
                fixed (byte* p = buff)
                {
                    return C2_Accessor._get()._Setup(cellId, p, -1, 0);
                }
            }
            else
            {
                Throw.cell_not_found();
                throw new Exception();
            }
        }
        #endregion
        #region LocalMemoryStorage Non-Tx accessors
        /// <summary>
        /// Allocate a cell accessor on the specified cell, which inteprets
        /// the cell as a C2. Any changes done to the accessor
        /// are written to the storage immediately.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock. Otherwise this method is wait-free.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">The id of the specified cell.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>A <see cref="Trinity.Extension.C2"/> instance.</returns>
        public unsafe static C2_Accessor UseC2(this Trinity.Storage.LocalMemoryStorage storage, long cellId, CellAccessOptions options)
        {
            return C2_Accessor._get()._Lock(cellId, options);
        }
        /// <summary>
        /// Allocate a cell accessor on the specified cell, which inteprets
        /// the cell as a C2. Any changes done to the accessor
        /// are written to the storage immediately.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">The id of the specified cell.</param>
        /// <returns>A <see cref="" + script.RootNamespace + ".C2"/> instance.</returns>
        public unsafe static C2_Accessor UseC2(this Trinity.Storage.LocalMemoryStorage storage, long cellId)
        {
            return C2_Accessor._get()._Lock(cellId, CellAccessOptions.ThrowExceptionOnCellNotFound);
        }
        #endregion
        #region LocalStorage Non-Tx logging
        /// <summary>
        /// Adds a new cell of type C2 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The value of the cell is specified in the method parameters.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC2(this Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions options, long cellId, int s = default(int), List<int> ls = default(List<int>))
        {
            
            byte* targetPtr;
            
            targetPtr = null;
            
            {
            targetPtr += 4;

if(ls!= null)
{
    targetPtr += ls.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

            }
            byte[] tmpcell = new byte[(int)(targetPtr)];
            fixed (byte* _tmpcellptr = tmpcell)
            {
                targetPtr = _tmpcellptr;
                
            {
            *(int*)targetPtr = s;
            targetPtr += 4;

if(ls!= null)
{
    *(int*)targetPtr = ls.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<ls.Count;++iterator_2)
    {
            *(int*)targetPtr = ls[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

            }
            }
            
            return storage.SaveCell(options, cellId, tmpcell, 0, tmpcell.Length, (ushort)CellType.C2) == TrinityErrorCode.E_SUCCESS;
        }
        /// <summary>
        /// Adds a new cell of type C2 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The parameter <paramref name="cellId"/> overrides the cell id in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC2(this Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions options, long cellId, C2 cellContent)
        {
            return SaveC2(storage, options, cellId  , cellContent.s  , cellContent.ls );
        }
        /// <summary>
        /// Adds a new cell of type C2 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. Cell Id is specified by the CellId field in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC2(this Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions options, C2 cellContent)
        {
            return SaveC2(storage, options, cellContent.CellId  , cellContent.s  , cellContent.ls );
        }
        /// <summary>
        /// Loads the content of the specified cell. Any changes done to this object are not written to the store, unless
        /// the content object is saved back into the storage.
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// </summary>
        public unsafe static C2 LoadC2(this Trinity.Storage.LocalMemoryStorage storage, long cellId)
        {
            using (var cell = C2_Accessor._get()._Lock(cellId, CellAccessOptions.ThrowExceptionOnCellNotFound))
            {
                return cell;
            }
        }
        #endregion
        #region LocalMemoryStorage Tx accessors
        /// <summary>
        /// Allocate a cell accessor on the specified cell, which inteprets
        /// the cell as a C2. Any changes done to the accessor
        /// are written to the storage immediately.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock. Otherwise this method is wait-free.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">The id of the specified cell.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>A <see cref="Trinity.Extension.C2"/> instance.</returns>
        public unsafe static C2_Accessor UseC2(this Trinity.Storage.LocalMemoryStorage storage, LocalTransactionContext tx, long cellId, CellAccessOptions options)
        {
            return C2_Accessor._get()._Lock(cellId, options, tx);
        }
        /// <summary>
        /// Allocate a cell accessor on the specified cell, which inteprets
        /// the cell as a C2. Any changes done to the accessor
        /// are written to the storage immediately.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">The id of the specified cell.</param>
        /// <returns>A <see cref="" + script.RootNamespace + ".C2"/> instance.</returns>
        public unsafe static C2_Accessor UseC2(this Trinity.Storage.LocalMemoryStorage storage, LocalTransactionContext tx, long cellId)
        {
            return C2_Accessor._get()._Lock(cellId, CellAccessOptions.ThrowExceptionOnCellNotFound, tx);
        }
        #endregion
        #region LocalStorage Tx logging
        /// <summary>
        /// Adds a new cell of type C2 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The value of the cell is specified in the method parameters.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC2(this Trinity.Storage.LocalMemoryStorage storage, LocalTransactionContext tx, CellAccessOptions options, long cellId, int s = default(int), List<int> ls = default(List<int>))
        {
            
            byte* targetPtr;
            
            targetPtr = null;
            
            {
            targetPtr += 4;

if(ls!= null)
{
    targetPtr += ls.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

            }
            byte[] tmpcell = new byte[(int)(targetPtr)];
            fixed (byte* _tmpcellptr = tmpcell)
            {
                targetPtr = _tmpcellptr;
                
            {
            *(int*)targetPtr = s;
            targetPtr += 4;

if(ls!= null)
{
    *(int*)targetPtr = ls.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<ls.Count;++iterator_2)
    {
            *(int*)targetPtr = ls[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

            }
            }
            
            return storage.SaveCell(tx, options, cellId, tmpcell, 0, tmpcell.Length, (ushort)CellType.C2) == TrinityErrorCode.E_SUCCESS;
        }
        /// <summary>
        /// Adds a new cell of type C2 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The parameter <paramref name="cellId"/> overrides the cell id in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC2(this Trinity.Storage.LocalMemoryStorage storage, LocalTransactionContext tx, CellAccessOptions options, long cellId, C2 cellContent)
        {
            return SaveC2(storage, tx, options, cellId  , cellContent.s  , cellContent.ls );
        }
        /// <summary>
        /// Adds a new cell of type C2 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. Cell Id is specified by the CellId field in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC2(this Trinity.Storage.LocalMemoryStorage storage, LocalTransactionContext tx, CellAccessOptions options, C2 cellContent)
        {
            return SaveC2(storage, tx, options, cellContent.CellId  , cellContent.s  , cellContent.ls );
        }
        /// <summary>
        /// Loads the content of the specified cell. Any changes done to this object are not written to the store, unless
        /// the content object is saved back into the storage.
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// </summary>
        public unsafe static C2 LoadC2(this Trinity.Storage.LocalMemoryStorage storage, LocalTransactionContext tx, long cellId)
        {
            using (var cell = C2_Accessor._get()._Lock(cellId, CellAccessOptions.ThrowExceptionOnCellNotFound, tx))
            {
                return cell;
            }
        }
        #endregion
    }
}

#pragma warning restore 162,168,649,660,661,1522

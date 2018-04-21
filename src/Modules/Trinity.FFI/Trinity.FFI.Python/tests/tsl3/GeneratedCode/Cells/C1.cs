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
    /// A .NET runtime object representation of C1 defined in TSL.
    /// </summary>
    public partial struct C1 : ICell
    {
        ///<summary>
        ///The id of the cell.
        ///</summary>
        public long CellId;
        ///<summary>
        ///Initializes a new instance of C1 with the specified parameters.
        ///</summary>
        public C1(long cell_id , List<int> lst = default(List<int>), string bar = default(string))
        {
            
            this.lst = lst;
            
            this.bar = bar;
            
            CellId = cell_id;
        }
        
        ///<summary>
        ///Initializes a new instance of C1 with the specified parameters.
        ///</summary>
        public C1( List<int> lst = default(List<int>), string bar = default(string))
        {
            
            this.lst = lst;
            
            this.bar = bar;
            
            CellId = CellIdFactory.NewCellId();
        }
        
        public List<int> lst;
        
        public string bar;
        
        public static bool operator ==(C1 a, C1 b)
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
                
                (a.lst == b.lst)
                &&
                (a.bar == b.bar)
                
                ;
            
        }
        public static bool operator !=(C1 a, C1 b)
        {
            return !(a == b);
        }
        #region Text processing
        /// <summary>
        /// Converts the string representation of a C1 to its
        /// struct equivalent. A return value indicates whether the 
        /// operation succeeded.
        /// </summary>
        /// <param name="input">A string to convert.</param>
        /// <param name="value">
        /// When this method returns, contains the struct equivalent of the value contained 
        /// in input, if the conversion succeeded, or default(C1) if the conversion
        /// failed. The conversion fails if the input parameter is null or String.Empty, or is 
        /// not of the correct format. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// True if input was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryParse(string input, out C1 value)
        {
            try
            {
                value = Newtonsoft.Json.JsonConvert.DeserializeObject<C1>(input);
                return true;
            }
            catch { value = default(C1); return false; }
        }
        public static C1 Parse(string input)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<C1>(input);
        }
        ///<summary>Converts a C1 to its string representation, in JSON format.</summary>
        ///<returns>A string representation of the C1.</returns>
        public override string ToString()
        {
            return Serializer.ToString(this);
        }
        #endregion
        #region Lookup tables
        internal static StringLookupTable FieldLookupTable = new StringLookupTable(
            
            "lst"
            ,
            "bar"
            
            );
        internal static HashSet<string> AppendToFieldRerouteSet = new HashSet<string>()
        {
            
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
                return TypeConverter<T>.ConvertFrom_List_int(this.lst);
                
                case 1:
                return TypeConverter<T>.ConvertFrom_string(this.bar);
                
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
                this.lst = TypeConverter<T>.ConvertTo_List_int(value);
                break;
                
                case 1:
                this.bar = TypeConverter<T>.ConvertTo_string(value);
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
                
                case 0:
                
                {
                    if (this.lst == null)
                        this.lst = new List<int>();
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.lst.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.lst.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.lst.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                case 1:
                
                {
                    if (this.bar == null)
                        this.bar = TypeConverter<T>.ConvertTo_string(value);
                    else
                        this.bar += TypeConverter<T>.ConvertTo_string(value);
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
                
                if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.bar, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar", TypeConverter<T>.ConvertFrom_string(this.bar));
                
                break;
                
                case 1:
                
                if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.lst, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("lst", TypeConverter<T>.ConvertFrom_List_int(this.lst));
                
                if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.bar, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar", TypeConverter<T>.ConvertFrom_string(this.bar));
                
                break;
                
                case 2:
                
                if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.lst, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("lst", TypeConverter<T>.ConvertFrom_List_int(this.lst));
                
                if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.bar, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar", TypeConverter<T>.ConvertFrom_string(this.bar));
                
                break;
                
                case 3:
                
                if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.lst, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("lst", TypeConverter<T>.ConvertFrom_List_int(this.lst));
                
                if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.bar, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar", TypeConverter<T>.ConvertFrom_string(this.bar));
                
                break;
                
                default:
                Throw.incompatible_with_cell();
                break;
            }
            yield break;
        }
        #region enumerate value constructs
        
        private IEnumerable<T> _enumerate_from_lst<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.lst;
                            
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
                            
                            var element0 = this.lst;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.lst);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.lst);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_bar<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_string(this.bar);
                        
                    }
                    break;
                
                case 1:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_string(this.bar);
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_string(this.bar);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_string(this.bar);
                        
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
                return _enumerate_from_lst<T>();
                
                case 1:
                return _enumerate_from_bar<T>();
                
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
                
                foreach (var val in _enumerate_from_lst<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_bar<T>())
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
            get { return StorageSchema.s_cellTypeName_C1; }
        }
        Type ITypeDescriptor.Type
        {
            get { return StorageSchema.s_cellType_C1; }
        }
        bool ITypeDescriptor.IsOfType<T>()
        {
            return typeof(T) == StorageSchema.s_cellType_C1;
        }
        bool ITypeDescriptor.IsList()
        {
            return false;
        }
        IEnumerable<IFieldDescriptor> ICellDescriptor.GetFieldDescriptors()
        {
            return StorageSchema.C1.GetFieldDescriptors();
        }
        IAttributeCollection ICellDescriptor.GetFieldAttributes(string fieldName)
        {
            return StorageSchema.C1.GetFieldAttributes(fieldName);
        }
        string IAttributeCollection.GetAttributeValue(string attributeKey)
        {
            return StorageSchema.C1.GetAttributeValue(attributeKey);
        }
        IReadOnlyDictionary<string, string> IAttributeCollection.Attributes
        {
            get { return StorageSchema.C1.Attributes; }
        }
        IEnumerable<string> ICellDescriptor.GetFieldNames()
        {
            
            {
                yield return "lst";
            }
            
            {
                yield return "bar";
            }
            
        }
        ushort ICellDescriptor.CellType
        {
            get
            {
                return (ushort)CellType.C1;
            }
        }
        #endregion
    }
    /// <summary>
    /// Provides in-place operations of C1 defined in TSL.
    /// </summary>
    public unsafe class C1_Accessor : ICellAccessor
    {
        #region Fields
        public   long                    CellId;
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
        private unsafe C1_Accessor()
        {
                    lst_Accessor_Field = new intListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.m_ptr);
                    this.ResizeFunction(this.m_ptr, ptr_offset + substructure_offset, delta);
                    return this.m_ptr + substructure_offset;
                });        bar_Accessor_Field = new StringAccessor(null,
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
            {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}
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
            {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}
            int size = (int)(targetPtr - m_ptr);
            return size;
        }
        public ResizeFunctionDelegate ResizeFunction { get; set; }
        #endregion
        private static byte[] s_default_content = null;
        private static unsafe byte[] construct( List<int> lst = default(List<int>) , string bar = default(string) )
        {
            if (s_default_content != null) return s_default_content;
            
            byte* targetPtr;
            
            targetPtr = null;
            
            {

if(lst!= null)
{
    targetPtr += lst.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

        if(bar!= null)
        {
            int strlen_2 = bar.Length * 2;
            targetPtr += strlen_2+sizeof(int);
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

if(lst!= null)
{
    *(int*)targetPtr = lst.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<lst.Count;++iterator_2)
    {
            *(int*)targetPtr = lst[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

        if(bar!= null)
        {
            int strlen_2 = bar.Length * 2;
            *(int*)targetPtr = strlen_2;
            targetPtr += sizeof(int);
            fixed(char* pstr_2 = bar)
            {
                Memory.Copy(pstr_2, targetPtr, strlen_2);
                targetPtr += strlen_2;
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
        intListAccessor lst_Accessor_Field;
        
        ///<summary>
        ///Provides in-place access to the object field lst.
        ///</summary>
        public unsafe intListAccessor lst
        {
            get
            {
                
                byte* targetPtr = m_ptr;
                {}lst_Accessor_Field.m_ptr = targetPtr + 4;
                lst_Accessor_Field.CellId = this.CellId;
                return lst_Accessor_Field;
                
            }
            set
            {
                
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                lst_Accessor_Field.CellId = this.CellId;
                
                byte* targetPtr = m_ptr;
                {}
                int length = *(int*)(value.m_ptr - 4);
                int oldlength = *(int*)targetPtr;
                if (value.CellId != lst_Accessor_Field.CellId)
                {
                    //if not in the same Cell
                    lst_Accessor_Field.m_ptr = lst_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.m_ptr - 4, lst_Accessor_Field.m_ptr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.m_ptr - 4, tmpcellptr, length + 4);
                        lst_Accessor_Field.m_ptr = lst_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, lst_Accessor_Field.m_ptr, length + 4);
                    }
                }

            }
        }
        StringAccessor bar_Accessor_Field;
        
        ///<summary>
        ///Provides in-place access to the object field bar.
        ///</summary>
        public unsafe StringAccessor bar
        {
            get
            {
                
                byte* targetPtr = m_ptr;
                {targetPtr += *(int*)targetPtr + sizeof(int);}bar_Accessor_Field.m_ptr = targetPtr + 4;
                bar_Accessor_Field.CellId = this.CellId;
                return bar_Accessor_Field;
                
            }
            set
            {
                
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                bar_Accessor_Field.CellId = this.CellId;
                
                byte* targetPtr = m_ptr;
                {targetPtr += *(int*)targetPtr + sizeof(int);}
                int length = *(int*)(value.m_ptr - 4);
                int oldlength = *(int*)targetPtr;
                if (value.CellId != bar_Accessor_Field.CellId)
                {
                    //if not in the same Cell
                    bar_Accessor_Field.m_ptr = bar_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.m_ptr - 4, bar_Accessor_Field.m_ptr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.m_ptr - 4, tmpcellptr, length + 4);
                        bar_Accessor_Field.m_ptr = bar_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, bar_Accessor_Field.m_ptr, length + 4);
                    }
                }

            }
        }
        
        public static unsafe implicit operator C1(C1_Accessor accessor)
        {
            
            return new C1(accessor.CellId
            
            ,
            
                    accessor.lst
            ,
            
                    accessor.bar
            );
        }
        
        public unsafe static implicit operator C1_Accessor(C1 field)
        {
            byte* targetPtr = null;
            
            {

if(field.lst!= null)
{
    targetPtr += field.lst.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

        if(field.bar!= null)
        {
            int strlen_2 = field.bar.Length * 2;
            targetPtr += strlen_2+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

            }
            byte* tmpcellptr = BufferAllocator.AllocBuffer((int)targetPtr);
            Memory.memset(tmpcellptr, 0, (ulong)targetPtr);
            targetPtr = tmpcellptr;
            
            {

if(field.lst!= null)
{
    *(int*)targetPtr = field.lst.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<field.lst.Count;++iterator_2)
    {
            *(int*)targetPtr = field.lst[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

        if(field.bar!= null)
        {
            int strlen_2 = field.bar.Length * 2;
            *(int*)targetPtr = strlen_2;
            targetPtr += sizeof(int);
            fixed(char* pstr_2 = field.bar)
            {
                Memory.Copy(pstr_2, targetPtr, strlen_2);
                targetPtr += strlen_2;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

            }C1_Accessor ret;
            
            ret = C1_Accessor._get()._Setup(field.CellId, tmpcellptr, -1, 0, null);
            ret.CellId = field.CellId;
            
            return ret;
        }
        
        public static bool operator ==(C1_Accessor a, C1_Accessor b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;
            if (a.m_ptr == b.m_ptr) return true;
            byte* targetPtr = a.m_ptr;
            {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}
            int lengthA = (int)(targetPtr - a.m_ptr);
            targetPtr = b.m_ptr;
            {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}
            int lengthB = (int)(targetPtr - b.m_ptr);
            if(lengthA != lengthB) return false;
            return Memory.Compare(a.m_ptr,b.m_ptr,lengthA);
        }
        public static bool operator != (C1_Accessor a, C1_Accessor b)
        {
            return !(a == b);
        }
        
        public static bool operator ==(C1_Accessor a, C1 b)
        {
            C1_Accessor bb = b;
            return (a == bb);
        }
        public static bool operator !=(C1_Accessor a, C1 b)
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
        internal unsafe C1_Accessor _Lock(long cellId, CellAccessOptions options)
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
                    if (cellType != (ushort)CellType.C1)
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
                        eResult                = Global.LocalStorage.AddOrUse(cellId, defaultContent, ref size, (ushort)CellType.C1, out this.m_ptr, out this.m_cellEntryIndex);
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
        internal unsafe C1_Accessor _Lock(long cellId, CellAccessOptions options, LocalTransactionContext tx)
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
                    if (cellType != (ushort)CellType.C1)
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
                        eResult                = Global.LocalStorage.AddOrUse(tx, cellId, defaultContent, ref size, (ushort)CellType.C1, out this.m_ptr, out this.m_cellEntryIndex);
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
        private class PoolPolicy : IPooledObjectPolicy<C1_Accessor>
        {
            public C1_Accessor Create()
            {
                return new C1_Accessor();
            }
            public bool Return(C1_Accessor obj)
            {
                return !obj.m_IsIterator;
            }
        }
        private static DefaultObjectPool<C1_Accessor> s_accessor_pool = new DefaultObjectPool<C1_Accessor>(new PoolPolicy());
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static C1_Accessor _get() => s_accessor_pool.Get();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void _put(C1_Accessor item) => s_accessor_pool.Return(item);
        /// <summary>
        /// For internal use only.
        /// Caller guarantees that entry lock is obtained.
        /// Does not handle CellAccessOptions. Only copy to the accessor.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal C1_Accessor _Setup(long CellId, byte* cellPtr, int entryIndex, CellAccessOptions options)
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
        internal C1_Accessor _Setup(long CellId, byte* cellPtr, int entryIndex, CellAccessOptions options, LocalTransactionContext tx)
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
        internal static C1_Accessor AllocIterativeAccessor(CellInfo info, LocalTransactionContext tx)
        {
            C1_Accessor accessor = new C1_Accessor();
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
        /// If write-ahead-log behavior is specified on <see cref="Trinity.Extension.StorageExtension_C1.UseC1"/>,
        /// the changes will be committed to the write-ahead log.
        /// </summary>
        public void Dispose()
        {
            if (m_cellEntryIndex >= 0)
            {
                if ((m_options & c_WALFlags) != 0)
                {
                    LocalMemoryStorage.CWriteAheadLog(this.CellId, this.m_ptr, this.CellSize, (ushort)CellType.C1, m_options);
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
        /// <summary>Converts a C1_Accessor to its string representation, in JSON format.</summary>
        /// <returns>A string representation of the C1.</returns>
        public override string ToString()
        {
            return Serializer.ToString(this);
        }
        #endregion
        #region Lookup tables
        internal static StringLookupTable FieldLookupTable = new StringLookupTable(
            
            "lst"
            ,
            "bar"
            
            );
        static HashSet<string> AppendToFieldRerouteSet = new HashSet<string>()
        {
            
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
                return TypeConverter<T>.ConvertFrom_List_int(this.lst);
                
                case 1:
                return TypeConverter<T>.ConvertFrom_string(this.bar);
                
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
                    List<int> conversion_result = TypeConverter<T>.ConvertTo_List_int(value);
                    
            {
                this.lst = conversion_result;
            }
            
                }
                break;
                
                case 1:
                {
                    string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                    
            {
                this.bar = conversion_result;
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
                
                case 0:
                
                {
                    
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.lst.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.lst.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.lst.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                case 1:
                
                {
                    
                    this.bar += TypeConverter<T>.ConvertTo_string(value);
                }
                
                break;
                
                default:
                Throw.target__field_not_list();
                break;
            }
        }
        long ICell.CellId { get { return CellId; } set { CellId = value; } }
        IEnumerable<KeyValuePair<string, T>> ICell.SelectFields<T>(string attributeKey, string attributeValue)
        {
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                
                if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.bar, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar", TypeConverter<T>.ConvertFrom_string(this.bar));
                
                break;
                
                case 1:
                
                if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.lst, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("lst", TypeConverter<T>.ConvertFrom_List_int(this.lst));
                
                if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.bar, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar", TypeConverter<T>.ConvertFrom_string(this.bar));
                
                break;
                
                case 2:
                
                if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.lst, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("lst", TypeConverter<T>.ConvertFrom_List_int(this.lst));
                
                if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.bar, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar", TypeConverter<T>.ConvertFrom_string(this.bar));
                
                break;
                
                case 3:
                
                if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.lst, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("lst", TypeConverter<T>.ConvertFrom_List_int(this.lst));
                
                if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.bar, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar", TypeConverter<T>.ConvertFrom_string(this.bar));
                
                break;
                
                default:
                Throw.incompatible_with_cell();
                break;
            }
            yield break;
        }
        #region enumerate value methods
        
        private IEnumerable<T> _enumerate_from_lst<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.lst;
                            
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
                            
                            var element0 = this.lst;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.lst);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.lst);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_bar<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_string(this.bar);
                        
                    }
                    break;
                
                case 1:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_string(this.bar);
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_string(this.bar);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_string(this.bar);
                        
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
                return _enumerate_from_lst<T>();
                
                case 1:
                return _enumerate_from_bar<T>();
                
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
                
                foreach (var val in _enumerate_from_lst<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_bar<T>())
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
                yield return "lst";
            }
            
            {
                yield return "bar";
            }
            
        }
        IAttributeCollection ICellDescriptor.GetFieldAttributes(string fieldName)
        {
            return StorageSchema.C1.GetFieldAttributes(fieldName);
        }
        IEnumerable<IFieldDescriptor> ICellDescriptor.GetFieldDescriptors()
        {
            return StorageSchema.C1.GetFieldDescriptors();
        }
        string ITypeDescriptor.TypeName
        {
            get { return StorageSchema.s_cellTypeName_C1; }
        }
        Type ITypeDescriptor.Type
        {
            get { return StorageSchema.s_cellType_C1; }
        }
        bool ITypeDescriptor.IsOfType<T>()
        {
            return typeof(T) == StorageSchema.s_cellType_C1;
        }
        bool ITypeDescriptor.IsList()
        {
            return false;
        }
        IReadOnlyDictionary<string, string> IAttributeCollection.Attributes
        {
            get { return StorageSchema.C1.Attributes; }
        }
        string IAttributeCollection.GetAttributeValue(string attributeKey)
        {
            return StorageSchema.C1.GetAttributeValue(attributeKey);
        }
        ushort ICellDescriptor.CellType
        {
            get
            {
                return (ushort)CellType.C1;
            }
        }
        #endregion
        public ICell Deserialize()
        {
            return (C1)this;
        }
    }
    ///<summary>
    ///Provides interfaces for accessing C1 cells
    ///on <see cref="Trinity.Storage.LocalMemorySotrage"/>.
    static public class StorageExtension_C1
    {
        #region IKeyValueStore non logging
        /// <summary>
        /// Adds a new cell of type C1 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The value of the cell is specified in the method parameters.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.IKeyValueStore"/> instance.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC1(this IKeyValueStore storage, long cellId, List<int> lst = default(List<int>), string bar = default(string))
        {
            
            byte* targetPtr;
            
            targetPtr = null;
            
            {

if(lst!= null)
{
    targetPtr += lst.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

        if(bar!= null)
        {
            int strlen_2 = bar.Length * 2;
            targetPtr += strlen_2+sizeof(int);
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

if(lst!= null)
{
    *(int*)targetPtr = lst.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<lst.Count;++iterator_2)
    {
            *(int*)targetPtr = lst[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

        if(bar!= null)
        {
            int strlen_2 = bar.Length * 2;
            *(int*)targetPtr = strlen_2;
            targetPtr += sizeof(int);
            fixed(char* pstr_2 = bar)
            {
                Memory.Copy(pstr_2, targetPtr, strlen_2);
                targetPtr += strlen_2;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

            }
            }
            
            return storage.SaveCell(cellId, tmpcell, (ushort)CellType.C1) == TrinityErrorCode.E_SUCCESS;
        }
        /// <summary>
        /// Adds a new cell of type C1 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The parameter <paramref name="cellId"/> overrides the cell id in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.IKeyValueStore"/> instance.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="cellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC1(this IKeyValueStore storage, long cellId, C1 cellContent)
        {
            return SaveC1(storage, cellId  , cellContent.lst  , cellContent.bar );
        }
        /// <summary>
        /// Adds a new cell of type C1 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. Cell Id is specified by the CellId field in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.IKeyValueStore"/> instance.</param>
        /// <param name="cellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC1(this IKeyValueStore storage, C1 cellContent)
        {
            return SaveC1(storage, cellContent.CellId  , cellContent.lst  , cellContent.bar );
        }
        /// <summary>
        /// Loads the content of the specified cell. Any changes done to this object are not written to the store, unless
        /// the content object is saved back into the storage.
        /// <param name="storage"/>A <see cref="Trinity.Storage.IKeyValueStore"/> instance.</param>
        /// </summary>
        public unsafe static C1 LoadC1(this IKeyValueStore storage, long cellId)
        {
            if (TrinityErrorCode.E_SUCCESS == storage.LoadCell(cellId, out var buff))
            {
                fixed (byte* p = buff)
                {
                    return C1_Accessor._get()._Setup(cellId, p, -1, 0);
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
        /// the cell as a C1. Any changes done to the accessor
        /// are written to the storage immediately.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock. Otherwise this method is wait-free.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">The id of the specified cell.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>A <see cref="Trinity.Extension.C1"/> instance.</returns>
        public unsafe static C1_Accessor UseC1(this Trinity.Storage.LocalMemoryStorage storage, long cellId, CellAccessOptions options)
        {
            return C1_Accessor._get()._Lock(cellId, options);
        }
        /// <summary>
        /// Allocate a cell accessor on the specified cell, which inteprets
        /// the cell as a C1. Any changes done to the accessor
        /// are written to the storage immediately.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">The id of the specified cell.</param>
        /// <returns>A <see cref="" + script.RootNamespace + ".C1"/> instance.</returns>
        public unsafe static C1_Accessor UseC1(this Trinity.Storage.LocalMemoryStorage storage, long cellId)
        {
            return C1_Accessor._get()._Lock(cellId, CellAccessOptions.ThrowExceptionOnCellNotFound);
        }
        #endregion
        #region LocalStorage Non-Tx logging
        /// <summary>
        /// Adds a new cell of type C1 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The value of the cell is specified in the method parameters.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC1(this Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions options, long cellId, List<int> lst = default(List<int>), string bar = default(string))
        {
            
            byte* targetPtr;
            
            targetPtr = null;
            
            {

if(lst!= null)
{
    targetPtr += lst.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

        if(bar!= null)
        {
            int strlen_2 = bar.Length * 2;
            targetPtr += strlen_2+sizeof(int);
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

if(lst!= null)
{
    *(int*)targetPtr = lst.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<lst.Count;++iterator_2)
    {
            *(int*)targetPtr = lst[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

        if(bar!= null)
        {
            int strlen_2 = bar.Length * 2;
            *(int*)targetPtr = strlen_2;
            targetPtr += sizeof(int);
            fixed(char* pstr_2 = bar)
            {
                Memory.Copy(pstr_2, targetPtr, strlen_2);
                targetPtr += strlen_2;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

            }
            }
            
            return storage.SaveCell(options, cellId, tmpcell, 0, tmpcell.Length, (ushort)CellType.C1) == TrinityErrorCode.E_SUCCESS;
        }
        /// <summary>
        /// Adds a new cell of type C1 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The parameter <paramref name="cellId"/> overrides the cell id in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC1(this Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions options, long cellId, C1 cellContent)
        {
            return SaveC1(storage, options, cellId  , cellContent.lst  , cellContent.bar );
        }
        /// <summary>
        /// Adds a new cell of type C1 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. Cell Id is specified by the CellId field in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC1(this Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions options, C1 cellContent)
        {
            return SaveC1(storage, options, cellContent.CellId  , cellContent.lst  , cellContent.bar );
        }
        /// <summary>
        /// Loads the content of the specified cell. Any changes done to this object are not written to the store, unless
        /// the content object is saved back into the storage.
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// </summary>
        public unsafe static C1 LoadC1(this Trinity.Storage.LocalMemoryStorage storage, long cellId)
        {
            using (var cell = C1_Accessor._get()._Lock(cellId, CellAccessOptions.ThrowExceptionOnCellNotFound))
            {
                return cell;
            }
        }
        #endregion
        #region LocalMemoryStorage Tx accessors
        /// <summary>
        /// Allocate a cell accessor on the specified cell, which inteprets
        /// the cell as a C1. Any changes done to the accessor
        /// are written to the storage immediately.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock. Otherwise this method is wait-free.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">The id of the specified cell.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>A <see cref="Trinity.Extension.C1"/> instance.</returns>
        public unsafe static C1_Accessor UseC1(this Trinity.Storage.LocalMemoryStorage storage, LocalTransactionContext tx, long cellId, CellAccessOptions options)
        {
            return C1_Accessor._get()._Lock(cellId, options, tx);
        }
        /// <summary>
        /// Allocate a cell accessor on the specified cell, which inteprets
        /// the cell as a C1. Any changes done to the accessor
        /// are written to the storage immediately.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">The id of the specified cell.</param>
        /// <returns>A <see cref="" + script.RootNamespace + ".C1"/> instance.</returns>
        public unsafe static C1_Accessor UseC1(this Trinity.Storage.LocalMemoryStorage storage, LocalTransactionContext tx, long cellId)
        {
            return C1_Accessor._get()._Lock(cellId, CellAccessOptions.ThrowExceptionOnCellNotFound, tx);
        }
        #endregion
        #region LocalStorage Tx logging
        /// <summary>
        /// Adds a new cell of type C1 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The value of the cell is specified in the method parameters.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC1(this Trinity.Storage.LocalMemoryStorage storage, LocalTransactionContext tx, CellAccessOptions options, long cellId, List<int> lst = default(List<int>), string bar = default(string))
        {
            
            byte* targetPtr;
            
            targetPtr = null;
            
            {

if(lst!= null)
{
    targetPtr += lst.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

        if(bar!= null)
        {
            int strlen_2 = bar.Length * 2;
            targetPtr += strlen_2+sizeof(int);
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

if(lst!= null)
{
    *(int*)targetPtr = lst.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<lst.Count;++iterator_2)
    {
            *(int*)targetPtr = lst[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

        if(bar!= null)
        {
            int strlen_2 = bar.Length * 2;
            *(int*)targetPtr = strlen_2;
            targetPtr += sizeof(int);
            fixed(char* pstr_2 = bar)
            {
                Memory.Copy(pstr_2, targetPtr, strlen_2);
                targetPtr += strlen_2;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

            }
            }
            
            return storage.SaveCell(tx, options, cellId, tmpcell, 0, tmpcell.Length, (ushort)CellType.C1) == TrinityErrorCode.E_SUCCESS;
        }
        /// <summary>
        /// Adds a new cell of type C1 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The parameter <paramref name="cellId"/> overrides the cell id in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC1(this Trinity.Storage.LocalMemoryStorage storage, LocalTransactionContext tx, CellAccessOptions options, long cellId, C1 cellContent)
        {
            return SaveC1(storage, tx, options, cellId  , cellContent.lst  , cellContent.bar );
        }
        /// <summary>
        /// Adds a new cell of type C1 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. Cell Id is specified by the CellId field in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC1(this Trinity.Storage.LocalMemoryStorage storage, LocalTransactionContext tx, CellAccessOptions options, C1 cellContent)
        {
            return SaveC1(storage, tx, options, cellContent.CellId  , cellContent.lst  , cellContent.bar );
        }
        /// <summary>
        /// Loads the content of the specified cell. Any changes done to this object are not written to the store, unless
        /// the content object is saved back into the storage.
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// </summary>
        public unsafe static C1 LoadC1(this Trinity.Storage.LocalMemoryStorage storage, LocalTransactionContext tx, long cellId)
        {
            using (var cell = C1_Accessor._get()._Lock(cellId, CellAccessOptions.ThrowExceptionOnCellNotFound, tx))
            {
                return cell;
            }
        }
        #endregion
    }
}

#pragma warning restore 162,168,649,660,661,1522

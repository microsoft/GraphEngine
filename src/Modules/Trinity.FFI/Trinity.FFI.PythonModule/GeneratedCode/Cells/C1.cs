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
namespace CellAssembly
{
    
    /// <summary>
    /// A .NET runtime object representation of C1 defined in TSL.
    /// </summary>
    public partial struct C1 : ICell
    {
        ///<summary>
        ///The id of the cell.
        ///</summary>
        public long CellID;
        ///<summary>
        ///Initializes a new instance of C1 with the specified parameters.
        ///</summary>
        public C1(long cell_id , int foo = default(int), int? baz = default(int?), string bar = default(string))
        {
            
            this.foo = foo;
            
            this.baz = baz;
            
            this.bar = bar;
            
            CellID = cell_id;
        }
        
        ///<summary>
        ///Initializes a new instance of C1 with the specified parameters.
        ///</summary>
        public C1( int foo = default(int), int? baz = default(int?), string bar = default(string))
        {
            
            this.foo = foo;
            
            this.baz = baz;
            
            this.bar = bar;
            
            CellID = CellIDFactory.NewCellID();
        }
        
        public int foo;
        
        public int? baz;
        
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
                
                (a.foo == b.foo)
                &&
                (a.baz == b.baz)
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
            
            "foo"
            ,
            "baz"
            ,
            "bar"
            
            );
        internal static HashSet<string> AppendToFieldRerouteSet = new HashSet<string>()
        {
            
            "foo"
            ,
            "baz"
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
                    return TypeConverter<T>.ConvertFrom_int(this.foo);
                    
                case 1:
                    return TypeConverter<T>.ConvertFrom_int_nullable(this.baz);
                    
                case 2:
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
                    this.foo = TypeConverter<T>.ConvertTo_int(value);
                    break;
                
                case 1:
                    this.baz = TypeConverter<T>.ConvertTo_int_nullable(value);
                    break;
                
                case 2:
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
                    
                    return this.baz != null;
                    
                case 2:
                    
                    return true;
                    
                default:
                    return false;
            }
        }
        /// <summary>
        /// Append <paramref name="value"/> to the target field. Note that if the target field
        /// is not appendable(string or list), calling this method is equivalent to <see cref="CellAssembly.GenericCellAccessor.SetField(string, T)"/>.
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
                
                case 2:
                    
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
        long ICell.CellID { get { return CellID; } set { CellID = value; } }
        public IEnumerable<KeyValuePair<string, T>> SelectFields<T>(string attributeKey, string attributeValue)
        {
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    
                    if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.foo, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("foo", TypeConverter<T>.ConvertFrom_int(this.foo));
                    
                    if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.baz, attributeKey, attributeValue))
                        
                        if (this.baz != null)
                            
                            yield return new KeyValuePair<string, T>("baz", TypeConverter<T>.ConvertFrom_int_nullable(this.baz));
                    
                    if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.bar, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("bar", TypeConverter<T>.ConvertFrom_string(this.bar));
                    
                    break;
                
                case 1:
                    
                    if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.foo, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("foo", TypeConverter<T>.ConvertFrom_int(this.foo));
                    
                    if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.baz, attributeKey, attributeValue))
                        
                        if (this.baz != null)
                            
                            yield return new KeyValuePair<string, T>("baz", TypeConverter<T>.ConvertFrom_int_nullable(this.baz));
                    
                    if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.bar, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("bar", TypeConverter<T>.ConvertFrom_string(this.bar));
                    
                    break;
                
                case 2:
                    
                    if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.foo, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("foo", TypeConverter<T>.ConvertFrom_int(this.foo));
                    
                    if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.baz, attributeKey, attributeValue))
                        
                        if (this.baz != null)
                            
                            yield return new KeyValuePair<string, T>("baz", TypeConverter<T>.ConvertFrom_int_nullable(this.baz));
                    
                    if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.bar, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("bar", TypeConverter<T>.ConvertFrom_string(this.bar));
                    
                    break;
                
                case 3:
                    
                    if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.foo, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("foo", TypeConverter<T>.ConvertFrom_int(this.foo));
                    
                    if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.baz, attributeKey, attributeValue))
                        
                        if (this.baz != null)
                            
                            yield return new KeyValuePair<string, T>("baz", TypeConverter<T>.ConvertFrom_int_nullable(this.baz));
                    
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
        
        private IEnumerable<T> _enumerate_from_foo<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_int(this.foo);
                        
                    }
                    break;
                
                case 1:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_int(this.foo);
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_int(this.foo);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_int(this.foo);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_baz<T>()
        {
            
            if (this.baz != null)
                
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_int_nullable(this.baz);
                        
                    }
                    break;
                
                case 1:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_int_nullable(this.baz);
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_int_nullable(this.baz);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_int_nullable(this.baz);
                        
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
                    return _enumerate_from_foo<T>();
                
                case 1:
                    return _enumerate_from_baz<T>();
                
                case 2:
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
                
                foreach (var val in _enumerate_from_foo<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_baz<T>())
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
                yield return "foo";
            }
            
            {
                if (this.baz != null)
                    yield return "baz";
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
        internal C1_Accessor(long cellId, byte[] buffer)
        {
            this.CellID       = cellId;
            this.handle       = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            this.CellPtr      = (byte*)handle.AddrOfPinnedObject().ToPointer();
                    bar_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });
            this.CellEntryIndex = -1;
        }
        
        internal static string[] optional_field_names = null;
        ///<summary>
        ///Get an array of the names of all optional fields for object type t_struct_name.
        ///</summary>
        public static string[] GetOptionalFieldNames()
        {
            if (optional_field_names == null)
                optional_field_names = new string[]
                {
                    
                    "baz"
                    ,
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
            
            byte [] bytes = new byte[1];
            Memory.Copy(CellPtr, 0, bytes, 0, 1);
            return bytes;
            
        }
        
        #region IAccessor Implementation
        public byte[] ToByteArray()
        {
            byte* targetPtr = CellPtr;
            {            byte* optheader_0 = targetPtr;
            targetPtr += 1;

                if ((0 != (*(optheader_0 + 0) & 0x01)))
                {
            targetPtr += 4;

                }
            targetPtr += 4;
targetPtr += *(int*)targetPtr + sizeof(int);}
            int size = (int)(targetPtr - CellPtr);
            byte[] ret = new byte[size];
            Memory.Copy(CellPtr, 0, ret, 0, size);
            return ret;
        }
        public unsafe byte* GetUnderlyingBufferPointer()
        {
            return CellPtr;
        }
        public unsafe int GetBufferLength()
        {
            byte* targetPtr = CellPtr;
            {            byte* optheader_0 = targetPtr;
            targetPtr += 1;

                if ((0 != (*(optheader_0 + 0) & 0x01)))
                {
            targetPtr += 4;

                }
            targetPtr += 4;
targetPtr += *(int*)targetPtr + sizeof(int);}
            int size = (int)(targetPtr - CellPtr);
            return size;
        }
        public ResizeFunctionDelegate ResizeFunction { get; set; }
        #endregion
        internal unsafe C1_Accessor(long cellId, CellAccessOptions options)
        {
            Initialize(cellId, options);
                    bar_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });
            this.CellID = cellId;
        }
        public unsafe C1_Accessor(byte* _CellPtr)
        {
            CellPtr = _CellPtr;
                    bar_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });
            this.CellEntryIndex = -1;
        }
        internal static unsafe byte[] construct(long CellID , int foo = default(int) , int? baz = default(int?) , string bar = default(string) )
        {
            
            byte* targetPtr;
            
            targetPtr = null;
            
            {
            targetPtr += 1;
            targetPtr += 4;
            if( baz!= null)
            {
            targetPtr += 4;

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
            byte* optheader_1 = targetPtr;
            *(optheader_1 + 0) = 0x00;            targetPtr += 1;
            *(int*)targetPtr = foo;
            targetPtr += 4;
            if( baz!= null)
            {
            *(int*)targetPtr = baz.Value;
            targetPtr += 4;
*(optheader_1 + 0) |= 0x01;
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
            
            return tmpcell;
        }
        
        ///<summary>
        ///Provides in-place access to the object field foo.
        ///</summary>
        public unsafe int foo
        {
            get
            {
                
                byte* targetPtr = CellPtr;
                {            byte* optheader_1 = targetPtr;
            targetPtr += 1;
}
                return *(int*)(targetPtr);
                
            }
            set
            {
                
                byte* targetPtr = CellPtr;
                {            byte* optheader_1 = targetPtr;
            targetPtr += 1;
}                *(int*)targetPtr = value;
            }
        }
        
        ///<summary>
        ///Represents the presence of the optional field baz.
        ///</summary>
        public bool Contains_baz
        {
            get
            {
                unchecked
                {
                    return (0 != (*(this.CellPtr + 0) & 0x01)) ;
                }
            }
            internal set
            {
                unchecked
                {
                    if (value)
                    {
                        *(this.CellPtr + 0) |= 0x01;
                    }
                    else
                    {
                        *(this.CellPtr + 0) &= 0xFE;
                    }
                }
            }
        }
        ///<summary>
        ///Removes the optional field baz from the object being operated.
        ///</summary>
        public unsafe void Remove_baz()
        {
            if (!this.Contains_baz)
            {
                throw new Exception("Optional field baz doesn't exist for current cell.");
            }
            this.Contains_baz = false;
            byte* targetPtr = CellPtr;
            {            byte* optheader_1 = targetPtr;
            targetPtr += 1;
            targetPtr += 4;
}
            byte* startPtr = targetPtr;
            targetPtr += 4;
            this.ResizeFunction(startPtr, 0, (int)(startPtr - targetPtr));
        }
        
        ///<summary>
        ///Provides in-place access to the object field baz.
        ///</summary>
        public unsafe int baz
        {
            get
            {
                
                if (!this.Contains_baz)
                {
                    throw new Exception("Optional field baz doesn't exist for current cell.");
                }
                
                byte* targetPtr = CellPtr;
                {            byte* optheader_1 = targetPtr;
            targetPtr += 1;
            targetPtr += 4;
}
                return *(int*)(targetPtr);
                
            }
            set
            {
                
                byte* targetPtr = CellPtr;
                {            byte* optheader_1 = targetPtr;
            targetPtr += 1;
            targetPtr += 4;
}
                bool creatingOptionalField = (!this.Contains_baz);
                if (creatingOptionalField)
                {
                    this.Contains_baz = true;
                                    targetPtr = this.ResizeFunction(targetPtr, 0, 4);                *(int*)targetPtr = value;
                }
                else
                {
                                    *(int*)targetPtr = value;
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
                
                byte* targetPtr = CellPtr;
                {            byte* optheader_1 = targetPtr;
            targetPtr += 1;

                if ((0 != (*(optheader_1 + 0) & 0x01)))
                {
            targetPtr += 4;

                }
            targetPtr += 4;
}bar_Accessor_Field.CellPtr = targetPtr + 4;
                bar_Accessor_Field.CellID = this.CellID;
                return bar_Accessor_Field;
                
            }
            set
            {
                
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                bar_Accessor_Field.CellID = this.CellID;
                
                byte* targetPtr = CellPtr;
                {            byte* optheader_1 = targetPtr;
            targetPtr += 1;

                if ((0 != (*(optheader_1 + 0) & 0x01)))
                {
            targetPtr += 4;

                }
            targetPtr += 4;
}
                int length = *(int*)(value.CellPtr - 4);
                int oldlength = *(int*)targetPtr;
                if (value.CellID != bar_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    bar_Accessor_Field.CellPtr = bar_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, bar_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        bar_Accessor_Field.CellPtr = bar_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, bar_Accessor_Field.CellPtr, length + 4);
                    }
                }

            }
        }
        
        public static unsafe implicit operator C1(C1_Accessor accessor)
        {
            int? _baz = default(int?);
            if (accessor.Contains_baz)
            {
                
                _baz = (int)accessor.baz;
                
            }
            
            if (accessor.CellID != null)
            {
                return new C1(accessor.CellID.Value
                
                ,
                
                        accessor.foo
                ,
                
                _baz 
                ,
                
                        accessor.bar
                );
            }
            else
            {
                return new C1(
                
                        accessor.foo,
                _baz ,
                        accessor.bar
                );
            }
        }
        
        public unsafe static implicit operator C1_Accessor(C1 field)
        {
            byte* targetPtr = null;
            
            {
            targetPtr += 1;
            targetPtr += 4;
            if( field.baz!= null)
            {
            targetPtr += 4;

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
            byte* optheader_1 = targetPtr;
            *(optheader_1 + 0) = 0x00;            targetPtr += 1;
            *(int*)targetPtr = field.foo;
            targetPtr += 4;
            if( field.baz!= null)
            {
            *(int*)targetPtr = field.baz.Value;
            targetPtr += 4;
*(optheader_1 + 0) |= 0x01;
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
            
            ret = new C1_Accessor(tmpcellptr);
            
            ret.CellID = field.CellID;
            
            return ret;
        }
        
        public static bool operator ==(C1_Accessor a, C1_Accessor b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;
            if (a.CellPtr == b.CellPtr) return true;
            byte* targetPtr = a.CellPtr;
            {            byte* optheader_1 = targetPtr;
            targetPtr += 1;

                if ((0 != (*(optheader_1 + 0) & 0x01)))
                {
            targetPtr += 4;

                }
            targetPtr += 4;
targetPtr += *(int*)targetPtr + sizeof(int);}
            int lengthA = (int)(targetPtr - a.CellPtr);
            targetPtr = b.CellPtr;
            {            byte* optheader_1 = targetPtr;
            targetPtr += 1;

                if ((0 != (*(optheader_1 + 0) & 0x01)))
                {
            targetPtr += 4;

                }
            targetPtr += 4;
targetPtr += *(int*)targetPtr + sizeof(int);}
            int lengthB = (int)(targetPtr - b.CellPtr);
            if(lengthA != lengthB) return false;
            return Memory.Compare(a.CellPtr,b.CellPtr,lengthA);
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
                            eResult                   = Global.LocalStorage.AddOrUse(cellId, defaultContent, ref size, (ushort)CellType.C1, out cellPtr, out cellEntryIndex);
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
                        if (cellType != (ushort)CellType.C1)
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
            this.ResizeFunction = (byte* ptr, int ptr_offset, int delta) =>
            {
                int offset = (int)(ptr - CellPtr) + ptr_offset;
                CellPtr = Global.LocalStorage.ResizeCell((long)CellID, CellEntryIndex, offset, delta);
                return CellPtr + (offset - ptr_offset);
            };
        }
        [ThreadStatic]
        internal static C1_Accessor s_accessor = null;
        internal static C1_Accessor New(long CellID, CellAccessOptions options)
        {
            C1_Accessor ret = null;
            if (s_accessor != (C1_Accessor)null)
            {
                ret = s_accessor;
                ret.Initialize(CellID, options);
                s_accessor = null;
            }
            else
            {
                ret = new C1_Accessor(CellID, options);
            }
            if (ret.CellPtr == null)
            {
                s_accessor = ret;
                ret        = null;
            }
            return ret;
        }
        internal static C1_Accessor New(long CellId, byte* cellPtr, int entryIndex, CellAccessOptions options)
        {
            C1_Accessor ret = null;
            if (s_accessor != (C1_Accessor)null)
            {
                ret = s_accessor;
                s_accessor = null;
                ret.CellPtr = cellPtr;
            }
            else
            {
                ret = new C1_Accessor(cellPtr);
            }
            ret.CellID         = CellId;
            ret.CellEntryIndex = entryIndex;
            ret.m_options      = options;
            return ret;
        }
        internal static C1_Accessor AllocIterativeAccessor(CellInfo info)
        {
            C1_Accessor ret = null;
            if (s_accessor != (C1_Accessor)null)
            {
                ret                = s_accessor;
                ret.CellPtr        = info.CellPtr;
                s_accessor         = null;
            }
            else
            {
                ret                = new C1_Accessor(info.CellPtr);
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
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// the cell lock will be released.
        /// If write-ahead-log behavior is specified on <see cref="CellAssembly.StorageExtension_C1.UseC1"/>,
        /// the changes will be committed to the write-ahead log.
        /// </summary>
        public void Dispose()
        {
            if (CellEntryIndex >= 0)
            {
                if ((m_options & c_WALFlags) != 0)
                {
                    LocalMemoryStorage.CWriteAheadLog(this.CellID.Value, this.CellPtr, this.CellSize, (ushort)CellType.C1, m_options);
                }
                if (!m_IsIterator)
                {
                    Global.LocalStorage.ReleaseCellLock(CellID.Value, CellEntryIndex);
                }
                if (s_accessor == (C1_Accessor)null)
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
        /// <summary>Converts a C1_Accessor to its string representation, in JSON format.</summary>
        /// <returns>A string representation of the C1.</returns>
        public override string ToString()
        {
            return Serializer.ToString(this);
        }
        #endregion
        #region Lookup tables
        internal static StringLookupTable FieldLookupTable = new StringLookupTable(
            
            "foo"
            ,
            "baz"
            ,
            "bar"
            
            );
        static HashSet<string> AppendToFieldRerouteSet = new HashSet<string>()
        {
            
            "foo"
            ,
            "baz"
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
                    return TypeConverter<T>.ConvertFrom_int(this.foo);
                    
                case 1:
                    return TypeConverter<T>.ConvertFrom_int_nullable(this.baz);
                    
                case 2:
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
                        int conversion_result = TypeConverter<T>.ConvertTo_int(value);
                        
            {
                this.foo = conversion_result;
            }
            
                    }
                    break;
                    
                case 1:
                    {
                        int? conversion_result = TypeConverter<T>.ConvertTo_int_nullable(value);
                        
            {
                if (conversion_result.HasValue)
                    this.baz = conversion_result.Value;
                else
                    this.Remove_baz();
            }
            
                    }
                    break;
                    
                case 2:
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
                    
                    return this.Contains_baz;
                    
                case 2:
                    
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
                
                case 2:
                    
                    {
                        
                        this.bar += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
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
                
                case 0:
                    
                    if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.foo, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("foo", TypeConverter<T>.ConvertFrom_int(this.foo));
                    
                    if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.baz, attributeKey, attributeValue))
                        
                        if (Contains_baz)
                            
                            yield return new KeyValuePair<string, T>("baz", TypeConverter<T>.ConvertFrom_int_nullable(this.baz));
                    
                    if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.bar, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("bar", TypeConverter<T>.ConvertFrom_string(this.bar));
                    
                    break;
                
                case 1:
                    
                    if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.foo, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("foo", TypeConverter<T>.ConvertFrom_int(this.foo));
                    
                    if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.baz, attributeKey, attributeValue))
                        
                        if (Contains_baz)
                            
                            yield return new KeyValuePair<string, T>("baz", TypeConverter<T>.ConvertFrom_int_nullable(this.baz));
                    
                    if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.bar, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("bar", TypeConverter<T>.ConvertFrom_string(this.bar));
                    
                    break;
                
                case 2:
                    
                    if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.foo, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("foo", TypeConverter<T>.ConvertFrom_int(this.foo));
                    
                    if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.baz, attributeKey, attributeValue))
                        
                        if (Contains_baz)
                            
                            yield return new KeyValuePair<string, T>("baz", TypeConverter<T>.ConvertFrom_int_nullable(this.baz));
                    
                    if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.bar, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("bar", TypeConverter<T>.ConvertFrom_string(this.bar));
                    
                    break;
                
                case 3:
                    
                    if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.foo, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("foo", TypeConverter<T>.ConvertFrom_int(this.foo));
                    
                    if (StorageSchema.C1_descriptor.check_attribute(StorageSchema.C1_descriptor.baz, attributeKey, attributeValue))
                        
                        if (Contains_baz)
                            
                            yield return new KeyValuePair<string, T>("baz", TypeConverter<T>.ConvertFrom_int_nullable(this.baz));
                    
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
        
        private IEnumerable<T> _enumerate_from_foo<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_int(this.foo);
                        
                    }
                    break;
                
                case 1:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_int(this.foo);
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_int(this.foo);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_int(this.foo);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_baz<T>()
        {
            
            if (this.Contains_baz)
                
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_int_nullable(this.baz);
                        
                    }
                    break;
                
                case 1:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_int_nullable(this.baz);
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_int_nullable(this.baz);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_int_nullable(this.baz);
                        
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
                    return _enumerate_from_foo<T>();
                
                case 1:
                    return _enumerate_from_baz<T>();
                
                case 2:
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
                
                foreach (var val in _enumerate_from_foo<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_baz<T>())
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
                yield return "foo";
            }
            
            {
                if (Contains_baz)
                    yield return "baz";
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
        /// <param name="CellID">A 64-bit cell Id.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC1(this IKeyValueStore storage, long CellID, int foo = default(int), int? baz = default(int?), string bar = default(string))
        {
            
            byte* targetPtr;
            
            targetPtr = null;
            
            {
            targetPtr += 1;
            targetPtr += 4;
            if( baz!= null)
            {
            targetPtr += 4;

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
            byte* optheader_1 = targetPtr;
            *(optheader_1 + 0) = 0x00;            targetPtr += 1;
            *(int*)targetPtr = foo;
            targetPtr += 4;
            if( baz!= null)
            {
            *(int*)targetPtr = baz.Value;
            targetPtr += 4;
*(optheader_1 + 0) |= 0x01;
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
            
            return storage.SaveCell(CellID, tmpcell, (ushort)CellType.C1) == TrinityErrorCode.E_SUCCESS;
        }
        /// <summary>
        /// Adds a new cell of type C1 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The parameter <paramref name="CellID"/> overrides the cell id in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.IKeyValueStore"/> instance.</param>
        /// <param name="CellID">A 64-bit cell Id.</param>
        /// <param name="CellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC1(this IKeyValueStore storage, long CellID, C1 CellContent)
        {
            return SaveC1(storage, CellID  , CellContent.foo  , CellContent.baz  , CellContent.bar );
        }
        /// <summary>
        /// Adds a new cell of type C1 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. Cell Id is specified by the CellID field in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.IKeyValueStore"/> instance.</param>
        /// <param name="CellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC1(this IKeyValueStore storage, C1 CellContent)
        {
            return SaveC1(storage, CellContent.CellID  , CellContent.foo  , CellContent.baz  , CellContent.bar );
        }
        /// <summary>
        /// Loads the content of the specified cell. Any changes done to this object are not written to the store, unless
        /// the content object is saved back into the storage.
        /// <param name="storage"/>A <see cref="Trinity.Storage.IKeyValueStore"/> instance.</param>
        /// </summary>
        public unsafe static C1 LoadC1(this IKeyValueStore storage, long CellID)
        {
            using (var cell = new C1_Accessor(CellID, CellAccessOptions.ThrowExceptionOnCellNotFound))
            {
                C1 ret = cell;
                ret.CellID = CellID;
                return ret;
            }
        }
        #endregion
        #region LocalMemoryStorage
        /// <summary>
        /// Allocate a cell accessor on the specified cell, which inteprets
        /// the cell as a C1. Any changes done to the accessor
        /// are written to the storage immediately.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock. Otherwise this method is wait-free.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellId">The id of the specified cell.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>A <see cref="CellAssembly.C1"/> instance.</returns>
        public unsafe static C1_Accessor UseC1(this Trinity.Storage.LocalMemoryStorage storage, long CellID, CellAccessOptions options)
        {
            return C1_Accessor.New(CellID, options);
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
        /// <param name="CellId">The id of the specified cell.</param>
        /// <returns>A <see cref="" + script.RootNamespace + ".C1"/> instance.</returns>
        public unsafe static C1_Accessor UseC1(this Trinity.Storage.LocalMemoryStorage storage, long CellID)
        {
            return C1_Accessor.New(CellID, CellAccessOptions.ThrowExceptionOnCellNotFound);
        }
        #endregion
        #region LocalStorage logging
        /// <summary>
        /// Adds a new cell of type C1 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The value of the cell is specified in the method parameters.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellID">A 64-bit cell Id.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC1(this Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions options, long CellID, int foo = default(int), int? baz = default(int?), string bar = default(string))
        {
            
            byte* targetPtr;
            
            targetPtr = null;
            
            {
            targetPtr += 1;
            targetPtr += 4;
            if( baz!= null)
            {
            targetPtr += 4;

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
            byte* optheader_1 = targetPtr;
            *(optheader_1 + 0) = 0x00;            targetPtr += 1;
            *(int*)targetPtr = foo;
            targetPtr += 4;
            if( baz!= null)
            {
            *(int*)targetPtr = baz.Value;
            targetPtr += 4;
*(optheader_1 + 0) |= 0x01;
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
            
            return storage.SaveCell(options, CellID, tmpcell, (ushort)CellType.C1) == TrinityErrorCode.E_SUCCESS;
        }
        /// <summary>
        /// Adds a new cell of type C1 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The parameter <paramref name="CellID"/> overrides the cell id in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellID">A 64-bit cell Id.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="CellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC1(this Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions options, long CellID, C1 CellContent)
        {
            return SaveC1(storage, options, CellID  , CellContent.foo  , CellContent.baz  , CellContent.bar );
        }
        /// <summary>
        /// Adds a new cell of type C1 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. Cell Id is specified by the CellID field in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="CellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC1(this Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions options, C1 CellContent)
        {
            return SaveC1(storage, options, CellContent.CellID  , CellContent.foo  , CellContent.baz  , CellContent.bar );
        }
        #endregion
    }
}

#pragma warning restore 162,168,649,660,661,1522

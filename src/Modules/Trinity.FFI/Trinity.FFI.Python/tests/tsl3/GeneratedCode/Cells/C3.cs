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
    /// A .NET runtime object representation of C3 defined in TSL.
    /// </summary>
    public partial struct C3 : ICell
    {
        ///<summary>
        ///The id of the cell.
        ///</summary>
        public long CellId;
        ///<summary>
        ///Initializes a new instance of C3 with the specified parameters.
        ///</summary>
        public C3(long cell_id , List<int> bar1 = default(List<int>), List<int> bar2 = default(List<int>), List<int> bar3 = default(List<int>), List<int> bar4 = default(List<int>), List<int> bar5 = default(List<int>), List<int> bar6 = default(List<int>), List<int> bar7 = default(List<int>), List<int> bar8 = default(List<int>), List<int> bar9 = default(List<int>), List<int> bar10 = default(List<int>), List<int> bar11 = default(List<int>), List<int> bar12 = default(List<int>))
        {
            
            this.bar1 = bar1;
            
            this.bar2 = bar2;
            
            this.bar3 = bar3;
            
            this.bar4 = bar4;
            
            this.bar5 = bar5;
            
            this.bar6 = bar6;
            
            this.bar7 = bar7;
            
            this.bar8 = bar8;
            
            this.bar9 = bar9;
            
            this.bar10 = bar10;
            
            this.bar11 = bar11;
            
            this.bar12 = bar12;
            
            CellId = cell_id;
        }
        
        ///<summary>
        ///Initializes a new instance of C3 with the specified parameters.
        ///</summary>
        public C3( List<int> bar1 = default(List<int>), List<int> bar2 = default(List<int>), List<int> bar3 = default(List<int>), List<int> bar4 = default(List<int>), List<int> bar5 = default(List<int>), List<int> bar6 = default(List<int>), List<int> bar7 = default(List<int>), List<int> bar8 = default(List<int>), List<int> bar9 = default(List<int>), List<int> bar10 = default(List<int>), List<int> bar11 = default(List<int>), List<int> bar12 = default(List<int>))
        {
            
            this.bar1 = bar1;
            
            this.bar2 = bar2;
            
            this.bar3 = bar3;
            
            this.bar4 = bar4;
            
            this.bar5 = bar5;
            
            this.bar6 = bar6;
            
            this.bar7 = bar7;
            
            this.bar8 = bar8;
            
            this.bar9 = bar9;
            
            this.bar10 = bar10;
            
            this.bar11 = bar11;
            
            this.bar12 = bar12;
            
            CellId = CellIdFactory.NewCellId();
        }
        
        public List<int> bar1;
        
        public List<int> bar2;
        
        public List<int> bar3;
        
        public List<int> bar4;
        
        public List<int> bar5;
        
        public List<int> bar6;
        
        public List<int> bar7;
        
        public List<int> bar8;
        
        public List<int> bar9;
        
        public List<int> bar10;
        
        public List<int> bar11;
        
        public List<int> bar12;
        
        public static bool operator ==(C3 a, C3 b)
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
                
                (a.bar1 == b.bar1)
                &&
                (a.bar2 == b.bar2)
                &&
                (a.bar3 == b.bar3)
                &&
                (a.bar4 == b.bar4)
                &&
                (a.bar5 == b.bar5)
                &&
                (a.bar6 == b.bar6)
                &&
                (a.bar7 == b.bar7)
                &&
                (a.bar8 == b.bar8)
                &&
                (a.bar9 == b.bar9)
                &&
                (a.bar10 == b.bar10)
                &&
                (a.bar11 == b.bar11)
                &&
                (a.bar12 == b.bar12)
                
                ;
            
        }
        public static bool operator !=(C3 a, C3 b)
        {
            return !(a == b);
        }
        #region Text processing
        /// <summary>
        /// Converts the string representation of a C3 to its
        /// struct equivalent. A return value indicates whether the 
        /// operation succeeded.
        /// </summary>
        /// <param name="input">A string to convert.</param>
        /// <param name="value">
        /// When this method returns, contains the struct equivalent of the value contained 
        /// in input, if the conversion succeeded, or default(C3) if the conversion
        /// failed. The conversion fails if the input parameter is null or String.Empty, or is 
        /// not of the correct format. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// True if input was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryParse(string input, out C3 value)
        {
            try
            {
                value = Newtonsoft.Json.JsonConvert.DeserializeObject<C3>(input);
                return true;
            }
            catch { value = default(C3); return false; }
        }
        public static C3 Parse(string input)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<C3>(input);
        }
        ///<summary>Converts a C3 to its string representation, in JSON format.</summary>
        ///<returns>A string representation of the C3.</returns>
        public override string ToString()
        {
            return Serializer.ToString(this);
        }
        #endregion
        #region Lookup tables
        internal static StringLookupTable FieldLookupTable = new StringLookupTable(
            
            "bar1"
            ,
            "bar2"
            ,
            "bar3"
            ,
            "bar4"
            ,
            "bar5"
            ,
            "bar6"
            ,
            "bar7"
            ,
            "bar8"
            ,
            "bar9"
            ,
            "bar10"
            ,
            "bar11"
            ,
            "bar12"
            
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
                return TypeConverter<T>.ConvertFrom_List_int(this.bar1);
                
                case 1:
                return TypeConverter<T>.ConvertFrom_List_int(this.bar2);
                
                case 2:
                return TypeConverter<T>.ConvertFrom_List_int(this.bar3);
                
                case 3:
                return TypeConverter<T>.ConvertFrom_List_int(this.bar4);
                
                case 4:
                return TypeConverter<T>.ConvertFrom_List_int(this.bar5);
                
                case 5:
                return TypeConverter<T>.ConvertFrom_List_int(this.bar6);
                
                case 6:
                return TypeConverter<T>.ConvertFrom_List_int(this.bar7);
                
                case 7:
                return TypeConverter<T>.ConvertFrom_List_int(this.bar8);
                
                case 8:
                return TypeConverter<T>.ConvertFrom_List_int(this.bar9);
                
                case 9:
                return TypeConverter<T>.ConvertFrom_List_int(this.bar10);
                
                case 10:
                return TypeConverter<T>.ConvertFrom_List_int(this.bar11);
                
                case 11:
                return TypeConverter<T>.ConvertFrom_List_int(this.bar12);
                
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
                this.bar1 = TypeConverter<T>.ConvertTo_List_int(value);
                break;
                
                case 1:
                this.bar2 = TypeConverter<T>.ConvertTo_List_int(value);
                break;
                
                case 2:
                this.bar3 = TypeConverter<T>.ConvertTo_List_int(value);
                break;
                
                case 3:
                this.bar4 = TypeConverter<T>.ConvertTo_List_int(value);
                break;
                
                case 4:
                this.bar5 = TypeConverter<T>.ConvertTo_List_int(value);
                break;
                
                case 5:
                this.bar6 = TypeConverter<T>.ConvertTo_List_int(value);
                break;
                
                case 6:
                this.bar7 = TypeConverter<T>.ConvertTo_List_int(value);
                break;
                
                case 7:
                this.bar8 = TypeConverter<T>.ConvertTo_List_int(value);
                break;
                
                case 8:
                this.bar9 = TypeConverter<T>.ConvertTo_List_int(value);
                break;
                
                case 9:
                this.bar10 = TypeConverter<T>.ConvertTo_List_int(value);
                break;
                
                case 10:
                this.bar11 = TypeConverter<T>.ConvertTo_List_int(value);
                break;
                
                case 11:
                this.bar12 = TypeConverter<T>.ConvertTo_List_int(value);
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
                
                case 2:
                
                return true;
                
                case 3:
                
                return true;
                
                case 4:
                
                return true;
                
                case 5:
                
                return true;
                
                case 6:
                
                return true;
                
                case 7:
                
                return true;
                
                case 8:
                
                return true;
                
                case 9:
                
                return true;
                
                case 10:
                
                return true;
                
                case 11:
                
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
                    if (this.bar1 == null)
                        this.bar1 = new List<int>();
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.bar1.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.bar1.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.bar1.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                case 1:
                
                {
                    if (this.bar2 == null)
                        this.bar2 = new List<int>();
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.bar2.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.bar2.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.bar2.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                case 2:
                
                {
                    if (this.bar3 == null)
                        this.bar3 = new List<int>();
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.bar3.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.bar3.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.bar3.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                case 3:
                
                {
                    if (this.bar4 == null)
                        this.bar4 = new List<int>();
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.bar4.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.bar4.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.bar4.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                case 4:
                
                {
                    if (this.bar5 == null)
                        this.bar5 = new List<int>();
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.bar5.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.bar5.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.bar5.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                case 5:
                
                {
                    if (this.bar6 == null)
                        this.bar6 = new List<int>();
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.bar6.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.bar6.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.bar6.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                case 6:
                
                {
                    if (this.bar7 == null)
                        this.bar7 = new List<int>();
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.bar7.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.bar7.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.bar7.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                case 7:
                
                {
                    if (this.bar8 == null)
                        this.bar8 = new List<int>();
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.bar8.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.bar8.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.bar8.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                case 8:
                
                {
                    if (this.bar9 == null)
                        this.bar9 = new List<int>();
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.bar9.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.bar9.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.bar9.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                case 9:
                
                {
                    if (this.bar10 == null)
                        this.bar10 = new List<int>();
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.bar10.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.bar10.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.bar10.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                case 10:
                
                {
                    if (this.bar11 == null)
                        this.bar11 = new List<int>();
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.bar11.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.bar11.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.bar11.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                case 11:
                
                {
                    if (this.bar12 == null)
                        this.bar12 = new List<int>();
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.bar12.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.bar12.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.bar12.Add(TypeConverter<T>.ConvertTo_int(value));
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
                
                case 1:
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar1, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar1", TypeConverter<T>.ConvertFrom_List_int(this.bar1));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar2, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar2", TypeConverter<T>.ConvertFrom_List_int(this.bar2));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar3, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar3", TypeConverter<T>.ConvertFrom_List_int(this.bar3));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar4, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar4", TypeConverter<T>.ConvertFrom_List_int(this.bar4));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar5, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar5", TypeConverter<T>.ConvertFrom_List_int(this.bar5));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar6, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar6", TypeConverter<T>.ConvertFrom_List_int(this.bar6));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar7, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar7", TypeConverter<T>.ConvertFrom_List_int(this.bar7));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar8, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar8", TypeConverter<T>.ConvertFrom_List_int(this.bar8));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar9, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar9", TypeConverter<T>.ConvertFrom_List_int(this.bar9));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar10, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar10", TypeConverter<T>.ConvertFrom_List_int(this.bar10));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar11, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar11", TypeConverter<T>.ConvertFrom_List_int(this.bar11));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar12, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar12", TypeConverter<T>.ConvertFrom_List_int(this.bar12));
                
                break;
                
                case 2:
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar1, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar1", TypeConverter<T>.ConvertFrom_List_int(this.bar1));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar2, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar2", TypeConverter<T>.ConvertFrom_List_int(this.bar2));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar3, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar3", TypeConverter<T>.ConvertFrom_List_int(this.bar3));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar4, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar4", TypeConverter<T>.ConvertFrom_List_int(this.bar4));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar5, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar5", TypeConverter<T>.ConvertFrom_List_int(this.bar5));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar6, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar6", TypeConverter<T>.ConvertFrom_List_int(this.bar6));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar7, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar7", TypeConverter<T>.ConvertFrom_List_int(this.bar7));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar8, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar8", TypeConverter<T>.ConvertFrom_List_int(this.bar8));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar9, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar9", TypeConverter<T>.ConvertFrom_List_int(this.bar9));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar10, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar10", TypeConverter<T>.ConvertFrom_List_int(this.bar10));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar11, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar11", TypeConverter<T>.ConvertFrom_List_int(this.bar11));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar12, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar12", TypeConverter<T>.ConvertFrom_List_int(this.bar12));
                
                break;
                
                case 3:
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar1, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar1", TypeConverter<T>.ConvertFrom_List_int(this.bar1));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar2, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar2", TypeConverter<T>.ConvertFrom_List_int(this.bar2));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar3, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar3", TypeConverter<T>.ConvertFrom_List_int(this.bar3));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar4, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar4", TypeConverter<T>.ConvertFrom_List_int(this.bar4));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar5, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar5", TypeConverter<T>.ConvertFrom_List_int(this.bar5));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar6, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar6", TypeConverter<T>.ConvertFrom_List_int(this.bar6));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar7, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar7", TypeConverter<T>.ConvertFrom_List_int(this.bar7));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar8, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar8", TypeConverter<T>.ConvertFrom_List_int(this.bar8));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar9, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar9", TypeConverter<T>.ConvertFrom_List_int(this.bar9));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar10, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar10", TypeConverter<T>.ConvertFrom_List_int(this.bar10));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar11, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar11", TypeConverter<T>.ConvertFrom_List_int(this.bar11));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar12, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar12", TypeConverter<T>.ConvertFrom_List_int(this.bar12));
                
                break;
                
                default:
                Throw.incompatible_with_cell();
                break;
            }
            yield break;
        }
        #region enumerate value constructs
        
        private IEnumerable<T> _enumerate_from_bar1<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.bar1;
                            
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
                            
                            var element0 = this.bar1;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar1);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar1);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_bar2<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.bar2;
                            
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
                            
                            var element0 = this.bar2;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar2);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar2);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_bar3<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.bar3;
                            
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
                            
                            var element0 = this.bar3;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar3);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar3);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_bar4<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.bar4;
                            
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
                            
                            var element0 = this.bar4;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar4);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar4);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_bar5<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.bar5;
                            
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
                            
                            var element0 = this.bar5;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar5);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar5);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_bar6<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.bar6;
                            
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
                            
                            var element0 = this.bar6;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar6);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar6);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_bar7<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.bar7;
                            
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
                            
                            var element0 = this.bar7;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar7);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar7);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_bar8<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.bar8;
                            
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
                            
                            var element0 = this.bar8;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar8);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar8);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_bar9<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.bar9;
                            
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
                            
                            var element0 = this.bar9;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar9);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar9);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_bar10<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.bar10;
                            
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
                            
                            var element0 = this.bar10;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar10);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar10);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_bar11<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.bar11;
                            
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
                            
                            var element0 = this.bar11;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar11);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar11);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_bar12<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.bar12;
                            
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
                            
                            var element0 = this.bar12;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar12);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar12);
                        
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
                return _enumerate_from_bar1<T>();
                
                case 1:
                return _enumerate_from_bar2<T>();
                
                case 2:
                return _enumerate_from_bar3<T>();
                
                case 3:
                return _enumerate_from_bar4<T>();
                
                case 4:
                return _enumerate_from_bar5<T>();
                
                case 5:
                return _enumerate_from_bar6<T>();
                
                case 6:
                return _enumerate_from_bar7<T>();
                
                case 7:
                return _enumerate_from_bar8<T>();
                
                case 8:
                return _enumerate_from_bar9<T>();
                
                case 9:
                return _enumerate_from_bar10<T>();
                
                case 10:
                return _enumerate_from_bar11<T>();
                
                case 11:
                return _enumerate_from_bar12<T>();
                
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
                
                foreach (var val in _enumerate_from_bar1<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_bar2<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_bar3<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_bar4<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_bar5<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_bar6<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_bar7<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_bar8<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_bar9<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_bar10<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_bar11<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_bar12<T>())
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
            get { return StorageSchema.s_cellTypeName_C3; }
        }
        Type ITypeDescriptor.Type
        {
            get { return StorageSchema.s_cellType_C3; }
        }
        bool ITypeDescriptor.IsOfType<T>()
        {
            return typeof(T) == StorageSchema.s_cellType_C3;
        }
        bool ITypeDescriptor.IsList()
        {
            return false;
        }
        IEnumerable<IFieldDescriptor> ICellDescriptor.GetFieldDescriptors()
        {
            return StorageSchema.C3.GetFieldDescriptors();
        }
        IAttributeCollection ICellDescriptor.GetFieldAttributes(string fieldName)
        {
            return StorageSchema.C3.GetFieldAttributes(fieldName);
        }
        string IAttributeCollection.GetAttributeValue(string attributeKey)
        {
            return StorageSchema.C3.GetAttributeValue(attributeKey);
        }
        IReadOnlyDictionary<string, string> IAttributeCollection.Attributes
        {
            get { return StorageSchema.C3.Attributes; }
        }
        IEnumerable<string> ICellDescriptor.GetFieldNames()
        {
            
            {
                yield return "bar1";
            }
            
            {
                yield return "bar2";
            }
            
            {
                yield return "bar3";
            }
            
            {
                yield return "bar4";
            }
            
            {
                yield return "bar5";
            }
            
            {
                yield return "bar6";
            }
            
            {
                yield return "bar7";
            }
            
            {
                yield return "bar8";
            }
            
            {
                yield return "bar9";
            }
            
            {
                yield return "bar10";
            }
            
            {
                yield return "bar11";
            }
            
            {
                yield return "bar12";
            }
            
        }
        ushort ICellDescriptor.CellType
        {
            get
            {
                return (ushort)CellType.C3;
            }
        }
        #endregion
    }
    /// <summary>
    /// Provides in-place operations of C3 defined in TSL.
    /// </summary>
    public unsafe class C3_Accessor : ICellAccessor
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
        private unsafe C3_Accessor()
        {
                    bar1_Accessor_Field = new intListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.m_ptr);
                    this.ResizeFunction(this.m_ptr, ptr_offset + substructure_offset, delta);
                    return this.m_ptr + substructure_offset;
                });        bar2_Accessor_Field = new intListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.m_ptr);
                    this.ResizeFunction(this.m_ptr, ptr_offset + substructure_offset, delta);
                    return this.m_ptr + substructure_offset;
                });        bar3_Accessor_Field = new intListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.m_ptr);
                    this.ResizeFunction(this.m_ptr, ptr_offset + substructure_offset, delta);
                    return this.m_ptr + substructure_offset;
                });        bar4_Accessor_Field = new intListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.m_ptr);
                    this.ResizeFunction(this.m_ptr, ptr_offset + substructure_offset, delta);
                    return this.m_ptr + substructure_offset;
                });        bar5_Accessor_Field = new intListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.m_ptr);
                    this.ResizeFunction(this.m_ptr, ptr_offset + substructure_offset, delta);
                    return this.m_ptr + substructure_offset;
                });        bar6_Accessor_Field = new intListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.m_ptr);
                    this.ResizeFunction(this.m_ptr, ptr_offset + substructure_offset, delta);
                    return this.m_ptr + substructure_offset;
                });        bar7_Accessor_Field = new intListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.m_ptr);
                    this.ResizeFunction(this.m_ptr, ptr_offset + substructure_offset, delta);
                    return this.m_ptr + substructure_offset;
                });        bar8_Accessor_Field = new intListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.m_ptr);
                    this.ResizeFunction(this.m_ptr, ptr_offset + substructure_offset, delta);
                    return this.m_ptr + substructure_offset;
                });        bar9_Accessor_Field = new intListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.m_ptr);
                    this.ResizeFunction(this.m_ptr, ptr_offset + substructure_offset, delta);
                    return this.m_ptr + substructure_offset;
                });        bar10_Accessor_Field = new intListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.m_ptr);
                    this.ResizeFunction(this.m_ptr, ptr_offset + substructure_offset, delta);
                    return this.m_ptr + substructure_offset;
                });        bar11_Accessor_Field = new intListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.m_ptr);
                    this.ResizeFunction(this.m_ptr, ptr_offset + substructure_offset, delta);
                    return this.m_ptr + substructure_offset;
                });        bar12_Accessor_Field = new intListAccessor(null,
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
            {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}
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
            {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}
            int size = (int)(targetPtr - m_ptr);
            return size;
        }
        public ResizeFunctionDelegate ResizeFunction { get; set; }
        #endregion
        private static byte[] s_default_content = null;
        private static unsafe byte[] construct( List<int> bar1 = default(List<int>) , List<int> bar2 = default(List<int>) , List<int> bar3 = default(List<int>) , List<int> bar4 = default(List<int>) , List<int> bar5 = default(List<int>) , List<int> bar6 = default(List<int>) , List<int> bar7 = default(List<int>) , List<int> bar8 = default(List<int>) , List<int> bar9 = default(List<int>) , List<int> bar10 = default(List<int>) , List<int> bar11 = default(List<int>) , List<int> bar12 = default(List<int>) )
        {
            if (s_default_content != null) return s_default_content;
            
            byte* targetPtr;
            
            targetPtr = null;
            
            {

if(bar1!= null)
{
    targetPtr += bar1.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar2!= null)
{
    targetPtr += bar2.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar3!= null)
{
    targetPtr += bar3.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar4!= null)
{
    targetPtr += bar4.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar5!= null)
{
    targetPtr += bar5.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar6!= null)
{
    targetPtr += bar6.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar7!= null)
{
    targetPtr += bar7.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar8!= null)
{
    targetPtr += bar8.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar9!= null)
{
    targetPtr += bar9.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar10!= null)
{
    targetPtr += bar10.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar11!= null)
{
    targetPtr += bar11.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar12!= null)
{
    targetPtr += bar12.Count*4+sizeof(int);
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

if(bar1!= null)
{
    *(int*)targetPtr = bar1.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar1.Count;++iterator_2)
    {
            *(int*)targetPtr = bar1[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar2!= null)
{
    *(int*)targetPtr = bar2.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar2.Count;++iterator_2)
    {
            *(int*)targetPtr = bar2[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar3!= null)
{
    *(int*)targetPtr = bar3.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar3.Count;++iterator_2)
    {
            *(int*)targetPtr = bar3[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar4!= null)
{
    *(int*)targetPtr = bar4.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar4.Count;++iterator_2)
    {
            *(int*)targetPtr = bar4[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar5!= null)
{
    *(int*)targetPtr = bar5.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar5.Count;++iterator_2)
    {
            *(int*)targetPtr = bar5[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar6!= null)
{
    *(int*)targetPtr = bar6.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar6.Count;++iterator_2)
    {
            *(int*)targetPtr = bar6[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar7!= null)
{
    *(int*)targetPtr = bar7.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar7.Count;++iterator_2)
    {
            *(int*)targetPtr = bar7[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar8!= null)
{
    *(int*)targetPtr = bar8.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar8.Count;++iterator_2)
    {
            *(int*)targetPtr = bar8[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar9!= null)
{
    *(int*)targetPtr = bar9.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar9.Count;++iterator_2)
    {
            *(int*)targetPtr = bar9[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar10!= null)
{
    *(int*)targetPtr = bar10.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar10.Count;++iterator_2)
    {
            *(int*)targetPtr = bar10[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar11!= null)
{
    *(int*)targetPtr = bar11.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar11.Count;++iterator_2)
    {
            *(int*)targetPtr = bar11[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar12!= null)
{
    *(int*)targetPtr = bar12.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar12.Count;++iterator_2)
    {
            *(int*)targetPtr = bar12[iterator_2];
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
        intListAccessor bar1_Accessor_Field;
        
        ///<summary>
        ///Provides in-place access to the object field bar1.
        ///</summary>
        public unsafe intListAccessor bar1
        {
            get
            {
                
                byte* targetPtr = m_ptr;
                {}bar1_Accessor_Field.m_ptr = targetPtr + 4;
                bar1_Accessor_Field.CellId = this.CellId;
                return bar1_Accessor_Field;
                
            }
            set
            {
                
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                bar1_Accessor_Field.CellId = this.CellId;
                
                byte* targetPtr = m_ptr;
                {}
                int length = *(int*)(value.m_ptr - 4);
                int oldlength = *(int*)targetPtr;
                if (value.CellId != bar1_Accessor_Field.CellId)
                {
                    //if not in the same Cell
                    bar1_Accessor_Field.m_ptr = bar1_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.m_ptr - 4, bar1_Accessor_Field.m_ptr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.m_ptr - 4, tmpcellptr, length + 4);
                        bar1_Accessor_Field.m_ptr = bar1_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, bar1_Accessor_Field.m_ptr, length + 4);
                    }
                }

            }
        }
        intListAccessor bar2_Accessor_Field;
        
        ///<summary>
        ///Provides in-place access to the object field bar2.
        ///</summary>
        public unsafe intListAccessor bar2
        {
            get
            {
                
                byte* targetPtr = m_ptr;
                {targetPtr += *(int*)targetPtr + sizeof(int);}bar2_Accessor_Field.m_ptr = targetPtr + 4;
                bar2_Accessor_Field.CellId = this.CellId;
                return bar2_Accessor_Field;
                
            }
            set
            {
                
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                bar2_Accessor_Field.CellId = this.CellId;
                
                byte* targetPtr = m_ptr;
                {targetPtr += *(int*)targetPtr + sizeof(int);}
                int length = *(int*)(value.m_ptr - 4);
                int oldlength = *(int*)targetPtr;
                if (value.CellId != bar2_Accessor_Field.CellId)
                {
                    //if not in the same Cell
                    bar2_Accessor_Field.m_ptr = bar2_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.m_ptr - 4, bar2_Accessor_Field.m_ptr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.m_ptr - 4, tmpcellptr, length + 4);
                        bar2_Accessor_Field.m_ptr = bar2_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, bar2_Accessor_Field.m_ptr, length + 4);
                    }
                }

            }
        }
        intListAccessor bar3_Accessor_Field;
        
        ///<summary>
        ///Provides in-place access to the object field bar3.
        ///</summary>
        public unsafe intListAccessor bar3
        {
            get
            {
                
                byte* targetPtr = m_ptr;
                {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}bar3_Accessor_Field.m_ptr = targetPtr + 4;
                bar3_Accessor_Field.CellId = this.CellId;
                return bar3_Accessor_Field;
                
            }
            set
            {
                
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                bar3_Accessor_Field.CellId = this.CellId;
                
                byte* targetPtr = m_ptr;
                {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}
                int length = *(int*)(value.m_ptr - 4);
                int oldlength = *(int*)targetPtr;
                if (value.CellId != bar3_Accessor_Field.CellId)
                {
                    //if not in the same Cell
                    bar3_Accessor_Field.m_ptr = bar3_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.m_ptr - 4, bar3_Accessor_Field.m_ptr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.m_ptr - 4, tmpcellptr, length + 4);
                        bar3_Accessor_Field.m_ptr = bar3_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, bar3_Accessor_Field.m_ptr, length + 4);
                    }
                }

            }
        }
        intListAccessor bar4_Accessor_Field;
        
        ///<summary>
        ///Provides in-place access to the object field bar4.
        ///</summary>
        public unsafe intListAccessor bar4
        {
            get
            {
                
                byte* targetPtr = m_ptr;
                {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}bar4_Accessor_Field.m_ptr = targetPtr + 4;
                bar4_Accessor_Field.CellId = this.CellId;
                return bar4_Accessor_Field;
                
            }
            set
            {
                
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                bar4_Accessor_Field.CellId = this.CellId;
                
                byte* targetPtr = m_ptr;
                {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}
                int length = *(int*)(value.m_ptr - 4);
                int oldlength = *(int*)targetPtr;
                if (value.CellId != bar4_Accessor_Field.CellId)
                {
                    //if not in the same Cell
                    bar4_Accessor_Field.m_ptr = bar4_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.m_ptr - 4, bar4_Accessor_Field.m_ptr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.m_ptr - 4, tmpcellptr, length + 4);
                        bar4_Accessor_Field.m_ptr = bar4_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, bar4_Accessor_Field.m_ptr, length + 4);
                    }
                }

            }
        }
        intListAccessor bar5_Accessor_Field;
        
        ///<summary>
        ///Provides in-place access to the object field bar5.
        ///</summary>
        public unsafe intListAccessor bar5
        {
            get
            {
                
                byte* targetPtr = m_ptr;
                {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}bar5_Accessor_Field.m_ptr = targetPtr + 4;
                bar5_Accessor_Field.CellId = this.CellId;
                return bar5_Accessor_Field;
                
            }
            set
            {
                
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                bar5_Accessor_Field.CellId = this.CellId;
                
                byte* targetPtr = m_ptr;
                {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}
                int length = *(int*)(value.m_ptr - 4);
                int oldlength = *(int*)targetPtr;
                if (value.CellId != bar5_Accessor_Field.CellId)
                {
                    //if not in the same Cell
                    bar5_Accessor_Field.m_ptr = bar5_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.m_ptr - 4, bar5_Accessor_Field.m_ptr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.m_ptr - 4, tmpcellptr, length + 4);
                        bar5_Accessor_Field.m_ptr = bar5_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, bar5_Accessor_Field.m_ptr, length + 4);
                    }
                }

            }
        }
        intListAccessor bar6_Accessor_Field;
        
        ///<summary>
        ///Provides in-place access to the object field bar6.
        ///</summary>
        public unsafe intListAccessor bar6
        {
            get
            {
                
                byte* targetPtr = m_ptr;
                {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}bar6_Accessor_Field.m_ptr = targetPtr + 4;
                bar6_Accessor_Field.CellId = this.CellId;
                return bar6_Accessor_Field;
                
            }
            set
            {
                
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                bar6_Accessor_Field.CellId = this.CellId;
                
                byte* targetPtr = m_ptr;
                {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}
                int length = *(int*)(value.m_ptr - 4);
                int oldlength = *(int*)targetPtr;
                if (value.CellId != bar6_Accessor_Field.CellId)
                {
                    //if not in the same Cell
                    bar6_Accessor_Field.m_ptr = bar6_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.m_ptr - 4, bar6_Accessor_Field.m_ptr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.m_ptr - 4, tmpcellptr, length + 4);
                        bar6_Accessor_Field.m_ptr = bar6_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, bar6_Accessor_Field.m_ptr, length + 4);
                    }
                }

            }
        }
        intListAccessor bar7_Accessor_Field;
        
        ///<summary>
        ///Provides in-place access to the object field bar7.
        ///</summary>
        public unsafe intListAccessor bar7
        {
            get
            {
                
                byte* targetPtr = m_ptr;
                {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}bar7_Accessor_Field.m_ptr = targetPtr + 4;
                bar7_Accessor_Field.CellId = this.CellId;
                return bar7_Accessor_Field;
                
            }
            set
            {
                
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                bar7_Accessor_Field.CellId = this.CellId;
                
                byte* targetPtr = m_ptr;
                {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}
                int length = *(int*)(value.m_ptr - 4);
                int oldlength = *(int*)targetPtr;
                if (value.CellId != bar7_Accessor_Field.CellId)
                {
                    //if not in the same Cell
                    bar7_Accessor_Field.m_ptr = bar7_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.m_ptr - 4, bar7_Accessor_Field.m_ptr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.m_ptr - 4, tmpcellptr, length + 4);
                        bar7_Accessor_Field.m_ptr = bar7_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, bar7_Accessor_Field.m_ptr, length + 4);
                    }
                }

            }
        }
        intListAccessor bar8_Accessor_Field;
        
        ///<summary>
        ///Provides in-place access to the object field bar8.
        ///</summary>
        public unsafe intListAccessor bar8
        {
            get
            {
                
                byte* targetPtr = m_ptr;
                {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}bar8_Accessor_Field.m_ptr = targetPtr + 4;
                bar8_Accessor_Field.CellId = this.CellId;
                return bar8_Accessor_Field;
                
            }
            set
            {
                
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                bar8_Accessor_Field.CellId = this.CellId;
                
                byte* targetPtr = m_ptr;
                {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}
                int length = *(int*)(value.m_ptr - 4);
                int oldlength = *(int*)targetPtr;
                if (value.CellId != bar8_Accessor_Field.CellId)
                {
                    //if not in the same Cell
                    bar8_Accessor_Field.m_ptr = bar8_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.m_ptr - 4, bar8_Accessor_Field.m_ptr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.m_ptr - 4, tmpcellptr, length + 4);
                        bar8_Accessor_Field.m_ptr = bar8_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, bar8_Accessor_Field.m_ptr, length + 4);
                    }
                }

            }
        }
        intListAccessor bar9_Accessor_Field;
        
        ///<summary>
        ///Provides in-place access to the object field bar9.
        ///</summary>
        public unsafe intListAccessor bar9
        {
            get
            {
                
                byte* targetPtr = m_ptr;
                {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}bar9_Accessor_Field.m_ptr = targetPtr + 4;
                bar9_Accessor_Field.CellId = this.CellId;
                return bar9_Accessor_Field;
                
            }
            set
            {
                
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                bar9_Accessor_Field.CellId = this.CellId;
                
                byte* targetPtr = m_ptr;
                {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}
                int length = *(int*)(value.m_ptr - 4);
                int oldlength = *(int*)targetPtr;
                if (value.CellId != bar9_Accessor_Field.CellId)
                {
                    //if not in the same Cell
                    bar9_Accessor_Field.m_ptr = bar9_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.m_ptr - 4, bar9_Accessor_Field.m_ptr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.m_ptr - 4, tmpcellptr, length + 4);
                        bar9_Accessor_Field.m_ptr = bar9_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, bar9_Accessor_Field.m_ptr, length + 4);
                    }
                }

            }
        }
        intListAccessor bar10_Accessor_Field;
        
        ///<summary>
        ///Provides in-place access to the object field bar10.
        ///</summary>
        public unsafe intListAccessor bar10
        {
            get
            {
                
                byte* targetPtr = m_ptr;
                {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}bar10_Accessor_Field.m_ptr = targetPtr + 4;
                bar10_Accessor_Field.CellId = this.CellId;
                return bar10_Accessor_Field;
                
            }
            set
            {
                
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                bar10_Accessor_Field.CellId = this.CellId;
                
                byte* targetPtr = m_ptr;
                {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}
                int length = *(int*)(value.m_ptr - 4);
                int oldlength = *(int*)targetPtr;
                if (value.CellId != bar10_Accessor_Field.CellId)
                {
                    //if not in the same Cell
                    bar10_Accessor_Field.m_ptr = bar10_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.m_ptr - 4, bar10_Accessor_Field.m_ptr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.m_ptr - 4, tmpcellptr, length + 4);
                        bar10_Accessor_Field.m_ptr = bar10_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, bar10_Accessor_Field.m_ptr, length + 4);
                    }
                }

            }
        }
        intListAccessor bar11_Accessor_Field;
        
        ///<summary>
        ///Provides in-place access to the object field bar11.
        ///</summary>
        public unsafe intListAccessor bar11
        {
            get
            {
                
                byte* targetPtr = m_ptr;
                {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}bar11_Accessor_Field.m_ptr = targetPtr + 4;
                bar11_Accessor_Field.CellId = this.CellId;
                return bar11_Accessor_Field;
                
            }
            set
            {
                
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                bar11_Accessor_Field.CellId = this.CellId;
                
                byte* targetPtr = m_ptr;
                {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}
                int length = *(int*)(value.m_ptr - 4);
                int oldlength = *(int*)targetPtr;
                if (value.CellId != bar11_Accessor_Field.CellId)
                {
                    //if not in the same Cell
                    bar11_Accessor_Field.m_ptr = bar11_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.m_ptr - 4, bar11_Accessor_Field.m_ptr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.m_ptr - 4, tmpcellptr, length + 4);
                        bar11_Accessor_Field.m_ptr = bar11_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, bar11_Accessor_Field.m_ptr, length + 4);
                    }
                }

            }
        }
        intListAccessor bar12_Accessor_Field;
        
        ///<summary>
        ///Provides in-place access to the object field bar12.
        ///</summary>
        public unsafe intListAccessor bar12
        {
            get
            {
                
                byte* targetPtr = m_ptr;
                {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}bar12_Accessor_Field.m_ptr = targetPtr + 4;
                bar12_Accessor_Field.CellId = this.CellId;
                return bar12_Accessor_Field;
                
            }
            set
            {
                
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                bar12_Accessor_Field.CellId = this.CellId;
                
                byte* targetPtr = m_ptr;
                {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}
                int length = *(int*)(value.m_ptr - 4);
                int oldlength = *(int*)targetPtr;
                if (value.CellId != bar12_Accessor_Field.CellId)
                {
                    //if not in the same Cell
                    bar12_Accessor_Field.m_ptr = bar12_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.m_ptr - 4, bar12_Accessor_Field.m_ptr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.m_ptr - 4, tmpcellptr, length + 4);
                        bar12_Accessor_Field.m_ptr = bar12_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, bar12_Accessor_Field.m_ptr, length + 4);
                    }
                }

            }
        }
        
        public static unsafe implicit operator C3(C3_Accessor accessor)
        {
            
            return new C3(accessor.CellId
            
            ,
            
                    accessor.bar1
            ,
            
                    accessor.bar2
            ,
            
                    accessor.bar3
            ,
            
                    accessor.bar4
            ,
            
                    accessor.bar5
            ,
            
                    accessor.bar6
            ,
            
                    accessor.bar7
            ,
            
                    accessor.bar8
            ,
            
                    accessor.bar9
            ,
            
                    accessor.bar10
            ,
            
                    accessor.bar11
            ,
            
                    accessor.bar12
            );
        }
        
        public unsafe static implicit operator C3_Accessor(C3 field)
        {
            byte* targetPtr = null;
            
            {

if(field.bar1!= null)
{
    targetPtr += field.bar1.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(field.bar2!= null)
{
    targetPtr += field.bar2.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(field.bar3!= null)
{
    targetPtr += field.bar3.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(field.bar4!= null)
{
    targetPtr += field.bar4.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(field.bar5!= null)
{
    targetPtr += field.bar5.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(field.bar6!= null)
{
    targetPtr += field.bar6.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(field.bar7!= null)
{
    targetPtr += field.bar7.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(field.bar8!= null)
{
    targetPtr += field.bar8.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(field.bar9!= null)
{
    targetPtr += field.bar9.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(field.bar10!= null)
{
    targetPtr += field.bar10.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(field.bar11!= null)
{
    targetPtr += field.bar11.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(field.bar12!= null)
{
    targetPtr += field.bar12.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

            }
            byte* tmpcellptr = BufferAllocator.AllocBuffer((int)targetPtr);
            Memory.memset(tmpcellptr, 0, (ulong)targetPtr);
            targetPtr = tmpcellptr;
            
            {

if(field.bar1!= null)
{
    *(int*)targetPtr = field.bar1.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<field.bar1.Count;++iterator_2)
    {
            *(int*)targetPtr = field.bar1[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(field.bar2!= null)
{
    *(int*)targetPtr = field.bar2.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<field.bar2.Count;++iterator_2)
    {
            *(int*)targetPtr = field.bar2[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(field.bar3!= null)
{
    *(int*)targetPtr = field.bar3.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<field.bar3.Count;++iterator_2)
    {
            *(int*)targetPtr = field.bar3[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(field.bar4!= null)
{
    *(int*)targetPtr = field.bar4.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<field.bar4.Count;++iterator_2)
    {
            *(int*)targetPtr = field.bar4[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(field.bar5!= null)
{
    *(int*)targetPtr = field.bar5.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<field.bar5.Count;++iterator_2)
    {
            *(int*)targetPtr = field.bar5[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(field.bar6!= null)
{
    *(int*)targetPtr = field.bar6.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<field.bar6.Count;++iterator_2)
    {
            *(int*)targetPtr = field.bar6[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(field.bar7!= null)
{
    *(int*)targetPtr = field.bar7.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<field.bar7.Count;++iterator_2)
    {
            *(int*)targetPtr = field.bar7[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(field.bar8!= null)
{
    *(int*)targetPtr = field.bar8.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<field.bar8.Count;++iterator_2)
    {
            *(int*)targetPtr = field.bar8[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(field.bar9!= null)
{
    *(int*)targetPtr = field.bar9.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<field.bar9.Count;++iterator_2)
    {
            *(int*)targetPtr = field.bar9[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(field.bar10!= null)
{
    *(int*)targetPtr = field.bar10.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<field.bar10.Count;++iterator_2)
    {
            *(int*)targetPtr = field.bar10[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(field.bar11!= null)
{
    *(int*)targetPtr = field.bar11.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<field.bar11.Count;++iterator_2)
    {
            *(int*)targetPtr = field.bar11[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(field.bar12!= null)
{
    *(int*)targetPtr = field.bar12.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<field.bar12.Count;++iterator_2)
    {
            *(int*)targetPtr = field.bar12[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

            }C3_Accessor ret;
            
            ret = C3_Accessor._get()._Setup(field.CellId, tmpcellptr, -1, 0, null);
            ret.CellId = field.CellId;
            
            return ret;
        }
        
        public static bool operator ==(C3_Accessor a, C3_Accessor b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;
            if (a.m_ptr == b.m_ptr) return true;
            byte* targetPtr = a.m_ptr;
            {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}
            int lengthA = (int)(targetPtr - a.m_ptr);
            targetPtr = b.m_ptr;
            {targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);targetPtr += *(int*)targetPtr + sizeof(int);}
            int lengthB = (int)(targetPtr - b.m_ptr);
            if(lengthA != lengthB) return false;
            return Memory.Compare(a.m_ptr,b.m_ptr,lengthA);
        }
        public static bool operator != (C3_Accessor a, C3_Accessor b)
        {
            return !(a == b);
        }
        
        public static bool operator ==(C3_Accessor a, C3 b)
        {
            C3_Accessor bb = b;
            return (a == bb);
        }
        public static bool operator !=(C3_Accessor a, C3 b)
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
        internal unsafe C3_Accessor _Lock(long cellId, CellAccessOptions options)
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
                    if (cellType != (ushort)CellType.C3)
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
                        eResult                = Global.LocalStorage.AddOrUse(cellId, defaultContent, ref size, (ushort)CellType.C3, out this.m_ptr, out this.m_cellEntryIndex);
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
        internal unsafe C3_Accessor _Lock(long cellId, CellAccessOptions options, LocalTransactionContext tx)
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
                    if (cellType != (ushort)CellType.C3)
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
                        eResult                = Global.LocalStorage.AddOrUse(tx, cellId, defaultContent, ref size, (ushort)CellType.C3, out this.m_ptr, out this.m_cellEntryIndex);
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
        private class PoolPolicy : IPooledObjectPolicy<C3_Accessor>
        {
            public C3_Accessor Create()
            {
                return new C3_Accessor();
            }
            public bool Return(C3_Accessor obj)
            {
                return !obj.m_IsIterator;
            }
        }
        private static DefaultObjectPool<C3_Accessor> s_accessor_pool = new DefaultObjectPool<C3_Accessor>(new PoolPolicy());
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static C3_Accessor _get() => s_accessor_pool.Get();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void _put(C3_Accessor item) => s_accessor_pool.Return(item);
        /// <summary>
        /// For internal use only.
        /// Caller guarantees that entry lock is obtained.
        /// Does not handle CellAccessOptions. Only copy to the accessor.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal C3_Accessor _Setup(long CellId, byte* cellPtr, int entryIndex, CellAccessOptions options)
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
        internal C3_Accessor _Setup(long CellId, byte* cellPtr, int entryIndex, CellAccessOptions options, LocalTransactionContext tx)
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
        internal static C3_Accessor AllocIterativeAccessor(CellInfo info, LocalTransactionContext tx)
        {
            C3_Accessor accessor = new C3_Accessor();
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
        /// If write-ahead-log behavior is specified on <see cref="Trinity.Extension.StorageExtension_C3.UseC3"/>,
        /// the changes will be committed to the write-ahead log.
        /// </summary>
        public void Dispose()
        {
            if (m_cellEntryIndex >= 0)
            {
                if ((m_options & c_WALFlags) != 0)
                {
                    LocalMemoryStorage.CWriteAheadLog(this.CellId, this.m_ptr, this.CellSize, (ushort)CellType.C3, m_options);
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
        /// <summary>Converts a C3_Accessor to its string representation, in JSON format.</summary>
        /// <returns>A string representation of the C3.</returns>
        public override string ToString()
        {
            return Serializer.ToString(this);
        }
        #endregion
        #region Lookup tables
        internal static StringLookupTable FieldLookupTable = new StringLookupTable(
            
            "bar1"
            ,
            "bar2"
            ,
            "bar3"
            ,
            "bar4"
            ,
            "bar5"
            ,
            "bar6"
            ,
            "bar7"
            ,
            "bar8"
            ,
            "bar9"
            ,
            "bar10"
            ,
            "bar11"
            ,
            "bar12"
            
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
                return TypeConverter<T>.ConvertFrom_List_int(this.bar1);
                
                case 1:
                return TypeConverter<T>.ConvertFrom_List_int(this.bar2);
                
                case 2:
                return TypeConverter<T>.ConvertFrom_List_int(this.bar3);
                
                case 3:
                return TypeConverter<T>.ConvertFrom_List_int(this.bar4);
                
                case 4:
                return TypeConverter<T>.ConvertFrom_List_int(this.bar5);
                
                case 5:
                return TypeConverter<T>.ConvertFrom_List_int(this.bar6);
                
                case 6:
                return TypeConverter<T>.ConvertFrom_List_int(this.bar7);
                
                case 7:
                return TypeConverter<T>.ConvertFrom_List_int(this.bar8);
                
                case 8:
                return TypeConverter<T>.ConvertFrom_List_int(this.bar9);
                
                case 9:
                return TypeConverter<T>.ConvertFrom_List_int(this.bar10);
                
                case 10:
                return TypeConverter<T>.ConvertFrom_List_int(this.bar11);
                
                case 11:
                return TypeConverter<T>.ConvertFrom_List_int(this.bar12);
                
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
                this.bar1 = conversion_result;
            }
            
                }
                break;
                
                case 1:
                {
                    List<int> conversion_result = TypeConverter<T>.ConvertTo_List_int(value);
                    
            {
                this.bar2 = conversion_result;
            }
            
                }
                break;
                
                case 2:
                {
                    List<int> conversion_result = TypeConverter<T>.ConvertTo_List_int(value);
                    
            {
                this.bar3 = conversion_result;
            }
            
                }
                break;
                
                case 3:
                {
                    List<int> conversion_result = TypeConverter<T>.ConvertTo_List_int(value);
                    
            {
                this.bar4 = conversion_result;
            }
            
                }
                break;
                
                case 4:
                {
                    List<int> conversion_result = TypeConverter<T>.ConvertTo_List_int(value);
                    
            {
                this.bar5 = conversion_result;
            }
            
                }
                break;
                
                case 5:
                {
                    List<int> conversion_result = TypeConverter<T>.ConvertTo_List_int(value);
                    
            {
                this.bar6 = conversion_result;
            }
            
                }
                break;
                
                case 6:
                {
                    List<int> conversion_result = TypeConverter<T>.ConvertTo_List_int(value);
                    
            {
                this.bar7 = conversion_result;
            }
            
                }
                break;
                
                case 7:
                {
                    List<int> conversion_result = TypeConverter<T>.ConvertTo_List_int(value);
                    
            {
                this.bar8 = conversion_result;
            }
            
                }
                break;
                
                case 8:
                {
                    List<int> conversion_result = TypeConverter<T>.ConvertTo_List_int(value);
                    
            {
                this.bar9 = conversion_result;
            }
            
                }
                break;
                
                case 9:
                {
                    List<int> conversion_result = TypeConverter<T>.ConvertTo_List_int(value);
                    
            {
                this.bar10 = conversion_result;
            }
            
                }
                break;
                
                case 10:
                {
                    List<int> conversion_result = TypeConverter<T>.ConvertTo_List_int(value);
                    
            {
                this.bar11 = conversion_result;
            }
            
                }
                break;
                
                case 11:
                {
                    List<int> conversion_result = TypeConverter<T>.ConvertTo_List_int(value);
                    
            {
                this.bar12 = conversion_result;
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
                
                case 2:
                
                return true;
                
                case 3:
                
                return true;
                
                case 4:
                
                return true;
                
                case 5:
                
                return true;
                
                case 6:
                
                return true;
                
                case 7:
                
                return true;
                
                case 8:
                
                return true;
                
                case 9:
                
                return true;
                
                case 10:
                
                return true;
                
                case 11:
                
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
                            this.bar1.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.bar1.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.bar1.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                case 1:
                
                {
                    
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.bar2.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.bar2.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.bar2.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                case 2:
                
                {
                    
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.bar3.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.bar3.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.bar3.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                case 3:
                
                {
                    
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.bar4.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.bar4.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.bar4.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                case 4:
                
                {
                    
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.bar5.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.bar5.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.bar5.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                case 5:
                
                {
                    
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.bar6.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.bar6.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.bar6.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                case 6:
                
                {
                    
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.bar7.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.bar7.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.bar7.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                case 7:
                
                {
                    
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.bar8.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.bar8.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.bar8.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                case 8:
                
                {
                    
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.bar9.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.bar9.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.bar9.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                case 9:
                
                {
                    
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.bar10.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.bar10.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.bar10.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                case 10:
                
                {
                    
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.bar11.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.bar11.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.bar11.Add(TypeConverter<T>.ConvertTo_int(value));
                        break;
                        default:
                        Throw.data_type_incompatible_with_list(typeof(T).ToString());
                        break;
                    }
                }
                
                break;
                
                case 11:
                
                {
                    
                    switch (TypeConverter<T>.GetConversionActionTo_List_int())
                    {
                        case TypeConversionAction.TC_ASSIGN:
                        foreach (var element in value as List<int>)
                            this.bar12.Add(element);
                        break;
                        case TypeConversionAction.TC_CONVERTLIST:
                        case TypeConversionAction.TC_ARRAYTOLIST:
                        foreach (var element in TypeConverter<T>.Enumerate_int(value))
                            this.bar12.Add(element);
                        break;
                        case TypeConversionAction.TC_WRAPINLIST:
                        case TypeConversionAction.TC_PARSESTRING:
                        this.bar12.Add(TypeConverter<T>.ConvertTo_int(value));
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
        IEnumerable<KeyValuePair<string, T>> ICell.SelectFields<T>(string attributeKey, string attributeValue)
        {
            switch (TypeConverter<T>.type_id)
            {
                
                case 1:
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar1, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar1", TypeConverter<T>.ConvertFrom_List_int(this.bar1));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar2, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar2", TypeConverter<T>.ConvertFrom_List_int(this.bar2));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar3, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar3", TypeConverter<T>.ConvertFrom_List_int(this.bar3));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar4, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar4", TypeConverter<T>.ConvertFrom_List_int(this.bar4));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar5, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar5", TypeConverter<T>.ConvertFrom_List_int(this.bar5));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar6, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar6", TypeConverter<T>.ConvertFrom_List_int(this.bar6));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar7, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar7", TypeConverter<T>.ConvertFrom_List_int(this.bar7));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar8, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar8", TypeConverter<T>.ConvertFrom_List_int(this.bar8));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar9, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar9", TypeConverter<T>.ConvertFrom_List_int(this.bar9));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar10, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar10", TypeConverter<T>.ConvertFrom_List_int(this.bar10));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar11, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar11", TypeConverter<T>.ConvertFrom_List_int(this.bar11));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar12, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar12", TypeConverter<T>.ConvertFrom_List_int(this.bar12));
                
                break;
                
                case 2:
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar1, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar1", TypeConverter<T>.ConvertFrom_List_int(this.bar1));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar2, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar2", TypeConverter<T>.ConvertFrom_List_int(this.bar2));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar3, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar3", TypeConverter<T>.ConvertFrom_List_int(this.bar3));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar4, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar4", TypeConverter<T>.ConvertFrom_List_int(this.bar4));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar5, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar5", TypeConverter<T>.ConvertFrom_List_int(this.bar5));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar6, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar6", TypeConverter<T>.ConvertFrom_List_int(this.bar6));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar7, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar7", TypeConverter<T>.ConvertFrom_List_int(this.bar7));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar8, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar8", TypeConverter<T>.ConvertFrom_List_int(this.bar8));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar9, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar9", TypeConverter<T>.ConvertFrom_List_int(this.bar9));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar10, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar10", TypeConverter<T>.ConvertFrom_List_int(this.bar10));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar11, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar11", TypeConverter<T>.ConvertFrom_List_int(this.bar11));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar12, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar12", TypeConverter<T>.ConvertFrom_List_int(this.bar12));
                
                break;
                
                case 3:
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar1, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar1", TypeConverter<T>.ConvertFrom_List_int(this.bar1));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar2, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar2", TypeConverter<T>.ConvertFrom_List_int(this.bar2));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar3, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar3", TypeConverter<T>.ConvertFrom_List_int(this.bar3));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar4, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar4", TypeConverter<T>.ConvertFrom_List_int(this.bar4));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar5, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar5", TypeConverter<T>.ConvertFrom_List_int(this.bar5));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar6, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar6", TypeConverter<T>.ConvertFrom_List_int(this.bar6));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar7, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar7", TypeConverter<T>.ConvertFrom_List_int(this.bar7));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar8, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar8", TypeConverter<T>.ConvertFrom_List_int(this.bar8));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar9, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar9", TypeConverter<T>.ConvertFrom_List_int(this.bar9));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar10, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar10", TypeConverter<T>.ConvertFrom_List_int(this.bar10));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar11, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar11", TypeConverter<T>.ConvertFrom_List_int(this.bar11));
                
                if (StorageSchema.C3_descriptor.check_attribute(StorageSchema.C3_descriptor.bar12, attributeKey, attributeValue))
                    
                        yield return new KeyValuePair<string, T>("bar12", TypeConverter<T>.ConvertFrom_List_int(this.bar12));
                
                break;
                
                default:
                Throw.incompatible_with_cell();
                break;
            }
            yield break;
        }
        #region enumerate value methods
        
        private IEnumerable<T> _enumerate_from_bar1<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.bar1;
                            
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
                            
                            var element0 = this.bar1;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar1);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar1);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_bar2<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.bar2;
                            
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
                            
                            var element0 = this.bar2;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar2);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar2);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_bar3<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.bar3;
                            
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
                            
                            var element0 = this.bar3;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar3);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar3);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_bar4<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.bar4;
                            
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
                            
                            var element0 = this.bar4;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar4);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar4);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_bar5<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.bar5;
                            
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
                            
                            var element0 = this.bar5;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar5);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar5);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_bar6<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.bar6;
                            
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
                            
                            var element0 = this.bar6;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar6);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar6);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_bar7<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.bar7;
                            
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
                            
                            var element0 = this.bar7;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar7);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar7);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_bar8<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.bar8;
                            
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
                            
                            var element0 = this.bar8;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar8);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar8);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_bar9<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.bar9;
                            
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
                            
                            var element0 = this.bar9;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar9);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar9);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_bar10<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.bar10;
                            
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
                            
                            var element0 = this.bar10;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar10);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar10);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_bar11<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.bar11;
                            
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
                            
                            var element0 = this.bar11;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar11);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar11);
                        
                    }
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            
        }
        
        private IEnumerable<T> _enumerate_from_bar12<T>()
        {
            
            switch (TypeConverter<T>.type_id)
            {
                
                case 0:
                    {
                        
                        {
                            
                            var element0 = this.bar12;
                            
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
                            
                            var element0 = this.bar12;
                            
                            foreach (var element1 in  element0)
                            
                            {
                                yield return TypeConverter<T>.ConvertFrom_int(element1);
                            }
                        }
                        
                    }
                    break;
                
                case 2:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar12);
                        
                    }
                    break;
                
                case 3:
                    {
                        
                        yield return TypeConverter<T>.ConvertFrom_List_int(this.bar12);
                        
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
                return _enumerate_from_bar1<T>();
                
                case 1:
                return _enumerate_from_bar2<T>();
                
                case 2:
                return _enumerate_from_bar3<T>();
                
                case 3:
                return _enumerate_from_bar4<T>();
                
                case 4:
                return _enumerate_from_bar5<T>();
                
                case 5:
                return _enumerate_from_bar6<T>();
                
                case 6:
                return _enumerate_from_bar7<T>();
                
                case 7:
                return _enumerate_from_bar8<T>();
                
                case 8:
                return _enumerate_from_bar9<T>();
                
                case 9:
                return _enumerate_from_bar10<T>();
                
                case 10:
                return _enumerate_from_bar11<T>();
                
                case 11:
                return _enumerate_from_bar12<T>();
                
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
                
                foreach (var val in _enumerate_from_bar1<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_bar2<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_bar3<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_bar4<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_bar5<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_bar6<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_bar7<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_bar8<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_bar9<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_bar10<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_bar11<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_bar12<T>())
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
                yield return "bar1";
            }
            
            {
                yield return "bar2";
            }
            
            {
                yield return "bar3";
            }
            
            {
                yield return "bar4";
            }
            
            {
                yield return "bar5";
            }
            
            {
                yield return "bar6";
            }
            
            {
                yield return "bar7";
            }
            
            {
                yield return "bar8";
            }
            
            {
                yield return "bar9";
            }
            
            {
                yield return "bar10";
            }
            
            {
                yield return "bar11";
            }
            
            {
                yield return "bar12";
            }
            
        }
        IAttributeCollection ICellDescriptor.GetFieldAttributes(string fieldName)
        {
            return StorageSchema.C3.GetFieldAttributes(fieldName);
        }
        IEnumerable<IFieldDescriptor> ICellDescriptor.GetFieldDescriptors()
        {
            return StorageSchema.C3.GetFieldDescriptors();
        }
        string ITypeDescriptor.TypeName
        {
            get { return StorageSchema.s_cellTypeName_C3; }
        }
        Type ITypeDescriptor.Type
        {
            get { return StorageSchema.s_cellType_C3; }
        }
        bool ITypeDescriptor.IsOfType<T>()
        {
            return typeof(T) == StorageSchema.s_cellType_C3;
        }
        bool ITypeDescriptor.IsList()
        {
            return false;
        }
        IReadOnlyDictionary<string, string> IAttributeCollection.Attributes
        {
            get { return StorageSchema.C3.Attributes; }
        }
        string IAttributeCollection.GetAttributeValue(string attributeKey)
        {
            return StorageSchema.C3.GetAttributeValue(attributeKey);
        }
        ushort ICellDescriptor.CellType
        {
            get
            {
                return (ushort)CellType.C3;
            }
        }
        #endregion
        public ICell Deserialize()
        {
            return (C3)this;
        }
    }
    ///<summary>
    ///Provides interfaces for accessing C3 cells
    ///on <see cref="Trinity.Storage.LocalMemorySotrage"/>.
    static public class StorageExtension_C3
    {
        #region IKeyValueStore non logging
        /// <summary>
        /// Adds a new cell of type C3 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The value of the cell is specified in the method parameters.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.IKeyValueStore"/> instance.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC3(this IKeyValueStore storage, long cellId, List<int> bar1 = default(List<int>), List<int> bar2 = default(List<int>), List<int> bar3 = default(List<int>), List<int> bar4 = default(List<int>), List<int> bar5 = default(List<int>), List<int> bar6 = default(List<int>), List<int> bar7 = default(List<int>), List<int> bar8 = default(List<int>), List<int> bar9 = default(List<int>), List<int> bar10 = default(List<int>), List<int> bar11 = default(List<int>), List<int> bar12 = default(List<int>))
        {
            
            byte* targetPtr;
            
            targetPtr = null;
            
            {

if(bar1!= null)
{
    targetPtr += bar1.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar2!= null)
{
    targetPtr += bar2.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar3!= null)
{
    targetPtr += bar3.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar4!= null)
{
    targetPtr += bar4.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar5!= null)
{
    targetPtr += bar5.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar6!= null)
{
    targetPtr += bar6.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar7!= null)
{
    targetPtr += bar7.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar8!= null)
{
    targetPtr += bar8.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar9!= null)
{
    targetPtr += bar9.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar10!= null)
{
    targetPtr += bar10.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar11!= null)
{
    targetPtr += bar11.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar12!= null)
{
    targetPtr += bar12.Count*4+sizeof(int);
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

if(bar1!= null)
{
    *(int*)targetPtr = bar1.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar1.Count;++iterator_2)
    {
            *(int*)targetPtr = bar1[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar2!= null)
{
    *(int*)targetPtr = bar2.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar2.Count;++iterator_2)
    {
            *(int*)targetPtr = bar2[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar3!= null)
{
    *(int*)targetPtr = bar3.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar3.Count;++iterator_2)
    {
            *(int*)targetPtr = bar3[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar4!= null)
{
    *(int*)targetPtr = bar4.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar4.Count;++iterator_2)
    {
            *(int*)targetPtr = bar4[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar5!= null)
{
    *(int*)targetPtr = bar5.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar5.Count;++iterator_2)
    {
            *(int*)targetPtr = bar5[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar6!= null)
{
    *(int*)targetPtr = bar6.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar6.Count;++iterator_2)
    {
            *(int*)targetPtr = bar6[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar7!= null)
{
    *(int*)targetPtr = bar7.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar7.Count;++iterator_2)
    {
            *(int*)targetPtr = bar7[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar8!= null)
{
    *(int*)targetPtr = bar8.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar8.Count;++iterator_2)
    {
            *(int*)targetPtr = bar8[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar9!= null)
{
    *(int*)targetPtr = bar9.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar9.Count;++iterator_2)
    {
            *(int*)targetPtr = bar9[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar10!= null)
{
    *(int*)targetPtr = bar10.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar10.Count;++iterator_2)
    {
            *(int*)targetPtr = bar10[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar11!= null)
{
    *(int*)targetPtr = bar11.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar11.Count;++iterator_2)
    {
            *(int*)targetPtr = bar11[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar12!= null)
{
    *(int*)targetPtr = bar12.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar12.Count;++iterator_2)
    {
            *(int*)targetPtr = bar12[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

            }
            }
            
            return storage.SaveCell(cellId, tmpcell, (ushort)CellType.C3) == TrinityErrorCode.E_SUCCESS;
        }
        /// <summary>
        /// Adds a new cell of type C3 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The parameter <paramref name="cellId"/> overrides the cell id in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.IKeyValueStore"/> instance.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="cellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC3(this IKeyValueStore storage, long cellId, C3 cellContent)
        {
            return SaveC3(storage, cellId  , cellContent.bar1  , cellContent.bar2  , cellContent.bar3  , cellContent.bar4  , cellContent.bar5  , cellContent.bar6  , cellContent.bar7  , cellContent.bar8  , cellContent.bar9  , cellContent.bar10  , cellContent.bar11  , cellContent.bar12 );
        }
        /// <summary>
        /// Adds a new cell of type C3 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. Cell Id is specified by the CellId field in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.IKeyValueStore"/> instance.</param>
        /// <param name="cellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC3(this IKeyValueStore storage, C3 cellContent)
        {
            return SaveC3(storage, cellContent.CellId  , cellContent.bar1  , cellContent.bar2  , cellContent.bar3  , cellContent.bar4  , cellContent.bar5  , cellContent.bar6  , cellContent.bar7  , cellContent.bar8  , cellContent.bar9  , cellContent.bar10  , cellContent.bar11  , cellContent.bar12 );
        }
        /// <summary>
        /// Loads the content of the specified cell. Any changes done to this object are not written to the store, unless
        /// the content object is saved back into the storage.
        /// <param name="storage"/>A <see cref="Trinity.Storage.IKeyValueStore"/> instance.</param>
        /// </summary>
        public unsafe static C3 LoadC3(this IKeyValueStore storage, long cellId)
        {
            if (TrinityErrorCode.E_SUCCESS == storage.LoadCell(cellId, out var buff))
            {
                fixed (byte* p = buff)
                {
                    return C3_Accessor._get()._Setup(cellId, p, -1, 0);
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
        /// the cell as a C3. Any changes done to the accessor
        /// are written to the storage immediately.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock. Otherwise this method is wait-free.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">The id of the specified cell.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>A <see cref="Trinity.Extension.C3"/> instance.</returns>
        public unsafe static C3_Accessor UseC3(this Trinity.Storage.LocalMemoryStorage storage, long cellId, CellAccessOptions options)
        {
            return C3_Accessor._get()._Lock(cellId, options);
        }
        /// <summary>
        /// Allocate a cell accessor on the specified cell, which inteprets
        /// the cell as a C3. Any changes done to the accessor
        /// are written to the storage immediately.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">The id of the specified cell.</param>
        /// <returns>A <see cref="" + script.RootNamespace + ".C3"/> instance.</returns>
        public unsafe static C3_Accessor UseC3(this Trinity.Storage.LocalMemoryStorage storage, long cellId)
        {
            return C3_Accessor._get()._Lock(cellId, CellAccessOptions.ThrowExceptionOnCellNotFound);
        }
        #endregion
        #region LocalStorage Non-Tx logging
        /// <summary>
        /// Adds a new cell of type C3 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The value of the cell is specified in the method parameters.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC3(this Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions options, long cellId, List<int> bar1 = default(List<int>), List<int> bar2 = default(List<int>), List<int> bar3 = default(List<int>), List<int> bar4 = default(List<int>), List<int> bar5 = default(List<int>), List<int> bar6 = default(List<int>), List<int> bar7 = default(List<int>), List<int> bar8 = default(List<int>), List<int> bar9 = default(List<int>), List<int> bar10 = default(List<int>), List<int> bar11 = default(List<int>), List<int> bar12 = default(List<int>))
        {
            
            byte* targetPtr;
            
            targetPtr = null;
            
            {

if(bar1!= null)
{
    targetPtr += bar1.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar2!= null)
{
    targetPtr += bar2.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar3!= null)
{
    targetPtr += bar3.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar4!= null)
{
    targetPtr += bar4.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar5!= null)
{
    targetPtr += bar5.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar6!= null)
{
    targetPtr += bar6.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar7!= null)
{
    targetPtr += bar7.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar8!= null)
{
    targetPtr += bar8.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar9!= null)
{
    targetPtr += bar9.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar10!= null)
{
    targetPtr += bar10.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar11!= null)
{
    targetPtr += bar11.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar12!= null)
{
    targetPtr += bar12.Count*4+sizeof(int);
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

if(bar1!= null)
{
    *(int*)targetPtr = bar1.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar1.Count;++iterator_2)
    {
            *(int*)targetPtr = bar1[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar2!= null)
{
    *(int*)targetPtr = bar2.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar2.Count;++iterator_2)
    {
            *(int*)targetPtr = bar2[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar3!= null)
{
    *(int*)targetPtr = bar3.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar3.Count;++iterator_2)
    {
            *(int*)targetPtr = bar3[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar4!= null)
{
    *(int*)targetPtr = bar4.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar4.Count;++iterator_2)
    {
            *(int*)targetPtr = bar4[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar5!= null)
{
    *(int*)targetPtr = bar5.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar5.Count;++iterator_2)
    {
            *(int*)targetPtr = bar5[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar6!= null)
{
    *(int*)targetPtr = bar6.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar6.Count;++iterator_2)
    {
            *(int*)targetPtr = bar6[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar7!= null)
{
    *(int*)targetPtr = bar7.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar7.Count;++iterator_2)
    {
            *(int*)targetPtr = bar7[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar8!= null)
{
    *(int*)targetPtr = bar8.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar8.Count;++iterator_2)
    {
            *(int*)targetPtr = bar8[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar9!= null)
{
    *(int*)targetPtr = bar9.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar9.Count;++iterator_2)
    {
            *(int*)targetPtr = bar9[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar10!= null)
{
    *(int*)targetPtr = bar10.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar10.Count;++iterator_2)
    {
            *(int*)targetPtr = bar10[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar11!= null)
{
    *(int*)targetPtr = bar11.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar11.Count;++iterator_2)
    {
            *(int*)targetPtr = bar11[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar12!= null)
{
    *(int*)targetPtr = bar12.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar12.Count;++iterator_2)
    {
            *(int*)targetPtr = bar12[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

            }
            }
            
            return storage.SaveCell(options, cellId, tmpcell, 0, tmpcell.Length, (ushort)CellType.C3) == TrinityErrorCode.E_SUCCESS;
        }
        /// <summary>
        /// Adds a new cell of type C3 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The parameter <paramref name="cellId"/> overrides the cell id in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC3(this Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions options, long cellId, C3 cellContent)
        {
            return SaveC3(storage, options, cellId  , cellContent.bar1  , cellContent.bar2  , cellContent.bar3  , cellContent.bar4  , cellContent.bar5  , cellContent.bar6  , cellContent.bar7  , cellContent.bar8  , cellContent.bar9  , cellContent.bar10  , cellContent.bar11  , cellContent.bar12 );
        }
        /// <summary>
        /// Adds a new cell of type C3 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. Cell Id is specified by the CellId field in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC3(this Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions options, C3 cellContent)
        {
            return SaveC3(storage, options, cellContent.CellId  , cellContent.bar1  , cellContent.bar2  , cellContent.bar3  , cellContent.bar4  , cellContent.bar5  , cellContent.bar6  , cellContent.bar7  , cellContent.bar8  , cellContent.bar9  , cellContent.bar10  , cellContent.bar11  , cellContent.bar12 );
        }
        /// <summary>
        /// Loads the content of the specified cell. Any changes done to this object are not written to the store, unless
        /// the content object is saved back into the storage.
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// </summary>
        public unsafe static C3 LoadC3(this Trinity.Storage.LocalMemoryStorage storage, long cellId)
        {
            using (var cell = C3_Accessor._get()._Lock(cellId, CellAccessOptions.ThrowExceptionOnCellNotFound))
            {
                return cell;
            }
        }
        #endregion
        #region LocalMemoryStorage Tx accessors
        /// <summary>
        /// Allocate a cell accessor on the specified cell, which inteprets
        /// the cell as a C3. Any changes done to the accessor
        /// are written to the storage immediately.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock. Otherwise this method is wait-free.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">The id of the specified cell.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>A <see cref="Trinity.Extension.C3"/> instance.</returns>
        public unsafe static C3_Accessor UseC3(this Trinity.Storage.LocalMemoryStorage storage, LocalTransactionContext tx, long cellId, CellAccessOptions options)
        {
            return C3_Accessor._get()._Lock(cellId, options, tx);
        }
        /// <summary>
        /// Allocate a cell accessor on the specified cell, which inteprets
        /// the cell as a C3. Any changes done to the accessor
        /// are written to the storage immediately.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">The id of the specified cell.</param>
        /// <returns>A <see cref="" + script.RootNamespace + ".C3"/> instance.</returns>
        public unsafe static C3_Accessor UseC3(this Trinity.Storage.LocalMemoryStorage storage, LocalTransactionContext tx, long cellId)
        {
            return C3_Accessor._get()._Lock(cellId, CellAccessOptions.ThrowExceptionOnCellNotFound, tx);
        }
        #endregion
        #region LocalStorage Tx logging
        /// <summary>
        /// Adds a new cell of type C3 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The value of the cell is specified in the method parameters.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC3(this Trinity.Storage.LocalMemoryStorage storage, LocalTransactionContext tx, CellAccessOptions options, long cellId, List<int> bar1 = default(List<int>), List<int> bar2 = default(List<int>), List<int> bar3 = default(List<int>), List<int> bar4 = default(List<int>), List<int> bar5 = default(List<int>), List<int> bar6 = default(List<int>), List<int> bar7 = default(List<int>), List<int> bar8 = default(List<int>), List<int> bar9 = default(List<int>), List<int> bar10 = default(List<int>), List<int> bar11 = default(List<int>), List<int> bar12 = default(List<int>))
        {
            
            byte* targetPtr;
            
            targetPtr = null;
            
            {

if(bar1!= null)
{
    targetPtr += bar1.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar2!= null)
{
    targetPtr += bar2.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar3!= null)
{
    targetPtr += bar3.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar4!= null)
{
    targetPtr += bar4.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar5!= null)
{
    targetPtr += bar5.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar6!= null)
{
    targetPtr += bar6.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar7!= null)
{
    targetPtr += bar7.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar8!= null)
{
    targetPtr += bar8.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar9!= null)
{
    targetPtr += bar9.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar10!= null)
{
    targetPtr += bar10.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar11!= null)
{
    targetPtr += bar11.Count*4+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

if(bar12!= null)
{
    targetPtr += bar12.Count*4+sizeof(int);
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

if(bar1!= null)
{
    *(int*)targetPtr = bar1.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar1.Count;++iterator_2)
    {
            *(int*)targetPtr = bar1[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar2!= null)
{
    *(int*)targetPtr = bar2.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar2.Count;++iterator_2)
    {
            *(int*)targetPtr = bar2[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar3!= null)
{
    *(int*)targetPtr = bar3.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar3.Count;++iterator_2)
    {
            *(int*)targetPtr = bar3[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar4!= null)
{
    *(int*)targetPtr = bar4.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar4.Count;++iterator_2)
    {
            *(int*)targetPtr = bar4[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar5!= null)
{
    *(int*)targetPtr = bar5.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar5.Count;++iterator_2)
    {
            *(int*)targetPtr = bar5[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar6!= null)
{
    *(int*)targetPtr = bar6.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar6.Count;++iterator_2)
    {
            *(int*)targetPtr = bar6[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar7!= null)
{
    *(int*)targetPtr = bar7.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar7.Count;++iterator_2)
    {
            *(int*)targetPtr = bar7[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar8!= null)
{
    *(int*)targetPtr = bar8.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar8.Count;++iterator_2)
    {
            *(int*)targetPtr = bar8[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar9!= null)
{
    *(int*)targetPtr = bar9.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar9.Count;++iterator_2)
    {
            *(int*)targetPtr = bar9[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar10!= null)
{
    *(int*)targetPtr = bar10.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar10.Count;++iterator_2)
    {
            *(int*)targetPtr = bar10[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar11!= null)
{
    *(int*)targetPtr = bar11.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar11.Count;++iterator_2)
    {
            *(int*)targetPtr = bar11[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(bar12!= null)
{
    *(int*)targetPtr = bar12.Count*4;
    targetPtr += sizeof(int);
    for(int iterator_2 = 0;iterator_2<bar12.Count;++iterator_2)
    {
            *(int*)targetPtr = bar12[iterator_2];
            targetPtr += 4;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

            }
            }
            
            return storage.SaveCell(tx, options, cellId, tmpcell, 0, tmpcell.Length, (ushort)CellType.C3) == TrinityErrorCode.E_SUCCESS;
        }
        /// <summary>
        /// Adds a new cell of type C3 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The parameter <paramref name="cellId"/> overrides the cell id in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC3(this Trinity.Storage.LocalMemoryStorage storage, LocalTransactionContext tx, CellAccessOptions options, long cellId, C3 cellContent)
        {
            return SaveC3(storage, tx, options, cellId  , cellContent.bar1  , cellContent.bar2  , cellContent.bar3  , cellContent.bar4  , cellContent.bar5  , cellContent.bar6  , cellContent.bar7  , cellContent.bar8  , cellContent.bar9  , cellContent.bar10  , cellContent.bar11  , cellContent.bar12 );
        }
        /// <summary>
        /// Adds a new cell of type C3 to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. Cell Id is specified by the CellId field in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveC3(this Trinity.Storage.LocalMemoryStorage storage, LocalTransactionContext tx, CellAccessOptions options, C3 cellContent)
        {
            return SaveC3(storage, tx, options, cellContent.CellId  , cellContent.bar1  , cellContent.bar2  , cellContent.bar3  , cellContent.bar4  , cellContent.bar5  , cellContent.bar6  , cellContent.bar7  , cellContent.bar8  , cellContent.bar9  , cellContent.bar10  , cellContent.bar11  , cellContent.bar12 );
        }
        /// <summary>
        /// Loads the content of the specified cell. Any changes done to this object are not written to the store, unless
        /// the content object is saved back into the storage.
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// </summary>
        public unsafe static C3 LoadC3(this Trinity.Storage.LocalMemoryStorage storage, LocalTransactionContext tx, long cellId)
        {
            using (var cell = C3_Accessor._get()._Lock(cellId, CellAccessOptions.ThrowExceptionOnCellNotFound, tx))
            {
                return cell;
            }
        }
        #endregion
    }
}

#pragma warning restore 162,168,649,660,661,1522

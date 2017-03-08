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
using Trinity.Storage;
using Trinity.Utilities;
using Trinity.TSL.Lib;
using Trinity.Network;
using Trinity.Network.Sockets;
using Trinity.Network.Messaging;
using Trinity.TSL;
using System.Text.RegularExpressions;
using Trinity.Core.Lib;

/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
namespace t_Namespace
{
    [TARGET("NStruct")]
    [STRUCT]
    //It is possible to re-map variables on the fly.
    [MAP_LIST("t_field", "node->fieldList")]
    [MAP_VAR("t_field", "")]
    [MAP_VAR("t_field_name", "name")]
    [MAP_VAR("t_field_type", "fieldType")]
    [MAP_VAR("t_field_type_display", "Trinity::Codegen::GetDataTypeDisplayString($$->fieldType)")]
    [MAP_VAR("t_struct_name", "node->name")]
    [META_VAR("bool", "struct_nonempty", "node->fieldList->size() > 0")]
    [META_VAR("bool", "struct_fixed", "node->layoutType == LT_FIXED")]
    [META_VAR("OptionalFieldCalculator", "optcalc", "OptionalFieldCalculator(node)")]
    [MAP_VAR("t_int", "%optcalc.headerLength")]
    /// <summary>
    /// A .NET runtime object representation of t_struct_name defined in TSL.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public partial class t_struct_name : __meta
    {
        // Only generate constructor when it contains fields
        [IF("%struct_nonempty")]
        ///<summary>
        ///Initializes a new instance of t_struct_name with the specified parameters.
        ///</summary>
        public t_struct_name(/*FOREACH(",")*/t_field_type t_field_name = default(t_field_type)/*END*/)
        {
            FOREACH();
            this.t_field_name = t_field_name;
            END();
        }
        [END]

        public static bool operator ==(t_struct_name a, t_struct_name b)
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

        public static bool operator !=(t_struct_name a, t_struct_name b)
        {
            return !(a == b);
        }

        [FOREACH]
        public t_field_type t_field_name;
        [END]

        /// <summary>
        /// Converts the string representation of a t_struct_name to its
        /// struct equivalent. A return value indicates whether the 
        /// operation succeeded.
        /// </summary>
        /// <param name="input">A string to convert.</param>
        /// <param name="value">
        /// When this method returns, contains the struct equivalent of the value contained 
        /// in input, if the conversion succeeded, or default(t_struct_name) if the conversion
        /// failed. The conversion fails if the input parameter is null or String.Empty, or is 
        /// not of the correct format. This parameter is passed uninitialized. 
        /// </param>
        /// <returns>True if input was converted successfully; otherwise, false.</returns>
        public unsafe static bool TryParse(string input, out t_struct_name value)
        {
            try
            {
                value = Newtonsoft.Json.JsonConvert.DeserializeObject<t_struct_name>(input);
                return true;
            }
            catch { value = default(t_struct_name); return false; }
        }

        public static t_struct_name Parse(string input)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<t_struct_name>(input);
        }


        /// <summary>
        /// Serializes this object to a Json string.
        /// </summary>
        /// <returns>The Json string serialized.</returns>
        public override string ToString()
        {
            return Serializer.ToString(this);
        }
    }

    /// <summary>
    /// Provides in-place operations of t_struct_name defined in TSL.
    /// </summary>
    public unsafe class t_struct_name_Accessor : __meta
    {
        ///<summary>
        ///The pointer to the content of the object.
        ///</summary>
        internal byte* CellPtr;
        internal long? CellID;

        [IF("!%struct_fixed")]
        internal ResizeFunctionDelegate ResizeFunction;
        [END]

        internal unsafe t_struct_name_Accessor(byte* _CellPtr
            /*IF("!%struct_fixed")*/
            , ResizeFunctionDelegate func
            /*END*/)
        {
            CellPtr = _CellPtr;
            ResizeFunction = func;
            //foreach (var field in structDesc.Fields)
            //{
            //    ret += TSLCompiler.GenerateAccessorFieldAssignmentCode(new DynamicStructFieldType(structDesc), field.Type, isReadOnly, field.Name + "_Accessor_Field", false);
            //}
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
                    /*FOREACH(",")*/
                    /*META("if($t_field->is_optional())continue;")*/
                    "t_field_name"
                    /*END*/
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
            IF("%optcalc.headerLength > 0");
            byte [] bytes = new byte[t_int];
            Memory.Copy(CellPtr, 0, bytes, 0, t_int);
            return bytes;
            ELSE();
            return new byte[0];
            END();
        }

        ///<summary>
        ///Copies the struct content into a byte array.
        ///</summary>
        public byte[] ToByteArray()
        {
            byte* targetPtr = CellPtr;
            //ret += AccessorCodeTemplate.GenerateFieldPushPointerCode(structDesc, structDesc.Fields.Count, "this");
            int size = (int)(targetPtr - CellPtr);
            byte[] ret = new byte[size];
            Memory.Copy(CellPtr,0,ret,0,size);
            return ret;
        }

        //ret += AccessorCodeTemplate.GenerateFieldPropertiesCode(structDesc, isReadOnly);

        //    ret += ImplicitOperatorCodeTemplate.GenerateFormatImplicitOperatorCode(structDesc, isReadOnly, !structDesc.IsFixed());

        //    ret += EqualOperatorCodeTemplate.GenerateFormatEqualOperatorCode(structDesc, isReadOnly);
    }
}

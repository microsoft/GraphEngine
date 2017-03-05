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

/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
namespace t_Namespace
{
    [TARGET("NStruct")]
    [STRUCT]
    //It is possible to re-map variables on the fly.
    [MAP_LIST("t_field", "node->fieldList")]
    [MAP_VAR("t_field_name", "name")]
    [MAP_VAR("t_field_type", "fieldType")]
    [MAP_VAR("t_field_type_display", "Trinity::Codegen::GetDataTypeDisplayString($$->fieldType)")]

    [MAP_VAR("t_struct_name", "node->name")]

    /// <summary>
    /// A .NET runtime object representation of t_struct_name defined in TSL.
    /// </summary>
    public partial class t_struct_name : __meta
    {
        #region MUTE
        [MUTE]
        [FOREACH]
        public t_field_type t_field_name;
        /*END*/
        /*MUTE_END*/
        #endregion // MUTE
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
        public static bool TryParse(string input, out t_struct_name value)
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
}

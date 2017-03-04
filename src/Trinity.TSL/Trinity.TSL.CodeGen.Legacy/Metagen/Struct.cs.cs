using System.Text;
using System.Collections.Generic;

namespace Trinity.TSL.CodeTemplates
{
    internal partial class SourceFiles
    {
        internal static string 
Struct(
NStruct node)
        {
            StringBuilder source = new StringBuilder();
            
source.Append(@"
namespace ");
source.Append(Codegen.GetString(Trinity::Codegen::GetNamespace()));
source.Append(@"
{
    /// <summary>
    /// A .NET runtime object representation of ");
source.Append(Codegen.GetString(node->name));
source.Append(@" defined in TSL.
    /// </summary>
    public partial struct ");
source.Append(Codegen.GetString(node->name));
source.Append(@"
    {
        #region MUTE
        
        #endregion 
        /// <summary>
        /// Converts the string representation of a ");
source.Append(Codegen.GetString(node->name));
source.Append(@" to its
        /// struct equivalent. A return value indicates whether the 
        /// operation succeeded.
        /// </summary>
        /// <param name=""input"">A string to convert.</param>
        /// <param name=""value"">
        /// When this method returns, contains the struct equivalent of the value contained 
        /// in input, if the conversion succeeded, or default(");
source.Append(Codegen.GetString(node->name));
source.Append(@") if the conversion
        /// failed. The conversion fails if the input parameter is null or String.Empty, or is 
        /// not of the correct format. This parameter is passed uninitialized. 
        /// </param>
        /// <returns>True if input was converted successfully; otherwise, false.</returns>
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
");

            return source.ToString();
        }
    }
}

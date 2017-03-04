#include "common.h"
#include <string>
#include <SyntaxNode.h>

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
Struct(
NStruct* node)
        {
            string* source = new string();
            
source->append(R"::(
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(
{
    /// <summary>
    /// A .NET runtime object representation of )::");
source->append(Codegen::GetString(node->name));
source->append(R"::( defined in TSL.
    /// </summary>
    public partial struct )::");
source->append(Codegen::GetString(node->name));
source->append(R"::(
    {
        #region MUTE
        
        #endregion 
        /// <summary>
        /// Converts the string representation of a )::");
source->append(Codegen::GetString(node->name));
source->append(R"::( to its
        /// struct equivalent. A return value indicates whether the 
        /// operation succeeded.
        /// </summary>
        /// <param name="input">A string to convert.</param>
        /// <param name="value">
        /// When this method returns, contains the struct equivalent of the value contained 
        /// in input, if the conversion succeeded, or default()::");
source->append(Codegen::GetString(node->name));
source->append(R"::() if the conversion
        /// failed. The conversion fails if the input parameter is null or String.Empty, or is 
        /// not of the correct format. This parameter is passed uninitialized. 
        /// </param>
        /// <returns>True if input was converted successfully; otherwise, false.</returns>
        public static bool TryParse(string input, out )::");
source->append(Codegen::GetString(node->name));
source->append(R"::( value)
        {
            try
            {
                value = Newtonsoft.Json.JsonConvert.DeserializeObject<)::");
source->append(Codegen::GetString(node->name));
source->append(R"::(>(input);
                return true;
            }
            catch { value = default()::");
source->append(Codegen::GetString(node->name));
source->append(R"::(); return false; }
        }
        public static )::");
source->append(Codegen::GetString(node->name));
source->append(R"::( Parse(string input)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<)::");
source->append(Codegen::GetString(node->name));
source->append(R"::(>(input);
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
)::");

            return source;
        }
    }
}

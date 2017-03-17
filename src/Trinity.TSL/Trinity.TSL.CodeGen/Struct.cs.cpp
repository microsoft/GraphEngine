#include "common.h"
#include <string>
#include "SyntaxNode.h"

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
            
source->append(R"::(using System;
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
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(
{
    )::");
bool struct_nonempty_1 = node->fieldList->size() > 0;
bool struct_fixed_1 = node->layoutType == LT_FIXED;
source->append(R"::(
    /// <summary>
    /// A .NET runtime object representation of )::");
source->append(Codegen::GetString(node->name));
source->append(R"::( defined in TSL.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public partial struct )::");
source->append(Codegen::GetString(node->name));
source->append(R"::(
    {
        )::");
if (struct_nonempty_1)
{
source->append(R"::(
        ///<summary>
        ///Initializes a new instance of )::");
source->append(Codegen::GetString(node->name));
source->append(R"::( with the specified parameters.
        ///</summary>
        public )::");
source->append(Codegen::GetString(node->name));
source->append(R"::(()::");
for (size_t iterator_1 = 0; iterator_1 < (node->fieldList)->size();++iterator_1)
{
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->fieldType));
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::( = default()::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->fieldType));
source->append(R"::())::");
if (iterator_1 < (node->fieldList)->size() - 1)
source->append(",");
}
source->append(R"::()
        {
            )::");
for (size_t iterator_1 = 0; iterator_1 < (node->fieldList)->size();++iterator_1)
{
source->append(R"::(
            this.)::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::( = )::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::(;
            )::");
}
source->append(R"::(
        }
        )::");
}
source->append(R"::(
        public static bool operator ==()::");
source->append(Codegen::GetString(node->name));
source->append(R"::( a, )::");
source->append(Codegen::GetString(node->name));
source->append(R"::( b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }
            )::");
if (struct_nonempty_1)
{
source->append(R"::(
            return
                )::");
for (size_t iterator_1 = 0; iterator_1 < (node->fieldList)->size();++iterator_1)
{
source->append(R"::(
                (a.)::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::( == b.)::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::()
                )::");
if (iterator_1 < (node->fieldList)->size() - 1)
source->append("&&");
}
source->append(R"::(
                ;
            )::");
}
else
{
source->append(R"::(
            return true;
            )::");
}
source->append(R"::(
        }
        public static bool operator !=()::");
source->append(Codegen::GetString(node->name));
source->append(R"::( a, )::");
source->append(Codegen::GetString(node->name));
source->append(R"::( b)
        {
            return !(a == b);
        }
        )::");
for (size_t iterator_1 = 0; iterator_1 < (node->fieldList)->size();++iterator_1)
{
source->append(R"::(
        public )::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->fieldType));
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::(;
        )::");
}
source->append(R"::(
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
        public unsafe static bool TryParse(string input, out )::");
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
    /// <summary>
    /// Provides in-place operations of )::");
source->append(Codegen::GetString(node->name));
source->append(R"::( defined in TSL.
    /// </summary>
    public unsafe partial class )::");
source->append(Codegen::GetString(node->name));
source->append(R"::(_Accessor
    {
        ///<summary>
        ///The pointer to the content of the object.
        ///</summary>
        internal byte* CellPtr;
        internal long? CellID;
        )::");
if (!struct_fixed_1)
{
source->append(R"::(
        internal ResizeFunctionDelegate ResizeFunction;
        )::");
}
source->append(R"::(
        internal unsafe )::");
source->append(Codegen::GetString(node->name));
source->append(R"::(_Accessor(byte* _CellPtr
            )::");
if (!struct_fixed_1)
{
source->append(R"::(
            , ResizeFunctionDelegate func
            )::");
}
source->append(R"::()
        {
            CellPtr = _CellPtr;
            )::");
if (!struct_fixed_1)
{
source->append(R"::(
            ResizeFunction = func;
            )::");
}
for (size_t iterator_1 = 0; iterator_1 < (node->fieldList)->size();++iterator_1)
{

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = 0;
std::string* module_content = Modules::StructFieldAccessorInitialization((*(node->fieldList))[iterator_1], &module_ctx);
    source->append(*module_content);
    delete module_content;
}
}
source->append(R"::(
        }
        )::");

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = 0;
std::string* module_content = Modules::OptionalFields(node, &module_ctx);
    source->append(*module_content);
    delete module_content;
}
source->append(R"::(
        ///<summary>
        ///Copies the struct content into a byte array.
        ///</summary>
        public byte[] ToByteArray()
        {
            byte* targetPtr = CellPtr;
            )::");

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = 0;
std::string* module_content = Modules::PushPointerThroughStruct(node, &module_ctx);
    source->append(*module_content);
    delete module_content;
}
source->append(R"::(
            int size = (int)(targetPtr - CellPtr);
            byte[] ret = new byte[size];
            Memory.Copy(CellPtr, 0, ret, 0, size);
            return ret;
        }
        )::");

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = 0;
std::string* module_content = Modules::AccessorFieldsDefinition(node, &module_ctx);
    source->append(*module_content);
    delete module_content;
}
source->append(R"::(
        public static unsafe implicit operator )::");
source->append(Codegen::GetString(node->name));
source->append(R"::(()::");
source->append(Codegen::GetString(node->name));
source->append(R"::(_Accessor accessor)
        {
            )::");
for (size_t iterator_1 = 0; iterator_1 < (node->fieldList)->size();++iterator_1)
{
if ((*(node->fieldList))[iterator_1]->is_optional())
{
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->fieldType));
source->append(R"::( _)::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::( = default()::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->fieldType));
source->append(R"::();
            if (accessor.Contains_)::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::()
            {
                )::");
if ((*(node->fieldList))[iterator_1]->fieldType->is_value_type())
{
source->append(R"::(
                _)::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::( = ()::");
source->append(Codegen::GetString(Trinity::Codegen::GetNonNullableValueTypeString((*(node->fieldList))[iterator_1]->fieldType)));
source->append(R"::()accessor.)::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::(;
                )::");
}
else
{
source->append(R"::(
                _)::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::( = accessor.)::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::(;
                )::");
}
source->append(R"::(
            }
            )::");
}
}
source->append(R"::(
            return new )::");
source->append(Codegen::GetString(node->name));
source->append(R"::((
                )::");
for (size_t iterator_1 = 0; iterator_1 < (node->fieldList)->size();++iterator_1)
{
if ((*(node->fieldList))[iterator_1]->is_optional())
{
source->append(R"::(
                        _)::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
}
else
{
source->append(R"::(
                        accessor.)::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
}
if (iterator_1 < (node->fieldList)->size() - 1)
source->append(",");
}
source->append(R"::(
                );
        }
        )::");

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = 0;
std::string* module_content = Modules::StructAccessorReverseImplicitOperator(node, &module_ctx);
    source->append(*module_content);
    delete module_content;
}

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = 0;
std::string* module_content = Modules::StructAccessorEqualOperator(node, &module_ctx);
    source->append(*module_content);
    delete module_content;
}
source->append(R"::(
    }
}
)::");

            return source;
        }
    }
}

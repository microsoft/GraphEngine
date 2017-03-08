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
OptionalFieldCalculator optcalc_1 = OptionalFieldCalculator(node);
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
    public unsafe class )::");
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
            ResizeFunction = func;
        }
        internal static string[] optional_field_names = null;
        ///<summary>
        ///Get an array of the names of all optional fields for object type )::");
source->append(Codegen::GetString(node->name));
source->append(R"::(.
        ///</summary>
        public static string[] GetOptionalFieldNames()
        {
            if (optional_field_names == null)
                optional_field_names = new string[]
                {
                    )::");
for (size_t iterator_1 = 0; iterator_1 < (node->fieldList)->size();++iterator_1)
{
if((*(node->fieldList))[iterator_1]->is_optional())continue;
source->append(R"::(
                    ")::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::("
                    )::");
if (iterator_1 < (node->fieldList)->size() - 1)
source->append(",");
}
source->append(R"::(
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
            )::");
if (optcalc_1.headerLength > 0)
{
source->append(R"::(
            byte [] bytes = new byte[)::");
source->append(Codegen::GetString(optcalc_1.headerLength));
source->append(R"::(];
            Memory.Copy(CellPtr, 0, bytes, 0, )::");
source->append(Codegen::GetString(optcalc_1.headerLength));
source->append(R"::();
            return bytes;
            )::");
}
else
{
source->append(R"::(
            return new byte[0];
            )::");
}
source->append(R"::(
        }
        ///<summary>
        ///Copies the struct content into a byte array.
        ///</summary>
        public byte[] ToByteArray()
        {
            byte* targetPtr = CellPtr;
            int size = (int)(targetPtr - CellPtr);
            byte[] ret = new byte[size];
            Memory.Copy(CellPtr,0,ret,0,size);
            return ret;
        }
    }
}
)::");

            return source;
        }
    }
}

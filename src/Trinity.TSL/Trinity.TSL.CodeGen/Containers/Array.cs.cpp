#include "common.h"
#include <string>
#include <SyntaxNode.h>

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
Array(
NFieldType* node)
        {
            string* source = new string();
            
source->append(R"::(using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.TSL;
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(
{
    public unsafe class )::");
source->append(Codegen::GetString(node));
source->append(R"::( : IEnumerable<)::");
source->append(Codegen::GetString(data_type_get_accessor_name(node->arrayInfo.arrayElement)));
source->append(R"::(>
    {
        )::");
for (size_t iterator_1 = 0; iterator_1 < (node->arrayInfo.array_dimension_size)->size();++iterator_1)
{
source->append(R"::(
        private static readonly int SizeDim)::");
source->append(Codegen::GetString(Discard((*(node->arrayInfo.array_dimension_size))[iterator_1]) + GetString(iterator_1)));
source->append(R"::( = )::");
source->append(Codegen::GetString((*(node->arrayInfo.array_dimension_size))[iterator_1]));
source->append(R"::(;
        )::");
}
source->append(R"::(
        /// <summary>
        /// Gets the rank (number of dimentsions) of the Array.
        /// </summary>
        public static readonly int Rank = )::");
source->append(Codegen::GetString(node->arrayInfo.array_dimension_size->size()));
source->append(R"::(;
        internal byte* CellPtr;
        internal long? CellID;
        internal )::");
source->append(Codegen::GetString(node));
source->append(R"::((byte* _CellPtr)
        {
            this.CellPtr = _CellPtr;
        }
        /// <summary>
        /// Gets the total number of elements
        /// </summary>
        public int Length
        {
            get
            {
                return
                    /*)::");
for (size_t iterator_1 = 0; iterator_1 < (node->arrayInfo.array_dimension_size)->size();++iterator_1)
{
source->append(R"::(*/
                    SizeDim)::");
source->append(Codegen::GetString(Discard((*(node->arrayInfo.array_dimension_size))[iterator_1]) + GetString(iterator_1)));
if (iterator_1 < (node->arrayInfo.array_dimension_size)->size() - 1)
source->append("*");
}
source->append(R"::(
                    ;
            }
        }
        /// <summary>
        /// Returns a byte array that contains the value of this instance.
        /// </summary>
        /// <returns>a byte array.</returns>
        public unsafe byte[] ToByteArray()
        {
            byte[] ret = new byte[Length * )::");
source->append(Codegen::GetString(node->arrayInfo.arrayElement->type_size()));
source->append(R"::(];
            fixed (byte* ptr = ret)
            {
                Memory.Copy(CellPtr, ptr, Length * )::");
source->append(Codegen::GetString(node->arrayInfo.arrayElement->type_size()));
source->append(R"::();
            }
            return ret;
        }
        )::");
if (data_type_need_accessor(node->arrayInfo.arrayElement))
{
source->append(Codegen::GetString(data_type_get_accessor_name(node->arrayInfo.arrayElement)));
source->append(R"::( elementAccessor;
        )::");
}
source->append(R"::(
        /// <summary>
        /// Gets or sets the element at the specified index
        /// </summary>
        /// <returns>Corresponding element at the specified index</returns>
        public unsafe )::");
source->append(Codegen::GetString(data_type_get_accessor_name(node->arrayInfo.arrayElement)));
source->append(R"::( this[
            )::");
for (size_t iterator_1 = 0; iterator_1 < (node->arrayInfo.array_dimension_size)->size();++iterator_1)
{
source->append(R"::(
            int indexDim)::");
source->append(Codegen::GetString(Discard((*(node->arrayInfo.array_dimension_size))[iterator_1]) + GetString(iterator_1)));
if (iterator_1 < (node->arrayInfo.array_dimension_size)->size() - 1)
source->append(",");
}
source->append(R"::(
            ]
        {
            get
            {
                byte* offset = CellPtr + 
                    )::");
for (size_t iterator_1 = 0; iterator_1 < (node->arrayInfo.array_dimension_size)->size();++iterator_1)
{
source->append(R"::(
                    indexDim)::");
source->append(Codegen::GetString(Discard((*(node->arrayInfo.array_dimension_size))[iterator_1]) + GetString(iterator_1)));
source->append(R"::( * )::");
source->append(Codegen::GetString(([&](int idx){int offset = 1;for(int i=idx + 1;i<node->arrayInfo.array_dimension_size->size();++i){offset *= (*(node->arrayInfo.array_dimension_size))[i];} return offset;})(iterator_1)));
if (iterator_1 < (node->arrayInfo.array_dimension_size)->size() - 1)
source->append("+");
}
source->append(R"::(
                    ;
                )::");
if (data_type_need_accessor(node->arrayInfo.arrayElement))
{
source->append(R"::(
                {
                    elementAccessor.CellPtr = offset;
                    return elementAccessor;
                }
                )::");
}
else
{
source->append(R"::(
                {
                    return *(data_type_get_accessor_name(node->arrayInfo.arrayElement)*)offset;
                }
                )::");
}
source->append(R"::(
            }
            set
            {
                byte* offset = CellPtr + 
                    )::");
for (size_t iterator_1 = 0; iterator_1 < (node->arrayInfo.array_dimension_size)->size();++iterator_1)
{
source->append(R"::(
                    indexDim)::");
source->append(Codegen::GetString(Discard((*(node->arrayInfo.array_dimension_size))[iterator_1]) + GetString(iterator_1)));
source->append(R"::( * )::");
source->append(Codegen::GetString(([&](int idx){int offset = 1;for(int i=idx + 1;i<node->arrayInfo.array_dimension_size->size();++i){offset *= (*(node->arrayInfo.array_dimension_size))[i];} return offset;})(iterator_1)));
if (iterator_1 < (node->arrayInfo.array_dimension_size)->size() - 1)
source->append("+");
}
source->append(R"::(
                    ;
                )::");
if (data_type_need_accessor(node->arrayInfo.arrayElement))
{
source->append(R"::(
                {
                    if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                    Memory.Copy(value.CellPtr, offset, )::");
source->append(Codegen::GetString(node->arrayInfo.arrayElement->type_size()));
source->append(R"::();
                }
                )::");
}
else
{
source->append(R"::(
                {
                    *(data_type_get_accessor_name(node->arrayInfo.arrayElement)*)offset = value;
                }
                )::");
}
source->append(R"::(
            }
        }
        #region Foreach
        /// <summary>
        /// Performs the specified action on each element
        /// </summary>
        /// <param name=""action"">A lambda expression which has one parameter indicates element in array</param>
        public unsafe void ForEach(Action<)::");
source->append(Codegen::GetString(data_type_get_accessor_name(node->arrayInfo.arrayElement)));
source->append(R"::(> action)
        {
            byte* targetPtr = CellPtr;
            byte* endPtr = CellPtr + Length * )::");
source->append(Codegen::GetString(node->arrayInfo.arrayElement->type_size()));
source->append(R"::(;
            while (targetPtr < endPtr)
            {
                )::");
if (data_type_need_accessor(node->arrayInfo.arrayElement))
{
source->append(R"::(
                {
                    elementAccessor.CellPtr = targetPtr;
                    action(elementAccessor);
                    targetPtr += )::");
source->append(Codegen::GetString(node->arrayInfo.arrayElement->type_size()));
source->append(R"::(;
                }
                )::");
}
else
{
source->append(R"::(
                {
                    action(*(data_type_get_accessor_name(node->arrayInfo.arrayElement)*)targetPtr);
                    targetPtr += )::");
source->append(Codegen::GetString(node->arrayInfo.arrayElement->type_size()));
source->append(R"::(;
                }
                )::");
}
source->append(R"::(
            }
        }
        internal unsafe struct _iterator
        {
            byte* targetPtr;
            byte* endPtr;
            )::");
source->append(Codegen::GetString(node));
source->append(R"::( target;
            internal _iterator()::");
source->append(Codegen::GetString(node));
source->append(R"::( target)
            {
                targetPtr   = target.CellPtr;
                endPtr      = target.CellPtr + target.Length * )::");
source->append(Codegen::GetString(node->arrayInfo.arrayElement->type_size()));
source->append(R"::(;
                this.target = target;
            }
            internal bool good()
            {
                return (targetPtr < endPtr);
            }
            internal )::");
source->append(Codegen::GetString(data_type_get_accessor_name(node->arrayInfo.arrayElement)));
source->append(R"::( current()
            {
                )::");
if (data_type_need_accessor(node->arrayInfo.arrayElement))
{
source->append(R"::(
                {
                    target.elementAccessor.CellPtr = targetPtr;
                    return target.elementAccessor;
                }
                )::");
}
else
{
source->append(R"::(
                {
                    return (*(data_type_get_accessor_name(node->arrayInfo.arrayElement)*)targetPtr);
                }
                )::");
}
source->append(R"::(
            }
            internal void move_next()
            {
                targetPtr += )::");
source->append(Codegen::GetString(node->arrayInfo.arrayElement->type_size()));
source->append(R"::(;
            }
        }
        public IEnumerator<)::");
source->append(Codegen::GetString(data_type_get_accessor_name(node->arrayInfo.arrayElement)));
source->append(R"::(> GetEnumerator()
        {
            _iterator _it = new _iterator(this);
            while (_it.good())
            {
                yield return _it.current();
                _it.move_next();
            }
        }
        unsafe IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
        /// <summary>
        /// Sets a range of elements in the Array to zero, to false, or to null, depending on the element type.
        /// </summary>
        /// <param name=""index"">The starting index of the range of elements to clear.</param>
        /// <param name=""length"">The number of elements to clear.</param>
        public unsafe void Clear(int index, int length)
        {
            if (index < 0 || length < 0 ||index >= Length || index+length > Length) throw new IndexOutOfRangeException();
            Memory.memset(CellPtr + index* )::");
source->append(Codegen::GetString(node->arrayInfo.arrayElement->type_size()));
source->append(R"::(, 0, (ulong)(length * )::");
source->append(Codegen::GetString(node->arrayInfo.arrayElement->type_size()));
source->append(R"::());
        }
        public static bool operator == ()::");
source->append(Codegen::GetString(node));
source->append(R"::( a, )::");
source->append(Codegen::GetString(node));
source->append(R"::( b)
        {
            if (ReferenceEquals(a, b))
              return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
              return false;
            if (a.CellPtr == b.CellPtr) return true;
            return Memory.Compare(a.CellPtr, b.CellPtr, a.Length * )::");
source->append(Codegen::GetString(node->arrayInfo.arrayElement->type_size()));
source->append(R"::();
        }
        public static bool operator != ()::");
source->append(Codegen::GetString(node));
source->append(R"::( a,)::");
source->append(Codegen::GetString(node));
source->append(R"::( b)
        {
            return !(a == b);
        }
    }
}
)::");

            return source;
        }
    }
}

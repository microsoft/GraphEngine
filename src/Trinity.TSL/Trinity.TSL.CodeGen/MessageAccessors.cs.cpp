#include "common.h"
#include <string>
#include "SyntaxNode.h"

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
MessageAccessors(
std::vector<NStructBase*>* node)
        {
            string* source = new string();
            
source->append(R"::(using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.Network.Messaging;
using Trinity.Storage;
using Trinity.TSL;
using Trinity.TSL.Lib;
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(
{
    )::");
std::string base_accessor_name_1;
for (size_t iterator_1 = 0; iterator_1 < (node)->size();++iterator_1)
{
if ((*(node))[iterator_1]->is_struct())
{
base_accessor_name_1 = *(*(node))[iterator_1]->name + "_Accessor";
}
else
{
base_accessor_name_1 = *(*(node))[iterator_1]->name + "_Message_Accessor";
}
source->append(R"::(
    /// <summary>
    /// Represents a read-only accessor on the message of type )::");
source->append(Codegen::GetString((*(node))[iterator_1]->name));
source->append(R"::( defined in the TSL protocols.
    /// The message readers will be instantiated by the system and passed to user's logic.
    /// After finished accessing the message. It is the user's responsibility to call Dispose()
    /// on the reader object. Recommend wrapping the reader with a <c>using Statement block</c>.
    /// <seealso ref="https://msdn.microsoft.com/en-us/library/yh598w02.aspx"/>
    /// </summary>
    public unsafe sealed class )::");
source->append(Codegen::GetString((*(node))[iterator_1]->name));
source->append(R"::(Reader : )::");
source->append(Codegen::GetString(base_accessor_name_1));
source->append(R"::(, IDisposable
    {
        byte * buffer;
        internal )::");
source->append(Codegen::GetString((*(node))[iterator_1]->name));
source->append(R"::(Reader(byte* buf, int offset)
            : base(buf + offset
                  )::");
if ((*(node))[iterator_1]->getLayoutType() != LT_FIXED)
{
source->append(R"::(
                  , ReaderResizeFunc
                  )::");
}
source->append(R"::( )
        {
            buffer = buf;
        }
        )::");
if ((*(node))[iterator_1]->getLayoutType() != LT_FIXED)
{
source->append(R"::(
        /** 
         * )::");
source->append(Codegen::GetString((*(node))[iterator_1]->name));
source->append(R"::(Reader is not resizable because it may be attached
         * to a buffer passed in from the network layer and we don't know how
         * to resize it.
         */
        static byte* ReaderResizeFunc(byte* ptr, int offset, int delta)
        {
            throw new InvalidOperationException(")::");
source->append(Codegen::GetString((*(node))[iterator_1]->name));
source->append(R"::(Reader is not resizable");
        }
        )::");
}
source->append(R"::(
        /// <summary>
        /// Dispose the message reader and release the memory resource.
        /// It is the user's responsibility to call this method after finished accessing the message.
        /// </summary>
        public void Dispose()
        {
            Memory.free(buffer);
            buffer = null;
        }
    }
    /// <summary>
    /// Represents a writer accessor on the message of type )::");
source->append(Codegen::GetString((*(node))[iterator_1]->name));
source->append(R"::( defined in the TSL protocols.
    /// The message writers should be instantiated by the user's logic and passed to the system to send it out.
    /// After finished accessing the message. It is the user's responsibility to call Dispose()
    /// on the writer object. Recommend wrapping the reader with a <c>using Statement block</c>.
    /// </summary>
    /// <seealso ref="https://msdn.microsoft.com/en-us/library/yh598w02.aspx"/>
    /// <remarks>Calling <c>Dispose()</c> does not send the message out.</remarks>
    public unsafe sealed class )::");
source->append(Codegen::GetString((*(node))[iterator_1]->name));
source->append(R"::(Writer : )::");
source->append(Codegen::GetString(base_accessor_name_1));
source->append(R"::(, IDisposable
    {
        internal byte* buffer = null;
        internal int BufferLength;
        internal int Length; 
        public unsafe )::");
source->append(Codegen::GetString((*(node))[iterator_1]->name));
source->append(R"::(Writer()::");
for (size_t iterator_2 = 0; iterator_2 < ((*(node))[iterator_1]->fieldList)->size();++iterator_2)
{
source->append(R"::( )::");
source->append(Codegen::GetString((*((*(node))[iterator_1]->fieldList))[iterator_2]->fieldType));
source->append(R"::( )::");
source->append(Codegen::GetString((*((*(node))[iterator_1]->fieldList))[iterator_2]->name));
source->append(R"::( = default()::");
source->append(Codegen::GetString((*((*(node))[iterator_1]->fieldList))[iterator_2]->fieldType));
source->append(R"::() )::");
if (iterator_2 < ((*(node))[iterator_1]->fieldList)->size() - 1)
source->append(",");
}
source->append(R"::()
            : base(null
                  )::");
if ((*(node))[iterator_1]->getLayoutType() != LT_FIXED)
{
source->append(R"::(
                  , null
                  )::");
}
source->append(R"::( )
        {
            int preservedHeaderLength = TrinityProtocol.MsgHeader;
            )::");

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = 0;
module_ctx.m_arguments.push_back(Codegen::GetString("message"));
std::string* module_content = Modules::SerializeParametersToBuffer((*(node))[iterator_1], &module_ctx);
    source->append(*module_content);
    delete module_content;
}
source->append(R"::(
            buffer = tmpcellptr - preservedHeaderLength;
            this.CellPtr = buffer + preservedHeaderLength;
            Length = BufferLength - preservedHeaderLength;
            )::");
if ((*(node))[iterator_1]->getLayoutType() != LT_FIXED)
{
source->append(R"::(
            this.ResizeFunction = WriterResizeFunction;
            )::");
}
source->append(R"::(
        }
        internal unsafe )::");
source->append(Codegen::GetString((*(node))[iterator_1]->name));
source->append(R"::(Writer(int asyncRspHeaderLength, )::");
for (size_t iterator_2 = 0; iterator_2 < ((*(node))[iterator_1]->fieldList)->size();++iterator_2)
{
source->append(R"::( )::");
source->append(Codegen::GetString((*((*(node))[iterator_1]->fieldList))[iterator_2]->fieldType));
source->append(R"::( )::");
source->append(Codegen::GetString((*((*(node))[iterator_1]->fieldList))[iterator_2]->name));
source->append(R"::( = default()::");
source->append(Codegen::GetString((*((*(node))[iterator_1]->fieldList))[iterator_2]->fieldType));
source->append(R"::() )::");
if (iterator_2 < ((*(node))[iterator_1]->fieldList)->size() - 1)
source->append(",");
}
source->append(R"::()
            : base(null
                  )::");
if ((*(node))[iterator_1]->getLayoutType() != LT_FIXED)
{
source->append(R"::(
                  , null
                  )::");
}
source->append(R"::( )
        {
            int preservedHeaderLength = TrinityProtocol.MsgHeader + asyncRspHeaderLength;
            )::");

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = 0;
module_ctx.m_arguments.push_back(Codegen::GetString("message"));
std::string* module_content = Modules::SerializeParametersToBuffer((*(node))[iterator_1], &module_ctx);
    source->append(*module_content);
    delete module_content;
}
source->append(R"::(
            buffer = tmpcellptr - preservedHeaderLength;
            this.CellPtr = buffer + preservedHeaderLength;
            Length = BufferLength - preservedHeaderLength;
            )::");
if ((*(node))[iterator_1]->getLayoutType() != LT_FIXED)
{
source->append(R"::(
            this.ResizeFunction = WriterResizeFunction;
            )::");
}
source->append(R"::(
        }
        )::");
if ((*(node))[iterator_1]->getLayoutType() != LT_FIXED)
{
source->append(R"::(
        private byte* WriterResizeFunction(byte* ptr, int ptr_offset, int delta)
        {
            int curlen = Length;
            int tgtlen = curlen + delta;
            if (delta >= 0)
            {
                byte* currentBufferPtr = buffer;
                int required_length = (int)(tgtlen + (this.CellPtr - currentBufferPtr));
                if(required_length < curlen) throw new AccessorResizeException("Accessor size overflow.");
                if (required_length <= BufferLength)
                {
                    Memory.memmove(
                        ptr + ptr_offset + delta,
                        ptr + ptr_offset,
                        (ulong)(curlen - (ptr + ptr_offset - this.CellPtr)));
                    Length = tgtlen;
                    return ptr;
                }
                else
                {
                    while (BufferLength < required_length)
                    {
                        if (int.MaxValue - BufferLength >= (Buff)::");
source->append(R"::(erLength>>1)) BufferLength += (BufferLength >> 1);
                        else if (int.MaxValue - BufferLength >= (1 << 20)) BufferLength += (1 << 20);
                        else BufferLength = int.MaxValue;
                    }
                    byte* tmpBuffer = (byte*)Memory.malloc((ulong)BufferLength);
                    Memory.memcpy(
                        tmpBuffer,
                        currentBufferPtr,
                        (ulong)(ptr + ptr_offset - currentBufferPtr));
                    byte* newCellPtr = tmpBuffer + (this.CellPtr - currentBufferPtr);
                    Memory.memcpy(
                        newCellPtr + (ptr_offset + delta),
                        ptr + ptr_offset,
                        (ulong)(curlen - (ptr + ptr_offset - this.CellPtr)));
                    Length = tgtlen;
                    this.CellPtr = newCellPtr;
                    Memory.free(buffer);
                    buffer = tmpBuffer;
                    return tmpBuffer + (ptr )::");
source->append(R"::(- currentBufferPtr);
                }
            }
            else
            {
                if (curlen + delta < 0) throw new AccessorResizeException("Accessor target size underflow.");
                Memory.memmove(
                    ptr + ptr_offset,
                    ptr + ptr_offset - delta,
                    (ulong)(Length - (ptr + ptr_offset - delta - this.CellPtr)));
                Length = tgtlen;
                return ptr;
            }
        }
        )::");
}
source->append(R"::(
        /// <summary>
        /// Dispose the message writer and release the memory resource.
        /// It is the user's responsibility to call this method after finished accessing the message.
        /// </summary>
        public void Dispose()
        {
            Memory.free(buffer);
            buffer = null;
        }
    }
    )::");
}
source->append(R"::(
}
)::");

            return source;
        }
    }
}

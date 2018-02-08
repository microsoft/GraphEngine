using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.Network.Messaging;
using Trinity.Storage;
using Trinity.TSL;
using Trinity.TSL.Lib;

/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
namespace t_Namespace
{
    [TARGET("std::vector<NStructBase*>")]
    [MAP_LIST("t_accessor", "node")]
    [MAP_VAR("t_accessor", "")]
    [MAP_VAR("t_accessor_name", "name")]
    [META_VAR("std::string", "base_accessor_name")]
    [MAP_VAR("t_struct_name_Accessor", "%base_accessor_name")]
    [MAP_LIST("t_field", "fieldList", MemberOf = "t_accessor")]
    [MAP_VAR("t_field_name", "name")]
    [MAP_VAR("t_field_type", "fieldType")]
    [FOREACH]
    [IF("$t_accessor->is_struct()")]
    [META("%base_accessor_name = *$t_accessor->name + \"_Accessor\";")]
    [ELSE]
    [META("%base_accessor_name = *$t_accessor->name + \"_Message_Accessor\";")]
    [END]
    /// <summary>
    /// Represents a read-only accessor on the message of type t_accessor_name defined in the TSL protocols.
    /// The message readers will be instantiated by the system and passed to user's logic.
    /// After finished accessing the message. It is the user's responsibility to call Dispose()
    /// on the reader object. Recommend wrapping the reader with a <c>using Statement block</c>.
    /// <seealso ref="https://msdn.microsoft.com/en-us/library/yh598w02.aspx"/>
    /// </summary>
    public unsafe sealed class t_accessor_nameReader : t_struct_name_Accessor, IDisposable
    {
        byte * buffer;
        internal t_accessor_nameReader(byte* buf, int offset)
            : base(buf + offset
                  /*IF("$t_accessor->getLayoutType() != LT_FIXED")*/
                  , ReaderResizeFunc
                  /*END*/ )
        {
            buffer = buf;
        }

        [IF("$t_accessor->getLayoutType() != LT_FIXED")]
        /** 
         * t_accessor_nameReader is not resizable because it may be attached
         * to a buffer passed in from the network layer and we don't know how
         * to resize it.
         */
        static byte* ReaderResizeFunc(byte* ptr, int offset, int delta)
        {
            throw new InvalidOperationException("t_accessor_nameReader is not resizable");
        }
        [END]

        /// <summary>
        /// Dispose the message reader and release the memory resource.
        /// It is the user's responsibility to call this method after finished accessing the message.
        /// </summary>
        public void Dispose()
        {
            //XXX in a message handler one should not call Dispose(), as this would cause double free
            //on the message buffer.
            Memory.free(buffer);
            buffer = null;
        }
    }

    /// <summary>
    /// Represents a writer accessor on the message of type t_accessor_name defined in the TSL protocols.
    /// The message writers should be instantiated by the user's logic and passed to the system to send it out.
    /// After finished accessing the message. It is the user's responsibility to call Dispose()
    /// on the writer object. Recommend wrapping the reader with a <c>using Statement block</c>.
    /// </summary>
    /// <seealso ref="https://msdn.microsoft.com/en-us/library/yh598w02.aspx"/>
    /// <remarks>Calling <c>Dispose()</c> does not send the message out.</remarks>
    public unsafe sealed class t_accessor_nameWriter : t_struct_name_Accessor, IDisposable
    {
        //For a writer, the buffer pointer is not passed in, instead, the writer itself will allocate a buffer
        //Also, we should add a finalizer for the writer so that we can free that buffer when the writer is GCed.

        internal byte* buffer = null;
        internal int BufferLength;
        internal int Length; // message body length

        public unsafe t_accessor_nameWriter([FOREACH(",")] t_field_type t_field_name = default(t_field_type) /*END*/)
            : base(null
                  /*IF("$t_accessor->getLayoutType() != LT_FIXED")*/
                  , null
                  /*END*/ )
        {
            int preservedHeaderLength = TrinityProtocol.MsgHeader;
            MUTE();
            byte* tmpcellptr = null;
            MUTE_END();

            MODULE_CALL("SerializeParametersToBuffer", "$t_accessor", "\"message\"");

            buffer = tmpcellptr - preservedHeaderLength;
            this.CellPtr = buffer + preservedHeaderLength;
            Length = BufferLength - preservedHeaderLength;

            IF("$t_accessor->getLayoutType() != LT_FIXED");
            this.ResizeFunction = WriterResizeFunction;
            END();
        }
        [IF("$t_accessor->getLayoutType() != LT_FIXED")]
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
                        // first try: 1.5x growth
                        if (int.MaxValue - BufferLength >= (BufferLength>>1)) BufferLength += (BufferLength >> 1);
                        // second try: step size
                        else if (int.MaxValue - BufferLength >= (1 << 20)) BufferLength += (1 << 20);
                        // third try: approach intmax
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
                    return tmpBuffer + (ptr - currentBufferPtr);
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
        [END]
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
    /*END*/
}

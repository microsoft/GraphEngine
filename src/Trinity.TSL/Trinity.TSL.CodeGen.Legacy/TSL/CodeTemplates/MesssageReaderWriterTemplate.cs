using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.Network.Messaging;
using Trinity.Utilities;

namespace Trinity.TSL
{
    internal class MessageReaderWriterTemplate
    {
        private static string generate_reader_impl(StructDescriptor struct_desc, string base_class_suffix)
        {
            string ret = "";

            // TODO in the new codegen we dropped readonly accessors.
            // here we should adapt the code a bit, that we can introduce 
            // a resize function that always throws an exception when it
            // is called.

            string className = struct_desc.Name + "Reader";
            string baseClassName = struct_desc.Name + base_class_suffix;
            string baseConstructor = "base(buf + offset" + (struct_desc.IsFixed() ? (")") : (",null)"));
            string baseConstructorWithNullCellPtr = "base(null" + (struct_desc.IsFixed() ? (")") : (",null)"));
            ret += @"
    /// <summary>
    /// Represents a read-only accessor on the message of type " + struct_desc.Name + @" defined in the TSL protocols.
    /// The message readers will be instantiated by the system and passed to user's logic.
    /// After finished accessing the message. It is the user's responsibility to call Dispose()
    /// on the reader object. Recommend wrapping the reader with a <c>using Statement block</c>.
    /// <seealso ref=""https://msdn.microsoft.com/en-us/library/yh598w02.aspx""/>
    /// </summary>
    public unsafe sealed class " + className + " : " + baseClassName + @", IDisposable
    {
        byte * buffer;
        internal unsafe " + className + @"(byte* buf, int offset)
            : " + baseConstructor + @"
        {
            buffer = buf;
        }
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

";
            return ret;
        }

        private static string generate_writer_impl(StructDescriptor struct_desc, string base_class_suffix)
        {
            CodeWriter ret = new CodeWriter();

            string className = struct_desc.Name + "Writer";
            string baseClassName = struct_desc.Name + base_class_suffix;
            string baseConstructor = struct_desc.IsFixed() ? "base(null)" : "base(null,null)";
            string resizeFunction = struct_desc.IsFixed() ? "" :
                @"
            this.ResizeFunction = (ptr, ptr_offset, delta)=>
            {
                if(delta >= 0)
                {
                    byte* currentBufferPtr = buffer;
                    int required_length = (int)(this.Length + delta + (this.CellPtr - currentBufferPtr));
                    if(required_length <= BufferLength)
                    {
                        Memory.memmove(
                            ptr + ptr_offset + delta,
                            ptr + ptr_offset,
                            (ulong)(Length - (ptr + ptr_offset - this.CellPtr)));
                        Length += delta;
                        return ptr;
                    }
                    else
                    {
                        int target_length = BufferLength << 1;
                        while(target_length < required_length)
                        {
                            target_length = (target_length << 1);
                        }
                        byte* tmpBuffer = (byte*)Memory.malloc((ulong)target_length);
                        Memory.memcpy(
                            tmpBuffer,
                            currentBufferPtr,
                            (ulong)(ptr + ptr_offset - currentBufferPtr));
                        byte* newCellPtr = tmpBuffer + (this.CellPtr - currentBufferPtr);
                        Memory.memcpy(
                            newCellPtr + (ptr_offset + delta),
                            ptr + ptr_offset,
                            (ulong)(Length - (ptr + ptr_offset - this.CellPtr)));
                        Length += delta;
                        this.CellPtr = newCellPtr;
                        Memory.free(buffer);
                        buffer = tmpBuffer;
                        BufferLength = target_length;
                        return tmpBuffer + (ptr - currentBufferPtr);
                    }
                }else
                {
                    Memory.memmove(
                        ptr + ptr_offset,
                        ptr + ptr_offset - delta,
                        (ulong)(Length - (ptr + ptr_offset - delta - this.CellPtr)));
                    Length += delta;
                    return ptr;
                }
            };";

            ret += @"
    /// <summary>
    /// Represents a writer accessor on the message of type " + struct_desc.Name + @" defined in the TSL protocols.
    /// The message writers should be instantiated by the user's logic and passed to the system to send it out.
    /// After finished accessing the message. It is the user's responsibility to call Dispose()
    /// on the writer object. Recommend wrapping the reader with a <c>using Statement block</c>.
    /// </summary>
    /// <seealso ref=""https://msdn.microsoft.com/en-us/library/yh598w02.aspx""/>
    /// <remarks>Calling <c>Dispose()</c> does not send the message out.</remarks>
    public unsafe sealed class " + className + @": " + baseClassName + @", IDisposable
    {
        //For a writer, the buffer pointer is not passed in, instead, the writer itself will allocate a buffer
        //Also, we should add a finalizer for the writer so that we can free that buffer when the writer is GCed.

        internal byte* buffer = null;
        internal int BufferLength;
        internal int Length; // message body length

";
            ret += TSLCompiler.GenerateAssignmentPrototypeParameterList2("public unsafe " + className + "(", struct_desc);

            ret += @": " + baseConstructor + @"
        {
";
            ret += @"
        int preservedHeaderLength = " + TrinityProtocol.MsgHeader + @";";

            ret += CellCodeTemplate.GenerateParametersToByteArrayCode(struct_desc, generatePreserveHeaderCode: true, forCell: false, unmanagedBuf: true) + @"
        buffer = tmpcellptr - preservedHeaderLength;
        this.CellPtr = buffer + preservedHeaderLength;
        Length = BufferLength - preservedHeaderLength;"
        + resizeFunction + @"
        }
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
";
            return ret;
        }

        internal static string GenerateStructMessageReader(StructDescriptor struct_desc)
        {
            return generate_reader_impl(struct_desc, "_Accessor_ReadOnly");
        }

        internal static string GenerateStructMessageWriter(StructDescriptor struct_desc)
        {
            return generate_writer_impl(struct_desc, "_Accessor");
        }

        internal static string GenerateCellMessageReader(StructDescriptor struct_desc)
        {
            return generate_reader_impl(struct_desc, "_Message_Accessor_ReadOnly");
        }

        internal static string GenerateCellMessageWriter(StructDescriptor struct_desc)
        {
            return generate_writer_impl(struct_desc, "_Message_Accessor");
        }
    }
}
